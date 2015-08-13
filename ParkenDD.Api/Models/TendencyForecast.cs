using Newtonsoft.Json;

namespace ParkenDD.Api.Models
{
    public class TendencyForecast : ForecastBase
    {
        [JsonProperty("data")]
        public Tendency Tendency { get; set; }
    }
}
