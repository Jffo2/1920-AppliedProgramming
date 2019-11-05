using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ImageProcessing.Logic;
using ImageProcessing.Logic.Ditherers;
using ImageProcessing.Util;

namespace ImageProcessing.Presentation
{
    public class AsynchronousDitheredDrawer : Drawer
    {
        private double TotalError;
        private int completed;

        public override event EventHandler<ProgressEventArgs> ProgressUpdate;

        public AsynchronousDitheredDrawer(ImageStore imageStore) : base(imageStore)
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
            IDitherer ditherer = imageStore.Ditherer;
            // Lock the image for thread-safeness
            lock (imageStore.Image)
            {
                return GenerateBitmap(imageStore.Image, ditherer);
            }
        }

        private Bitmap GenerateBitmap(Bitmap bitmap, IDitherer ditherer)
        {
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
                StartWritingProcess(bitmap.Height, bitmap.Width, sourceData.Scan0.ToInt64(), targetData.Scan0.ToInt64(), sourceData.Stride, targetData.Stride, ditherer);
            }

            finally
            {
                bitmap.UnlockBits(sourceData);
                canvas.UnlockBits(targetData);
            }

            return canvas;
        }

        public override Task<Bitmap> DrawAsync()
        {
            return Task.Run(() => { return Draw(); });
        }

        public override string ToString()
        {
            return "ASynchronousDitheredDrawer";
        }

        /// <summary>
        /// Creates tasks for writing to pixels and calculates the average error
        /// </summary>
        /// <param name="height">the height of the bitmap</param>
        /// <param name="width">the width of the bitmap</param>
        /// <param name="sourceOffset">the offset of the source bitmap in memory</param>
        /// <param name="targetOffset">the offset of the target bitmap in memory</param>
        /// <param name="sourceStride">the stride of the source bitmap in memory</param>
        /// <param name="targetStride">the stride of the trarget bitmap in memory</param>
        /// <param name="ditherer">the ditherer to be used</param>
        private void StartWritingProcess(int height, int width, long sourceOffset, long targetOffset, int sourceStride, int targetStride, IDitherer ditherer)
        {
            int behind = imageStore.Ditherer.GetBehind() + 1;

            var tasks = GetTasks(height, width, sourceOffset, targetOffset, sourceStride, targetStride, ditherer, behind);
            
            Task.WaitAll(tasks);
            lock (ProgressUpdate)
            {
                ProgressUpdate?.Invoke(this, new ProgressEventArgs(100));
            }
            AverageError = TotalError / (width*height);
        }

        /// <summary>
        /// Spawns tasks that will each write a line to the target bitmap
        /// </summary>
        /// <param name="height">the height of the bitmap</param>
        /// <param name="width">the width of the bitmap</param>
        /// <param name="sourceOffset">the offset of the source bitmap in memory</param>
        /// <param name="targetOffset">the offset of the target bitmap in memory</param>
        /// <param name="sourceStride">the stride of the source bitmap in memory</param>
        /// <param name="targetStride">the stride of the trarget bitmap in memory</param>
        /// <param name="ditherer">the ditherer to be used</param>
        /// <param name="behind">how many pixels to the left of the currently processed pixel the ditherer writes distortion to</param>
        /// <returns>A list of writing tasks</returns>
        private Task[] GetTasks(int height, int width, long sourceOffset, long targetOffset, long sourceStride, int targetStride, IDitherer ditherer, int behind)
        {
            Models.Color[] ditherDistortion = new Models.Color[width*height];
            
            // Keeps track of the progress of every task, so they can know when to start
            // since they have to leave enough time so they won't quantize before dithering is finished
            // this is what behind is used for
            int[] progress = new int[height];
            Task[] tasks = new Task[height];
            completed = 0;
            for (int row = 0; row < height; row++)
            {
                Task t = GenerateTask(sourceOffset, targetOffset, width, height, row, behind, progress, ditherDistortion, ditherer);

                tasks[row] = t;
                // Set ConfigureAwait to false, to avoid deadlocking
                t.ConfigureAwait(false);
                t.Start();
                sourceOffset += sourceStride;
                targetOffset += targetStride;
            }
            return tasks;
        }

        /// <summary>
        /// Generate a task that writes to the target bitmap
        /// </summary>
        /// <param name="sourceOffset">the offset to the source bitmap in memory</param>
        /// <param name="targetOffset">the offset to the target bitmap in memory</param>
        /// <param name="width">the width of the image</param>
        /// <param name="height">the height of the image</param>
        /// <param name="row">the row this task has to process</param>
        /// <param name="behind">the behind constant of the ditherer</param>
        /// <param name="progress">the progress array, this will get updated as the task progresses, and is used for checking if this task can continue</param>
        /// <param name="ditherDistortion">the dither distortion overlay, this will be edited by this task</param>
        /// <param name="ditherer">the ditherer to be used</param>
        /// <returns>the task</returns>
        private Task GenerateTask(long sourceOffset, long targetOffset, int width, int height, int row, int behind, int[] progress, Models.Color[] ditherDistortion, IDitherer ditherer)
        {
            // Re initialize the source and target rows
            byte[] targetLine = new byte[width];
            int[] sourceLine = new int[width];
            var id = row;
            

            Task t = new Task(() =>
            {
                Marshal.Copy(new IntPtr(sourceOffset), sourceLine, 0, width);

                for (int index = 0; index < width; index++)
                {
                    if (id != 0)
                    {
                        // Check if the task processing the row above ours is done dithering the pixel this task is processing
                        while (index > (progress[id - 1] - behind))
                        {
                            System.Threading.Thread.Sleep(1);
                        }
                    }
                    // Read the color from the source image
                    Color color = Color.FromArgb(sourceLine[index]);
                    // Add the dithering to the pixel
                    var colorAsColor = new Models.Color(color) + ditherDistortion[index + id * width];
                    // Get the index of the color closest to the dithered pixel
                    targetLine[index] = (byte)imageStore.Quantizer.GetPaletteIndex(colorAsColor);
                    // Get the distance to dither the other pixels!
                    var distance = System.Math.Sqrt(Util.Math.Distance(new Models.Color(color), imageStore.Quantizer.GetColorByIndex(targetLine[index])));
                    ditherer.Dither(colorAsColor, imageStore.Quantizer.GetColorByIndex(targetLine[index]), ditherDistortion, index, id, width, height);
                    TotalError += distance;
                    // Free up memory by deleting this entry in the dither distortion matrix
                    ditherDistortion[index + id * width] = null;
                    progress[id]++;
                }
                // Increase this significantly so task below this can get to the end
                progress[id] += width;

                Marshal.Copy(targetLine, 0, new IntPtr(targetOffset), width);
                // Update progress
                completed++;

                lock (ProgressUpdate)
                {
                    ProgressUpdate?.Invoke(this, new ProgressEventArgs((int)(completed / (float)height * 100)));
                }
            });

            return t;
        }
    }
}
