using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ImageProcessing.Models;

namespace ImageProcessing.Logic.Quantizers
{
    public class BWQuantizer : Quantizer
    {

        public BWQuantizer() : base()
        {

        }

        public override void AddColor(System.Drawing.Color c)
        {
            return;
        }

        protected override void PopulatePalette()
        {
            palette.Add(new Models.Color(0, 0, 0));
            palette.Add(new Models.Color(255, 255, 255));
        }

        public override int GetPaletteIndex(Models.Color c)
        {
            if (palette == null) PopulatePalette();
            int index = 0;
            Models.Color bestColor = palette[0];
            c += new Models.Color(30, 30, 30);
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
    }
}
