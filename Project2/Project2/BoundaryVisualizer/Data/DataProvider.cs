using BoundaryVisualizer.Data.DataProviders.Models;
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
        public event EventHandler<EventArgs> DataProviderReady;
        public bool IsDataProviderReady { get; private set; }
        public virtual string ViewId { get; protected set; }

        public IDataModel JSonContent { get; protected set; }

        public DataProvider()
        {
            IsDataProviderReady = false;
            SetJSon();
        }

        private async void SetJSon()
        {
            JSonContent = await RetrieveJsonAsync();
            DataProviderReady?.Invoke(this, new EventArgs());
            IsDataProviderReady = true;
        }

        private async Task<IDataModel> RetrieveJsonAsync()
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
                return DeserializeJSon(content);
            }
        }

        protected abstract IDataModel DeserializeJSon(string content);


        protected abstract string GenerateURL();

        public abstract double GetValue(IDictionary<string, object> featureProperties);
    }
}
