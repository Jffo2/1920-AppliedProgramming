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
        public event EventHandler<EventArgs> OnVisualizerReady;

        public bool IsVisualizerReady { get; private set; }

        private List<Area3D> areas = new List<Area3D>();

        private Dispatcher dispatcher;

        private float scale;
        private FeatureCollection featureCollection;
        private DataProvider dataProvider;

        public Visualizer(IGeoJsonLoader geoJsonLoader, Dispatcher dispatcher,DataProvider dataProvider, float scale)
        {
            IsVisualizerReady = false;
            this.scale = scale;
            this.dispatcher = dispatcher;
            this.featureCollection = geoJsonLoader.Load();
            this.dataProvider = dataProvider;
            if (!dataProvider.IsDataProviderReady)
            {
                dataProvider.DataProviderReady += DoSetup;
            }
            else DoSetup(null, null);
        }

        private void DoSetup(object sender, EventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("Setting up visualizer");
            int index = 0;
            foreach (Feature feature in featureCollection.Features)
            {
                if (feature.Properties.ContainsKey("name"))
                    System.Diagnostics.Debug.WriteLine(feature.Properties["name"]);
                var height = (float)dataProvider.GetValue(feature.Properties);
                System.Diagnostics.Debug.WriteLine(height);
                Area3D area = new Area3D((MultiPolygon)feature.Geometry, dispatcher, scale, height);
                area.Model.Save($"model{index}.png");
                index++;
                areas.Add(area);
            }
            System.Diagnostics.Debug.WriteLine("Set up");
            IsVisualizerReady = true;
            OnVisualizerReady?.Invoke(this, new EventArgs());
        }

        public Model3DGroup CreateModelGroup()
        {
            if (!IsVisualizerReady)
            {
                throw new InvalidOperationException("The visualizer was not ready");
            }
            Model3DGroup model3DGroup = new Model3DGroup();
            Color[] colors = { Colors.Red, Colors.Green, Colors.Blue, Colors.Cyan, Colors.Lime, Colors.Magenta, Colors.Black, Colors.Coral, Colors.Salmon, Colors.Silver };

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
                        group.Children.Add(new ScaleTransform3D(1, 1, -1));
                        group.Children.Add(new TranslateTransform3D((v.WorldPosition.X - minX)*scale, (v.WorldPosition.Y - minY) * scale, 0));
                        model.Transform = group;
                        model3DGroup.Children.Add(model.Clone());
                    }
                }
            });
            return model3DGroup;
        }
    }
}
