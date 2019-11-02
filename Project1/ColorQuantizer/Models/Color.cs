using System;

namespace ImageProcessing.Models
{
    [Serializable]
    public class Color : IEquatable<Color>
    {
        public int Channel1 { get; set; }
        public int Channel2 { get; set; }
        public int Channel3 { get; set; }

        public Color() { }
        public Color(int channel1, int channel2, int channel3)
        {
            if (channel1 > 255 || channel2 > 255 || channel3 > 255 || channel1 < -255 || channel2 < -255 || channel3 < -255) throw new ArgumentException("Invalid range");
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
        public Color(Models.Color c)
        {
            Channel1 = c.Channel1;
            Channel2 = c.Channel2;
            Channel3 = c.Channel3;
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

        public static Color operator +(Color c, Color d)
        {
            if (c == null && d == null) return new Color(0, 0, 0);
            if (c == null) return new Color(d);
            if (d == null) return new Color(c);
            var sum1 = c.Channel1 + d.Channel1;
            var sum2 = c.Channel2 + d.Channel2;
            var sum3 = c.Channel3 + d.Channel3;
            return new Color((sum1 < -255) ? -255 : (sum1 > 255) ? 255 : sum1, (sum2 < -255) ? -255 : (sum2 > 255) ? 255 : sum2, (sum3 < -255) ? -255 : (sum3 > 255) ? 255 : sum3);
        }
        public static Color operator +(Color c, int s)
        {
            return c + new Color(s, s, s);
        }

        public static int[] operator -(Color c, Color d)
        {
            return new int[] { c.Channel1 - d.Channel1, c.Channel2 - d.Channel2, c.Channel3 - d.Channel3 };
        }

        public static Color operator *(Color c, int s)
        {
            return new Color(c.Channel1 * s, c.Channel2 * s, c.Channel3 * s);
        }
    }

    public enum Channel
    {
        CHANNEL1,
        CHANNEL2,
        CHANNEL3
    }
}
