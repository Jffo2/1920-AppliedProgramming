﻿using System;
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
        public override event EventHandler<ProgressEventArgs> ProgressUpdate;

        private double TotalError;
        private int completed;

        public AsynchronousDitheredDrawer(ImageStore imageStore) : base(imageStore)
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
                
                
                ProcessPixels(height, width, sourceOffset, targetOffset, sourceData.Stride, targetData.Stride, ditherer);
            }

            finally
            {
                bitmap.UnlockBits(sourceData);
                canvas.UnlockBits(targetData);
            }

            Save(canvas);
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

        private void ProcessPixels(int height, int width, long sourceOffset, long targetOffset, int sourceStride, int targetStride, IDitherer ditherer)
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

        private Task[] GetTasks(int height, int width, long sourceOffset, long targetOffset, long sourceStride, int targetStride, IDitherer ditherer, int behind)
        {
            Models.Color[] ditherDistortion = new Models.Color[width*height];
            
            int[] progress = new int[height];
            Task[] tasks = new Task[height];
            completed = 0;
            for (int row = 0; row < height; row++)
            {
                Task t = GenerateTask(sourceOffset, targetOffset, width, height, row, behind, progress, ditherDistortion, ditherer);

                tasks[row] = t;
                t.ConfigureAwait(false);
                t.Start();
                sourceOffset += sourceStride;
                targetOffset += targetStride;
            }
            return tasks;
        }

        private Task GenerateTask(long sourceOffsett, long targetOffsett, int width, int height, int row, int behind, int[] progress, Models.Color[] ditherDistortion, IDitherer ditherer)
        {
            // Re initialize the source and target rows
            byte[] targetLine = new byte[width];
            int[] sourceLine = new int[width];
            var id = row;
            

            Task t = new Task(() =>
            {
                Marshal.Copy(new IntPtr(sourceOffsett), sourceLine, 0, width);

                for (int index = 0; index < width; index++)
                {
                    if (id != 0)
                    {
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
                    progress[id]++;
                }

                progress[id] += width;

                Marshal.Copy(targetLine, 0, new IntPtr(targetOffsett), width);
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
