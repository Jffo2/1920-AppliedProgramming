using BoundaryVisualizer.Data;
using BoundaryVisualizer.Data.DataProviders;
using BoundaryVisualizer.Logic;
using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Project2
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public const int MODEL_SCALE = 10;

        Visualizer Visualizer;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void BrowseGeoJsonFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                Filter = "OSM Boundary maps|*.GeoJson",
                Title = "Please select a boundary map file",
            };
            if (openFileDialog.ShowDialog() == true)
            {
                FileGeoJsonLoader fileGeoJsonLoader = new FileGeoJsonLoader(openFileDialog.FileName);
                await Task.Run(() =>
                {
                    Visualizer = new Visualizer(fileGeoJsonLoader, this.Dispatcher, GetRequiredDataProvider(), MODEL_SCALE);
                    if (Visualizer.IsVisualizerReady) RenderModel(null, null);
                    else Visualizer.OnVisualizerReady += RenderModel;
                });
            }

        }

        private DataProvider GetRequiredDataProvider()
        {
            int selectedIndex = 0;
            Dispatcher.Invoke(() =>
            {
                selectedIndex = HeightSelector.SelectedIndex;
            });
            switch (selectedIndex)
            {
                case 0:
                    return new PopulationProviderBelgianProvinces();
                case 1:
                    return new PercentageMarriedCouplesProviderBelgianProvinces();
                default:
                    return new PopulationProviderBelgianProvinces();
            }

        }

        private void RenderModel(object sender, EventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("Rendering model");
            Dispatcher.Invoke(() =>
            {

                Model3DGroup modelgroup = Visualizer.CreateModelGroup();

                DirectionalLight dirLight1 = new DirectionalLight
                {
                    Color = Colors.White,
                    Direction = new Vector3D(0, -1, -8)
                };
                DirectionalLight dirlight2 = new DirectionalLight
                {
                    Color = Colors.White,
                    Direction = new Vector3D(0, 1, 8)
                };
                DirectionalLight dirlight3 = new DirectionalLight
                {
                    Color = Colors.White,
                    Direction = new Vector3D(1, 1, 8)
                };
                DirectionalLight dirLight4 = new DirectionalLight
                {
                    Color = Colors.White,
                    Direction = new Vector3D(1, -1, -8)
                };
                var maxSize = (modelgroup.Bounds.SizeX > modelgroup.Bounds.SizeY) ? modelgroup.Bounds.SizeX : modelgroup.Bounds.SizeY;

                modelgroup.Transform = new TranslateTransform3D(modelgroup.Bounds.SizeX / -2.0, modelgroup.Bounds.SizeY / 2.0, modelgroup.Bounds.SizeZ / -2.0);
                modelgroup.Children.Add(dirLight1);
                modelgroup.Children.Add(dirlight2);
                modelgroup.Children.Add(dirlight3);
                modelgroup.Children.Add(dirLight4);

                ModelVisual3D modelVisual3D = new ModelVisual3D
                {
                    Content = (modelgroup)
                };
                Camera.Position = new Point3D(0, 0, maxSize * -1 * (MODEL_SCALE/5));
                Viewport.Children.Clear();
                Viewport.Children.Add(modelVisual3D);
                Viewport.FixedRotationPoint = new Point3D(modelgroup.Bounds.SizeX / 2.0 + modelgroup.Bounds.X, modelgroup.Bounds.SizeY / 2.0 + modelgroup.Bounds.Y, modelgroup.Bounds.SizeZ / 2.0 + modelgroup.Bounds.Z);
                Viewport.FixedRotationPointEnabled = true;
                Camera.LookDirection = new Vector3D(modelgroup.Bounds.SizeX / 2.0 + modelgroup.Bounds.X - Camera.Position.X, modelgroup.Bounds.SizeY / 2.0 + modelgroup.Bounds.Y - Camera.Position.Y, modelgroup.Bounds.SizeZ / 2.0 + modelgroup.Bounds.Z - Camera.Position.Z);
            });
        }
    }
}
