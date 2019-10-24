using ImageProcessing.Logic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ImageProcessing.Models;
using System.Threading.Tasks;
using System.Drawing.Imaging;

namespace ImageProcessing.Presentation
{
    public abstract class Drawer : IDrawer
    {
        public double AverageError { get; protected set; }

        public abstract event EventHandler<ProgressEventArgs> ProgressUpdate;

        protected readonly ImageStore imageStore;
        public Drawer(ImageStore imageStore)
        {
            this.imageStore = imageStore;
        }

        public abstract Bitmap Draw();
        public abstract Task<Bitmap> DrawAsync();

        public Bitmap VisualizeHistogram(int height, int width)
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
                    float ydelta = histogramSize / (float)Util.Math.Max(colorsOfChannel);
                    g.DrawString("Channel" + (channel + 1), new Font(FontFamily.GenericSansSerif, 8), new SolidBrush(System.Drawing.Color.Black), 2 * padding, (channel + 1) * padding + channel * histogramSize, StringFormat.GenericDefault);
                    for (int xoffset = 0; xoffset < 255; xoffset += 1)
                    {
                        g.DrawLine(new Pen(new SolidBrush(System.Drawing.Color.Red)), xoffset * xdelta + padding, (padding + histogramSize) * (channel + 1) - colorsOfChannel[xoffset] * ydelta, (xoffset + 1) * xdelta + padding, (padding + histogramSize) * (channel + 1) - colorsOfChannel[xoffset + 1] * ydelta);
                    }
                }

            }
            return bmp;
        }

        private int[] GetChannelCount(Channel channel)
        {
            int[] count = new int[256];

            foreach (KeyValuePair<System.Drawing.Color, int> keyValue in imageStore.Histogram.ColorCount)
            {
                count[channel == Channel.CHANNEL1 ? (keyValue.Key.R) : (channel == Channel.CHANNEL2) ? keyValue.Key.G : keyValue.Key.B] += keyValue.Value;
            }

            return count;
        }

        public Task<Bitmap> VisualizeHistogramAsync(int height, int width)
        {
            return Task.Run(() =>
            {
                return VisualizeHistogram(height, width);
            });
        }

        public void CopyPalette(Bitmap b)
        {
            List<Models.Color> palette = imageStore.Quantizer.GetPalette();
            ColorPalette newPalette = b.Palette;
            for (int i = 0; i < palette.Count; i++)
            {
                System.Diagnostics.Debug.WriteLine($"Adding {palette[i]} to the palette as color {i}");
                newPalette.Entries[i] = System.Drawing.Color.FromArgb(255, palette[i].Channel1, palette[i].Channel2, palette[i].Channel3);
            }
            b.Palette = newPalette;
        }

    }
    public class ProgressEventArgs : EventArgs
    {
        public int Progress { get; }

        public ProgressEventArgs(int progress)
        {
            Progress = progress;
        }
    }
}
