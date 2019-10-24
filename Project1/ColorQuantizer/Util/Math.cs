using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessing.Util
{
    public static class Math
    {
        public static int Distance(Models.Color color, Models.Color other)
        {
            return (color.Channel1 - other.Channel1) * (color.Channel1 - other.Channel1) + (color.Channel2 - other.Channel2) * (color.Channel2 - other.Channel2) + (color.Channel3 - other.Channel3) * (color.Channel3 - other.Channel3);
        }
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
