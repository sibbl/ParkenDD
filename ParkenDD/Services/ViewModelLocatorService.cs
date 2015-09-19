using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using ParkenDD.Api;
using ParkenDD.Api.Interfaces;
using ParkenDD.Design;
using ParkenDD.ViewModels;

namespace ParkenDD.Services
{
    public class ViewModelLocatorService
    {
        static ViewModelLocatorService()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<INavigationService, NavigationService>();
            SimpleIoc.Default.Register<VoiceCommandService>();
            SimpleIoc.Default.Register<MapDrawingService>();
            SimpleIoc.Default.Register<GeolocationService>();
            SimpleIoc.Default.Register<ParkingLotListFilterService>();
            SimpleIoc.Default.Register<SettingsService>();
            SimpleIoc.Default.Register<LifecycleService>();
            SimpleIoc.Default.Register<StorageService>();
            SimpleIoc.Default.Register<TrackingService>();

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IParkenDdClient, DesignParkenDdClient>();
            }
            else
            {
                SimpleIoc.Default.Register<IParkenDdClient, ParkenDdClient>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
        }
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
    }
}
