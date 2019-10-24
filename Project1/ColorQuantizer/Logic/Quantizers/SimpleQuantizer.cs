using ImageProcessing.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace ImageProcessing.Logic.Quantizers
{
    public class SimpleQuantizer : IQuantizer
    {
        private readonly List<Models.Color> palette;

        public List<Models.Color> GetPalette()
        {
            PopulatePalette();
            return palette;
        }

        private byte minR = 255;
        private byte maxR = 0;
        private byte minG = 255;
        private byte maxG = 0;
        private byte minB = 255;
        private byte maxB = 0;

        public SimpleQuantizer()
        {
            palette = new List<Models.Color>();
        }

        private void PopulatePalette()
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

        public void AddColor(System.Drawing.Color c)
        {
            minR = (c.R < minR) ? c.R : minR;
            maxR = (c.R > maxR) ? c.R : maxR;

            minG = (c.G < minG) ? c.G : minG;
            maxG = (c.G > maxG) ? c.G : maxG;

            minB = (c.B < minB) ? c.B : minB;
            maxB = (c.B > maxB) ? c.B : maxB;
        }

        public byte GetPaletteIndex(Models.Color c)
        {
            byte index = 0;
            Models.Color bestColor = palette[0];
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

        public Task<byte> GetPaletteIndexAsync(Models.Color c)
        {
            return Task.Run(() =>
            {
                return GetPaletteIndex(c);
            });
        }

        

        public byte GetPaletteIndex(System.Drawing.Color c)
        {
            Models.Color color = new Models.Color(c.R, c.G, c.B);
            return GetPaletteIndex(color);
        }

        public Models.Color GetColorByIndex(byte index)
        {
            return palette[index];
        }
    }
}
