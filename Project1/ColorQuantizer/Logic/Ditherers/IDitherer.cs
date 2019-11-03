using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace ImageProcessing.Logic.Ditherers
{
    public interface IDitherer
    {
        /// <summary>
        /// How many pixels to the left of the current pixel will be affected by the dither distortion
        /// </summary>
        /// <returns>the amount of pixels as an int</returns>
        int GetBehind();

        /// <summary>
        /// Spreads the dithering to surrounding pixels
        /// </summary>
        /// <param name="original">the original pixel</param>
        /// <param name="palette">the color the original pixel was mapped to</param>
        /// <param name="ditherDistortionArray">a reference to the overlay array that keeps track of dither distortion</param>
        /// <param name="currentColumn">the column of the pixel</param>
        /// <param name="currentRow">the row of the pixel</param>
        /// <param name="width">the width of the image</param>
        /// <param name="height">the height of the image</param>
        void Dither(Models.Color original, Models.Color palette, Models.Color[] ditherDistortionArray, long currentColumn, long currentRow, long width, long height);
        
    }
}
