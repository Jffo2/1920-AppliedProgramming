using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ImageProcessing.Presentation
{
    public interface IDrawer
    {
        Bitmap Draw();
        Bitmap VisualizeHistogram(int height, int width);
        Bitmap VisualizePallet(int height, int width);
    }
}
