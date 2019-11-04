using System;

namespace ImageProcessing.Models
{
    [Serializable]
    public class Color : IEquatable<Color>
    {
        /// <summary>
        /// First channel of the color, usually Red
        /// </summary>
        public int Channel1 { get; set; }
        /// <summary>
        /// Second channel of the color, usually Green
        /// </summary>
        public int Channel2 { get; set; }
        /// <summary>
        /// Third channel of the color, usually Blue
        /// </summary>
        public int Channel3 { get; set; }

        public Color() { }
        /// <summary>
        /// Constructor that initializes the color
        /// </summary>
        /// <param name="channel1">first channel, usually Red, between -255 and 255</param>
        /// <param name="channel2">second channel, usually Green, between -255 and 255</param>
        /// <param name="channel3">third channel, usually Blue, between -255 and 255</param>
        public Color(int channel1, int channel2, int channel3)
        {
            if (channel1 > 255 || channel2 > 255 || channel3 > 255 || channel1 < -255 || channel2 < -255 || channel3 < -255) throw new ArgumentException("Invalid range");
            Channel1 = channel1;
            Channel2 = channel2;
            Channel3 = channel3;
        }

        /// <summary>
        /// Constructorthat initializes the color from a System.Drawing.Color object
        /// </summary>
        /// <param name="c">the System.Drawing.Color object</param>
        public Color(System.Drawing.Color c)
        {
            Channel1 = c.R;
            Channel2 = c.G;
            Channel3 = c.B;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="c"></param>
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

        /// <summary>
        /// Addition for colors
        /// </summary>
        /// <param name="c">the first color</param>
        /// <param name="d">the second color</param>
        /// <returns>a new color where respectively R, G and B have been added to the R, G and B values of the other color</returns>
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

        /// <summary>
        /// Adds a scalar to a color
        /// </summary>
        /// <param name="c">the color</param>
        /// <param name="s">the scalar</param>
        /// <returns>returns a new color where each R, G and B have been increased by the scalar</returns>
        public static Color operator +(Color c, int s)
        {
            return c + new Color(s, s, s);
        }

        /// <summary>
        /// Subtraction for colors
        /// </summary>
        /// <param name="c">first color</param>
        /// <param name="d">second color</param>
        /// <returns>an array of 3 integers where the elements represent respectively R, G and B</returns>
        public static int[] operator -(Color c, Color d)
        {
            return new int[] { c.Channel1 - d.Channel1, c.Channel2 - d.Channel2, c.Channel3 - d.Channel3 };
        }
    }

    public enum Channel
    {
        CHANNEL1,
        CHANNEL2,
        CHANNEL3
    }
}
