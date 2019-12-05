using BoundaryVisualizer.Data;
using BoundaryVisualizer.Models;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.Windows.Media;

namespace BoundaryVisualizer.Logic
{
    public class Visualizer
    {
        private List<Area3D> areas = new List<Area3D>();

        private Dispatcher dispatcher;

        private float scale;

        public Visualizer(IGeoJsonLoader geoJsonLoader, Dispatcher dispatcher,IDataProvider dataProvider, float scale)
        {
            this.scale = scale;
            this.dispatcher = dispatcher;
            FeatureCollection featureCollection = geoJsonLoader.Load();
            int index = 0;
            foreach (Feature feature in featureCollection.Features)
            {
                if (feature.Properties.ContainsKey("name"))
                    System.Diagnostics.Debug.WriteLine(feature.Properties["name"]);
                var height = (float)dataProvider.GetValue(feature.Properties);
                Area3D area = new Area3D((MultiPolygon)feature.Geometry,dispatcher,scale, height);
                area.Model.Save($"model{index}.png");
                index ++;
                areas.Add(area);
            }
            
            System.Diagnostics.Debug.WriteLine("Done!");
        }

        public Model3DGroup CreateModelGroup()
        {
            Model3DGroup model3DGroup = new Model3DGroup();
            System.Windows.Media.Color[] colors = { Colors.Red, Colors.Green, Colors.Blue, Colors.Cyan, Colors.Lime, Colors.Magenta, Colors.Black, Colors.Coral, Colors.Salmon, Colors.Silver };

            var minX = areas.Select((area) => area.WorldPosition.X).Min();
            var minY = areas.Select((area) => area.WorldPosition.Y).Min();

            dispatcher.Invoke(() =>
            {
                foreach (Area3D v in areas)
                { 
                    v.Material = new DiffuseMaterial(new SolidColorBrush(colors[areas.IndexOf(v)%colors.Length]));
                    foreach (var model in v.Area.Children)
                    {
                        model.Transform = new TranslateTransform3D((v.WorldPosition.X - minX)*scale, (v.WorldPosition.Y - minY) * scale, 0);
                        model3DGroup.Children.Add(model.Clone());
                    }
                }
            });
            return model3DGroup;
        }
    }
}
