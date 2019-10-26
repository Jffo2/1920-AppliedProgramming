using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using ImageProcessing.Models;

namespace ImageProcessing.Logic.Ditherers
{
    public class FloydSteinbergDitherer : IDitherer
    {
        public void Dither(int distance, BitmapData data)
        {
            throw new NotImplementedException();
        }

        public Rectangle GetRelativeRect()
        {
            throw new NotImplementedException();
        }

        void IDitherer.Dither(Models.Color original, Models.Color palette, Models.Color[] ditherDistortionArray, long currentColumn, long currentRow, long width, long height)
        {
            lock (ditherDistortionArray)
            {
                var offset = currentRow * width + currentColumn;
                var distances = original - palette;
                if (currentColumn != width - 1)
                {
                    ditherDistortionArray[offset + 1] += Apply(distances, 7); ;
                }
                if (currentRow != height - 1)
                {
                    if (currentColumn != 0)
                    {
                        ditherDistortionArray[offset + width - 1] += Apply(distances, 3);
                    }
                    ditherDistortionArray[offset + width] += Apply(distances, 5);
                    if (currentColumn != width - 1)
                    {
                        ditherDistortionArray[offset + width + 1] += Apply(distances, 1);
                    }
                }
            }
        }

        Models.Color Apply(int[] distances, int multiplier)
        {
            return new Models.Color((multiplier * distances[0]) >> 4, (multiplier * distances[1]) >> 4, (multiplier * distances[2]) >> 4);
        }
    }
}
