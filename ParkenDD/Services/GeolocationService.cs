using System;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using ParkenDD.Messages;
using ParkenDD.ViewModels;

namespace ParkenDD.Services
{
    public class GeolocationService
    {
        private static MainViewModel MainVm => SimpleIoc.Default.GetInstance<MainViewModel>();
        public async Task<Geoposition> GetUserLocation()
        {
            var accessStatus = await Geolocator.RequestAccessAsync();
            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    var geolocator = new Geolocator();
                    var pos = await geolocator.GetGeopositionAsync();
                    MainVm.UserLocation = pos;
                    return pos;
                case GeolocationAccessStatus.Denied:
                    //TODO: show some error?
                    MainVm.UserLocation = null;
                    return null;
                case GeolocationAccessStatus.Unspecified:
                    //TODO: show some error?
                    MainVm.UserLocation = null;
                    return null;
            }
            return null;
        }
    }
}
