using BoundaryVisualizer.Data;
using BoundaryVisualizer.Models;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Media3D;
using BoundaryVisualizer.Util;
using System.Windows.Threading;
using System.Windows.Media;

namespace BoundaryVisualizer.Logic
{
    public class Visualizer
    {
        private List<Area3D> areas = new List<Area3D>();

        private Dispatcher dispatcher;

        public Visualizer(IGeoJsonLoader geoJsonLoader, Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            FeatureCollection featureCollection = geoJsonLoader.Load();
            int index = 0;
            foreach (Feature feature in featureCollection.Features)
            {
                if (feature.Properties.ContainsKey("name"))
                    System.Diagnostics.Debug.WriteLine(feature.Properties["name"]);

                //if (index == 9) continue;
                Area3D area = new Area3D((MultiPolygon)feature.Geometry,dispatcher);
                area.Model.Save($"model{index}.png");
                index ++;
                areas.Add(area);
            }
            
            System.Diagnostics.Debug.WriteLine("Done!");
        }

        public Model3DGroup CreateModelGroup(int index)
        {
            Model3DGroup model3DGroup = new Model3DGroup();

            dispatcher.Invoke(() =>
            {
                areas[index].Area.Material = new DiffuseMaterial(new SolidColorBrush(Colors.Red));
                model3DGroup.Children.Add(areas[index].Area.Clone());
            });
            return model3DGroup;
        }
    }
}
