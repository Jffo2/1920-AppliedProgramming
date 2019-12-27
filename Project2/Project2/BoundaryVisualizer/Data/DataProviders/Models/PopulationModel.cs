using Newtonsoft.Json;
using System.Collections.Generic;

namespace BoundaryVisualizer.Data.DataProviders.Models
{
    public class PopulationModel : IDataModel
    {
        [JsonProperty("facts")]
        public List<ProvincePopulationModel> Facts { get; set; }

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
                        return b.Population * (0.000001 * scale);
                    }
                    if (b.Province != null && b.Province.ToLower().Contains(name.ToLower().Replace("-", " ")))
                    {
                        return b.Population * (0.000001 * scale);
                    }
                    if (b.Region != null && b.Region.Contains(name))
                        return b.Population * (0.000001 * scale);
                }
            }
            return scale;
        }

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
                        return b.Population;
                    }
                    if (b.Province != null && b.Province.ToLower().Contains(name.ToLower().Replace("-", " ")))
                    {
                        return b.Population;
                    }
                    if (b.Region != null && b.Region.Contains(name))
                        return b.Population;
                }
            }
            return double.NaN;
        }

    }

    public class ProvincePopulationModel
    {
        [JsonProperty("Belgique")]
        public string Country { get; set; }

        [JsonProperty("Région")]
        public string Region { get; set; }

        [JsonProperty("Province")]
        public string Province { get; set; }

        [JsonProperty("Population totale")]
        public long Population { get; set; }

        public override string ToString()
        {
            return $"Country: {Country}; Region: {Region}; Province: {Province}; Population {Population}";
        }
    }
}
