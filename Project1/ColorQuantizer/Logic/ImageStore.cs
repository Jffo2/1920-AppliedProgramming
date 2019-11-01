using ImageProcessing.Logic.Ditherers;
using ImageProcessing.Logic.Quantizers;
using ImageProcessing.Util;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ImageProcessing.Logic
{
    public class ImageStore
    {
        private Bitmap image;
        private IQuantizer quantizer;
        private IDitherer ditherer;

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

        public IDitherer Ditherer
        {
            get
            {
                return ditherer;
            }
            set
            {
                ditherer = value ?? throw new ArgumentNullException();
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


        public ImageStore(Bitmap image, IQuantizer quantizer, IDitherer ditherer)
        {
            Image = Cloner.DeepClone(image);
            Quantizer = quantizer;
            Ditherer = ditherer;
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
                var colorCount = new ConcurrentDictionary<Color, int>();
                Bitmap i;
                lock (Image)
                {
                    i = Cloner.DeepClone(Image);
                }
                
                var totalHeight = i.Height;
                var totalWidth = i.Width;
                BitmapData data = i.LockBits(new Rectangle(0, 0, totalWidth, totalHeight), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                var offset = data.Scan0.ToInt64();
                int dataStride = data.Stride;

                var tasks = PopulateAll(totalHeight, totalWidth, offset, dataStride, colorCount);
                
                Task.WaitAll(tasks);
                return new Histogram(new Dictionary<Color, int>(colorCount));
            });
        }

        private Task[] PopulateAll(int totalHeight, int totalWidth, long offset, int stride, ConcurrentDictionary<Color, int> colorCount)
        {
            Task[] tasks = new Task[totalHeight];
            for (int height = 0; height < totalHeight; height++)
            {
                var h = height;
                Task t = Task.Run(() =>
                {
                    int[] line = new int[totalWidth];

                    Marshal.Copy(new IntPtr(offset), line, 0, totalWidth);

                    for (int width = 0; width < totalWidth; width++)
                    {
                        Color color = Color.FromArgb(line[width]);

                        quantizer.AddColor(color);

                        if (!colorCount.ContainsKey(color))
                        {
                            colorCount.TryAdd(color, 1);
                        }
                        else
                        {
                            colorCount[color]++;
                        }
                    }
                    offset += stride;
                });
                tasks[height] = t;
            }
            return tasks;
        }

    }
}
