using ImageProcessing.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace ImageProcessing.Logic.Quantizers
{
    public class SimpleQuantizer : Quantizer
    {
        private byte minR = 255;
        private byte maxR = 0;
        private byte minG = 255;
        private byte maxG = 0;
        private byte minB = 255;
        private byte maxB = 0;

        protected override void PopulatePalette()
        {
            if (palette.Count == 0)
            {
                var rDelta = (byte)((maxR - minR) / 8.0);
                var gDelta = (byte)((maxG - minG) / 8.0);
                var bDelta = (byte)((maxB - minB) / 4.0);
                lock (palette)
                {
                    for (int R = 0; R < 8; R++)
                    {
                        for (int G = 0; G < 8; G++)
                        {
                            for (int B = 0; B < 4; B++)
                            {
                                palette.Add(new Models.Color((byte)(R * (rDelta) + minR), (byte)(G * gDelta + minG), (byte)(B * bDelta + minB)));
                            }
                        }
                    }
                }
            }
        }

        public override void AddColor(System.Drawing.Color c)
        {
            minR = (c.R < minR) ? c.R : minR;
            maxR = (c.R > maxR) ? c.R : maxR;

            minG = (c.G < minG) ? c.G : minG;
            maxG = (c.G > maxG) ? c.G : maxG;

            minB = (c.B < minB) ? c.B : minB;
            maxB = (c.B > maxB) ? c.B : maxB;
        }
    }
}
