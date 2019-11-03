using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImageProcessing.Logic.Quantizers
{
    public class HSLQuantizer : Quantizer
    {
        /// <summary>
        /// Histogram like dictionary keeping track of the colors and their frequency
        /// </summary>
        private Dictionary<Color, int> colorCount;

        /// <summary>
        /// The amount of colors in the palette, for our assignment this is 256
        /// </summary>
        private const int PALETTE_MAX_COUNT = 256;

        /// <summary>
        /// Initialize the histogram
        /// </summary>
        public HSLQuantizer() : base()
        {
            colorCount = new Dictionary<Color, int>();
        }

        /// <summary>
        /// Register a color to the quantizer and add it to the histogram
        /// </summary>
        /// <param name="c">the color to register</param>
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

        /// <summary>
        /// Generate the palette
        /// </summary>
        protected override void PopulatePalette()
        {
            lock (colorCount)
            {
                lock (palette)
                {
                    if (colorCount == null) throw new Exception("Histogram was not ready");

                    // Use a random to order the colors
                    Random rd = new Random(666);
                    
                    // Cut the colors
                    var colors = Cut(colorCount.OrderBy(keypair => rd.NextDouble()).Select(keypair => keypair.Key));

                    // if there are still too many colors in the palette, order them by frequency and take the first 256
                    if (colors.Count() > PALETTE_MAX_COUNT)
                    {
                        colors = colors.OrderByDescending(color => colorCount[color]).Take(PALETTE_MAX_COUNT);
                    }
                    // Empty the palette
                    palette.Clear();
                    // Add every color to the palette
                    foreach (Color c in colors)
                    {
                        palette.Add(new Models.Color(c));
                    }
                }
            }
        }

        /// <summary>
        /// Eliminate colors from the histogram to get closer to the palette colors
        /// </summary>
        /// <param name="colorList">the histogram</param>
        /// <returns>The list of unique colors that will become the palette</returns>
        private IEnumerable<Color> Cut(IEnumerable<Color> colorList)
        {
            // If there's less or equal to the allowed amount of colors, we're done
            if (colorList.Count() <= PALETTE_MAX_COUNT) return colorList;

            // Create the comparers
            var hueComparer = new HueComparer();
            var saturationComparer = new SaturationComparer();
            var brightnessComparer = new BrightnessComparer();

            // Get the colors with unique hue, staturation and brightness
            var hue = colorList.Distinct(hueComparer);
            var saturation = colorList.Distinct(saturationComparer);
            var brightness = colorList.Distinct(brightnessComparer);

            var hueCount = hue.Count();
            var saturationCount = saturation.Count();
            var brightnessCount = brightness.Count();

            // If there are many distinct hue's in the image, make sure to include most of them and prepare to eliminate more
            if (hueCount > saturationCount && hueCount > brightnessCount)
            {
                return Cut2(hue, saturationComparer, brightnessComparer);
            }
            // If there are many distinct saturations in the image, make sure to inclode most of them and prepare to eliminate more
            else if (saturationCount > hueCount && saturationCount > brightnessCount)
            {
                return Cut2(saturation, hueComparer, brightnessComparer);
            }
            // If there are many distinct luminations in the image, make sure to inclode most of them and prepare to eliminate more
            else
            {
                return Cut2(brightness, hueComparer, saturationComparer);
            }
        }

        /// <summary>
        /// Second level of eliminating colors
        /// </summary>
        /// <param name="colorList">the histogram where at least 1 parameter is unique</param>
        /// <param name="comp1">the comparator of another parameter</param>
        /// <param name="comp2">the comparator of another parameter</param>
        /// <returns>The list of unique colors that will become the palette</returns>
        private IEnumerable<Color> Cut2(IEnumerable<Color> colorList, IEqualityComparer<Color> comp1, IEqualityComparer<Color> comp2)
        {
            // If there's less or equal to the allowed amount of colors, we're done
            if (colorList.Count() <= PALETTE_MAX_COUNT) return colorList;

            // Get the colors with unique parameters
            var comp1Distinct = colorList.Distinct(comp1);
            var comp2Distinct = colorList.Distinct(comp2);

            // Find the parameter that is most unique and include all it's distinct values
            if (comp1Distinct.Count() > comp2Distinct.Count())
            {
                return Cut3(comp1Distinct, comp2);
            }
            else
            {
                return Cut3(comp2Distinct, comp1);
            }
        }

        /// <summary>
        /// Final step in the elimination process
        /// </summary>
        /// <param name="colorList">the histogram with at least 2 parameters unique</param>
        /// <param name="comp">the comparator for the last non-unique parameter</param>
        /// <returns>The list of unique colors that will become the palette</returns>
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
