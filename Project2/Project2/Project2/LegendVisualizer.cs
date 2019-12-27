using BoundaryVisualizer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Project2
{
    public class LegendVisualizer
    {
        /// <summary>
        /// Event handler that will trigger when the checkbox next to an area name is toggled
        /// </summary>
        public event EventHandler<ToggleEventArgs> ToggleArea;

        /// <summary>
        /// Event handler that will trigger when the user has requested the areas to be transparent
        /// </summary>
        public event EventHandler<ToggleEventArgs> ToggleTransparency;

        /// <summary>
        /// Event handler that will trigger when the user has requested a bottom reference plate to be shown
        /// </summary>
        public event EventHandler<ToggleEventArgs> ToggleBottomPlate;

        private readonly List<CheckBox> checkBoxes = new List<CheckBox>();

        /// <summary>
        /// Visualize a legend for all the areas on screen and their values
        /// </summary>
        /// <param name="c">the canvas to draw on</param>
        /// <param name="legendItems">the items and their values to visualize</param>
        /// <param name="paddingTopBottom">the padding for the legend, default is 10</param>
        public void VisualizeLegend(Canvas c, List<LegendItem> legendItems, int paddingTopBottom = 10)
        {
            c.Children.Clear();
            checkBoxes.Clear();
            Rectangle r = new Rectangle
            {
                Fill = new SolidColorBrush(Color.FromArgb(128, 180, 180, 180)),
                Height = legendItems.Count * 20 + 2 * paddingTopBottom + 60,
                Width = Math.Max(legendItems.Select((legendItem) => (legendItem.Name.Length + legendItem.Value.ToString().Length)).Max() * 7 + 60, 210)
            };
            c.Children.Add(r);
            Canvas.SetTop(r, 10);
            Canvas.SetLeft(r, 0);
            AddLegend(c, legendItems, paddingTopBottom);
            AddSpecialOptions(c, legendItems, paddingTopBottom);
        }

        /// <summary>
        /// Add the special options like transparency and bottom plate
        /// </summary>
        /// <param name="c">the canvas to draw on</param>
        /// <param name="legendItems">the items and their values to visualize</param>
        /// <param name="paddingTopBottom">the padding for the legend, default is 10</param>
        private void AddSpecialOptions(Canvas c, List<LegendItem> legendItems, int paddingTopBottom)
        {
            CheckBox transparencyCheckbox = new CheckBox
            {
                IsChecked = false
            };
            TextBlock transparencyText = new TextBlock
            {
                Text = "Models Glass Effect"
            };
            CheckBox addbottomPlateCheckbox = new CheckBox
            {
                IsChecked = false
            };
            TextBlock bottomPlateText = new TextBlock
            {
                Text = "Add bottom reference plate"
            };
            c.Children.Add(transparencyCheckbox);
            Canvas.SetTop(transparencyCheckbox, 20 * legendItems.Count + 13 + paddingTopBottom);
            Canvas.SetLeft(transparencyCheckbox, 10);
            c.Children.Add(addbottomPlateCheckbox);
            Canvas.SetTop(addbottomPlateCheckbox, 20 * (legendItems.Count + 1) + 13 + paddingTopBottom);
            Canvas.SetLeft(addbottomPlateCheckbox, 10);
            c.Children.Add(transparencyText);
            Canvas.SetTop(transparencyText, 20 * legendItems.Count + 13 + paddingTopBottom);
            Canvas.SetLeft(transparencyText, 35);
            c.Children.Add(bottomPlateText);
            Canvas.SetTop(bottomPlateText, 20 * (legendItems.Count + 1) + 13 + paddingTopBottom);
            Canvas.SetLeft(bottomPlateText, 35);
            transparencyCheckbox.Click += TransparencyCheckboxClick;
            addbottomPlateCheckbox.Click += AddbottomPlateCheckboxClick;
        }

        /// <summary>
        /// The add bottom plate checkbox was ticked, notify all listeners so the viewport can be updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddbottomPlateCheckboxClick(object sender, System.Windows.RoutedEventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            ToggleBottomPlate?.Invoke(sender, new ToggleEventArgs(check.IsChecked, checkBoxes.IndexOf(check)));
        }

        /// <summary>
        /// The transparency checkbox was ticked, notify all listeners so the viewport can be updated
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TransparencyCheckboxClick(object sender, System.Windows.RoutedEventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            ToggleTransparency?.Invoke(sender, new ToggleEventArgs(check.IsChecked, checkBoxes.IndexOf(check)));
        }

        /// <summary>
        /// A checkbox was ticked or unticked, notify the viewport so the element can be (un)hidden
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckboxClick(object sender, System.Windows.RoutedEventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            ToggleArea?.Invoke(sender, new ToggleEventArgs(check.IsChecked, checkBoxes.IndexOf(check)));
        }

        /// <summary>
        /// Add the actual parts that make up the legend
        /// </summary>
        /// <param name="c">the canvas to draw on</param>
        /// <param name="legendItems">the items and their values to visualize</param>
        /// <param name="paddingTopBottom">the padding for the legend, default is 10</param>
        private void AddLegend(Canvas c, List<LegendItem> legendItems, int paddingTopBottom)
        {
            for (int i = 0; i < legendItems.Count; i++)
            {
                CheckBox check = new CheckBox
                {
                    IsChecked = true
                };
                checkBoxes.Add(check);
                check.Click += CheckboxClick;
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
                c.Children.Add(check);
                Canvas.SetTop(check, 20 * i + 13 + paddingTopBottom);
                Canvas.SetLeft(check, 10);
                Canvas.SetTop(colorRect, 20 * i + 12 + paddingTopBottom);
                Canvas.SetLeft(colorRect, 35);
                Canvas.SetTop(text, 20 * i + 13 + paddingTopBottom);
                Canvas.SetLeft(text, 65);
            }
        }
    }

    public class ToggleEventArgs : EventArgs
    {
        public bool State { get; set; }
        public int Index { get; set; }

        public ToggleEventArgs(bool? state, int index)
        {
            State = (bool)state;
            Index = index;
        }
    }
}
