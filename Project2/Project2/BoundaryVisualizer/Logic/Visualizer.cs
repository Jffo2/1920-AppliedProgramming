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

namespace BoundaryVisualizer.Logic
{
    public class Visualizer
    {
        public Visualizer(IGeoJsonLoader geoJsonLoader)
        {
            FeatureCollection featureCollection = geoJsonLoader.Load();
            int index = 0;
            foreach (Feature feature in featureCollection.Features)
            {
                Area3D area = new Area3D((MultiPolygon)feature.Geometry);
                area.Model.Save($"model{index}.png");
                index ++;
            }
        }
    }
}
