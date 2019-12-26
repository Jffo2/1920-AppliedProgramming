using BoundaryVisualizer.Models;
using BoundaryVisualizer.Util;
using GeoJSON.Net.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BoundaryVisualizer.Logic
{
    public static class CustomHoleTriangulator
    {
        public static List<PointF> ConstructPolygon(List<List<PointF>> polygon)
        {
            //return polygon[0];
            List<PointF> topVertices = new List<PointF>(FindTopVertices(polygon));
            List<PointF> constructedPolygon = CreateSimplePolygon(polygon, topVertices);
            return constructedPolygon;
        }

        private static List<PointF> CreateSimplePolygon(List<List<PointF>> polygon, List<PointF> topVertices)
        {

            var mainPolygon = Cloner.DeepClone(polygon[0]);
            if (IsPolygonClockwise(mainPolygon)) mainPolygon.Reverse();
            int insertionIndex = 0;

            for (int i = 1; i<polygon.Count; i++)
            {
                // Don't bother adding really small holes, they'll just mess up triangulation and add no value
                var distances = mainPolygon.Select((point) => CalculateDistance(point, topVertices[i])).ToList();
                var closestVertexIndex = distances.IndexOf(distances.Min());

                mainPolygon.Insert(closestVertexIndex, mainPolygon[closestVertexIndex]);
                insertionIndex = closestVertexIndex+1;

                //if (IsPolygonClockwise(polygon[i])) polygon[i].Reverse();
                var rotatedPolygon = polygon[i].Skip(polygon[i].IndexOf(topVertices[i])).ToList();
                rotatedPolygon.AddRange(polygon[i].Take(polygon[i].IndexOf(topVertices[i])));

                rotatedPolygon.Add(rotatedPolygon[0]);
                // If there's a hole that intersects our polygon, just inserting it will not help,
                // if we just insert it the polygon will self intersect and triangulation is not currently equipped for that.
                if (PolygonIntersections(mainPolygon, rotatedPolygon)>0)
                {
                    continue;
                }
                mainPolygon.InsertRange(insertionIndex, rotatedPolygon);
                return mainPolygon;

            }
            return mainPolygon;
        }

        private static int PolygonIntersections(List<PointF> polygon1, List<PointF> polygon2, double epsilon = 0.005)
        {
            int intersections = 0;
            foreach (var point1 in polygon1)
            {
                intersections += polygon2.Select((point2) => CalculateDistance(point1, point2)).Where((distance) => distance <= epsilon).Count();
            }
            return intersections;
        }

        public static IEnumerable<PointF> FindTopVertices(List<List<PointF>> polygon)
        {
            return polygon.Select((gon) => gon.OrderBy((point) => point.Y).First());
        }

        private static double CalculateDistance(PointF p1, PointF p2)
        {
            return (p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y);
        }

        /// <summary>
        /// Checks if a polygon is ordered clockwise
        /// </summary>
        /// <param name="points">a list of points representing the polygon</param>
        /// <returns>true if clockwise, false if counter-clockwise</returns>
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
    }
}

