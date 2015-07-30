using Newtonsoft.Json;

namespace ParkenDD.Win10.Models
{
    public class TendencyForecast : ForecastBase
    {
        [JsonProperty("data")]
        public Tendency Tendency { get; set; }
    }
}
