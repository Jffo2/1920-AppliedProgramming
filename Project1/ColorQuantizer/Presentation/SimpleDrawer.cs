using ImageProcessing.Logic;
using ImageProcessing.Util;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ImageProcessing.Presentation
{
    public class SimpleDrawer : Drawer
    {
        private double TotalError;
        private int completed;

        public override event EventHandler<ProgressEventArgs> ProgressUpdate;

        /// <summary>
        /// The amount of rows of the image each thread should process
        /// </summary>
        public const int ROWS_PER_THREAD = 6;

        public SimpleDrawer(ImageStore imageStore) : base(imageStore)
        {

        }

        /// <summary>
        /// Generates the bitmap async
        /// </summary>
        /// <returns>the bitmap as a taskresult</returns>
        public override Task<Bitmap> DrawAsync()
        {
            return Task.Run(() =>
            {
                return Draw();
            });
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
            Bitmap canvas;
            BitmapData sourceData;
            BitmapData targetData;
            int width;
            int height;
            lock (imageStore.Image)
            {
                width = imageStore.Image.Width;
                height = imageStore.Image.Height;

                Rectangle bounds = Rectangle.FromLTRB(0, 0, imageStore.Image.Width, imageStore.Image.Height);

                // Lock source bits for reading
                sourceData = imageStore.Image.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

                // Lock target bits for writing
                canvas = new Bitmap(imageStore.Image.Width, imageStore.Image.Height, PixelFormat.Format8bppIndexed);
                targetData = canvas.LockBits(bounds, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);

            }

            // Copy the palette to the target bitmap
            CopyPalette(canvas);

            try
            {
                long sourceOffset = sourceData.Scan0.ToInt64();
                long targetOffset = targetData.Scan0.ToInt64();
                long total = width * height;

                StartProcess(height, width, sourceOffset, targetOffset, sourceData.Stride, targetData.Stride);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            finally
            {
                lock (imageStore.Image)
                {
                    imageStore.Image.UnlockBits(sourceData);
                }
                canvas.UnlockBits(targetData);
            }

            return canvas;

        }

        /// <summary>
        /// Start the image generation process
        /// </summary>
        /// <param name="height">the height of the image</param>
        /// <param name="width">the width of the image</param>
        /// <param name="sourceOffset">the offset to the source image in memory</param>
        /// <param name="targetOffset">the offset to the target image in memory</param>
        /// <param name="sourceStride">the stride of the source image in memory</param>
        /// <param name="targetStride">the stride of the target image in memory</param>
        private void StartProcess(int height, int width, long sourceOffset, long targetOffset, int sourceStride, int targetStride)
        {
            // For progress reporting
            completed = 0;

            // Create a list of tasks for all the threads
            Task[] tasks = new Task[(int)System.Math.Ceiling(height / (double)ROWS_PER_THREAD)];

            // Start a thread for every ROWS_PER_THREAD rows in the image
            for (int row = 0; row < height; row += ROWS_PER_THREAD)
            {
                // Run actual quantization on another thread
                Task t = RunQuantization(row, sourceOffset, targetOffset, width, height, sourceStride, targetStride);

                t.Start();

                sourceOffset += ROWS_PER_THREAD * sourceStride;
                targetOffset += ROWS_PER_THREAD * targetStride;
                tasks[row / ROWS_PER_THREAD] = t;
            }

            Task.WaitAll(tasks);
            AverageError = TotalError / (width * height);
        }

        /// <summary>
        /// Run the quantization
        /// </summary>
        /// <param name="row">the row being processed</param>
        /// <param name="sourceOffset">the source offset</param>
        /// <param name="targetOffset">the target offset</param>
        /// <param name="width">the width of the image</param>
        /// <param name="height">the height of the image</param>
        /// <param name="sourceStride">the stride of the source image</param>
        /// <param name="targetStride">the stride of the target image</param>
        /// <returns></returns>
        private Task RunQuantization(int row, long sourceOffset, long targetOffset, int width, int height, int sourceStride, int targetStride)
        {
            return new Task(() =>
            {
                // For the amount of rows that was assigned to this task
                for (int i = 0; i < ROWS_PER_THREAD; i++)
                {
                    // If the row is out of bounds, break, this is the end of the image
                    if (row + i >= height) break;

                    // Re initialize the source and target rows
                    byte[] targetLine = new byte[width];
                    int[] sourceLine = new int[width];

                    Marshal.Copy(new IntPtr(sourceOffset), sourceLine, 0, width);

                    for (int index = 0; index < width; index++)
                    {
                        Color color = Color.FromArgb(sourceLine[index]);
                        targetLine[index] = (byte)imageStore.Quantizer.GetPaletteIndex(color);
                        TotalError += System.Math.Sqrt(Util.Math.Distance(new Models.Color(color), imageStore.Quantizer.GetColorByIndex(targetLine[index])));
                    }

                    Marshal.Copy(targetLine, 0, new IntPtr(targetOffset), width);

                    sourceOffset += sourceStride;
                    targetOffset += targetStride;
                }

                // Update progress
                completed += ROWS_PER_THREAD;
                ProgressUpdate?.Invoke(this, new ProgressEventArgs((int)(System.Math.Min(completed,height) / (float)height * 100)));

            });
        }

    }


}
