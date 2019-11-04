using ImageProcessing.Logic;
using ImageProcessing.Logic.Ditherers;
using ImageProcessing.Util;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Reflection;
using System.Linq;

namespace ImageProcessing.Presentation
{
    public class SynchronousDitheredDrawer : Drawer
    {
        public override event EventHandler<ProgressEventArgs> ProgressUpdate;

        private double TotalError;

        public SynchronousDitheredDrawer(ImageStore imageStore) : base(imageStore)
        {

        }

        /// <summary>
        /// Draws a bitmap while applying dithering and/or quantization
        /// </summary>
        /// <returns>the altered bitmap</returns>
        public override Bitmap Draw()
        {
            if (!imageStore.QuantizerReady)
            {
                throw new Exception("The Quantizer was not ready yet.");
            }
            TotalError = 0;
            Bitmap bitmap;
            IDitherer ditherer = imageStore.Ditherer;
            lock (imageStore.Image)
            {
                bitmap = Cloner.DeepClone(imageStore.Image);
            }
            Rectangle bounds = Rectangle.FromLTRB(0, 0, bitmap.Width, bitmap.Height);

            // Lock source bits for reading
            BitmapData sourceData = bitmap.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            // Lock target bits for writing
            Bitmap canvas = new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format8bppIndexed);
            BitmapData targetData = canvas.LockBits(bounds, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            // Copy the palette to the target bitmap
            CopyPalette(canvas);

            try
            {
                int width = bitmap.Width;
                int height = bitmap.Height;
                long total = bitmap.Width * bitmap.Height;
                Models.Color[] ditherDistortion = new Models.Color[total];

                LoopRows(width, height, sourceData, targetData, ditherDistortion, ditherer);
                
                AverageError = TotalError / total;
            }
            
            finally
            {
                bitmap.UnlockBits(sourceData);
                canvas.UnlockBits(targetData);
            }

            canvas.Save($"result{imageStore.Quantizer.ToString()}.gif");
            return canvas;
        }

        /// <summary>
        /// Draw bitmap async
        /// </summary>
        /// <returns>The drawn bitmap as a task result</returns>
        public override Task<Bitmap> DrawAsync()
        {
            return Task.Run(() => { return Draw(); });
        }

        /// <summary>
        /// Loop over all bitmap rows and replace them with a palette color
        /// </summary>
        /// <param name="width">the width of the bitmap</param>
        /// <param name="height">the height of the bitmap</param>
        /// <param name="sourceData">the source bitmap data</param>
        /// <param name="targetData">the target bitmap data</param>
        /// <param name="ditherDistortion">the dither distortion overlay</param>
        /// <param name="ditherer">the ditherer to use</param>
        private void LoopRows(int width, int height, BitmapData sourceData, BitmapData targetData, Models.Color[] ditherDistortion, IDitherer ditherer)
        {
            int completed = 0;
            long sourceOffset = sourceData.Scan0.ToInt64();
            long targetOffset = targetData.Scan0.ToInt64();
            for (int row = 0; row < height; row++)
            {
                // Re initialize the source and target rows
                byte[] targetLine = new byte[width];
                int[] sourceLine = new int[width];

                Marshal.Copy(new IntPtr(sourceOffset), sourceLine, 0, width);

                GenerateLine(width, height, row, ditherDistortion, sourceLine, targetLine, ditherer);

                Marshal.Copy(targetLine, 0, new IntPtr(targetOffset), width);

                sourceOffset += sourceData.Stride;
                targetOffset += targetData.Stride;

                // Update progress
                completed++;
                ProgressUpdate?.Invoke(this, new ProgressEventArgs((int)(completed / (float)height * 100)));
            }
        }

        /// <summary>
        /// Process a line from the bitmap image
        /// </summary>
        /// <param name="width">the width of the image</param>
        /// <param name="height">the height of the image</param>
        /// <param name="row">the current row being processed</param>
        /// <param name="ditherDistortion">the dither distortion overlay</param>
        /// <param name="sourceLine">the source row of pixels</param>
        /// <param name="targetLine">the target row of pixels</param>
        /// <param name="ditherer">the ditherer to use</param>
        private void GenerateLine(int width, int height, int row, Models.Color[] ditherDistortion, int[] sourceLine, byte[] targetLine, IDitherer ditherer)
        {
            for (int index = 0; index < width; index++)
            {
                // Read the color from the source image
                Color color = Color.FromArgb(sourceLine[index]);
                // Add the dithering to the pixel
                var colorAsColor = new Models.Color(color) + ditherDistortion[index + row * width];
                // Get the index of the color closest to the dithered pixel
                targetLine[index] = (byte)imageStore.Quantizer.GetPaletteIndex(colorAsColor);
                // Get the distance to dither the other pixels!
                var distance = System.Math.Sqrt(Util.Math.Distance(new Models.Color(color), imageStore.Quantizer.GetColorByIndex(targetLine[index])));
                ditherer.Dither(colorAsColor, imageStore.Quantizer.GetColorByIndex(targetLine[index]), ditherDistortion, index, row, width, height);
                TotalError += distance;
            }
        }
    }
}
