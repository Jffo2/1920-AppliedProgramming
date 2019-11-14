using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using GeoJSON.Net.Feature;
using Newtonsoft.Json;

namespace BoundaryVisualizer.Data
{
    public class FileGeoJsonLoader : IGeoJsonLoader
    {
        public string Path { get; }

        public FileGeoJsonLoader(string path)
        {
            Path = path;
        }

        public FeatureCollection Load()
        {
            FeatureCollection featureCollection = JsonConvert.DeserializeObject<FeatureCollection>(File.ReadAllText(Path));
            return featureCollection;
        }
    }
}
