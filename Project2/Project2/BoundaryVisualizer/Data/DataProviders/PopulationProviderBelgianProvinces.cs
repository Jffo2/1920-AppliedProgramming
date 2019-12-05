using System;
using System.Collections.Generic;
using System.Text;

namespace BoundaryVisualizer.Data.DataProviders
{
    public class PopulationProviderBelgianProvinces : DataProvider
    {

        public override string ViewId => base.ViewId;

        public PopulationProviderBelgianProvinces() : base()
        {
            ViewId = "760c4d9e-d859-42f7-82f4-21a7232ceaf5";
        }

        public override double GetValue(IDictionary<string, object> featureProperties)
        {
            System.Diagnostics.Debug.WriteLine(JSonContent);
            return 400.0;
        }

        protected override string GenerateURL()
        {
            return ("https://bestat.statbel.fgov.be/bestat/api/views/" + ViewId + "/result/JSON");
        }
    }
}
