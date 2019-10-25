using ImageProcessing.Logic;
using ImageProcessing.Logic.Ditherers;
using ImageProcessing.Util;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ImageProcessing.Presentation
{
    public class SynchronousDitheredDrawer : Drawer
    {
        public override event EventHandler<ProgressEventArgs> ProgressUpdate;

        private double TotalError;

        public SynchronousDitheredDrawer(ImageStore imageStore) : base(imageStore)
        {

        }

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
                long sourceOffset = sourceData.Scan0.ToInt64();
                long targetOffset = targetData.Scan0.ToInt64();
                int width = bitmap.Width;
                int height = bitmap.Height;
                int completed = 0;
                long total = bitmap.Width * bitmap.Height;
                Models.Color[] ditherDistortion = new Models.Color[total];

                for (int row = 0; row < bitmap.Height; row++)
                {
                    // shallow copy memory objects
                    var r = row;
                    var sourceOffsett = sourceOffset;
                    var targetOffsett = targetOffset;

                    // Re initialize the source and target rows
                    byte[] targetLine = new byte[width];
                    int[] sourceLine = new int[width];

                    Marshal.Copy(new IntPtr(sourceOffsett), sourceLine, 0, width);

                    for (int index = 0; index < width; index++)
                    {
                        // Read the color from the source image
                        Color color = Color.FromArgb(sourceLine[index]);
                        // Add the dithering to the pixel
                        var colorAsColor = new Models.Color(color) + ditherDistortion[index * row];
                        // Get the index of the color closest to the dithered pixel
                        targetLine[index] = imageStore.Quantizer.GetPaletteIndex(colorAsColor);
                        // Get the distance to dither the other pixels!
                        var distance = System.Math.Sqrt(Util.Math.Distance(new Models.Color(color), imageStore.Quantizer.GetColorByIndex(targetLine[index])));
                        if (index != 0 && index != width - 1 && row != height - 1)
                            // TODO: Instead of using a single positive distance, add per pixel distance and allow it to be negative!
                            ditherer.Dither((int)distance, ditherDistortion, row*width + index, width);
                        TotalError += distance;
                    }

                    Marshal.Copy(targetLine, 0, new IntPtr(targetOffsett), width);

                    sourceOffsett += sourceData.Stride;
                    targetOffsett += targetData.Stride;

                    // Update progress
                    completed++;
                    ProgressUpdate?.Invoke(this, new ProgressEventArgs((int)(completed / (float)bitmap.Height * 100)));

                    sourceOffset += sourceData.Stride;
                    targetOffset += targetData.Stride;
                }

                AverageError = TotalError / total;
            }
            
            finally
            {
                bitmap.UnlockBits(sourceData);
                canvas.UnlockBits(targetData);
            }

            canvas.Save("result.gif");
            return canvas;
        }

        public override Task<Bitmap> DrawAsync()
        {
            return Task.Run(() => { return Draw(); });
        }
    }
}
