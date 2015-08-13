using Newtonsoft.Json;

namespace ParkenDD.Api.Models
{
    public abstract class ForecastBase
    {
        [JsonProperty("data_type")]
        public ForecastDataType DataType { get; set; }
    }
}
