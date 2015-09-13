using Windows.Devices.Geolocation;

namespace ParkenDD.Messages
{
    public class ZoomMapToCoordinateMessage
    {
        public Geopoint Point { get; set; }

        public ZoomMapToCoordinateMessage(Geopoint point)
        {
            Point = point;
        }
    }
}
