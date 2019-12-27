using Newtonsoft.Json;
using System.Collections.Generic;

namespace BoundaryVisualizer.Data.DataProviders.Models
{
    public class PercentageMarriedModel : IDataModel
    {
        [JsonProperty("facts")]
        public List<ProvinceMarriedModel> Facts { get; set; }

        /// <summary>
        /// Gets the best fitting value for a feature
        /// </summary>
        /// <param name="featureProperties">the properties of a feature</param>
        /// <returns>the value for the feature that is most alike to the feature passed in</returns>
        public double GetValue(string name)
        {
            if (name != null)
            {
                foreach (var b in Facts)
                {

                    if (b.Province != null && b.Province.ToLower().Contains(name.ToLower()))
                    {
                        return b.PercentageMarried;
                    }
                    if (b.Province != null && b.Province.ToLower().Contains(name.ToLower().Replace("-", " ")))
                    {
                        return b.PercentageMarried;
                    }
                    if (b.Region != null && b.Region.Contains(name))
                        return b.PercentageMarried;
                }
            }
            return double.NaN;
        }

        /// <summary>
        /// Gets the best fitting value for a feature, scaled to fit the scene
        /// </summary>
        /// <param name="featureProperties">the properties of a feature</param>
        /// <param name="scale">the scale of the scene</param>
        /// <returns>the scaled value for the feature that is most alike to the feature passed in</returns>
        public double GetScaledValue(string name, float scale)
        {
            if (name != null)
            {
                foreach (var b in Facts)
                {

                    if (b.Province != null && b.Province.ToLower().Contains(name.ToLower()))
                    {
                        return b.PercentageMarried * (scale * 20 / 3.0);
                    }
                    if (b.Province != null && b.Province.ToLower().Contains(name.ToLower().Replace("-", " ")))
                    {
                        return b.PercentageMarried * (scale * 20 / 3.0);
                    }
                    if (b.Region != null && b.Region.Contains(name))
                        return b.PercentageMarried * (scale * 20 / 3.0);
                }
            }
            return scale;
        }
    }


    public class ProvinceMarriedModel
    {
        [JsonProperty("Belgique")]
        public string Country { get; set; }

        [JsonProperty("Région")]
        public string Region { get; set; }

        [JsonProperty("Province")]
        public string Province { get; set; }

        [JsonProperty("Population mariée")]
        public double PercentageMarried { get; set; }

        public override string ToString()
        {
            return $"Country: {Country}; Region: {Region}; Province: {Province}; Population {PercentageMarried}";
        }
    }
}