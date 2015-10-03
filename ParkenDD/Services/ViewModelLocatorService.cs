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
            SimpleIoc.Default.Register<StorageService>();
            SimpleIoc.Default.Register<TrackingService>();
            SimpleIoc.Default.Register<ResourceService>();
            SimpleIoc.Default.Register<ExceptionService>();

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IParkenDdClient, DesignParkenDdClient>();
            }
            else
            {
                SimpleIoc.Default.Register<IParkenDdClient, ParkenDdClient>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<InfoDialogViewModel>();
        }
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public InfoDialogViewModel InfoDialog => ServiceLocator.Current.GetInstance<InfoDialogViewModel>();
    }
}
