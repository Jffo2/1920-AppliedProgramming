using ImageProcessing.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ImageProcessing.Logic.Quantizers
{
    public interface IQuantizer
    {
        void AddColor(Color c);
        byte GetPaletteIndex(Color c);
        byte GetPaletteIndex(System.Drawing.Color c);
        List<Color> GetPalette();
    }
}
