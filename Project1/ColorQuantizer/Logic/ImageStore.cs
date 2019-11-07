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
        #region private variables
        private Bitmap image;
        private IQuantizer quantizer;
        private IDitherer ditherer;
        #endregion

        /// <summary>
        /// A boolean representing if the quantizer is ready to quantize pixels (if all colors have been registered)
        /// </summary>
        public bool QuantizerReady { get; private set; }

        /// <summary>
        /// Event that triggers when the ImageStore has finished initializing the histogram and quantizer
        /// </summary>
        public event EventHandler<EventArgs> InitFinished;

        /// <summary>
        /// The Image the ImageStore is storing
        /// </summary>
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

        /// <summary>
        /// The ditherer the ImageStore uses
        /// </summary>
        public IDitherer Ditherer
        {
            get
            {
                return ditherer;
            }
            private set
            {
                ditherer = value ?? throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// The quantizer the ImageStore uses
        /// </summary>
        public IQuantizer Quantizer
        {
            get
            {
                return quantizer;
            }
            private set
            {
                quantizer = value ?? throw new ArgumentNullException();
            }
        }

        /// <summary>
        /// The histogram of the image
        /// </summary>
        public Histogram Histogram { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="image">the image this class is storing</param>
        /// <param name="quantizer">the quantizer that will be used</param>
        /// <param name="ditherer">the ditherer that will be used</param>
        public ImageStore(Bitmap image, IQuantizer quantizer, IDitherer ditherer)
        {
            Image = Cloner.DeepClone(image);
            Quantizer = quantizer;
            Ditherer = ditherer;
        }

        /// <summary>
        /// Init method, initializes the histogram and quantizer then triggers the event
        /// </summary>
        /// <see cref="InitFinished"/>
        public async void Init()
        {
            Histogram = await CountColors();
            QuantizerReady = true;
            InitFinished?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Counts the colors and registers them to the quantizer
        /// </summary>
        /// <returns>a histogram for RGB values in the image</returns>
        public Task<Histogram> CountColors()
        {
            // Count colors on a different thread
            return Task.Run(() =>
            {
                var colorCount = new ConcurrentDictionary<Color, int>();
                // Lock the image first, to make it threadsafe
                lock (Image)
                {
                    var totalHeight = Image.Height;
                    var totalWidth = Image.Width;

                    // Prepare bits for reading
                    BitmapData data = Image.LockBits(new Rectangle(0, 0, totalWidth, totalHeight), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
                    var offset = data.Scan0.ToInt64();
                    int dataStride = data.Stride;

                    // Generate a bunch of "threads" that can each read a line from the image, to read a lot of lines at the same time
                    var tasks = PopulateAll(totalHeight, totalWidth, offset, dataStride, colorCount);
                
                    // Wait for the threads to finish
                    Task.WaitAll(tasks);
                    Image.UnlockBits(data);
                }
                return new Histogram(new Dictionary<Color, int>(colorCount));
            });
        }

        /// <summary>
        /// Spawns a lot of threads that will each read one line from an image
        /// </summary>
        /// <param name="totalHeight">the height of the image</param>
        /// <param name="totalWidth">the width of the image</param>
        /// <param name="offset">a pointer in memory to where the image begins</param>
        /// <param name="stride">the stride of the image in memory</param>
        /// <param name="colorCount">the dictionary representing the histogram, this will be populated</param>
        /// <returns></returns>
        private Task[] PopulateAll(int totalHeight, int totalWidth, long offset, int stride, ConcurrentDictionary<Color, int> colorCount)
        {
            long modifiedOffset = offset;
            // Keep a list of all tasks
            Task[] tasks = new Task[totalHeight];

            for (int height = 0; height < totalHeight; height++)
            {
                // Copy the value, otherwise this is not threadsafe
                var offsett = modifiedOffset;
                Task t = Task.Run(() =>
                {
                    int[] line = new int[totalWidth];

                    Marshal.Copy(new IntPtr(offsett), line, 0, totalWidth);

                    // Register the row to the quantizer and add it to the histogram
                    AddRow(totalWidth, line, colorCount);
                    
                });
                modifiedOffset += stride;
                // Add the task to the list
                tasks[height] = t;
            }

            // Return the list of current tasks
            return tasks;
        }

        /// <summary>
        /// Add each color in a row to the histogram and register them to the quantizer
        /// </summary>
        /// <param name="totalWidth">the width of the row</param>
        /// <param name="line">the row</param>
        /// <param name="colorCount">the histogram to which we will add</param>
        private void AddRow(int totalWidth, int[] line, ConcurrentDictionary<Color, int> colorCount)
        {
            for (int width = 0; width < totalWidth; width++)
            {
                Color color = Color.FromArgb(line[width]);

                quantizer.AddColor(color);

                colorCount.AddOrUpdate(color, 1, (c, count) => count + 1);
            }
        }
    }
}
