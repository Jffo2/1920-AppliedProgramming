using BoundaryVisualizer.Data;
using BoundaryVisualizer.Logic;
using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Project2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
        }

        private void BrowseGeoJsonFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "OSM Boundary maps|*.GeoJson",
                Title = "Please select a boundary map file",
            };
            if (openFileDialog.ShowDialog() == true)
            {
                FileGeoJsonLoader fileGeoJsonLoader = new FileGeoJsonLoader(openFileDialog.FileName);
                Visualizer visualizer = new Visualizer(new MockGeoJsonLoader());
                TextBoxChangeModel.TextChanged += ChangeModel;
                TextBoxChangeModel.Text = "1";
            }

        }

        private void ChangeModel(object sender, EventArgs e)
        {
            try
            {
                PolyGonImageBox.Source = new BitmapImage(new Uri(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + $"./model{TextBoxChangeModel.Text}.png"));
            }
            catch (Exception ex)
            {
                if (TextBoxChangeModel.Text != "")
                    MessageBox.Show("Invalid");
            }
        }
    }
}
