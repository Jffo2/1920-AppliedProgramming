using ImageProcessing.Logic;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ImageProcessing.Models;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using ImageProcessing.Util;

namespace ImageProcessing.Presentation
{
    public abstract class Drawer : IDrawer
    {
        /// <summary>
        /// A field containing the Average euclidean distance between an original pixel and it's quantized counterpart
        /// </summary>
        public double AverageError { get; protected set; }

        /// <summary>
        /// An event that fires when a progress update is available
        /// </summary>
        public abstract event EventHandler<ProgressEventArgs> ProgressUpdate;

        /// <summary>
        /// The ImageStore containing all info to draw/quantize/dither
        /// </summary>
        protected readonly ImageStore imageStore;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="imageStore">the ImageStore containing all info about the image to be drawn</param>
        public Drawer(ImageStore imageStore)
        {
            this.imageStore = imageStore;
        }

       
        public abstract Bitmap Draw();
        public abstract Task<Bitmap> DrawAsync();

        /// <summary>
        /// Visualize the palette
        /// </summary>
        /// <param name="height">height of the palette to be drawn in pixels</param>
        /// <param name="width">width of the palette to be drawn in pixels</param>
        /// <returns>a bitmap visualizing the palette</returns>
        public Bitmap VisualizePallet(int height, int width)
        {
            if (!imageStore.QuantizerReady) throw new Exception("Quantizer not ready!");

            Bitmap bmp = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                var pallet = Cloner.DeepClone(imageStore.Quantizer.GetPalette());
                int maxRows = System.Math.Max(pallet.Count / 16,1);
                int maxColumns = System.Math.Min(pallet.Count, 16);
                int xDelta = width / maxColumns;
                int yDelta = height / maxRows;

                for (int row = 0; row < maxRows; row++)
                {
                    for (int column = 0; column < maxColumns; column++)
                    {
                        Models.Color c = pallet[row * 16 + column];
                        System.Drawing.Color color = System.Drawing.Color.FromArgb(255,c.Channel1, c.Channel2, c.Channel3);
                        g.FillRectangle(new SolidBrush(color), column * xDelta, row * yDelta, (column + 1) * xDelta, (row + 1) * yDelta);
                    }
                }
            }

            return bmp;
        }

        /// <summary>
        /// Start palette visualization on a different thread
        /// </summary>
        /// <param name="height">height of the palette to be drawn in pixels</param>
        /// <param name="width">width of the palette to be drawn in pixels</param>
        /// <returns></returns>
        public Task<Bitmap> VisualizePalletAsync(int height, int width)
        {
            return Task.Run(() =>
            {
                return VisualizePallet(height, width);
            });
        }

        /// <summary>
        /// Generate a bitmap containing a visual representation of the histogram
        /// </summary>
        /// <param name="height">height of the histogram to be drawn in pixels</param>
        /// <param name="width">width of the histogram to be drawn in pixels</param>
        /// <returns>a bitmap visualizing the histogram</returns>
        public Bitmap VisualizeHistogram(int height, int width)
        {
            System.Drawing.Color[] drawingColors = { System.Drawing.Color.Red, System.Drawing.Color.Green, System.Drawing.Color.Blue };
            Bitmap bmp = new Bitmap(width, height);
            int padding = 10;
            var histogramSize = (height - padding * 3) / 3;
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Draw the axis'
                g.DrawLine(new Pen(new SolidBrush(System.Drawing.Color.Black)), padding, padding + histogramSize, padding, padding);
                g.DrawLine(new Pen(new SolidBrush(System.Drawing.Color.Black)), padding, padding + histogramSize, width - padding, padding + histogramSize);

                g.DrawLine(new Pen(new SolidBrush(System.Drawing.Color.Black)), padding, (padding + histogramSize) * 2, padding, padding * 2 + histogramSize);
                g.DrawLine(new Pen(new SolidBrush(System.Drawing.Color.Black)), padding, (padding + histogramSize) * 2, width - padding, (padding + histogramSize) * 2);

                g.DrawLine(new Pen(new SolidBrush(System.Drawing.Color.Black)), padding, (padding + histogramSize) * 3, padding, padding * 3 + 2 * histogramSize);
                g.DrawLine(new Pen(new SolidBrush(System.Drawing.Color.Black)), padding, (padding + histogramSize) * 3, width - padding, (padding + histogramSize) * 3);

                var xdelta = (width - 2 * padding) / (float)255;

                for (int channel = 0; channel < 3; channel++)
                {
                    // Get the histogram for just one channel
                    var colorsOfChannel = GetChannelCount((Channel)channel);

                    float ydelta = histogramSize / (float)Util.Math.Max(colorsOfChannel);

                    // Display a title for the graph
                    g.DrawString("Channel" + (channel + 1), new Font(FontFamily.GenericSansSerif, 8), new SolidBrush(System.Drawing.Color.Black), 2 * padding, (channel + 1) * padding + channel * histogramSize, StringFormat.GenericDefault);

                    // Draw all points
                    for (int xoffset = 0; xoffset < 255; xoffset += 1)
                    {
                        g.DrawLine(new Pen(new SolidBrush(drawingColors[channel])), xoffset * xdelta + padding, (padding + histogramSize) * (channel + 1) - colorsOfChannel[xoffset] * ydelta, (xoffset + 1) * xdelta + padding, (padding + histogramSize) * (channel + 1) - colorsOfChannel[xoffset + 1] * ydelta);
                    }
                }
            }
            return bmp;
        }

        /// <summary>
        /// Calculate the histogram for one channel
        /// </summary>
        /// <param name="channel">the channel for which to calculate</param>
        /// <returns>A list of points where the index is the x coordinate and value is the frequency and thus y value</returns>
        private int[] GetChannelCount(Channel channel)
        {
            int[] count = new int[256];

            foreach (KeyValuePair<System.Drawing.Color, int> keyValue in imageStore.Histogram.ColorCount)
            {
                count[channel == Channel.CHANNEL1 ? (keyValue.Key.R) : (channel == Channel.CHANNEL2) ? keyValue.Key.G : keyValue.Key.B] += keyValue.Value;
            }

            return count;
        }

        /// <summary>
        /// Generate a bitmap containing a visual representation of the histogram async
        /// </summary>
        /// <param name="height">height of the histogram to be drawn in pixels</param>
        /// <param name="width">width of the histogram to be drawn in pixels</param>
        /// <returns>a bitmap visualizing the histogram</returns>
        public Task<Bitmap> VisualizeHistogramAsync(int height, int width)
        {
            return Task.Run(() =>
            {
                return VisualizeHistogram(height, width);
            });
        }

        /// <summary>
        /// Copy the palette generated in the quantizer to a bitmap
        /// </summary>
        /// <param name="b"></param>
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

        /// <summary>
        /// Save the image
        /// </summary>
        /// <param name="b">the bitmap to save</param>
        public void Save(Bitmap b)
        {
            b.Save($"result-{this.ToString()}-{imageStore.Quantizer.ToString()}-{imageStore.Ditherer.ToString()}.gif");
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
