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
        /// <summary>
        /// Event that triggers when the data has been fetched from the webserver
        /// </summary>
        public event EventHandler<EventArgs> DataProviderReady;
        /// <summary>
        /// Boolean telling whether the DataProvider is ready
        /// </summary>
        public bool IsDataProviderReady { get; private set; }
        /// <summary>
        /// The ID for the view of the API
        /// </summary>
        public virtual string ViewId { get; protected set; }
        /// <summary>
        /// The actual JSon content as a DataModel
        /// </summary>
        public IDataModel JSonContent { get; protected set; }

        /// <summary>
        /// Constructor, start fetchin JSon
        /// </summary>
        public DataProvider()
        {
            IsDataProviderReady = false;
            SetJSon();
        }

        /// <summary>
        /// Fetch JSon async, trigger the event when ready
        /// </summary>
        private async void SetJSon()
        {
            JSonContent = await RetrieveJsonAsync();
            DataProviderReady?.Invoke(this, new EventArgs());
            IsDataProviderReady = true;
        }

        /// <summary>
        /// Fetch the JSon from the webserver async
        /// </summary>
        /// <returns>the data model with the data from the api</returns>
        private async Task<IDataModel> RetrieveJsonAsync()
        {
            using (var client = new HttpClient())
            {
                string content = null;
                // The first request to the webserver allways fails for some reason, so we must keep trying
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

        /// <summary>
        /// Abstract method to deserialize the JSon so the data can be remodelled
        /// </summary>
        /// <param name="content">the string containing the json</param>
        /// <returns>the data model</returns>
        protected abstract IDataModel DeserializeJSon(string content);

        /// <summary>
        /// The URL to the api
        /// </summary>
        /// <returns></returns>
        protected abstract string GenerateURL();

        /// <summary>
        /// Gets the best fitting value for a feature
        /// </summary>
        /// <param name="featureProperties">the properties of a feature</param>
        /// <returns>the value for the feature that is most alike to the feature passed in</returns>
        public abstract double GetValue(IDictionary<string, object> featureProperties, float scale);
    }
}
