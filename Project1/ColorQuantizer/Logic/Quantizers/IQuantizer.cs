using ImageProcessing.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessing.Logic.Quantizers
{
    public interface IQuantizer
    {
        /// <summary>
        /// Add the color to the quantizer
        /// </summary>
        /// <param name="c">the color to add</param>
        void AddColor(System.Drawing.Color c);

        /// <summary>
        /// Get the index of the color closest to the passed color in the palette
        /// </summary>
        /// <param name="c">the color to replace</param>
        /// <returns>the index of the closest color in the palette</returns>
        int GetPaletteIndex(Color c);

        /// <summary>
        /// Get the index of the color closest to the passed color in the palette
        /// </summary>
        /// <param name="c">the color to replace</param>
        /// <returns>the index of the closest color in the palette</returns>
        int GetPaletteIndex(System.Drawing.Color c);

        /// <summary>
        /// Get the palette
        /// </summary>
        /// <returns>the palette</returns>
        List<Color> GetPalette();

        /// <summary>
        /// Get the color in the palette by it's index
        /// </summary>
        /// <param name="index">the index of the color</param>
        /// <returns>the color in the palette</returns>
        Color GetColorByIndex(int index);
    }
}
