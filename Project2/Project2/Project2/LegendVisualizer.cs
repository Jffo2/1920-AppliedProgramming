using BoundaryVisualizer.Models;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Project2
{
    public class LegendVisualizer
    {
        public static void VisualizeLegend(Canvas c, List<LegendItem> legendItems)
        {
            c.Children.Clear();
            Rectangle r = new Rectangle
            {
                Fill = new SolidColorBrush(Color.FromArgb(128, 180, 180, 180)),
                Height = legendItems.Count * 20,
                Width = 200
            };
            c.Children.Add(r);
            Canvas.SetTop(r, 0);
            Canvas.SetLeft(r, 0);
            for (int i = 0; i < legendItems.Count; i++)
            {
                Rectangle colorRect = new Rectangle
                {
                    Fill = new SolidColorBrush(legendItems[i].Color),
                    Height = 16,
                    Width = 16
                };
                TextBlock text = new TextBlock
                {
                    Text = legendItems[i].Name + ": " + legendItems[i].Value
                };
                c.Children.Add(colorRect);
                c.Children.Add(text);
                Canvas.SetTop(colorRect, 20 * i +2);
                Canvas.SetLeft(colorRect, 10);
                Canvas.SetTop(text, 20 * i);
                Canvas.SetLeft(text, 30);
            }
        }
    }
}
