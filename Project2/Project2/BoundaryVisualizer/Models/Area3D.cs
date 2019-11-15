
using BoundaryVisualizer.Util;
using GeoJSON.Net.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BoundaryVisualizer.Models
{
    public class Area3D
    {
        public Bitmap Model { get; private set; }

        public Area3D(MultiPolygon multiPolygon)
        {
            Model = GenerateModelFromMultiPolygon(multiPolygon);
        }

        private Bitmap GenerateModelFromMultiPolygon(MultiPolygon multiPolygon)
        {
            int side = 400;
            Color[] colors = { Color.Red, Color.Green, Color.Blue, Color.Cyan, Color.Lime, Color.Magenta, Color.Black, Color.Coral, Color.Salmon, Color.Silver };
            Bitmap b = new Bitmap(side, side);
            using (Graphics g = Graphics.FromImage(b))
            {
                g.FillRectangle(new SolidBrush(Color.Orange), 0, 0, side, side);

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
                for (int i = 0; i < multiPolygon.Coordinates.Count; i++)
                {
                    foreach (LineString lstring in multiPolygon.Coordinates[i].Coordinates)
                    {
                        List<PointF> points = new List<PointF>();
                        foreach (IPosition point in lstring.Coordinates)
                        {
                            var xy = GetPointFromCoordinates(side, side, point);

                            var normalizedPoint = NormalizePoint(xy, minX, minY, maxX, maxY);

                            points.Add(normalizedPoint);
                            System.Diagnostics.Debug.WriteLine(normalizedPoint);
                        }

                        List<PointF> scarcePoints = EliminatePoints(points);
                        System.Diagnostics.Debug.WriteLine("Eliminated " + ((points.Count - scarcePoints.Count) / (float)points.Count * 100.0f) + "% of points");
                        List<Triangle> triangles = GetTriangles(scarcePoints);
                        //VisualizeLineString(g, scarcePoints, colors[i % colors.Length]);
                        VisualizeTriangles(g, triangles, colors[i % colors.Length]);
                    }
                }
            }
            return b;
        }

        private List<Triangle> GetTriangles(List<PointF> points)
        {
            List<PointF> tmpPoints = Cloner.DeepClone(points);
            List<Triangle> triangles = new List<Triangle>();
            int oldPointsLength = 1;

            while (oldPointsLength != tmpPoints.Count)
            {
                oldPointsLength = tmpPoints.Count;
                for (int i = 0; i < tmpPoints.Count; i++)
                {
                    Triangle t = new Triangle(tmpPoints[i], tmpPoints[(i + 1) % tmpPoints.Count], tmpPoints[(i + 2) % tmpPoints.Count]);
                    if (t.Angle <= Math.PI) { triangles.Add(t); tmpPoints.RemoveAt((i + 1) % tmpPoints.Count); break; }
                    System.Diagnostics.Debug.WriteLine("" + tmpPoints[i] + tmpPoints[(i + 1) % tmpPoints.Count] + tmpPoints[(i + 2) % tmpPoints.Count]);
                    System.Diagnostics.Debug.WriteLine(t.Angle);
                }
            }
            Bitmap b = new Bitmap(400, 400);
            using (Graphics g = Graphics.FromImage(b))
            {
                VisualizeLineString(g, tmpPoints, Color.Red);
            }
            b.Save("LeftoverPoints.png");
            return triangles;
        }

        private List<PointF> EliminatePoints(List<PointF> points)
        {
            return new List<PointF>(DouglasPeucker(points.GetRange(0, points.Count - 1), 0.02));//.Concat(new List<PointF>(new PointF[] { points.Last() })));
        }

        private List<PointF> DouglasPeucker(List<PointF> points, double epsilon)
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

        private double PerpendicularDistance(PointF point, PointF linePoint1, PointF linePoint2)
        {
            var a = linePoint1.Y - linePoint2.Y;
            var b = linePoint2.X - linePoint1.X;
            var c = linePoint1.X * linePoint2.Y - linePoint2.X * linePoint1.Y;

            return Math.Abs((a * point.X + b * point.Y + c)) / (Math.Sqrt(a * a + b * b));
        }
        private void VisualizeLineString(Graphics g, List<PointF> points, Color c)
        {
            Brush b = new SolidBrush(c);
            foreach (PointF point in points)
            {
                g.FillEllipse(b, point.X, point.Y, 2, 2);
            }
        }

        private void VisualizeTriangles(Graphics g, List<Triangle> triangles, Color c)
        {
            Brush b = new SolidBrush(c);
            Pen p = new Pen(Color.Black);
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

            return new PointF(((float)(x - minX)) * (390.0f / (yScale))+5, ((float)(y - minY)) * (390.0f / (yScale))+5);
        }
    }
}
