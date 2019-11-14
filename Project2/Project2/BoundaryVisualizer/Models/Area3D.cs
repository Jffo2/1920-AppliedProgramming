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
            float? latDivisor = null;
            float? lonDivisor = null;
            Color[] colors = { Color.Red, Color.Green, Color.Blue, Color.Cyan, Color.Lime, Color.Magenta, Color.Black, Color.Coral, Color.Salmon, Color.Silver };
            Bitmap b = new Bitmap(side, side);
            //Polygon p = multiPolygon.Coordinates.OrderBy((poly) => poly.Coordinates.Count).First();
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

                        }

                        VisualizeLineString(g, points, colors[i%colors.Length]);

                    }
                }
            }
            return b;
        }

        private List<PointF> EliminatePoints(List<PointF> points)
        {
            return null;
        }

        /*
        function DouglasPeucker(PointList[], epsilon)
            // Find the point with the maximum distance
            dmax = 0
            index = 0
            end = length(PointList)
            for i = 2 to ( end - 1) {
                d = perpendicularDistance(PointList[i], Line(PointList[1], PointList[end])) 
                if ( d > dmax ) {
                    index = i
                    dmax = d
                }
            }
    
            ResultList[] = empty;
    
            // If max distance is greater than epsilon, recursively simplify
            if ( dmax > epsilon ) {
                // Recursive call
                recResults1[] = DouglasPeucker(PointList[1...index], epsilon)
                recResults2[] = DouglasPeucker(PointList[index...end], epsilon)

                // Build the result list
                ResultList[] = {recResults1[1...length(recResults1)-1], recResults2[1...length(recResults2)]}
            } else {
                ResultList[] = {PointList[1], PointList[end]}
            }
            // Return the result
            return ResultList[]
        end
    */
        private void VisualizeLineString(Graphics g, List<PointF> points, Color c)
        {
            Brush b = new SolidBrush(c);
            foreach (PointF point in points)
            {
                g.FillEllipse(b, point.X, point.Y, 2, 2);
            }
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

            return new PointF(((float)(x - minX)) * (400.0f / (yScale)), ((float)(y - minY)) * (400.0f / (yScale)));
        }
    }
}
