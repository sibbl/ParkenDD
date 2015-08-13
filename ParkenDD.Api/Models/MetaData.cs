using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ParkenDD.Api.Models
{
    public class MetaData
    {
        /// <summary>
        ///     Version of all the json documents of ParkAPI itself
        /// </summary>
        [JsonProperty("api_version")]
        public string ApiVersion { get; set; }

        /// <summary>
        ///     Version of the server implementing the API
        /// </summary>
        [JsonProperty("server_version")]
        public string ServerVersion { get; set; }

        /// <summary>
        ///     Link to reference of this API
        /// </summary>
        [JsonProperty("reference")]
        public Uri Reference { get; set; }

        /// <summary>
        ///     List of cities available on the server (key: id of the city, value: name of the city)
        /// </summary>
        [JsonProperty("cities")]
        public MetaDataCities Cities { get; set; } 
    }
}
