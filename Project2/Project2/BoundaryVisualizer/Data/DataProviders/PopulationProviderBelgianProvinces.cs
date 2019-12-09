using BoundaryVisualizer.Data.DataProviders.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace BoundaryVisualizer.Data.DataProviders
{
    public class PopulationProviderBelgianProvinces : DataProvider
    {
        /// <summary>
        /// Constructor, set the view ID
        /// </summary>
        public PopulationProviderBelgianProvinces() : base()
        {
            ViewId = "760c4d9e-d859-42f7-82f4-21a7232ceaf5";
        }

        /// <summary>
        /// Gets the best fitting value for a feature
        /// </summary>
        /// <param name="featureProperties">the properties of a feature</param>
        /// <returns>the value for the feature that is most alike to the feature passed in</returns>
        public override double GetValue(IDictionary<string, object> featureProperties)
        {
            string frenchName = (string)((JObject)(featureProperties["alltags"])).SelectToken("name:fr");
            return JSonContent.GetValue(frenchName);
        }

        /// <summary>
        /// The URL to the api
        /// </summary>
        /// <returns></returns>
        protected override string GenerateURL()
        {
            return ("https://bestat.statbel.fgov.be/bestat/api/views/" + ViewId + "/result/JSON");
        }

        /// <summary>
        /// Method to deserialize the JSon
        /// </summary>
        /// <param name="content">the string containing the json</param>
        /// <returns>the data model</returns>
        protected override IDataModel DeserializeJSon(string content)
        {
            return JsonConvert.DeserializeObject<PopulationModel>(content);
        }
    }
}
