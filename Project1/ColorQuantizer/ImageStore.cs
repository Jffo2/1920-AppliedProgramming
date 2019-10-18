using ColorQuantizer.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorQuantizer
{
    public class ImageStore
    {
        private Bitmap image;
        public Bitmap Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
                Init();
            }
        }
        public Histogram Histogram { get; set; }


        public ImageStore(Bitmap image)
        {
            Image = Cloner.DeepClone(image); ;
        }

        public async void Init()
        {
            Histogram = await CountColors();
        }

        public Task<Histogram> CountColors()
        {
            return Task.Run(() =>
            {
                var colorCount = new Dictionary<Color, int>();
                lock (Image)
                {
                    for (int height = 0; height < Image.Height; height++)
                    {
                        for (int width = 0; width < Image.Width; width++)
                        {
                            var pixel = Image.GetPixel(width, height);
                            Color c = new Color(pixel.R, pixel.G, pixel.B);
                            if (!colorCount.ContainsKey(c))
                            {
                                colorCount.Add(c, 1);
                            }
                            else
                            {
                                colorCount[c]++;
                                Console.WriteLine(c.ToString() + ": " + colorCount[c] + "\r\n");
                            }
                        }
                    }
                }
                return new Histogram(colorCount);
            });
        }


    }
}
