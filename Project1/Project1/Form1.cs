using ImageProcessing.Logic;
using ImageProcessing.Logic.Ditherers;
using ImageProcessing.Logic.Quantizers;
using ImageProcessing.Presentation;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Project1
{
    public partial class Form1 : Form
    {
        private string ImagePath;
        private ImageStore imageStore;
        private Drawer drawer;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void ButtonLoadImage_Click(object sender, EventArgs e)
        {
            if (OpenFileDialogImageLoader.ShowDialog()==DialogResult.OK)
            {
                ProgressBarQuantization.Value = 0;
                ImagePath = OpenFileDialogImageLoader.FileName;
                LabelPath.Text = ImagePath;
                var image = new Bitmap(ImagePath);
                PictureBoxLoadedImage.Image = image;
                imageStore = new ImageStore(image, new SimpleQuantizer(),new FloydSteinbergDitherer());
                drawer = new SynchronousDitheredDrawer(imageStore);
                imageStore.InitFinished += AfterInit;
                drawer.ProgressUpdate += ProgressUpdate;
            }
        }

        private async void SetQuantizedImage()
        {
            PictureBoxQuantized.Image = await drawer.DrawAsync();
            LabelColorDistance.Text = ""+drawer.AverageError;
        }

        private async void AfterInit(object sender, EventArgs args)
        {
            PictureBoxHistogram.Image = await drawer.VisualizeHistogramAsync(PictureBoxHistogram.Height, PictureBoxHistogram.Width);
            // Quantizer must be populated first
            SetQuantizedImage();
        }

        private void ProgressUpdate(object sender, ProgressEventArgs args)
        {
            ProgressBarQuantization.Invoke(new Action(() =>
            {
                ProgressBarQuantization.Value = (ProgressBarQuantization.Value>args.Progress)? ProgressBarQuantization.Value : args.Progress;
            }));
        }
    }
}
