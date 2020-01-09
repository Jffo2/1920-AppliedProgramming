using BoundaryVisualizer.Data;
using BoundaryVisualizer.Data.DataProviders;
using BoundaryVisualizer.Logic;
using Microsoft.Win32;
using System;
using System.Linq;
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

        private Visualizer Visualizer;

        private readonly LegendVisualizer legendVisualizer;
        private Model3DGroup modelgroup;

        public MainWindow()
        {
            // Create a legendVisualizer and start listening on the events
            legendVisualizer = new LegendVisualizer();
            legendVisualizer.ToggleArea += ToggleCheckbox;
            legendVisualizer.ToggleTransparency += ToggleTransparency;
            legendVisualizer.ToggleBottomPlate += ToggleBottomPlate;
            InitializeComponent();
        }

        /// <summary>
        /// User has requested the bottom plate to be toggled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleBottomPlate(object sender, ToggleEventArgs e)
        {
            if (e.State)
            {
                var plate = GenerateBottomPlate();
                // Make the bototm plate big and translate it so it's center is in the model origin
                Transform3DGroup transformGroup = new Transform3DGroup();
                transformGroup.Children.Add(new ScaleTransform3D(modelgroup.Bounds.SizeX * 2, modelgroup.Bounds.SizeY*2, 1));
                transformGroup.Children.Add(new TranslateTransform3D(modelgroup.Bounds.SizeX *-1 , modelgroup.Bounds.SizeY *-0.25, modelgroup.Bounds.SizeZ / -2 +1));
                Viewport.Children.Add(new ModelVisual3D
                {
                    Content = new GeometryModel3D
                    {
                        Geometry = plate,
                        Material = new DiffuseMaterial(new SolidColorBrush(Color.FromArgb(255, 100, 100, 100))),
                        Transform = transformGroup
                    }
                });
            }
            else
            {
                Viewport.Children.Remove(Viewport.Children.Last());
            }
        }

        /// <summary>
        /// Create a MeshGeomtry3D for the bottom plate
        /// </summary>
        /// <returns>the mesh for the bottom plate</returns>
        private MeshGeometry3D GenerateBottomPlate()
        {
            var mesh = new MeshGeometry3D();
            Point3DCollection pointsCollection = new Point3DCollection();
            Int32Collection triangleIndices = new Int32Collection();
            pointsCollection.Add(new Point3D(0, 0, 0));
            pointsCollection.Add(new Point3D(1, 0, 0));
            pointsCollection.Add(new Point3D(1, 1, 0));
            pointsCollection.Add(new Point3D(0, 1, 0));
            pointsCollection.Add(new Point3D(0, 0, -1));
            pointsCollection.Add(new Point3D(1, 0, -1));
            pointsCollection.Add(new Point3D(1, 1, -1));
            pointsCollection.Add(new Point3D(0, 1, -1));
            triangleIndices.Add(0);
            triangleIndices.Add(1);
            triangleIndices.Add(2);
            triangleIndices.Add(6);
            triangleIndices.Add(5);
            triangleIndices.Add(4);
            triangleIndices.Add(0);
            triangleIndices.Add(2);
            triangleIndices.Add(3);
            triangleIndices.Add(7);
            triangleIndices.Add(6);
            triangleIndices.Add(4);
            mesh.Positions = pointsCollection;
            mesh.TriangleIndices = triangleIndices;
            return mesh;
        }

        /// <summary>
        /// The user has requested transparency mode to be toggled
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleTransparency(object sender, ToggleEventArgs e)
        {
            foreach (object m in modelgroup.Children)
            {
                // If the model is not a Model3DGroup, carry on
                if (!(m is Model3DGroup group)) continue;
                foreach (GeometryModel3D geometryModel in group.Children)
                {
                    var c = ((DiffuseMaterial)geometryModel.Material).Color;
                    ((DiffuseMaterial)geometryModel.Material).Color = new Color
                    {
                        A = (byte)(e.State == true ? 200 : 255),
                        R = c.R,
                        G = c.G,
                        B = c.B
                    };
                }
            }
        }

        /// <summary>
        /// Show a FileExplorer and start the visualization process of the selected Json file
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                Task.Run(() =>
                {
                    try
                    {

                        Visualizer = new Visualizer(fileGeoJsonLoader, this.Dispatcher, GetRequiredDataProvider(), MODEL_SCALE);
                        if (Visualizer.IsVisualizerReady) RenderModel(null, null);
                        else Visualizer.OnVisualizerReady += RenderModel;
                    } catch (System.IO.FileFormatException)
                    {
                        MessageBox.Show("Error in GeoJson file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            }

        }

        /// <summary>
        /// Use a select statement to find the correct DataProvider matching the Select object
        /// </summary>
        /// <returns>the dataprovider requested by the user</returns>
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

        /// <summary>
        /// The user has requested to toggle the visibility of an area
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ToggleCheckbox(object sender, ToggleEventArgs e)
        {
            // To hide the object, change it's scale to 0
            if (!e.State)
            {
                ((Model3DGroup)((ModelVisual3D)Viewport.Children[0]).Content).Children[e.Index].Transform = new ScaleTransform3D(0, 0, 0);
            }
            else
            {
                ((Model3DGroup)((ModelVisual3D)Viewport.Children[0]).Content).Children[e.Index].Transform = new ScaleTransform3D(1, 1, 1);
            }
        }

        /// <summary>
        /// Add the model to the scene after the visualizer is done creating the models
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void RenderModel(object sender, EventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("Rendering model");
            Dispatcher.Invoke(() =>
            {
                modelgroup = Visualizer.CreateModelGroup();
                System.Diagnostics.Debug.WriteLine(modelgroup.Children.Count);

                var maxSize = (modelgroup.Bounds.SizeX > modelgroup.Bounds.SizeY) ? modelgroup.Bounds.SizeX : modelgroup.Bounds.SizeY;

                modelgroup.Transform = new TranslateTransform3D(modelgroup.Bounds.SizeX / -2.0, modelgroup.Bounds.SizeY / 2.0, modelgroup.Bounds.SizeZ / -2.0);

                ModelVisual3D modelVisual3D = new ModelVisual3D
                {
                    Content = (modelgroup)
                };

                AddLights(modelgroup);

                Viewport.Children.Clear();
                Viewport.Children.Add(modelVisual3D);

                Viewport.Camera.Position = new Point3D(0, 0, -10);
                Viewport.ZoomExtents();
                Viewport.Camera.LookDirection = new Vector3D(modelgroup.Bounds.SizeX / 2.0 + modelgroup.Bounds.X - Camera.Position.X, modelgroup.Bounds.SizeY / 2.0 + modelgroup.Bounds.Y - Camera.Position.Y, modelgroup.Bounds.SizeZ / 2.0 + modelgroup.Bounds.Z - Camera.Position.Z);

                legendVisualizer.VisualizeLegend(Canvas, Visualizer.LegendItems);
            });
        }

        /// <summary>
        /// Add lights to the scene
        /// </summary>
        /// <param name="modelgroup">the modelgroup to add the lights to</param>
        private void AddLights(Model3DGroup modelgroup)
        {
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

            modelgroup.Children.Add(dirLight1);
            modelgroup.Children.Add(dirlight2);
            modelgroup.Children.Add(dirlight3);
            modelgroup.Children.Add(dirLight4);
        }
    }
}
