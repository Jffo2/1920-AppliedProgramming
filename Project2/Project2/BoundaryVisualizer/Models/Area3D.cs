
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
        public Bitmap Model { get; private set; }
        public Model3DGroup Area { get; private set; }
        public float Scale { get; private set; }
        public PointF WorldPosition { get; private set; }

        private readonly Dispatcher dispatcher;

        public Area3D(MultiPolygon multiPolygon, Dispatcher dispatcher)
        {
            dispatcher.Invoke(() =>
            {
                Area = new Model3DGroup();
            });
            this.dispatcher = dispatcher;
            Model = GenerateModelFromMultiPolygon(multiPolygon);
        }

        private Bitmap GenerateModelFromMultiPolygon(MultiPolygon multiPolygon)
        {
            int side = 400;
            System.Drawing.Color[] colors = { System.Drawing.Color.Red, System.Drawing.Color.Green, System.Drawing.Color.Blue, System.Drawing.Color.Cyan, System.Drawing.Color.Lime, System.Drawing.Color.Magenta, System.Drawing.Color.Black, System.Drawing.Color.Coral, System.Drawing.Color.Salmon, System.Drawing.Color.Silver };
            Bitmap b = new Bitmap(side, side);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.FillRectangle(new SolidBrush(System.Drawing.Color.Orange), 0, 0, side, side);

                float maxX = 0;
                float maxY = 0;
                float minX = float.MaxValue;
                float minY = float.MaxValue;

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
                    foreach (LineString lstring in o.ElementAt(i).Coordinates)
                    {
                        if (lstring.Coordinates.Count != o.ElementAt(i).Coordinates.Select(coordinate => coordinate.Coordinates.Count).Max()) continue;
                        List<PointF> points = new List<PointF>();
                        foreach (IPosition point in lstring.Coordinates)
                        {
                            var xy = GetPointFromCoordinates(side, side, point);

                            var normalizedPoint = NormalizePoint(xy, minX, minY, maxX, maxY);

                            points.Add(normalizedPoint);
                        }
                        List<PointF> scarcePoints = EliminatePoints(points);
                        if (IsPolygonClockwise(scarcePoints)) scarcePoints.Reverse();
                        //System.Diagnostics.Debug.WriteLine("Eliminated " + ((points.Count - scarcePoints.Count) / (float)points.Count * 100.0f) + "% of points");
                        List<Triangle> triangles = CustomTriangulator.Triangulate(scarcePoints);

                        VisualizeTriangles(g, triangles, colors[i % colors.Length]);
                        VisualizeLineString(g, scarcePoints, System.Drawing.Color.White);

                        AssembleModel(scarcePoints, triangles);
                    }
                }
            }
            return b;
        }

        private void AssembleModel(List<PointF> points, List<Triangle> triangles)
        {
            var mesh = new MeshGeometry3D();
            Point3DCollection pointsCollection = new Point3DCollection();
            Int32Collection triangleIndices = new Int32Collection();
            pointsCollection.Add(new Point3D(points[0].X, points[0].Y, 0));
            pointsCollection.Add(new Point3D(points[0].X, points[0].Y, 400));

            for (int i = 1; i < points.Count; i++)
            {
                // Add the points
                pointsCollection.Add(new Point3D(points[i].X, points[i].Y, 0));
                pointsCollection.Add(new Point3D(points[i].X, points[i].Y, 400));

                var previousBottomIndex = CustomTriangulator.CirculateIndex(i - 1, points.Count) * 2;
                var previousTopIndex = previousBottomIndex + 1;
                var currentBottomIndex = i*2;
                var currentTopIndex = i*2+1;

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

            dispatcher.Invoke(() =>
                {
                    Area.Children.Add(new GeometryModel3D
                    {
                        Geometry = mesh
                    });
                });
        }

        private static bool IsPolygonClockwise(List<PointF> points)
        {
            double sum = 0.0;
            for (int i = 0; i < points.Count; i++)
            {
                checked
                {
                    sum += (points[(i + 1) % points.Count].X - points[i].X) * (points[(i + 1) % points.Count].Y + points[i].Y);
                }
            }
            return sum < 0.0;
        }

        private static List<PointF> EliminatePoints(List<PointF> points)
        {
            return new List<PointF>(DouglasPeucker(points.GetRange(0, points.Count - 1), 0.2));//.Concat(new List<PointF>(new PointF[] { points.Last() })));
        }

        private static List<PointF> DouglasPeucker(List<PointF> points, double epsilon)
        {
            double dmax = 0;
            int index = 0;
            int length = points.Count;
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

            if (dmax > epsilon)
            {
                List<PointF> recursiveResults1 = DouglasPeucker(points.GetRange(0, index + 1), epsilon);
                List<PointF> recursiveResults2 = DouglasPeucker(points.GetRange(index, length - index), epsilon);

                resultList.AddRange(recursiveResults1.GetRange(0, recursiveResults1.Count - 1));
                resultList.AddRange(recursiveResults2);
            }
            else
            {
                resultList.Add(points[0]);
                resultList.Add(points[points.Count - 1]);
            }

            return resultList;
        }

        private static double PerpendicularDistance(PointF point, PointF l1, PointF l2)
        {
            return Math.Abs((l2.X - l1.X) * (l1.Y - point.Y) - (l1.X - point.X) * (l2.Y - l1.Y)) /
                    Math.Sqrt(Math.Pow(l2.X - l1.X, 2) + Math.Pow(l2.Y - l1.Y, 2));
        }
        private void VisualizeLineString(Graphics g, List<PointF> points, System.Drawing.Color c)
        {
            System.Drawing.Brush b = new SolidBrush(c);
            int index = 0;
            foreach (PointF point in points)
            {
                //g.DrawString("" + index, new Font(FontFamily.GenericMonospace, 11), new SolidBrush(Color.Black), point);
                //System.Diagnostics.Debug.WriteLine("Point" + index + ": " + point);
                index++;
                g.FillEllipse(b, point.X, point.Y, 2, 2);
            }
            var lowestVertex = CustomTriangulator.GetLowestVertex(points);
            g.FillEllipse(new SolidBrush(System.Drawing.Color.Red), lowestVertex.X, lowestVertex.Y, 2, 2);
        }

        private void VisualizeTriangles(Graphics g, List<Triangle> triangles, System.Drawing.Color c)
        {
            System.Drawing.Brush b = new SolidBrush(c);
            System.Drawing.Pen p = new System.Drawing.Pen(System.Drawing.Color.Black);
            foreach (Triangle t in triangles)
            {
                g.FillPolygon(b, new PointF[] { t.Point1, t.MiddlePoint, t.Point2, t.Point1 });
                g.DrawPolygon(p, new PointF[] { t.Point1, t.MiddlePoint, t.Point2, t.Point1 });
            }
            b.Dispose();
        }


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

        private PointF NormalizePoint(PointF xy, float minX, float minY, float maxX, float maxY)
        {
            var x = xy.X;
            var y = xy.Y;

            var yScale = (maxX - minX) > (maxY - minY) ? maxX - minX : maxY - minY;

            Scale = (400.0f / yScale);

            return new PointF((x - minX) * Scale, (y - minY) * Scale);
        }
    }
}
