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
        /// <summary>
        /// Path of the geoJson file
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">the path of the geoJson file to load</param>
        public FileGeoJsonLoader(string path)
        {
            Path = path;
        }

        /// <summary>
        /// Load the geoJson file and parse it to a FeatureCollection
        /// </summary>
        /// <returns>the feature Collection</returns>
        public FeatureCollection Load()
        {
            try
            {
                FeatureCollection featureCollection = JsonConvert.DeserializeObject<FeatureCollection>(File.ReadAllText(Path));
                return featureCollection;
            }
            catch
            {
                throw new FileFormatException("The provided JSON file is invalid");
            }
        }
    }
}
