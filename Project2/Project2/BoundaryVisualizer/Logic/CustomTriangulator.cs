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
        /// <summary>
        /// Start triangulating a polygon
        /// </summary>
        /// <param name="points">a list of points representing a polygon</param>
        /// <returns>a list of triangles</returns>
        public static List<Triangle> Triangulate(List<PointF> points)
        {
            var triangles = new List<Triangle>();
            // Cant' triangulate if there's less than 3 points
            if (points.Count < 3) return triangles;
            // Copy the points list, we will be removing points so best to create a copy first
            var copiedPoints = Util.Cloner.DeepClone(points);

            int oldPointsCount = 1;

            // Sometimes the algorithm might get stuck, this bool will try alternative ways then
            bool checkHighestVertex = false;

            while (copiedPoints.Count > 3)
            {
                // Check if nothing changed last time, this might mean we are stuck so we should try a different approach and take the highest vertex that time
                if (oldPointsCount == copiedPoints.Count)
                {
                    if (checkHighestVertex)
                    {
                        // We already tried the highest vertex approach, just eliminate a point and try to continue, if we're lucky no-one will notice
                        copiedPoints.RemoveAt(GetHighestVertexIndex(copiedPoints));
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

                checkHighestVertex = FindEars(copiedPoints, triangles, tSignLowestVertex);
            }
            // Add the remaining three points
            triangles.Add(new Triangle(copiedPoints[0], copiedPoints[1], copiedPoints[2]));

            return triangles;
        }

        /// <summary>
        /// Find ears, eliminate them from the points and add them to the triangles
        /// </summary>
        /// <param name="copiedPoints">list of points</param>
        /// <param name="triangles">list of triangles</param>
        /// <param name="tSignLowestVertex">the t value sign of the lowest vertex</param>
        /// <returns>a boolean representing if any ears were found</returns>
        private static bool FindEars(List<PointF> copiedPoints,List<Triangle> triangles , int tSignLowestVertex)
        {
            bool checkHighestVertex = true;
            for (int i = 0; i < copiedPoints.Count; i++)
            {
                var pi1 = copiedPoints[CirculateIndex(i - 1, copiedPoints.Count)];
                var pi2 = copiedPoints[CirculateIndex(i, copiedPoints.Count)];
                var pi3 = copiedPoints[CirculateIndex(i + 1, copiedPoints.Count)];

                var tSignVertexI = CalculateTvalueSign(pi1, pi2, pi3);

                // If points are collinear we can just remove the obsolete one
                if (tSignLowestVertex == 0)
                {
                    copiedPoints.RemoveAt(CirculateIndex(i, copiedPoints.Count));
                }

                if (tSignLowestVertex * tSignVertexI > 0)
                {
                    // We found a convex vertex, also check if it's an ear
                    if (VertexIsEar(i, copiedPoints))
                    {
                        if (copiedPoints.Count == 3) break;
                        triangles.Add(new Triangle(pi1, pi2, pi3));
                        copiedPoints.RemoveAt(i);
                        checkHighestVertex = false;
                    }
                }
            }
            return checkHighestVertex;
        }

        /// <summary>
        /// Check if the vertex is an ear, meaning that it's convex and no other points of the polygon lay inside it
        /// </summary>
        /// <param name="vertexIndex">the index of the vertex to test</param>
        /// <param name="polygon">the list of points representing the polygon</param>
        /// <returns>true if the vertex is an ear, otherwise false</returns>
        private static bool VertexIsEar(int vertexIndex, List<PointF> polygon)
        {
            var previousVertexIndex = CirculateIndex(vertexIndex - 1, polygon.Count);
            var nextVertexIndex = CirculateIndex(vertexIndex + 1, polygon.Count);
            for (int i = 0; i<polygon.Count; i++)
            {
                // The three verteces constructing the ear will automatically lay inside it, skip them we're only interested in the other points laying inside
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

        /// <summary>
        /// Given an index, make sure that if it is negative it will start at the end again and if it is bigger than the end start from the beginning again
        /// </summary>
        /// <param name="index">the index</param>
        /// <param name="upperBoundary">the and</param>
        /// <returns>the circulated index</returns>
        public static int CirculateIndex(int index, int upperBoundary)
        {
            return (index < 0 ? upperBoundary + index : index % upperBoundary);
        }

        /// <summary>
        /// Get the lowest vertex
        /// </summary>
        /// <param name="points">the list of points</param>
        /// <returns>the point that's lowest</returns>
        public static PointF GetLowestVertex(List<PointF> points)
        {
            return points.OrderBy(point => point.Y).First();
        }

        /// <summary>
        /// Opposite of GetLowestVertex, this finds the highest vertex
        /// </summary>
        /// <param name="points">the list of points</param>
        /// <returns>the point that's highest</returns>
        /// <see cref="GetLowestVertex(List{PointF})"/>
        private static PointF GetHighestVertex(List<PointF> points)
        {
            return points.OrderByDescending(point => point.Y).First();
        }

        /// <summary>
        /// Find the index of the lowest point
        /// </summary>
        /// <param name="points">a list of points</param>
        /// <returns>the index of the lowest point</returns>
        private static int GetLowestVertexIndex(List<PointF> points)
        {
            return points.IndexOf(GetLowestVertex(points));
        }

        /// <summary>
        /// Find the index of the highest vertex index
        /// </summary>
        /// <param name="points">the list of points</param>
        /// <returns>the index of the loweest point</returns>
        private static int GetHighestVertexIndex(List<PointF> points)
        {
            return points.IndexOf(GetHighestVertex(points));
        }

        /// <summary>
        /// Calculate the t value this is used in checking if a triangle is oriented clockwise or counter-clockwise
        /// </summary>
        /// <param name="p1">point one of the triangle</param>
        /// <param name="p2">middle point of the triangle</param>
        /// <param name="p3">last point of the triangle</param>
        /// <returns>the t value</returns>
        private static float CalculateTvalue(PointF p1, PointF p2, PointF p3)
        {
            var t = p1.X * (p2.Y - p3.Y) + p2.X * (p3.Y - p1.Y) + p3.X * (p1.Y - p2.Y);
            return t;
        }

        /// <summary>
        /// Calculates the sign of a float
        /// </summary>
        /// <param name="t">the float</param>
        /// <returns></returns>
        private static int Sign(float t)
        {
            return Math.Sign(t);
        }

        /// <summary>
        /// For t value to tell if a triangle is oriented clockwise or counter-clockwise only the sign is needed
        /// </summary>
        /// <param name="p1">point one of the triangle</param>
        /// <param name="p2">middle point of the triangle</param>
        /// <param name="p3">last point of the triangle</param>
        /// <returns>the sign of the t value</returns>
        private static int CalculateTvalueSign(PointF p1, PointF p2, PointF p3)
        {
            return Sign(CalculateTvalue(p1, p2, p3));
        }
    }
}
