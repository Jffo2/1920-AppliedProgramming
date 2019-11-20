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
            if (points.Count < 3) return triangles;
            var copiedPoints = Util.Cloner.DeepClone(points);

            int oldPointsCount = 1;

            List<int> convexVertices = new List<int>();

            bool checkHighestVertex = false;

            while (copiedPoints.Count > 3)
            {
                
                // Check if nothing changed last time, this might mean we are stuck so we should try a different approach and take the highest vertex that time
                if (oldPointsCount == copiedPoints.Count)
                {
                    if (checkHighestVertex)
                    {
                        // We already tried the highest vertex approach, just eliminate a point and try to continue, if we're lucky no-one will notice
                        copiedPoints.RemoveAt(GetLowestVertexIndex(copiedPoints));
                        checkHighestVertex = false;
                    }
                    checkHighestVertex = true;
                    System.Diagnostics.Debug.WriteLine("Got stuck, trying highest vertex!");
                }
                oldPointsCount = copiedPoints.Count;
                var lowestVertexIndex = (checkHighestVertex)? GetHighestVertexIndex(copiedPoints) : GetLowestVertexIndex(copiedPoints);

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

                    if (tSignLowestVertex*tSignVertexI>0)
                    {
                        // We found a convex vertex, also check if it's an ear
                        //convexVertices.Add(i);
                        if (VertexIsEar(i,copiedPoints))
                        {
                            triangles.Add(new Triangle(pi1, pi2, pi3));
                            copiedPoints.RemoveAt(i);
                            checkHighestVertex = false;
                            break;
                        }
                    }
                }
            }
            triangles.Add(new Triangle(copiedPoints[0], copiedPoints[1], copiedPoints[2]));

            return triangles;
        }

        private static bool VertexIsEar(int vertexIndex, List<PointF> polygon)
        {
            var previousVertexIndex = CirculateIndex(vertexIndex - 1, polygon.Count);
            var nextVertexIndex = CirculateIndex(vertexIndex + 1, polygon.Count);
            for (int i = 0; i<polygon.Count; i++)
            {
                // The three verteces constructing the ear will automatically lie inside it, skip them we're only interested in the other points laying inside
                if (i==previousVertexIndex || i==vertexIndex || i==nextVertexIndex)
                {
                    continue;
                }

                var t1 = CalculateTvalueSign(polygon[previousVertexIndex], polygon[vertexIndex], polygon[i]);
                var t2 = CalculateTvalueSign(polygon[vertexIndex], polygon[nextVertexIndex], polygon[i]);
                var t3 = CalculateTvalueSign(polygon[previousVertexIndex], polygon[i], polygon[nextVertexIndex]);
                var tOriginal = CalculateTvalueSign(polygon[previousVertexIndex], polygon[vertexIndex], polygon[nextVertexIndex]);

                // The point is on the same side of the edges as all other points, this means it is inside the triangle and thus we are in fact intersecting something
                if (tOriginal==t1 && tOriginal==t2 && tOriginal==t3)
                {
                    return false;
                }
            }

            return true;
        }

        private static int CirculateIndex(int index, int upperBoundary)
        {
            return (index < 0 ? upperBoundary + index : index % upperBoundary);
        }

        public static PointF GetLowestVertex(List<PointF> points)
        {
            return points.OrderBy(point => point.Y).First();
        }

        public static PointF GetHighestVertex(List<PointF> points)
        {
            return points.OrderByDescending(point => point.Y).First();
        }

        public static int GetLowestVertexIndex(List<PointF> points)
        {
            // Return the point with the lowest y value
            return points.IndexOf(GetLowestVertex(points));
        }

        public static int GetHighestVertexIndex(List<PointF> points)
        {
            return points.IndexOf(GetHighestVertex(points));
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
