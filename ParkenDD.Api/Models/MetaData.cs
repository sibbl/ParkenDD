using System;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace ParkenDD.Api.Models
{
    public sealed class MetaData : ViewModelBase
    {
        /// <summary>
        ///     Version of all the json documents of ParkAPI itself
        /// </summary>
        [JsonProperty("api_version")]
        public string ApiVersion
        {
            get { return _apiVersion; }
            set { Set(ref _apiVersion, value); }
        }
        private string _apiVersion;

        /// <summary>
        ///     Version of the server implementing the API
        /// </summary>
        [JsonProperty("server_version")]

        public string ServerVersion
        {
            get { return _serverVersion; }
            set { Set(ref _serverVersion, value); }
        }
        private string _serverVersion;

        /// <summary>
        ///     Link to reference of this API
        /// </summary>
        [JsonProperty("reference")]
        public Uri Reference
        {
            get { return _reference; }
            set { Set(ref _reference, value); }
        }
        private Uri _reference;

        /// <summary>
        ///     List of cities available on the server (key: id of the city, value: generic details of the city)
        /// </summary>
        [JsonProperty("cities")]
        public MetaDataCities Cities
        {
            get { return _cities; }
            set { Set(ref _cities, value); }
        }
        private MetaDataCities _cities;
    }
}
