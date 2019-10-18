using System;
using System.Collections.Generic;
using System.Text;

namespace ColorQuantizer
{
    public class Color : IEquatable<Color>
    {
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }

        public Color() { }
        public Color(int red, int green, int blue)
        {
            Red = red;
            Green = green;
            Blue = blue;
        }

        public override string ToString()
        {
            return $"({Red}, {Green}, {Blue})";
        }

        public bool Equals(Color other)
        {
            return other != null &&
                   Red == other.Red &&
                   Green == other.Green &&
                   Blue == other.Blue;
        }

        public override int GetHashCode()
        {
            var hashCode = -1058441243;
            hashCode = hashCode * -1521134295 + Red.GetHashCode();
            hashCode = hashCode * -1521134295 + Green.GetHashCode();
            hashCode = hashCode * -1521134295 + Blue.GetHashCode();
            return hashCode;
        }
    }
}
