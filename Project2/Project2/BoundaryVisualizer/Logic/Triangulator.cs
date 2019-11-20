using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BoundaryVisualizer.Logic
{
    class Triangulator
    {
        /// <summary>
        /// Calculate list of convex polygons or triangles.
        /// </summary>
        /// <param name="Polygon">Input polygon without self-intersections (it can be checked with SelfIntersection().</param>
        /// <returns></returns>
        public static List<List<PointF>> Triangulate(List<PointF> Polygon)
        {
            var result = new List<List<PointF>>();
            var tempPolygon = new List<PointF>(Polygon);
            List<PointF> convPolygon;

            int begin_ind = 0;
            int cur_ind;
            int N = Polygon.Count;
            int Range;

            // If the polygon is clockwise, reverse it so it's ordered counter clockwise
            if (Square(tempPolygon) < 0)
                tempPolygon.Reverse();

            while (N >= 3)
            {
                int tries = 0;
                convPolygon = new List<PointF>();
                // Search for a triangle that's not concave and does not intersect
                while ((PMSquare(tempPolygon[begin_ind], tempPolygon[(begin_ind + 1) % N],
                          tempPolygon[(begin_ind + 2) % N]) < 0) ||
                          (Intersect(tempPolygon, begin_ind, (begin_ind + 1) % N, (begin_ind + 2) % N) == true))
                {
                    begin_ind++;
                    if (tries == 25) return result;
                    if (begin_ind == N) tries++;
                    begin_ind %= N;
                }
                cur_ind = (begin_ind + 1) % N;
                convPolygon.Add(tempPolygon[begin_ind]);
                convPolygon.Add(tempPolygon[cur_ind]);
                convPolygon.Add(tempPolygon[(begin_ind + 2) % N]);

                Range = cur_ind - begin_ind;
                if (Range > 0)
                {
                    tempPolygon.RemoveRange(begin_ind + 1, Range);
                }
                else
                {
                    tempPolygon.RemoveRange(begin_ind + 1, N - begin_ind - 1);
                    tempPolygon.RemoveRange(0, cur_ind + 1);
                }
                N = tempPolygon.Count;
                begin_ind++;
                begin_ind %= N;

                result.Add(convPolygon);
            }

            return result;
        }

        /// <summary>
        /// Check if a polygon intersects with itself
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static int SelfIntersection(List<PointF> polygon)
        {
            if (polygon.Count < 3)
                return 0;
            int High = polygon.Count - 1;
            PointF O = new PointF();
            int i;
            for (i = 0; i < High; i++)
            {
                for (int j = i + 2; j < High; j++)
                {
                    if (LineIntersect(polygon[i], polygon[i + 1],
                                      polygon[j], polygon[j + 1], ref O) == 1)
                        return 1;
                }
            }
            for (i = 1; i < High - 1; i++)
                if (LineIntersect(polygon[i], polygon[i + 1], polygon[High], polygon[0], ref O) == 1)
                    return 1;
            return -1;
        }

        /// <summary>
        /// Check if polygon is clockwise oriented or counter clockwise
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        public static float Square(List<PointF> polygon)
        {
            float S = 0;
            if (polygon.Count >= 3)
            {
                for (int i = 0; i < polygon.Count - 1; i++)
                    S += PMSquare((PointF)polygon[i], (PointF)polygon[i + 1]);
                S += PMSquare((PointF)polygon[polygon.Count - 1], (PointF)polygon[0]);
            }
            return S;
        }

        public int IsConvex(List<PointF> Polygon)
        {
            if (Polygon.Count >= 3)
            {
                if (Square(Polygon) > 0)
                {
                    for (int i = 0; i < Polygon.Count - 2; i++)
                        if (PMSquare(Polygon[i], Polygon[i + 1], Polygon[i + 2]) < 0)
                            return -1;
                    if (PMSquare(Polygon[Polygon.Count - 2], Polygon[Polygon.Count - 1], Polygon[0]) < 0)
                        return -1;
                    if (PMSquare(Polygon[Polygon.Count - 1], Polygon[0], Polygon[1]) < 0)
                        return -1;
                }
                else
                {
                    for (int i = 0; i < Polygon.Count - 2; i++)
                        if (PMSquare(Polygon[i], Polygon[i + 1], Polygon[i + 2]) > 0)
                            return -1;
                    if (PMSquare(Polygon[Polygon.Count - 2], Polygon[Polygon.Count - 1], Polygon[0]) > 0)
                        return -1;
                    if (PMSquare(Polygon[Polygon.Count - 1], Polygon[0], Polygon[1]) > 0)
                        return -1;
                }
                return 1;
            }
            return 0;
        }

        /// <summary>
        /// Check if the triangle doesn't intersect the polygon
        /// https://algs4.cs.princeton.edu/91primitives/
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="vertex1Ind"></param>
        /// <param name="vertex2Ind"></param>
        /// <param name="vertex3Ind"></param>
        /// <returns></returns>
        static bool Intersect(List<PointF> polygon, int vertex1Ind, int vertex2Ind, int vertex3Ind)
        {
            float s1, s2, s3;
            for (int i = 0; i < polygon.Count; i++)
            {
                if ((i == vertex1Ind) || (i == vertex2Ind) || (i == vertex3Ind))
                    continue;
                s1 = PMSquare(polygon[vertex1Ind], polygon[vertex2Ind], polygon[i]);
                s2 = PMSquare(polygon[vertex2Ind], polygon[vertex3Ind], polygon[i]);
                if (((s1 < 0) && (s2 > 0)) || ((s1 > 0) && (s2 < 0)))
                    continue;
                s3 = PMSquare(polygon[vertex3Ind], polygon[vertex1Ind], polygon[i]);
                if (((s3 >= 0) && (s2 >= 0)) || ((s3 <= 0) && (s2 <= 0)))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check if two points are clockwise oriented or counter clockwise
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        static float PMSquare(PointF p1, PointF p2)
        {
            return (p2.X * p1.Y - p1.X * p2.Y);
        }

        /// <summary>
        /// Check if three points are clockwise oriented or counter clockwise
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        static float PMSquare(PointF p1, PointF p2, PointF p3)
        {
            //return -1*((p2.Y - p1.Y) * (p3.X - p2.X) - (p2.X - p1.X) * (p3.Y - p2.Y));
            //return (b.X - a.X) * (c.Y - a.Y) - (c.X - a.X) * (b.Y - a.Y);
            var result = (p3.X - p1.X) * (p2.Y - p1.Y) - (p2.X - p1.X) * (p3.Y - p1.Y);
            return result;
        }

        /// <summary>
        /// Check if the lines between two points intersect and find the intersection point in O
        /// </summary>
        /// <param name="A1"></param>
        /// <param name="A2"></param>
        /// <param name="B1"></param>
        /// <param name="B2"></param>
        /// <param name="O"></param>
        /// <returns></returns>
        static int LineIntersect(PointF A1, PointF A2, PointF B1, PointF B2, ref PointF O)
        {
            float a1 = A2.Y - A1.Y;
            float b1 = A1.X - A2.X;
            float d1 = -a1 * A1.X - b1 * A1.Y;
            float a2 = B2.Y - B1.Y;
            float b2 = B1.X - B2.X;
            float d2 = -a2 * B1.X - b2 * B1.Y;
            float t = a2 * b1 - a1 * b2;

            if (t == 0)
                return -1;

            O.Y = (a1 * d2 - a2 * d1) / t;
            O.X = (b2 * d1 - b1 * d2) / t;

            //If the point is to the right of the rightmost point or to the left of the leftmost point it can't intersect
            if (A1.X > A2.X)
            {
                if ((O.X < A2.X) || (O.X > A1.X))
                    return 0;
            }
            else
            {
                if ((O.X < A1.X) || (O.X > A2.X))
                    return 0;
            }

            // If the point is above the upmost point or below the lowest point it can't intersect
            if (A1.Y > A2.Y)
            {
                if ((O.Y < A2.Y) || (O.Y > A1.Y))
                    return 0;
            }
            else
            {
                if ((O.Y < A1.Y) || (O.Y > A2.Y))
                    return 0;
            }

            //If the point is to the right of the rightmost point or to the left of the leftmost point it can't intersect
            if (B1.X > B2.X)
            {
                if ((O.X < B2.X) || (O.X > B1.X))
                    return 0;
            }
            else
            {
                if ((O.X < B1.X) || (O.X > B2.X))
                    return 0;
            }

            // If the point is above the upmost point or below the lowest point it can't intersect
            if (B1.Y > B2.Y)
            {
                if ((O.Y < B2.Y) || (O.Y > B1.Y))
                    return 0;
            }
            else
            {
                if ((O.Y < B1.Y) || (O.Y > B2.Y))
                    return 0;
            }

            // They intersect in O
            return 1;
        }
    }
}
