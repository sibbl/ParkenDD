using System;
using Newtonsoft.Json;

namespace ParkenDD.Api.Models
{
    public class MetaDataCityRow
    {
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("source")]
        public Uri Source { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        /// <summary>
        ///     Coordinates of the parking lot
        /// </summary>
        [JsonProperty("coords")]
        public Coordinate Coordinates { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
