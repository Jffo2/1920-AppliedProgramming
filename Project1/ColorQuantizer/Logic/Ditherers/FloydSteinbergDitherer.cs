using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using ImageProcessing.Models;

namespace ImageProcessing.Logic.Ditherers
{
    public class FloydSteinbergDitherer : IDitherer
    {
        public void Dither(int distance, BitmapData data)
        {
            throw new NotImplementedException();
        }

        public Rectangle GetRelativeRect()
        {
            throw new NotImplementedException();
        }

        void IDitherer.Dither(int distance, Models.Color[] ditherDistortionArray, long offset, long stride)
        {
            ditherDistortionArray[offset + 1] += ((7 * distance) >> 4);
            ditherDistortionArray[offset + stride - 1] += ((3 * distance) >> 4);
            ditherDistortionArray[offset + stride] += ((5 * distance) >> 4);
            ditherDistortionArray[offset + stride + 1] += ((1 * distance) >> 4);
        }
    }
}
