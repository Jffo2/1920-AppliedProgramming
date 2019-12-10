using System;
using System.Collections.Generic;
using System.Text;

namespace BoundaryVisualizer.Models
{
    public struct LegendItem
    {
        public string Name { get; set; }
        public System.Windows.Media.Color Color { get; set; }
        public double Value { get; set; }
    }
}
