using System.Collections.Generic;
using System.Drawing;
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
            var histogramSize = (height - padding * 3) / 3;
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawLine(new Pen(new SolidBrush(System.Drawing.Color.Black)), padding, padding + histogramSize, padding, padding);
                g.DrawLine(new Pen(new SolidBrush(System.Drawing.Color.Black)), padding, padding + histogramSize, width - padding, padding + histogramSize);

                g.DrawLine(new Pen(new SolidBrush(System.Drawing.Color.Black)), padding, (padding + histogramSize) * 2, padding, padding * 2 + histogramSize);
                g.DrawLine(new Pen(new SolidBrush(System.Drawing.Color.Black)), padding, (padding + histogramSize) * 2, width - padding, (padding + histogramSize) * 2);

                g.DrawLine(new Pen(new SolidBrush(System.Drawing.Color.Black)), padding, (padding + histogramSize) * 3, padding, padding * 3 + 2 * histogramSize);
                g.DrawLine(new Pen(new SolidBrush(System.Drawing.Color.Black)), padding, (padding + histogramSize) * 3, width - padding, (padding + histogramSize) * 3);

                var xdelta = (width - 2 * padding) / (float)255;

                for (int channel = 0; channel <= 3; channel++)
                {
                    var colorsOfChannel = GetChannelCount((Channel)channel);
                    float ydelta = histogramSize / (float)Max(colorsOfChannel);
                    for (int xoffset = 0; xoffset < 255; xoffset += 1)
                    {
                        g.DrawLine(new Pen(new SolidBrush(System.Drawing.Color.Red)), xoffset * xdelta + padding, (padding + histogramSize) * (channel + 1) - colorsOfChannel[xoffset] * ydelta, (xoffset + 1) * xdelta + padding, (padding + histogramSize) * (channel + 1) - colorsOfChannel[xoffset + 1] * ydelta);
                    }
                }

            }
            return bmp;
        }

        private int Max(int[] values)
        {
            int max = 0;
            foreach (int value in values)
            {
                if (value > max) max = value;
            }
            return max;
        }

        public Task<Bitmap> VisualizeAsync(int height, int width)
        {
            return Task.Run(() =>
            {
                return Visualize(height, width);
            });
        }

        public int[] GetChannelCount(Channel channel)
        {
            int[] count = new int[256];

            foreach (KeyValuePair<Color, int> keyValue in ColorCount)
            {
                count[channel == Channel.CHANNEL1 ? (keyValue.Key.Channel1) : (channel == Channel.CHANNEL2) ? keyValue.Key.Channel2 : keyValue.Key.Channel3] += keyValue.Value;
            }

            return count;
        }

    }

    public enum Channel
    {
        CHANNEL1,
        CHANNEL2,
        CHANNEL3
    }
}
