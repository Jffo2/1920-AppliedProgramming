using BoundaryVisualizer.Models;
using System.Collections.Generic;
using System.Drawing;

namespace BoundaryVisualizer.Logic
{
    /// <summary>
    /// https://www.researchgate.net/publication/311245887_Accurate_simple_and_efficient_triangulation_of_a_polygon_by_ear_removal_with_lowest_memory_consumption
    /// </summary>
    public class CustomTriangulator
    {
        public static List<Triangle> Triangulate(List<PointF> points)
        {
            var triangles = new List<Triangle>();

            return triangles;
        }

        public int IsCounterClockwise(PointF p1, PointF p2, PointF p3)
        {
            var t = p1.X * (p2.Y - p3.Y) + p2.X * (p3.Y - p1.Y) + p3.X * (p1.Y - p2.Y);
            return t > 0? 1 : -1;
        }
    }
}
