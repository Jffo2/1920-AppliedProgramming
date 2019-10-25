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
        void Dither(int distance, Models.Color[] ditherDistortionArray, long offset, long stride);
        Rectangle GetRelativeRect();
    }
}
