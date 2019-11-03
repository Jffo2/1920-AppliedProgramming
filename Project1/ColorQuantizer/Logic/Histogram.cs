using System.Collections.Generic;
using ImageProcessing.Models;
using System.Threading.Tasks;
using System.Drawing;

namespace ImageProcessing.Logic
{
    public class Histogram
    {
        /// <summary>
        /// The actual histogram
        /// </summary>
        public Dictionary<System.Drawing.Color, int> ColorCount { get; }

        /// <summary>
        /// Constructor, assign the histogram
        /// </summary>
        /// <param name="colorCount">the histogram</param>
        public Histogram(Dictionary<System.Drawing.Color, int> colorCount)
        {
            ColorCount = colorCount;
        }

        public override string ToString()
        {
            string s = "";
            foreach (KeyValuePair<System.Drawing.Color, int> keyValuePair in ColorCount)
            {
                s += keyValuePair.Key.ToString() + ": " + keyValuePair.Value + "\r\n";
            }
            return s;
        }

    }

    
}
