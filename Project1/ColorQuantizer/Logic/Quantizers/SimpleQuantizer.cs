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
                System.Diagnostics.Debug.WriteLine($"min({minR},{minG},{minB})");
                System.Diagnostics.Debug.WriteLine($"max({maxR},{maxG},{maxB})");
                System.Diagnostics.Debug.WriteLine($"delta({rDelta},{gDelta},{bDelta})");
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

        public void AddColor(Models.Color c)
        {
            minR = (c.Channel1 < minR) ? c.Channel1 : minR;
            maxR = (c.Channel1 > maxR) ? c.Channel1 : maxR;

            minG = (c.Channel2 < minG) ? c.Channel2 : minG;
            maxG = (c.Channel2 > maxG) ? c.Channel2 : maxG;

            minB = (c.Channel3 < minB) ? c.Channel3 : minB;
            maxB = (c.Channel3 > maxB) ? c.Channel3 : maxB;
        }

        public byte GetPaletteIndex(Models.Color c)
        {
            //System.Diagnostics.Debug.WriteLine($"Quantizer is calculating palette index");
            byte index = 0;
            Models.Color bestColor = palette[0];
            for (int i = 0; i < palette.Count; i++)
            {
                if (Distance(palette[i], c) < Distance(bestColor, c))
                {
                    //System.Diagnostics.Debug.WriteLine($"Quantizer found shorter distance, adding color {palette[i]}");
                    bestColor = palette[i];
                    index = (byte)i;
                }
            }
            //System.Diagnostics.Debug.WriteLine($"Quantizer found best color at {index}");
            return index;
        }

        public Task<byte> GetPaletteIndexAsync(Models.Color c)
        {
            return Task.Run(() =>
            {
                return GetPaletteIndex(c);
            });
        }

        public int Distance(Models.Color color, Models.Color other)
        {
            return (color.Channel1 - other.Channel1) * (color.Channel1 - other.Channel1) + (color.Channel2 - other.Channel2) * (color.Channel2 - other.Channel2) + (color.Channel3 - other.Channel3) * (color.Channel3 - other.Channel3);
        }

        public byte GetPaletteIndex(System.Drawing.Color c)
        {
            Models.Color color = new Models.Color(c.R, c.G, c.B);
            return GetPaletteIndex(color);
        }
    }
}
