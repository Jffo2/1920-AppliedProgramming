using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using ImageProcessing.Models;

namespace ImageProcessing.Logic.Quantizers
{
    public abstract class Quantizer : IQuantizer
    {
        protected readonly List<Models.Color> palette;
        private readonly Dictionary<Models.Color, int> cache;

        protected abstract void PopulatePalette();

        public Quantizer()
        {
            palette = new List<Models.Color>();
            cache = new Dictionary<Models.Color, int>();
        }

        public abstract void AddColor(System.Drawing.Color c);

        public Models.Color GetColorByIndex(int index)
        {
            if (index < 0 || index >= palette.Count) throw new ArgumentOutOfRangeException();
            return palette[index];
        }

        public List<Models.Color> GetPalette()
        {
            if (palette.Count==0) PopulatePalette();
            return palette;
        }

        public virtual int GetPaletteIndex(Models.Color c)
        {
            if (palette.Count==0) PopulatePalette();
            if (cache.ContainsKey(c)) return cache[c];
            int index = 0;
            Models.Color bestColor = palette[0];
            for (int i = 0; i < palette.Count; i++)
            {
                if (Util.Math.Distance(palette[i], c) < Util.Math.Distance(bestColor, c))
                {
                    bestColor = palette[i];
                    index = (byte)i;
                }
            }
            cache[c] = index;
            return index;
        }

        public int GetPaletteIndex(System.Drawing.Color c)
        {
            Models.Color color = new Models.Color(c.R, c.G, c.B);
            return GetPaletteIndex(color);
        }
    }
}
