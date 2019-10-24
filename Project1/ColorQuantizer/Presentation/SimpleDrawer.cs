using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ImageProcessing.Logic;
using ImageProcessing.Util;
using Newtonsoft.Json;

namespace ImageProcessing.Presentation
{
    public class SimpleDrawer: Drawer
    {
        public override event EventHandler<ProgressEventArgs> ProgressUpdate;

        public const int ROWS_PER_THREAD = 6;

        public SimpleDrawer(ImageStore imageStore) : base(imageStore)
        {
            
        }

        public override Task<Bitmap> DrawAsync()
        {
            return Task.Run(() =>
            {
                return Draw();
            });
        }

        public override Bitmap Draw()
        {
            if (!imageStore.QuantizerReady)
            {
                throw new Exception("The Quantizer was not ready yet.");
            }
            Bitmap bitmap;
            lock (imageStore.Image)
            {
                bitmap = Cloner.DeepClone(imageStore.Image);
            }
            Rectangle bounds = Rectangle.FromLTRB(0, 0, bitmap.Width, bitmap.Height);

            // Lock source bits for reading
            BitmapData sourceData = bitmap.LockBits(bounds, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            // Lock target bits for writing
            Bitmap canvas = new Bitmap(bitmap.Width, bitmap.Height,PixelFormat.Format8bppIndexed);
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

                Task[] tasks = new Task[(int)System.Math.Ceiling(bitmap.Height/(double)ROWS_PER_THREAD)];
                
                // Start a thread for every ROWS_PER_THREAD rows in the image
                for (int row = 0; row < bitmap.Height; row+=ROWS_PER_THREAD)
                {
                    // shallow copy memory objects
                    var r = row;
                    var sourceOffsett = sourceOffset;
                    var targetOffsett = targetOffset;

                    // Run actual quantization on another thread
                    Task t = new Task(() =>
                    {
                        // For the amount of rows that was assigned to this task
                        for (int i = 0; i < ROWS_PER_THREAD; i++)
                        {
                            // If the row is out of bounds, break, this is the end of the image
                            if (r+i>=height) break;

                            // Re initialize the source and target rows
                            byte[] targetLine = new byte[width];
                            int[] sourceLine = new int[width];

                            Marshal.Copy(new IntPtr(sourceOffsett), sourceLine, 0, width);

                            for (int index = 0; index < width; index++)
                            {
                                Color color = Color.FromArgb(sourceLine[index]);
                                targetLine[index] = imageStore.Quantizer.GetPaletteIndex(color);
                            }

                            Marshal.Copy(targetLine, 0, new IntPtr(targetOffsett), width);

                            sourceOffsett += sourceData.Stride;
                            targetOffsett += targetData.Stride;
                        }

                        // Update progress
                        completed+=ROWS_PER_THREAD;
                        ProgressUpdate?.Invoke(this, new ProgressEventArgs((int)(completed/(float)row*100)));

                    });

                    t.Start();

                    sourceOffset += ROWS_PER_THREAD*sourceData.Stride;
                    targetOffset += ROWS_PER_THREAD*targetData.Stride;
                    tasks[row/ROWS_PER_THREAD] = t;
                }

                Task.WaitAll(tasks);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
            }
            finally
            {
                bitmap.UnlockBits(sourceData);
                canvas.UnlockBits(targetData);
            }

            canvas.Save("result.gif");
            return canvas;

        }
    }

    
}
