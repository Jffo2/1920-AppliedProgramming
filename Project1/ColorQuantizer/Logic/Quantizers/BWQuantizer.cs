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
            var avg = c.Channel1 + c.Channel2 + c.Channel3;

            return (avg/3 > 127)? 1 : 0;
        }
    }
}
