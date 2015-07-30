using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ParkenDD.Win10.Models
{
    public class Forecast : ForecastBase
    {
        /// <summary>
        ///     Short time forecast data (key: date of the data, value: used place in percent)
        /// </summary>
        [JsonProperty("data")]
        public Dictionary<DateTime, byte> Data { get; set; }
    }
}
