using System;
using System.Collections.Generic;
using System.Text;

namespace BoundaryVisualizer.Data
{
    public interface IGeoJsonLoader
    {
        GeoJSON.Net.Feature.FeatureCollection Load();
    }
}
