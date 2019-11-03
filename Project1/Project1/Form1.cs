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
            ComboBoxQuantizerSelection.SelectedIndex = 0;
            ComboBoxDithererSelection.SelectedIndex = 0;
            ComboBoxDrawerSelection.SelectedIndex = 0;
        }

        private Quantizer GetQuantizer()
        {
            Quantizer quantizer;
            if (ComboBoxQuantizerSelection.SelectedIndex == 0) quantizer = new SimpleQuantizer();
            else if (ComboBoxQuantizerSelection.SelectedIndex == 1) quantizer = new HSLQuantizer();
            else if (ComboBoxQuantizerSelection.SelectedIndex == 2) quantizer = new BWQuantizer();
            else throw new Exception("Invalid Quantizer option!");
            return quantizer;
        }

        private IDitherer GetDitherer()
        {
            IDitherer ditherer;
            if (ComboBoxDithererSelection.SelectedIndex == 0) ditherer = new FloydSteinbergDitherer();
            else if (ComboBoxDithererSelection.SelectedIndex == 1) ditherer = new JarvisJudiceNinkeDitherer();
            else throw new Exception("Invalid Ditherer option!");
            return ditherer;
        }

        private Drawer GetDrawer(ImageStore imageStore)
        {
            Drawer drawer;
            if (ComboBoxDrawerSelection.SelectedIndex == 0) drawer = new SimpleDrawer(imageStore);
            else if (ComboBoxDrawerSelection.SelectedIndex == 1) drawer = new SynchronousDitheredDrawer(imageStore);
            else if (ComboBoxDrawerSelection.SelectedIndex == 2) drawer = new AsynchronousDitheredDrawer(imageStore);
            else throw new Exception("Invalid Drawer option!");
            return drawer;
        }

        private void ButtonLoadImage_Click(object sender, EventArgs e)
        {
            if (OpenFileDialogImageLoader.ShowDialog()==DialogResult.OK)
            {
                Quantizer quantizer = GetQuantizer();
                IDitherer ditherer = GetDitherer();

                

                ProgressBarQuantization.Value = 0;
                ImagePath = OpenFileDialogImageLoader.FileName;
                LabelPath.Text = ImagePath;
                var image = new Bitmap(ImagePath);
                PictureBoxLoadedImage.Image = image;

                imageStore = new ImageStore(image, quantizer, ditherer);
                drawer = GetDrawer(imageStore);
                imageStore.InitFinished += AfterInit;
                drawer.ProgressUpdate += ProgressUpdate;
            }
        }

        private async void SetQuantizedImage()
        {
            PictureBoxQuantized.Image = await drawer.DrawAsync();
            LabelColorDistance.Text = ""+drawer.AverageError;
        }

        private async void SetPallet()
        {
            PictureBoxPallet.Image = await drawer.VisualizePalletAsync(PictureBoxPallet.Height, PictureBoxPallet.Width);
        }

        private async void AfterInit(object sender, EventArgs args)
        {
            PictureBoxHistogram.Image = await drawer.VisualizeHistogramAsync(PictureBoxHistogram.Height, PictureBoxHistogram.Width);

            SetPallet();
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

        private void Label1_Click(object sender, EventArgs e)
        {

        }
    }
}
