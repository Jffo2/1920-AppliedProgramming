using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImageProcessing.Logic.Quantizers
{
    public class HSLQuantizer : Quantizer
    {
        private Dictionary<Color, int> colorCount;

        private const int PALETTE_MAX_COUNT = 256;

        public HSLQuantizer() : base()
        {
            colorCount = new Dictionary<Color, int>();
        }

        public override void AddColor(Color c)
        {
            lock (colorCount)
            {
                if (!colorCount.ContainsKey(c))
                {
                    colorCount.Add(c, 1);
                }
                else
                {
                    colorCount[c]++;
                }
            }
        }

        protected override void PopulatePalette()
        {
            if (colorCount == null) throw new Exception("Histogram was not ready");
            Random rd = new Random(666);
            var colors = Cut(colorCount.OrderBy(keypair => rd.NextDouble()).Select(keypair => keypair.Key));

            if (colors.Count() > PALETTE_MAX_COUNT)
            {
                colors = colors.OrderByDescending(color => colorCount[color]).Take(PALETTE_MAX_COUNT);
            }
            lock (palette)
            {
                palette.Clear();
                foreach (Color c in colors)
                {
                    palette.Add(new Models.Color(c));
                }
            }
        }

        private IEnumerable<Color> Cut(IEnumerable<Color> colorList)
        {
            if (colorList.Count() <= PALETTE_MAX_COUNT) return colorList;
            var hueComparer = new HueComparer();
            var saturationComparer = new SaturationComparer();
            var brightnessComparer = new BrightnessComparer();

            var hue = colorList.Distinct(hueComparer);
            var saturation = colorList.Distinct(saturationComparer);
            var brightness = colorList.Distinct(brightnessComparer);

            var hueCount = hue.Count();
            var saturationCount = saturation.Count();
            var brightnessCount = brightness.Count();

            if (hueCount > saturationCount && hueCount > brightnessCount)
            {
                return Cut2(hue, saturationComparer, brightnessComparer);
            }
            else if (saturationCount > hueCount && saturationCount > brightnessCount)
            {
                return Cut2(saturation, hueComparer, brightnessComparer);
            }
            else
            {
                return Cut2(brightness, hueComparer, saturationComparer);
            }
        }

        private IEnumerable<Color> Cut2(IEnumerable<Color> colorList, IEqualityComparer<Color> comp1, IEqualityComparer<Color> comp2)
        {
            if (colorList.Count() <= PALETTE_MAX_COUNT) return colorList;
            var comp1Distinct = colorList.Distinct(comp1);
            var comp2Distinct = colorList.Distinct(comp2);

            if (comp1Distinct.Count() > comp2Distinct.Count())
            {
                return Cut3(comp1Distinct, comp2);
            }
            else
            {
                return Cut3(comp2Distinct, comp1);
            }
        }

        private IEnumerable<Color> Cut3(IEnumerable<Color> colorList, IEqualityComparer<Color> comp)
        {
            if (colorList.Count() <= PALETTE_MAX_COUNT) return colorList;
            return colorList.Distinct(comp);
        }

        private class HueComparer : IEqualityComparer<Color>
        {
            public bool Equals(Color x, Color y)
            {
                return x.GetHue() == y.GetHue();
            }

            public int GetHashCode(Color color)
            {
                return color.GetHue().GetHashCode();
            }
        }

        private class SaturationComparer : IEqualityComparer<Color>
        {
            public bool Equals(Color x, Color y)
            {
                return x.GetSaturation() == y.GetSaturation();
            }

            public int GetHashCode(Color color)
            {
                return color.GetSaturation().GetHashCode();
            }
        }

        private class BrightnessComparer : IEqualityComparer<Color>
        {
            public bool Equals(Color x, Color y)
            {
                return x.GetBrightness() == y.GetBrightness();
            }

            public int GetHashCode(Color color)
            {
                return color.GetBrightness().GetHashCode();
            }
        }
    }
}
