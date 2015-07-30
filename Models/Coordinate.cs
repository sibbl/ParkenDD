using Windows.Devices.Geolocation;
using Newtonsoft.Json;

namespace ParkenDD.Win10.Models
{
    public class Coordinate
    {
        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lng")]
        public double Longitude { get; set; }

        public Geopoint Point => new Geopoint(new BasicGeoposition
        {
            Latitude = Latitude,
            Longitude = Longitude
        });

        public Coordinate(double lat, double lon)
        {
            Latitude = lat;
            Longitude = lon;
        }
    }
}
