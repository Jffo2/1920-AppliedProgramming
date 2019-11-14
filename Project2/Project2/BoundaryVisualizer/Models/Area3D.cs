using GeoJSON.Net.Geometry;
using System;
using System.Drawing;
using System.Linq;

namespace BoundaryVisualizer.Models
{
    public class Area3D
    {
        public Bitmap Model { get; private set; }

        public Area3D(MultiPolygon multiPolygon)
        {
            Model = GenerateModelFromMultiPolygon(multiPolygon);
        }

        private Bitmap GenerateModelFromMultiPolygon(MultiPolygon multiPolygon)
        {
            double? latDivisor = null;
            double? lonDivisor = null;
            Color[] colors = { Color.Red, Color.Green, Color.Blue, Color.Cyan, Color.Lime, Color.Magenta, Color.Black, Color.Coral, Color.Salmon, Color.Silver };
            Bitmap b = new Bitmap(400, 400);
            Polygon p = multiPolygon.Coordinates.OrderBy((poly) => poly.Coordinates.Count).First();
            using (Graphics g = Graphics.FromImage(b))
            {
                g.FillRectangle(new SolidBrush(Color.Orange), 0, 0, 400, 400);
                for (int i = 0; i < multiPolygon.Coordinates.Count; i++)
                {
                    foreach (LineString lstring in multiPolygon.Coordinates[i].Coordinates)
                    {
                        foreach (IPosition point in lstring.Coordinates)
                        {
                            

                            var mapWidth = 400;
                            var mapHeight = 400;

                            // get x value
                            var x = (point.Longitude + 180.0) * (mapWidth / 360.0);

                            // convert from degrees to radians
                            var latRad = point.Latitude * Math.PI / 180.0;

                            // get y value
                            var mercN = Math.Log(Math.Tan((Math.PI / 4) + (latRad / 2.0)));
                            var y = (mapHeight / 2.0) - (mapHeight * mercN / (2 * Math.PI));

                            //System.Diagnostics.Debug.WriteLine("Found point: " + x + ", " + y);


                            if (latDivisor == null) latDivisor = (int)(y / 1);
                            if (lonDivisor == null) lonDivisor = (int)(x / 1);

                            //System.Diagnostics.Debug.WriteLine("The point in question will be mapped to: " + ((int)((x - lonDivisor) * 200 + 200)) + ", " + ((int)((y - latDivisor) * 200 + 200)));
                            g.FillEllipse(new SolidBrush(colors[i % colors.Length]), (int)((x-lonDivisor) * 100 + 200), (int)((y-latDivisor) * 100 + 200), 2, 2);
                        }
                    }
                }
            }
            return b;
        }
    }
}
