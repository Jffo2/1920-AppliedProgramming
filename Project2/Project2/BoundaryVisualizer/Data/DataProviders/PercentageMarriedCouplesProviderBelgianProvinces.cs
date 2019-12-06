using BoundaryVisualizer.Data.DataProviders.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoundaryVisualizer.Data.DataProviders
{
    public class PercentageMarriedCouplesProviderBelgianProvinces : DataProvider
    {
        public PercentageMarriedCouplesProviderBelgianProvinces() : base()
        {
            ViewId = "04a275c0-5b8d-4d45-9cfd-92cf1298cde7";
        }

        public override double GetValue(IDictionary<string, object> featureProperties)
        {
            string frenchName = (string)((JObject)(featureProperties["alltags"])).SelectToken("name:fr");
            return JSonContent.GetValue(frenchName);
        }

        protected override IDataModel DeserializeJSon(string content)
        {
            return JsonConvert.DeserializeObject<PercentageMarriedModel>(content);
        }

        protected override string GenerateURL()
        {
            return ("https://bestat.statbel.fgov.be/bestat/api/views/" + ViewId + "/result/JSON");
        }
    }
}
