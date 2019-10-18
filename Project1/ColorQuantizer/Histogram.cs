using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;

namespace ColorQuantizer
{
    public class Histogram
    {
        public Dictionary<Color, int> ColorCount { get; }

        public Histogram(Dictionary<Color, int> colorCount)
        {
            ColorCount = colorCount;
        }

        public override string ToString()
        {
            string s = "";
            foreach (KeyValuePair<Color, int> keyValuePair in ColorCount)
            {
                s += keyValuePair.Key.ToString() + ": " + keyValuePair.Value + "\r\n";
            }
            return s;
        }

        public Bitmap Visualize(int height, int width)
        {
            Bitmap bmp = new Bitmap(width, height);
            int padding = 10;
            var histogramSize = (height - padding*3) / 3;
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawLine(new Pen(new SolidBrush(System.Drawing.Color.Black)), padding, padding + histogramSize, padding, padding);
                g.DrawLine(new Pen(new SolidBrush(System.Drawing.Color.Black)), padding, padding + histogramSize, width-padding, padding + histogramSize);

                g.DrawLine(new Pen(new SolidBrush(System.Drawing.Color.Black)), padding, (padding + histogramSize)*2, padding, padding*2+histogramSize);
                g.DrawLine(new Pen(new SolidBrush(System.Drawing.Color.Black)), padding, (padding + histogramSize)*2, width - padding, (padding + histogramSize)*2);

                g.DrawLine(new Pen(new SolidBrush(System.Drawing.Color.Black)), padding, (padding + histogramSize)*3, padding, padding*3+2*histogramSize);
                g.DrawLine(new Pen(new SolidBrush(System.Drawing.Color.Black)), padding, (padding + histogramSize)*3, width - padding, (padding + histogramSize)*3);

            }
            return bmp;
        }

        public Task<Bitmap> VisualizeAsync(int height, int width)
        {
            return Task.Run(() =>
            {
                return Visualize(height, width);
            });
        }

    }
}
