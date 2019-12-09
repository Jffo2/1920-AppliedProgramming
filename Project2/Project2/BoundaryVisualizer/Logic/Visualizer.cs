using BoundaryVisualizer.Data;
using BoundaryVisualizer.Models;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Windows.Media;
using System;

namespace BoundaryVisualizer.Logic
{
    public class Visualizer
    {
        /// <summary>
        /// Event that triggers when the visualizer is ready to generate the model group
        /// </summary>
        public event EventHandler<EventArgs> OnVisualizerReady;

        /// <summary>
        /// Bool representing if the visualizer is ready to generate the model group
        /// </summary>
        public bool IsVisualizerReady { get; private set; }


        private readonly List<Area3D> areas = new List<Area3D>();
        private readonly Dispatcher dispatcher;
        private readonly float scale;
        private readonly FeatureCollection featureCollection;
        private readonly DataProvider dataProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="geoJsonLoader">a geojson loader object used to load in the data</param>
        /// <param name="dispatcher">the dispatcher to dispatch tasks to the main thread</param>
        /// <param name="dataProvider">the data provider used to get the height for the models</param>
        /// <param name="scale">the scale of the model</param>
        public Visualizer(IGeoJsonLoader geoJsonLoader, Dispatcher dispatcher,DataProvider dataProvider, float scale)
        {
            IsVisualizerReady = false;
            this.scale = scale;
            this.dispatcher = dispatcher;
            this.featureCollection = geoJsonLoader.Load();
            this.dataProvider = dataProvider;
            // Start setting up after data has been loaded by the dataProvider
            if (!dataProvider.IsDataProviderReady)
            {
                dataProvider.DataProviderReady += DoSetup;
            }
            else DoSetup(null, null);
        }

        /// <summary>
        /// Setup the visualizer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void DoSetup(object sender, EventArgs args)
        {
            int index = 0;
            foreach (Feature feature in featureCollection.Features)
            {
                // Output the name
                if (feature.Properties.ContainsKey("name"))
                    System.Diagnostics.Debug.WriteLine(feature.Properties["name"]);
                // Get the height from the dataProvider
                var height = (float)dataProvider.GetValue(feature.Properties);
                System.Diagnostics.Debug.WriteLine(height);
                // Generate the model
                Area3D area = new Area3D((MultiPolygon)feature.Geometry, dispatcher, scale, height);
                index++;
                areas.Add(area);
            }
            IsVisualizerReady = true;
            OnVisualizerReady?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// Create the model group, can only be used if the visualizer is ready
        /// </summary>
        /// <returns></returns>
        public Model3DGroup CreateModelGroup()
        {
            if (!IsVisualizerReady)
            {
                throw new InvalidOperationException("The visualizer was not ready");
            }
            Model3DGroup model3DGroup = new Model3DGroup();
            // Color list for the models so the colors differ
            Color[] colors = { Colors.Red, Colors.Green, Colors.Blue, Colors.Cyan, Colors.Lime, Colors.Magenta, Colors.Black, Colors.Coral, Colors.Salmon, Colors.Silver };

            // Get the minY and minX of all the objects, this will become our new origin
            var minX = areas.Select((area) => area.WorldPosition.X).Min();
            var minY = areas.Select((area) => area.WorldPosition.Y).Min();

            dispatcher.Invoke(() =>
            {
                foreach (Area3D v in areas)
                { 
                    v.Material = new DiffuseMaterial(new SolidColorBrush(colors[areas.IndexOf(v)%colors.Length]));
                    foreach (var model in v.Area.Children)
                    {
                        Transform3DGroup group = new Transform3DGroup();
                        // Mirror the z, since in our camera x and y are mirrored we view the model from the bottom, therefore we mirror the z so the height is visible from the bottom
                        group.Children.Add(new ScaleTransform3D(1, 1, -1));
                        group.Children.Add(new TranslateTransform3D((v.WorldPosition.X - minX)*scale, (v.WorldPosition.Y - minY) * scale, 0));
                        model.Transform = group;
                        // Clone the model otherwise there will be an error as to which thread is was created on, WPF is really sensitive about this
                        model3DGroup.Children.Add(model.Clone());
                    }
                }
            });
            return model3DGroup;
        }
    }
}
