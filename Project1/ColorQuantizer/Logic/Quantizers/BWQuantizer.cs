using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ImageProcessing.Models;

namespace ImageProcessing.Logic.Quantizers
{
    public class BWQuantizer : Quantizer
    {
        public override void AddColor(System.Drawing.Color c)
        {
            return;
        }

        protected override void PopulatePalette()
        {
            palette.Add(new Models.Color(0, 0, 0));
            palette.Add(new Models.Color(255, 255, 255));
        }
    }
}
