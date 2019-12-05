using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BoundaryVisualizer.Data
{
    public abstract class DataProvider : IDataProvider
    {
        public virtual string ViewId { get; protected set; }

        public List<dynamic> JSonContent { get; protected set; }

        public DataProvider()
        {
            SetJSon();
        }

        private async void SetJSon()
        {
            JSonContent = await RetrieveJsonAsync();
        }

        private async Task<List<dynamic>> RetrieveJsonAsync()
        {
            using (var client = new HttpClient())
            {
                var content = await client.GetStringAsync(GenerateURL());
                return JsonConvert.DeserializeObject<List<dynamic>>(content);
            }
        }

        protected abstract string GenerateURL();

        public abstract double GetValue(IDictionary<string, object> featureProperties);
    }
}
