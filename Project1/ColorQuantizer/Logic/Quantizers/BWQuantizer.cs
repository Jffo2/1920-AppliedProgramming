using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ImageProcessing.Models;

namespace ImageProcessing.Logic.Quantizers
{
    public class BWQuantizer : Quantizer
    {

        /// <summary>
        /// This method does nothing since we already know which 2 colors will be present in our palette
        /// </summary>
        /// <param name="c">the color to add</param>
        public override void AddColor(System.Drawing.Color c)
        {
            return;
        }

        /// <summary>
        /// Add the 2 colors, black and white, to the palette
        /// </summary>
        protected override void PopulatePalette()
        {
            palette.Add(new Models.Color(0, 0, 0));
            palette.Add(new Models.Color(255, 255, 255));
        }
    }
}
