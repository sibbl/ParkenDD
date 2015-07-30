using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ParkenDD.Win10.Models
{
    public class City
    {
        /// <summary>
        ///     Link to the source of the data
        /// </summary>
        [JsonProperty("data_source")]
        public Uri DataSource { get; set; }

        /// <summary>
        ///     Time and date of the last download of the data.
        /// </summary>
        [JsonProperty("last_downloaded")]
        public DateTime LastDownloaded { get; set; }

        /// <summary>
        ///     Time and date of the last update of the data source
        /// </summary>
        [JsonProperty("last_updated")]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        ///     Array of parking lots in the city.
        /// </summary>
        [JsonProperty("lots")]
        public List<ParkingLot> Lots { get; set; }
    }
}
