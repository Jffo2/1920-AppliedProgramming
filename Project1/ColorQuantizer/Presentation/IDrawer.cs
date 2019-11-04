using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace ImageProcessing.Presentation
{
    public interface IDrawer
    {
        /// <summary>
        /// Draws a bitmap while applying dithering and/or quantization
        /// </summary>
        /// <returns>the altered bitmap</returns>
        Bitmap Draw();

        /// <summary>
        /// Generate a bitmap containing a visual representation of the histogram
        /// </summary>
        /// <param name="height">height of the histogram to be drawn in pixels</param>
        /// <param name="width">width of the histogram to be drawn in pixels</param>
        /// <returns>a bitmap visualizing the histogram</returns>
        Bitmap VisualizeHistogram(int height, int width);

        /// <summary>
        /// Generate a bitmap containing a visual representation of the palette
        /// </summary>
        /// <param name="height">height of the palette do be drawn in pixels</param>
        /// <param name="width">width of the histogram to be drawn in pixels</param>
        /// <returns>a bitmap visualizing the palette</returns>
        Bitmap VisualizePallet(int height, int width);
    }
}
