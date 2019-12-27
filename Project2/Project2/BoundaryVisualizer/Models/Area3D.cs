
using BoundaryVisualizer.Logic;
using GeoJSON.Net.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Threading;

namespace BoundaryVisualizer.Models
{
    public class Area3D
    {
        /// <summary>
        /// The 3D model for the Area
        /// </summary>
        public Model3DGroup Area { get; private set; }
        /// <summary>
        /// The scale of this object
        /// </summary>
        public float Scale { get; private set; }
        /// <summary>
        /// The position in the world of this object, can be used to translate the model to it's actual position in the world,
        /// this is because the model itself is built starting from origin, to get the model to it's actual location, it has to be moved to this position
        /// (scale has to be incorporated as well)
        /// </summary>
        public PointF WorldPosition { get; private set; }

        /// <summary>
        /// The material of the object
        /// </summary>
        public Material Material
        {
            get
            {
                return material;
            }
            set
            {
                ApplyMaterial(value);
                material = value;
            }
        }

        private Material material;
        private readonly Dispatcher dispatcher;

        /// <summary>
        /// Constructor, this will start processing the polygon
        /// </summary>
        /// <param name="multiPolygon">the polygon to process</param>
        /// <param name="dispatcher">the dispatcher to dispatch to the main thread</param>
        /// <param name="scale">the scale of the model</param>
        /// <param name="height">the height of the model</param>
        public Area3D(MultiPolygon multiPolygon, Dispatcher dispatcher, float scale = 100.0f, float height = 400.0f)
        {
            dispatcher.Invoke(() =>
            {
                Area = new Model3DGroup();
            });
            this.dispatcher = dispatcher;
            GenerateModelFromMultiPolygon(multiPolygon, scale, height);
        }

        /// <summary>
        /// Apply a material to the Model
        /// </summary>
        /// <param name="m">the material to assign</param>
        private void ApplyMaterial(Material m)
        {
            dispatcher.Invoke(() =>
            {
                foreach (GeometryModel3D model in Area.Children)
                {
                    model.Material = m;
                }
            });
        }

        /// <summary>
        /// Interpret the MultiPolygon object and start processing it to create a 3D model
        /// </summary>
        /// <param name="multiPolygon">the multipolygon to process</param>
        /// <param name="scale">the scale of the 3d object</param>
        /// <param name="height">the height of the model</param>
        private void GenerateModelFromMultiPolygon(MultiPolygon multiPolygon, float scale, float height)
        {
            int side = 400;

            float maxX = 0;
            float maxY = 0;
            float minX = float.MaxValue;
            float minY = float.MaxValue;

            // Get the extreme points, these will be used for normalization
            foreach (Polygon p in multiPolygon.Coordinates)
            {
                foreach (LineString lstring in p.Coordinates)
                {
                    var maxX2 = lstring.Coordinates.Select(position => GetPointFromCoordinates(side, side, position).X).Max();
                    var maxY2 = lstring.Coordinates.Select(position => GetPointFromCoordinates(side, side, position).Y).Max();
                    var minX2 = lstring.Coordinates.Select(position => GetPointFromCoordinates(side, side, position).X).Min();
                    var minY2 = lstring.Coordinates.Select(position => GetPointFromCoordinates(side, side, position).Y).Min();
                    if (maxX2 > maxX) maxX = maxX2;
                    if (maxY2 > maxY) maxY = maxY2;
                    if (minX2 < minX) minX = minX2;
                    if (minY2 < minY) minY = minY2;
                }
            }

            WorldPosition = new PointF(minX, minY);
            var o = multiPolygon.Coordinates.OrderBy(coords => coords.Coordinates.Select(coordinate => coordinate.Coordinates.Count).Max());
            for (int i = 0; i < o.Count(); i++)
            {
                System.Diagnostics.Debug.WriteLine("Holecount: " + (o.ElementAt(i).Coordinates.Count-1));

                List<List<PointF>> normalizedPolygon = new List<List<PointF>>();

                foreach (LineString lstring in o.ElementAt(i).Coordinates)
                {

                    List<PointF> points = new List<PointF>();
                    // Loop over the points, rescale them and add them to the list
                    foreach (IPosition point in lstring.Coordinates)
                    {
                        var xy = GetPointFromCoordinates(side, side, point);

                        var normalizedPoint = NormalizePoint(xy, minX, minY, maxX, maxY, side);

                        points.Add(normalizedPoint);
                    }

                    // Use Peucker to eliminate points
                    List<PointF> scarcePoints = EliminatePoints(points);
                    // The algorithm requires the polygon to be in the first index and all holes to follow, we assume that the longest sequence of points is the polygon and all others are holes
                    if (lstring.Coordinates.Count == o.ElementAt(i).Coordinates.Select(coordinate => coordinate.Coordinates.Count).Max()) normalizedPolygon.Insert(0,scarcePoints);
                    else normalizedPolygon.Add(scarcePoints);
                }
                // Create a single polygon of the polygon and all it's holes
                List<PointF> compositePolygon = CustomHoleTriangulator.ConstructPolygon(normalizedPolygon);
                // Triangulate the polygon
                List<Triangle> triangles = CustomTriangulator.Triangulate(compositePolygon);
                DrawPoints(compositePolygon, normalizedPolygon);
                // Generate the model
                AssembleModel(compositePolygon, triangles, scale, height);
            }
        }

