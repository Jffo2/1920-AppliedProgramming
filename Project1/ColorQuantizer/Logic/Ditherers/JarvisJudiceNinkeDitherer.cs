using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ImageProcessing.Logic.Ditherers
{
    public class JarvisJudiceNinkeDitherer : IDitherer
    {

        /// <summary>
        /// Spreads the dithering to surrounding pixels according to the Jarvis Judice Ninke algorithm
        /// </summary>
        /// <param name="original">the original pixel</param>
        /// <param name="palette">the color the original pixel was mapped to</param>
        /// <param name="ditherDistortionArray">a reference to the overlay array that keeps track of dither distortion</param>
        /// <param name="currentColumn">the column of the pixel</param>
        /// <param name="currentRow">the row of the pixel</param>
        /// <param name="width">the width of the image</param>
        /// <param name="height">the height of the image</param>
        public void Dither(Models.Color original, Models.Color palette, Models.Color[] ditherDistortionArray, long currentColumn, long currentRow, long width, long height)
        {
            lock (ditherDistortionArray)
            {
                var offset = currentRow * width + currentColumn;
                var distances = original - palette;

                // Same row
                if (currentColumn < width - 1)
                {
                    ditherDistortionArray[offset + 1] += Apply(distances, 7);
                    if (currentColumn < (width - 2))
                    {
                        ditherDistortionArray[offset + 2] += Apply(distances, 5);
                    }
                }

                // Row below
                if (currentRow < height - 1)
                {
                    // Center
                    ditherDistortionArray[offset + width] += Apply(distances, 7);

                    // To the left
                    if (currentColumn > 0)
                    {
                        ditherDistortionArray[offset + width - 1] += Apply(distances, 5);
                        if (currentColumn > 1)
                        {
                            ditherDistortionArray[offset + width - 2] += Apply(distances, 3);
                        }
                    }

                    // To the right
                    if (currentColumn < width - 1)
                    {
                        ditherDistortionArray[offset + width + 1] += Apply(distances, 5);
                        if (currentColumn < width - 2)
                        {
                            ditherDistortionArray[offset + width + 2] += Apply(distances, 3);
                        }
                    }

                    // 2 rows below
                    if (currentRow < height - 2)
                    {
                        // Center
                        ditherDistortionArray[offset + 2 * width] += Apply(distances, 5);

                        // To the left
                        if (currentColumn > 0)
                        {
                            ditherDistortionArray[offset + 2 * width - 1] += Apply(distances, 3);
                            if (currentColumn > 1)
                            {
                                ditherDistortionArray[offset + 2 * width - 2] += Apply(distances, 1);
                            }
                        }

                        // To the right
                        if (currentColumn < width - 1)
                        {
                            ditherDistortionArray[offset + 2 * width + 1] += Apply(distances, 3);
                            if (currentColumn < width - 2)
                            {
                                ditherDistortionArray[offset + 2 * width + 2] += Apply(distances, 1);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the dither distortion
        /// </summary>
        /// <param name="distances">The distances in R, G and B from the original to the mapped pixel</param>
        /// <param name="multiplier">the dithering numerator</param>
        /// <returns>A color object that contains dither distortion values for R, G and B</returns>
        private Models.Color Apply(int[] distances, int multiplier)
        {
            return new Models.Color((multiplier * distances[0]) / 48, (multiplier * distances[1]) / 48, (multiplier * distances[2]) / 48);
        }

        /// <summary>
        /// How many pixels to the left of the current pixel will be affected by the dither distortion
        /// </summary>
        /// <returns>the amount of pixels as an int</returns>
        public int GetBehind()
        {
            return 2;
        }

        public override string ToString()
        {
            return "JJN";
        }
    }
}
