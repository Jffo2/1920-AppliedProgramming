using System.Collections.Generic;
using ImageProcessing.Models;
using System.Threading.Tasks;
using System.Drawing;

namespace ImageProcessing.Logic
{
    public class Histogram
    {
        public Dictionary<System.Drawing.Color, int> ColorCount { get; }

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
