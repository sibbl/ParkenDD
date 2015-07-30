using Newtonsoft.Json;

namespace ParkenDD.Win10.Models
{
    public abstract class ForecastBase
    {
        [JsonProperty("data_type")]
        public ForecastDataType DataType { get; set; }
    }
}
