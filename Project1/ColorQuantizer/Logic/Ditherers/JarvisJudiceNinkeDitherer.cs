using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ImageProcessing.Logic.Ditherers
{
    public class JarvisJudiceNinkeDitherer : IDitherer
    {

        public void Dither(Models.Color original, Models.Color palette, Models.Color[] ditherDistortionArray, long currentColumn, long currentRow, long width, long height)
        {
            lock (ditherDistortionArray)
            {
                var offset = currentRow * width + currentColumn;
                var distances = original - palette;

                if (currentRow==0)
                {
                    File.WriteAllText("Distances.txt", "");
                }
                if (currentRow == 320)
                {
                    // TODO: remove
                    
                    File.AppendAllText("Distances.txt", original.ToString() + " - " + palette.ToString() + " = " + $"([{distances[0]},{distances[1]},{distances[2]}]\r\n");
                }

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

        Models.Color Apply(int[] distances, int multiplier)
        {
            return new Models.Color((multiplier * distances[0]) / 48, (multiplier * distances[1]) / 48, (multiplier * distances[2]) / 48);
        }

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
