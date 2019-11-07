using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ImageProcessing.Models;

namespace ImageProcessing.Logic.Quantizers
{
    public abstract class Quantizer : IQuantizer
    {
        /// <summary>
        /// The palette to be used
        /// </summary>
        protected readonly List<Models.Color> palette;

        /// <summary>
        /// Initialize the palette
        /// </summary>
        protected abstract void PopulatePalette();

        /// <summary>
        /// Constructor, initialize palette and cache
        /// </summary>
        public Quantizer()
        {
            palette = new List<Models.Color>();
        }

        /// <summary>
        /// Register the color to the quantizer
        /// </summary>
        /// <param name="c">the color to register</param>
        public abstract void AddColor(System.Drawing.Color c);

        /// <summary>
        /// Get a color by it's index in the palette
        /// </summary>
        /// <param name="index">the index in the palette</param>
        /// <returns>the color of the palette</returns>
        public Models.Color GetColorByIndex(int index)
        {
            if (index < 0 || index >= palette.Count) throw new ArgumentOutOfRangeException();
            return palette[index];
        }

        /// <summary>
        /// Get the palette
        /// </summary>
        /// <returns>the palette</returns>
        public List<Models.Color> GetPalette()
        {
            // If the palette is not yet initialized, populate it with the colors
            lock (palette)
            {
                if (palette.Count == 0) PopulatePalette();
                return palette;
            }
        }

        /// <summary>
        /// Get the index of the color closest to a given color in the palette
        /// </summary>
        /// <param name="c">the color to replace</param>
        /// <returns>the index of the closest color in the palette</returns>
        public virtual int GetPaletteIndex(Models.Color c)
        {
            // If the palette is not yet initialized, populate it with the colors
            if (palette.Count==0) PopulatePalette();
            int index = 0;
            Models.Color bestColor = palette[0];
            // Loop over the palette to find the closest color
            for (int i = 0; i < palette.Count; i++)
            {
                if (Util.Math.Distance(palette[i], c) < Util.Math.Distance(bestColor, c))
                {
                    bestColor = palette[i];
                    index = (byte)i;
                }
            }
            return index;
        }

        /// <summary>
        /// Get the index of the color closest to a given color in the palette
        /// </summary>
        /// <param name="c">the color to replace</param>
        /// <returns>the index of the closest color in the palette</returns>
        public int GetPaletteIndex(System.Drawing.Color c)
        {
            return GetPaletteIndex(new Models.Color(c));
        }
    }
}
