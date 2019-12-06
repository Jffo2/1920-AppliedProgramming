using BoundaryVisualizer.Data.DataProviders.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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
            string frenchName = (string)((JObject)(featureProperties["alltags"])).SelectToken("name:fr");
            return JSonContent.GetValue(frenchName);
        }

        protected override string GenerateURL()
        {
            return ("https://bestat.statbel.fgov.be/bestat/api/views/" + ViewId + "/result/JSON");
        }

        protected override IDataModel DeserializeJSon(string content)
        {
            return JsonConvert.DeserializeObject<PopulationModel>(content);
        }
    }
}
