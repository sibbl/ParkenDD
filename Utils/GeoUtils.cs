using System;
using Windows.Devices.Geolocation;

namespace ParkenDD.Utils
{
    public static class GeoUtils 
    {
        public enum DistanceType { Miles, Kilometers };

        public static double GetDistanceTo(this BasicGeoposition pos1, BasicGeoposition pos2, DistanceType type = DistanceType.Kilometers)
        {
            var r = (type == DistanceType.Miles) ? 3960 : 6371;
            var dLat = ToRadian(pos2.Latitude - pos1.Latitude);
            var dLon = ToRadian(pos2.Longitude - pos1.Longitude);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadian(pos1.Latitude)) * Math.Cos(ToRadian(pos2.Latitude)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            var d = r * c;
            return d;
        }

        public static double GetDistanceTo(this Geopoint pos1, Geopoint pos2, DistanceType type = DistanceType.Kilometers)
        {
            return pos1.Position.GetDistanceTo(pos2.Position, type);
        }

        private static double ToRadian(double val)
        {
            return (Math.PI / 180) * val;
        }
    }
}
