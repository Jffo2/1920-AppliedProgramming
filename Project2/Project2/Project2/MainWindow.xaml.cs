using BoundaryVisualizer.Data;
using BoundaryVisualizer.Logic;
using Microsoft.Win32;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace Project2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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
                PolyGonImageBox.Source = null;
                //MockGeoJsonLoader fileGeoJsonLoader = new MockGeoJsonLoader();
                await Task.Run(() =>
                {
                    Visualizer = new Visualizer(fileGeoJsonLoader,this.Dispatcher);
                });
                TextBoxChangeModel.TextChanged += ChangeModel;
                TextBoxChangeModel.Text = "0";
                ChangeModel(null, null);
            }

        }

        private void ChangeModel(object sender, EventArgs e)
        {
            try
            {
                DirectionalLight DirLight1 = new DirectionalLight();
                DirLight1.Color = Colors.White;
                DirLight1.Direction = new Vector3D(0, 0, -1);

                PerspectiveCamera Camera1 = new PerspectiveCamera
                {
                    FarPlaneDistance = 50000,
                    NearPlaneDistance = 20,
                    FieldOfView = 45,
                    Position = new Point3D(1200, 1200, 2400),
                    LookDirection = new Vector3D(0,-1,-1),
                    UpDirection = new Vector3D(0,1,0),
                };
                PolyGonImageBox.Source = new BitmapImage(new Uri(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase) + $"./model{TextBoxChangeModel.Text}.png"));
                Model3DGroup modelgroup = Visualizer.CreateModelGroup(int.Parse(TextBoxChangeModel.Text));
                modelgroup.Children[0].Transform = new TranslateTransform3D(-200, -200, -200);
                modelgroup.Transform = new ScaleTransform3D(-1, -1, -1);
                modelgroup.Children.Add(DirLight1);
                ModelVisual3D modelVisual3D = new ModelVisual3D();
                modelVisual3D.Content = (modelgroup);
                //Viewport.Camera = Camera1;
                Viewport.Children.Clear();
                Viewport.Children.Add(modelVisual3D);
                Viewport.Height = 500;
                Viewport.Width = 500;
                
            }
            catch (Exception ex)
            {
                if (TextBoxChangeModel.Text != "")
                {
                    MessageBox.Show("Invalid");
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }
    }
}
