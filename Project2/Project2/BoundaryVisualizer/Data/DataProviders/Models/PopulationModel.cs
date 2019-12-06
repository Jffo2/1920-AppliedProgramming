using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BoundaryVisualizer.Data.DataProviders.Models
{
    public class PopulationModel : IDataModel
    {
        [JsonProperty("facts")]
        public List<ProvincePopulationModel> Facts { get; set; }

        public double GetValue(string name)
        {
            foreach (var b in Facts)
            {
                
                if (b.Province != null && b.Province.ToLower().Contains(name.ToLower()))
                {
                    return b.Population / 10000;
                }
                if (b.Province != null && b.Province.ToLower().Contains(name.ToLower().Replace("-", " ")))
                {
                    return b.Population / 10000;
                }
                if (b.Region != null && b.Region.Contains(name))
                    return b.Population / 10000;
            }
            return 400.0;
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
