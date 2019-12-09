using System;
using System.Drawing;

namespace BoundaryVisualizer.Models
{

    public struct Triangle
    {
        public PointF Point1 { get; set; }

        public PointF MiddlePoint { get; set; }

        public PointF Point2 { get; set; }

        public Triangle(PointF point1, PointF middlePoint, PointF point2)
        {
            Point1 = point1;
            MiddlePoint = middlePoint;
            Point2 = point2;
        }

        public override string ToString()
        {
            return $"{{{Point1},{MiddlePoint},{Point2}}}";
        }
    }
}
