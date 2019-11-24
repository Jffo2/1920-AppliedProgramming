using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using System.Windows.Media.Media3D;

namespace BoundaryVisualizer.Converter
{
    public class LookBackConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new Point3D(-400, -400, -300) - (Point3D)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
