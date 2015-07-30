using Windows.Devices.Geolocation;

namespace ParkenDD.Win10.Messages
{
    public class ZoomMapToBoundsMessage
    {
        public GeoboundingBox BoundingBox { get; private set; }
        public ZoomMapToBoundsMessage(GeoboundingBox bbox)
        {
            BoundingBox = bbox;
        }
    }
}
