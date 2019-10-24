using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessing.Models
{
    public class Color : IEquatable<Color>
    {
        public byte Channel1 { get; set; }
        public byte Channel2 { get; set; }
        public byte Channel3 { get; set; }

        public Color() { }
        public Color(byte channel1, byte channel2, byte channel3)
        {
            Channel1 = channel1;
            Channel2 = channel2;
            Channel3 = channel3;
        }

        public Color(System.Drawing.Color c)
        {
            Channel1 = c.R;
            Channel2 = c.G;
            Channel3 = c.B;
        }

        public override string ToString()
        {
            return $"({Channel1}, {Channel2}, {Channel3})";
        }

        public bool Equals(Color other)
        {
            return other != null &&
                   Channel1 == other.Channel1 &&
                   Channel2 == other.Channel2 &&
                   Channel3 == other.Channel3;
        }

        public override int GetHashCode()
        {
            var hashCode = -1058441243;
            hashCode = hashCode * -1521134295 + Channel1.GetHashCode();
            hashCode = hashCode * -1521134295 + Channel2.GetHashCode();
            hashCode = hashCode * -1521134295 + Channel3.GetHashCode();
            return hashCode;
        }
    }

    public enum Channel
    {
        CHANNEL1,
        CHANNEL2,
        CHANNEL3
    }
}
