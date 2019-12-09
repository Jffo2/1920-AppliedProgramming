using System;
using System.Collections.Generic;
using System.Text;

namespace BoundaryVisualizer.Data.DataProviders.Models
{
    public interface IDataModel
    {
        double GetValue(string s, float scale);
    }
}
