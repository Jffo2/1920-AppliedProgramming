using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using ImageProcessing.Logic.Quantizers;
using ImageProcessing.Util;

namespace ImageProcessing.Logic
{
    public class ImageStore
    {
        private Bitmap image;
        private IQuantizer quantizer;

        public bool QuantizerReady { get; private set; }

        public event EventHandler<EventArgs> InitFinished;

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

        public IQuantizer Quantizer
        {
            get
            {
                return quantizer;
            }
            set
            {
                quantizer = value ?? throw new ArgumentNullException();
            }
        }

        public Histogram Histogram { get; set; }


        public ImageStore(Bitmap image, IQuantizer quantizer)
        {
            Image = Cloner.DeepClone(image);
            Quantizer = quantizer;
        }

        public async void Init()
        {
            Histogram = await CountColors();
            QuantizerReady = true;
            InitFinished?.Invoke(this, new EventArgs());
        }

        public Task<Histogram> CountColors()
        {
            return Task.Run(() =>
            {
                var colorCount = new Dictionary<Models.Color, int>();
                Bitmap i;
                lock (Image)
                {
                    i = Cloner.DeepClone(Image);
                }
                    for (int height = 0; height < i.Height; height++)
                    {
                        for (int width = 0; width < i.Width; width++)
                        {
                            var pixel = i.GetPixel(width, height);
                            Models.Color c = new Models.Color(pixel.R, pixel.G, pixel.B);
                            quantizer.AddColor(c);
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
                
                return new Histogram(colorCount);
            });
        }

    }
}
