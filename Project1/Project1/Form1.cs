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
            // Make sure the value of the comboboxes cannot be null
            ComboBoxQuantizerSelection.SelectedIndex = 0;
            ComboBoxDithererSelection.SelectedIndex = 0;
            ComboBoxDrawerSelection.SelectedIndex = 0;
        }

        /// <summary>
        /// Find the quantizer selected in the combobox
        /// </summary>
        /// <returns>the requested quantizer</returns>
        private Quantizer GetQuantizer()
        {
            Quantizer quantizer;
            if (ComboBoxQuantizerSelection.SelectedIndex == 0) quantizer = new SimpleQuantizer();
            else if (ComboBoxQuantizerSelection.SelectedIndex == 1) quantizer = new HSLQuantizer();
            else if (ComboBoxQuantizerSelection.SelectedIndex == 2) quantizer = new BWQuantizer();
            else throw new Exception("Invalid Quantizer option!");
            return quantizer;
        }

        /// <summary>
        /// Find the ditherer selected in the combobox
        /// </summary>
        /// <returns>the requested ditherer</returns>
        private IDitherer GetDitherer()
        {
            IDitherer ditherer;
            if (ComboBoxDithererSelection.SelectedIndex == 0) ditherer = new FloydSteinbergDitherer();
            else if (ComboBoxDithererSelection.SelectedIndex == 1) ditherer = new JarvisJudiceNinkeDitherer();
            else throw new Exception("Invalid Ditherer option!");
            return ditherer;
        }

        /// <summary>
        /// Find the drawer selected in the combobox
        /// </summary>
        /// <param name="imageStore">the imagestore the drawer will be drawing</param>
        /// <returns>the requested drawer</returns>
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
                ButtonSaveImage.Enabled = false;
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

        /// <summary>
        /// Asks the drawer to generate a bitmap and set the PictureBox to said image
        /// </summary>
        private async void SetQuantizedImage()
        {
            PictureBoxQuantized.Image = await drawer.DrawAsync();
            LabelColorDistance.Text = ""+drawer.AverageError;
            ButtonSaveImage.Enabled = true;
        }

        /// <summary>
        /// Asks the drawer to generate a bitmap that contains a visual representation of the palette and set the PictureBox to display this image
        /// </summary>
        private async void SetPallet()
        {
            PictureBoxPallet.Image = await drawer.VisualizePalletAsync(PictureBoxPallet.Height, PictureBoxPallet.Width);
        }

        /// <summary>
        /// Called after the imageStore has finished initializing the quantizer and histogram
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private async void AfterInit(object sender, EventArgs args)
        {
            // Draw the histogram
            PictureBoxHistogram.Image = await drawer.VisualizeHistogramAsync(PictureBoxHistogram.Height, PictureBoxHistogram.Width);

            SetPallet();
            SetQuantizedImage();
        }

        /// <summary>
        /// Update the value of the progressbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
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

        private void ButtonSaveImage_Click(object sender, EventArgs e)
        {
            if (saveFileDialog1.ShowDialog()==DialogResult.OK)
            {
                PictureBoxQuantized.Image.Save(saveFileDialog1.FileName);
            }
        }
    }
}
