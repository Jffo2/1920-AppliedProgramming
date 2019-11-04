using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessing.Util
{
    public static class Math
    {
        /// <summary>
        /// Calculates the squared euclidean distance between two colors
        /// </summary>
        /// <param name="color">the first color</param>
        /// <param name="other">the second color</param>
        /// <returns>an integer representing the distance in euclidean space</returns>
        public static int Distance(Models.Color color, Models.Color other)
        {
            return (color.Channel1 - other.Channel1) * (color.Channel1 - other.Channel1) + (color.Channel2 - other.Channel2) * (color.Channel2 - other.Channel2) + (color.Channel3 - other.Channel3) * (color.Channel3 - other.Channel3);
        }

        /// <summary>
        /// Finds the greatest value in a list of integers
        /// </summary>
        /// <param name="values">the list of values</param>
        /// <returns>the largest value in the list of values</returns>
        public static int Max(int[] values)
        {
            int max = 0;
            foreach (int value in values)
            {
                if (value > max) max = value;
            }
            return max;
        }
    }
}
