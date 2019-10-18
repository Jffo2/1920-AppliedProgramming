using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ColorQuantizer;

namespace Project1
{
    public partial class Form1 : Form
    {
        private string ImagePath;
        private ImageStore colorQuantizer;

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
                ImagePath = OpenFileDialogImageLoader.FileName;
                LabelPath.Text = ImagePath;
                var image = new Bitmap(ImagePath);
                PictureBoxLoadedImage.Image = image;
                colorQuantizer = new ImageStore(image);
                while (colorQuantizer.Histogram==null)
                {
                    Application.DoEvents();
                }
                VisualizeHistogram();
            }
        }

        private async void VisualizeHistogram()
        {
            pictureBox1.Image = await colorQuantizer.Histogram.VisualizeAsync(pictureBox1.Height, pictureBox1.Width);
        }
    }
}
