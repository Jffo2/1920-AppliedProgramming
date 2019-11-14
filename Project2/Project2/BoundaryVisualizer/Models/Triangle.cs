using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace BoundaryVisualizer.Models
{
    public class Triangle
    {
        public PointF Point1 { get; set; }

        public PointF MiddlePoint { get; set; }

        public PointF Point2 { get; set; }

        public double Angle { get; private set; }

        public Triangle(PointF point1, PointF middlePoint, PointF point2)
        {
            Point1 = point1;
            MiddlePoint = middlePoint;
            Point2 = point2;
            CalculateAngle();
        }

        public double CalculateAngle()
        {
            PointF vector1 = new PointF(Point1.X - MiddlePoint.X, Point1.Y - MiddlePoint.Y);
            PointF vector2 = new PointF(Point2.X - MiddlePoint.X, Point2.Y - MiddlePoint.Y);
            double num = (vector1.X * vector2.X + vector1.Y * vector2.Y);
            double den = Math.Sqrt(Square(vector1.X) + Square(vector1.Y)) * Math.Sqrt(Square(vector2.X) + Square(vector2.Y));
            // TODO: Acos always returns the convexe angle, we have to find a way to know if our points are making a convexe angle or not tho
            Angle = Math.Acos(num / den);
            return Angle;
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