        private void DrawPoints(List<PointF> points, List<List<PointF>> normalizedPolygon)
        {
            try
            {
                Bitmap b = new Bitmap(400, 400);
                using (Graphics g = Graphics.FromImage(b))
                {
                    g.DrawPolygon(new System.Drawing.Pen(new SolidBrush(System.Drawing.Color.Green)), normalizedPolygon[0].ToArray());
                    g.DrawPolygon(new System.Drawing.Pen(new SolidBrush(System.Drawing.Color.Red)), points.ToArray());
                    foreach (List<PointF> hole in normalizedPolygon.Skip(1))
                    {
                        g.DrawPolygon(new System.Drawing.Pen(new SolidBrush(System.Drawing.Color.Blue)), hole.ToArray());
                    }
                }
                var l = "jorn123\\testdraw-" + DateTime.Now.Millisecond + (new Random()).NextDouble() + ".png";
                System.Diagnostics.Debug.WriteLine(l);
                b.Save(l);
            }
            catch { }
        }


        /// <summary>
        /// Generate the 3D model and assign it to the Geometry property
        /// </summary>
        /// <param name="points">a list of points representing a polygon</param>
        /// <param name="triangles">a list of triangles representing the polygon shape</param>
        /// <param name="scale">the scale of our object</param>
        /// <param name="height">the height of the model</param>
        private void AssembleModel(List<PointF> points, List<Triangle> triangles, float scale, float height)
        {
            var mesh = new MeshGeometry3D();
            Point3DCollection pointsCollection = new Point3DCollection();
            Int32Collection triangleIndices = new Int32Collection();

            for (int i = 0; i < points.Count; i++)
            {
                // Add the points
                pointsCollection.Add(new Point3D((points[i].X / Scale/* + WorldPosition.X*/) * scale, (points[i].Y / Scale/* + WorldPosition.Y*/) * scale, 0));
                pointsCollection.Add(new Point3D((points[i].X / Scale/* + WorldPosition.X*/) * scale, (points[i].Y / Scale/* + WorldPosition.Y*/) * scale, height));


                var previousBottomIndex = CustomTriangulator.CirculateIndex(i - 1, points.Count) * 2;
                var previousTopIndex = previousBottomIndex + 1;
                var currentBottomIndex = i * 2;
                var currentTopIndex = i * 2 + 1;

                //Add the triangles
                triangleIndices.Add(currentTopIndex);
                triangleIndices.Add(currentBottomIndex);
                triangleIndices.Add(previousBottomIndex);

                triangleIndices.Add(previousTopIndex);
                triangleIndices.Add(currentTopIndex);
                triangleIndices.Add(previousBottomIndex);
            }
            for (int i = 0; i < triangles.Count; i++)
            {
                var p1Index = points.IndexOf(triangles[i].Point1) * 2;
                var p2Index = points.IndexOf(triangles[i].MiddlePoint) * 2;
                var p3Index = points.IndexOf(triangles[i].Point2) * 2;

                // Bottom Face triangle
                triangleIndices.Add(p1Index);
                triangleIndices.Add(p2Index);
                triangleIndices.Add(p3Index);

                // Top Face triangle
                triangleIndices.Add(p3Index + 1);
                triangleIndices.Add(p2Index + 1);
                triangleIndices.Add(p1Index + 1);
            }

            mesh.Positions = pointsCollection;
            mesh.TriangleIndices = triangleIndices;

            // Add children on the main thread, otherwise wpf throws an error
            dispatcher.Invoke(() =>
                {
                    Area.Children.Add(new GeometryModel3D
                    {
                        Geometry = mesh
                    });
                });
        }

