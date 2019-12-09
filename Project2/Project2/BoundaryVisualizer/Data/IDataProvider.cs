using System;
using System.Collections.Generic;
using System.Text;

namespace BoundaryVisualizer.Data
{
    public interface IDataProvider
    {
        double GetValue(IDictionary<string, object> featureProperties, float scale);
    }
}
