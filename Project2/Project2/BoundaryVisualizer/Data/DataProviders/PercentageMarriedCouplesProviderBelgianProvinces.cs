﻿using BoundaryVisualizer.Data.DataProviders.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoundaryVisualizer.Data.DataProviders
{
    public class PercentageMarriedCouplesProviderBelgianProvinces : DataProvider
    {
        /// <summary>
        /// Constructor, set the view ID
        /// </summary>
        public PercentageMarriedCouplesProviderBelgianProvinces() : base()
        {
            ViewId = "04a275c0-5b8d-4d45-9cfd-92cf1298cde7";
            BackupResourceName = "BoundaryVisualizer.Data.BackupJsonFiles.PercentageMarriedPeople.json";
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
        /// Gets the best fitting value for a feature, scaled to fit the scene
        /// </summary>
        /// <param name="featureProperties">the properties of a feature</param>
        /// <param name="scale">the scale of the scene</param>
        /// <returns>the scaled value for the feature that is most alike to the feature passed in</returns>
        public override double GetScaledValue(IDictionary<string, object> featureProperties, float scale)
        {
            string frenchName = (string)((JObject)(featureProperties["alltags"])).SelectToken("name:fr");
            return JSonContent.GetScaledValue(frenchName, scale);
        }

        /// <summary>
        /// Method to deserialize the JSon
        /// </summary>
        /// <param name="content">the string containing the json</param>
        /// <returns>the data model</returns>
        protected override IDataModel DeserializeJSon(string content)
        {
            return JsonConvert.DeserializeObject<PercentageMarriedModel>(content);
        }

        /// /// <summary>
        /// The URL to the api
        /// </summary>
        /// <returns></returns>
        protected override string GenerateURL()
        {
            return ("https://bestat.statbel.fgov.be/bestat/api/views/" + ViewId + "/result/JSON");
        }
    }
}