        /// <summary>
        /// Use DouglasPeucker to eliminate points
        /// </summary>
        /// <see cref="DouglasPeucker(List{PointF}, double)"/>
        /// <param name="points">the list of points to be eliminated</param>
        /// <returns>the list of points after elimination</returns>
        private static List<PointF> EliminatePoints(List<PointF> points)
        {
            return new List<PointF>(DouglasPeucker(points.GetRange(0, points.Count - 1), 1));//.Concat(new List<PointF>(new PointF[] { points.Last() })));
        }

        /// <summary>
        /// Eliminate points from a list of points recursively
        /// </summary>
        /// <param name="points">list of points to be eliminated</param>
        /// <param name="epsilon">the distance at which to eliminate</param>
        /// <returns>the list of points after elimination</returns>
        /// https://en.wikipedia.org/wiki/Ramer%E2%80%93Douglas%E2%80%93Peucker_algorithm
        private static List<PointF> DouglasPeucker(List<PointF> points, double epsilon)
        {
            double dmax = 0;
            int index = 0;
            int length = points.Count;
            // Find the point that's furthest from the line between the first and last point
            for (int i = 1; i < length - 1; i++)
            {
                double d = PerpendicularDistance(points[i], points[0], points[length - 1]);
                if (d > dmax)
                {
                    index = i;
                    dmax = d;
                }
            }

            List<PointF> resultList = new List<PointF>();

            // If the furthest point is larger than the threshold
            if (dmax > epsilon)
            {
                // Start recursively again, we cannot eliminate this point since it is too far
                // Basically we divide from the starting point to the furthest point and from the furthest point to the end point then we recursively try again
                List<PointF> recursiveResults1 = DouglasPeucker(points.GetRange(0, index + 1), epsilon);
                List<PointF> recursiveResults2 = DouglasPeucker(points.GetRange(index, length - index), epsilon);

                resultList.AddRange(recursiveResults1.GetRange(0, recursiveResults1.Count - 1));
                resultList.AddRange(recursiveResults2);
            }
            else
            {
                // All points between the starting point and end point are too close, eliminate all of them!
                resultList.Add(points[0]);
                resultList.Add(points[points.Count - 1]);
            }

            return resultList;
        }

        /// <summary>
        /// The perpendicular distance from a point to a line
        /// </summary>
        /// <param name="point">the point</param>
        /// <param name="l1">the starting point of the line</param>
        /// <param name="l2">the end point of the line</param>
        /// <returns>the distance from the point to the line as a double</returns>
        private static double PerpendicularDistance(PointF point, PointF l1, PointF l2)
        {
            return Math.Abs((l2.X - l1.X) * (l1.Y - point.Y) - (l1.X - point.X) * (l2.Y - l1.Y)) /
                    Math.Sqrt(Math.Pow(l2.X - l1.X, 2) + Math.Pow(l2.Y - l1.Y, 2));
        }

        /// <summary>
        /// Use Mercator transformation to go from World Coordinates to map coordinates
        /// </summary>
        /// <param name="mapWidth">the width of the map to map to</param>
        /// <param name="mapHeight">the height of the map to map to</param>
        /// <param name="point">the point to map to the map</param>
        /// <returns>the point now in euclidean coordinates</returns>
        private PointF GetPointFromCoordinates(int mapWidth, int mapHeight, IPosition point)
        {
            // get x value
            var x = (point.Longitude + 180.0) * (mapWidth / 360.0);

            // convert from degrees to radians
            var latRad = point.Latitude * Math.PI / 180.0;

            // get y value
            var mercN = Math.Log(Math.Tan((Math.PI / 4) + (latRad / 2.0)));
            var y = (mapHeight / 2.0) - (mapHeight * mercN / (2 * Math.PI));

            return new PointF((float)x, (float)y);
        }

        /// <summary>
        /// Normalize a point to be represented
        /// </summary>
        /// <param name="xy">the point to normalize</param>
        /// <param name="minX">the minimum x</param>
        /// <param name="minY">the maximum x</param>
        /// <param name="maxX">the minimum y</param>
        /// <param name="maxY">the maximum y</param>
        /// <param name="side">the width and height of the space to normalize to</param>
        /// <returns>the normalized point</returns>
        private PointF NormalizePoint(PointF xy, float minX, float minY, float maxX, float maxY, float side)
        {
            var x = xy.X;
            var y = xy.Y;

            var yScale = (maxX - minX) > (maxY - minY) ? maxX - minX : maxY - minY;

            Scale = (side / yScale);

            return new PointF((x - minX) * Scale, (y - minY) * Scale);
        }
    }
}
