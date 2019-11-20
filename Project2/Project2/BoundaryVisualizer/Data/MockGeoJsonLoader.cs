using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;
using System.Collections.Generic;

namespace BoundaryVisualizer.Data
{
    public class MockGeoJsonLoader : IGeoJsonLoader
    {
        public FeatureCollection Load()
        {
            return new FeatureCollection(new List<Feature>( new Feature[] { new Feature(new MultiPolygon(new Polygon[] { new Polygon(new LineString[] { new LineString(new Position[]
            {
                new Position(0, 10),
                new Position(20,5),
                new Position(40,10),
                new Position(40,20),
                new Position(20,12),
                new Position(0,20),
                new Position(0,10)
            }) }) })) }));
        }
    }
}
