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

        public dynamic JSonContent { get; protected set; }

        public DataProvider()
        {
            SetJSon();
        }

        private async void SetJSon()
        {
            JSonContent = await RetrieveJsonAsync();
        }

        private async Task<dynamic> RetrieveJsonAsync()
        {
            using (var client = new HttpClient())
            {

                string content = null;
                while (content == null)
                {
                    try
                    {
                        content = await client.GetStringAsync(GenerateURL());
                    } catch { content = null; }
                }
                System.Diagnostics.Debug.WriteLine(content);
                return JsonConvert.DeserializeObject<dynamic>(content);
            }
        }

        protected abstract string GenerateURL();

        public abstract double GetValue(IDictionary<string, object> featureProperties);
    }
}
