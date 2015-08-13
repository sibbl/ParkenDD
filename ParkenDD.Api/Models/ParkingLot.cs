using Newtonsoft.Json;

namespace ParkenDD.Api.Models
{
    /// <summary>
    ///     Data for single parking lot
    /// </summary>
    public class ParkingLot
    {
        /// <summary>
        ///     Address of the parking lot
        /// </summary>
        [JsonProperty("address")]
        public string Address { get; set; }

        /// <summary>
        ///     Coordinates of the parking lot
        /// </summary>
        [JsonProperty("coords")]
        public Coordinate Coordinates { get; set; }

        /// <summary>
        ///     Amount of free parking space
        /// </summary>
        [JsonProperty("free")]
        public int FreeLots { get; set; }

        /// <summary>
        ///     Total amount of parking space
        /// </summary>
        [JsonProperty("total")]
        public int TotalLots { get; set; }

        /// <summary>
        ///     Id of the parking lot
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        ///     Type of the Parking lot
        /// </summary>
        [JsonProperty("lot_type")]
        public string LotType { get; set; }

        /// <summary>
        ///     State of the parking lot
        /// </summary>
        [JsonProperty("state")]
        public ParkingLotState State { get; set; }

        /// <summary>
        ///     Name of the Parking lot
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        ///     Shows if long time data is available
        /// </summary>
        [JsonProperty("long_forecast_available")]
        public bool HasLongForecast { get; set; }

        /// <summary>
        ///     Short time data for parking lot
        /// </summary>
        [JsonProperty("forecast")]
        public ForecastBase Forecast { get; set; }

        /// <summary>
        ///     City region of this parking lot
        /// </summary>
        [JsonProperty("region")]
        public string Region { get; set; }

        /// <summary>
        ///     Indicates whether a forecast or tendency is available
        /// </summary>
        public bool HasForecast => Forecast != null;
    }
}
