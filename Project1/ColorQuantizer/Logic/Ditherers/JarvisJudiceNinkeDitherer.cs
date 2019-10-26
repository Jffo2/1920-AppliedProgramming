using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageProcessing.Logic.Ditherers
{
    public class JarvisJudiceNinkeDitherer : IDitherer
    {
        public void Dither(int distance, BitmapData data)
        {
            throw new NotImplementedException();
        }

        public void Dither(Models.Color original, Models.Color palette, Models.Color[] ditherDistortionArray, long currentColumn, long currentRow, long width, long height)
        {
            lock (ditherDistortionArray)
            {
                var offset = currentRow * width + currentColumn;
                var distances = original - palette;
                if (currentColumn != width - 1)
                {
                    ditherDistortionArray[offset + 1] += new Models.Color((7 * distances[0]) / 48, (7 * distances[1]) / 48, (7 * distances[2]) / 48);
                }
                if (currentColumn != width -2)
                {
                    ditherDistortionArray[offset + 1] += new Models.Color((5 * distances[0]) / 48, (5 * distances[1]) / 48, (5 * distances[2]) / 48);
                }
                if (currentRow != height - 1)
                {
                    if (currentColumn != 0)
                    {
                        ditherDistortionArray[offset + width - 1] += new Models.Color((3 * distances[0]) >> 4, (3 * distances[1]) >> 4, (3 * distances[2]) >> 4);
                    }
                    ditherDistortionArray[offset + width] += new Models.Color((5 * distances[0]) >> 4, (5 * distances[1]) >> 4, (5 * distances[2]) >> 4);
                    if (currentColumn != width - 1)
                    {
                        ditherDistortionArray[offset + width + 1] += new Models.Color((1 * distances[0]) >> 4, (1 * distances[1]) >> 4, (1 * distances[2]) >> 4);
                    }
                }
                /*if (currentRow != height - 2)
                {
                    if (currentColumn != 0)
                    {
                        ditherDistortionArray[offset + width - 1] += new Models.Color((3 * distances[0]) >> 4, (3 * distances[1]) >> 4, (3 * distances[2]) >> 4);
                    }
                    ditherDistortionArray[offset + width] += new Models.Color((5 * distances[0]) >> 4, (5 * distances[1]) >> 4, (5 * distances[2]) >> 4);
                    if (currentColumn != width - 1)
                    {
                        ditherDistortionArray[offset + width + 1] += new Models.Color((1 * distances[0]) >> 4, (1 * distances[1]) >> 4, (1 * distances[2]) >> 4);
                    }
                }*/
            }
        }

        public Rectangle GetRelativeRect()
        {
            throw new NotImplementedException();
        }
    }
}
