using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;

namespace ParkenDD.Services
{
    public class GeolocationService
    {
        public async Task<Geoposition> GetUserLocation()
        {
            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    var geolocator = new Geolocator();
                    // Carry out the operation
                    return await geolocator.GetGeopositionAsync();
                case GeolocationAccessStatus.Denied:
                    //TODO: show some error?
                    return null;
                case GeolocationAccessStatus.Unspecified:
                    //TODO: show some error?
                    return null;
            }
            return null;
        }
    }
}
