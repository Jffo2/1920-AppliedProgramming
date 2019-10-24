using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessing.Util
{
    public class Math
    {
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
