using BoundaryVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace BoundaryVisualizer.Logic
{
    /// <summary>
    /// https://www.researchgate.net/publication/311245887_Accurate_simple_and_efficient_triangulation_of_a_polygon_by_ear_removal_with_lowest_memory_consumption
    /// </summary>
    public static class CustomTriangulator
    {
        public static List<Triangle> Triangulate(List<PointF> points)
        {
            var triangles = new List<Triangle>();
            var copiedPoints = Util.Cloner.DeepClone(points);

            List<int> convexVertices = new List<int>();

            while (copiedPoints.Count != 3)
            {
                var lowestVertexIndex = GetLowestVertexIndex(copiedPoints);

                var p1 = copiedPoints[CirculateIndex(lowestVertexIndex - 1, copiedPoints.Count)];
                var p2 = copiedPoints[CirculateIndex(lowestVertexIndex, copiedPoints.Count)];
                var p3 = copiedPoints[CirculateIndex(lowestVertexIndex + 1, copiedPoints.Count)];

                var tSignLowestVertex = CalculateTvalueSign(p1, p2, p3);
                for (int i = 0; i < copiedPoints.Count; i++)
                {
                    var pi1 = copiedPoints[CirculateIndex(i - 1, copiedPoints.Count)];
                    var pi2 = copiedPoints[CirculateIndex(i, copiedPoints.Count)];
                    var pi3 = copiedPoints[CirculateIndex(i + 1, copiedPoints.Count)];

                    var tSignVertexI = CalculateTvalueSign(pi1, pi2, pi3);
                }

            }



            return triangles;
        }

        private static int CirculateIndex(int index, int upperBoundary)
        {
            return (index < 0 ? upperBoundary + index : index % upperBoundary);
        }

        public static PointF GetLowestVertex(List<PointF> points)
        {
            return points.OrderBy(point => point.Y).First();
        }

        public static int GetLowestVertexIndex(List<PointF> points)
        {
            // Return the point with the lowest y value
            return points.IndexOf(GetLowestVertex(points));
        }

        public static float CalculateTvalue(PointF p1, PointF p2, PointF p3)
        {
            var t = p1.X * (p2.Y - p3.Y) + p2.X * (p3.Y - p1.Y) + p3.X * (p1.Y - p2.Y);
            return t;
        }

        public static int Sign(float t)
        {
            return Math.Sign(t);
        }

        public static int CalculateTvalueSign(PointF p1, PointF p2, PointF p3)
        {
            return Sign(CalculateTvalue(p1, p2, p3));
        }
    }
}
