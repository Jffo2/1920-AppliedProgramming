using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BoundaryVisualizer.Models
{
    public enum TriangleOrientation
    {
        CLOCKWISE,
        COLINEAR,
        COUNTER_CLOCKWISE
    }

    public class Triangle
    {
        public PointF Point1 { get; set; }

        public PointF MiddlePoint { get; set; }

        public PointF Point2 { get; set; }

        public double Angle
        {
            get
            {
                return CalculateAngle();
            }
        }
        public TriangleOrientation Orientation
        {
            get
            {
                float val = (MiddlePoint.Y - Point1.Y) * (Point2.X - MiddlePoint.X) - (MiddlePoint.X - Point1.X) * (Point2.Y - MiddlePoint.Y);

                if (val == 0) return TriangleOrientation.COLINEAR; // colinear 

                // clock or counterclock wise 
                return (val > 0) ?  TriangleOrientation.CLOCKWISE : TriangleOrientation.COUNTER_CLOCKWISE;
            }
        }

        public Triangle(PointF point1, PointF middlePoint, PointF point2)
        {
            Point1 = point1;
            MiddlePoint = middlePoint;
            Point2 = point2;
        }

        public double CalculateAngle()
        {
            if (Orientation == TriangleOrientation.COLINEAR) return Math.PI;
            PointF vector1 = new PointF(MiddlePoint.X - Point1.X , MiddlePoint.Y - Point1.Y);
            PointF vector2 = new PointF(Point2.X - MiddlePoint.X, Point2.Y - MiddlePoint.Y);
            double num = (vector1.X * vector2.X + vector1.Y * vector2.Y);
            double den = Math.Sqrt(Square(vector1.X) + Square(vector1.Y)) * Math.Sqrt(Square(vector2.X) + Square(vector2.Y));
            // TODO: Acos always returns the convexe angle, we have to find a way to know if our points are making a convexe angle or not tho
            var convexAngle = Math.Acos(num / den);
            
            return (Orientation == TriangleOrientation.CLOCKWISE) ? 2 * Math.PI - convexAngle : convexAngle;
        }

        private double Square(double a)
        {
            return a * a;
        }

        public override string ToString()
        {
            return $"{{{Point1},{MiddlePoint},{Point2}}}";
        }
    }
}
