using BoundaryVisualizer.Data.DataProviders.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Reflection;
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
        /// The resource file path for use when the website is down
        /// </summary>
        public string BackupResourceName { get; protected set; }

        private const int MAX_TRIES_DATA_FETCH = 10;

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
            int tries = 0;
            using (var client = new HttpClient())
            {
                string content = null;
                // The first request to the webserver allways fails for some reason, so we must keep trying
                while (content == null)
                {
                    try
                    {
                        content = await client.GetStringAsync(GenerateURL());
                    } catch { content = null; tries++; }
                    if (tries==MAX_TRIES_DATA_FETCH)
                    {
                        return UseBackupJsonFile();
                    }
                }
                return DeserializeJSon(content);
            }
        }

        /// <summary>
        /// Read the backup json file
        /// </summary>
        /// <returns>the data model with the data from the json file</returns>
        private IDataModel UseBackupJsonFile()
        {
            var assembly = Assembly.GetExecutingAssembly();
            using (Stream stream = assembly.GetManifestResourceStream(BackupResourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    return DeserializeJSon(result);
                }
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
        public abstract double GetValue(IDictionary<string, object> featureProperties);

        /// <summary>
        /// Gets the best fitting value for a feature, scaled to fit the scene
        /// </summary>
        /// <param name="featureProperties">the properties of a feature</param>
        /// <param name="scale">the scale of the scene</param>
        /// <returns>the scaled value for the feature that is most alike to the feature passed in</returns>
        public abstract double GetScaledValue(IDictionary<string, object> featureProperties, float scale);
    }
}
