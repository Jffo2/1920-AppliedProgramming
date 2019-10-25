using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace ImageProcessing.Logic.Ditherers
{
    public interface IDitherer
    {
        void Dither(int distance, BitmapData data);
        void Dither(Models.Color original, Models.Color palette, Models.Color[] ditherDistortionArray, long offset, long stride);
        Rectangle GetRelativeRect();
    }
}
