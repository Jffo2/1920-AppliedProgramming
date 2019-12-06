using Newtonsoft.Json;
using System.Collections.Generic;

namespace BoundaryVisualizer.Data.DataProviders.Models
{
    public class PercentageMarriedModel : IDataModel
    {
        [JsonProperty("facts")]
        public List<ProvinceMarriedModel> Facts { get; set; }

        public double GetValue(string name)
        {
            if (name != null)
            {
                foreach (var b in Facts)
                {

                    if (b.Province != null && b.Province.ToLower().Contains(name.ToLower()))
                    {
                        return b.PercentageMarried * 20;
                    }
                    if (b.Province != null && b.Province.ToLower().Contains(name.ToLower().Replace("-", " ")))
                    {
                        return b.PercentageMarried * 20;
                    }
                    if (b.Region != null && b.Region.Contains(name))
                        return b.PercentageMarried * 20;
                }
            }
            return 400.0;
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