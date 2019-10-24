using System.Collections.Generic;
using ImageProcessing.Models;
using System.Threading.Tasks;
using System.Drawing;

namespace ImageProcessing.Logic
{
    public class Histogram
    {
        public Dictionary<Models.Color, int> ColorCount { get; }

        public Histogram(Dictionary<Models.Color, int> colorCount)
        {
            ColorCount = colorCount;
        }

        public override string ToString()
        {
            string s = "";
            foreach (KeyValuePair<Models.Color, int> keyValuePair in ColorCount)
            {
                s += keyValuePair.Key.ToString() + ": " + keyValuePair.Value + "\r\n";
            }
            return s;
        }

    }

    
}
