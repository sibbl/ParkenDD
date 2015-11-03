using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using ParkenDD.Api;
using ParkenDD.Api.Interfaces;
using ParkenDD.ViewModels;

namespace ParkenDD.Services
{
    public class ViewModelLocatorService
    {
        static ViewModelLocatorService()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<VoiceCommandService>();
            SimpleIoc.Default.Register<MapDrawingService>();
            SimpleIoc.Default.Register<GeolocationService>();
            SimpleIoc.Default.Register<ParkingLotListFilterService>();
            SimpleIoc.Default.Register<SettingsService>();
            SimpleIoc.Default.Register<StorageService>();
            SimpleIoc.Default.Register<TrackingService>();
            SimpleIoc.Default.Register<ResourceService>();
            SimpleIoc.Default.Register<ExceptionService>();
            SimpleIoc.Default.Register<JumpListService>();
            SimpleIoc.Default.Register<LocalizationService>();
            SimpleIoc.Default.Register<IParkenDdClient, ParkenDdClient>();

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<InfoDialogViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
        }
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public InfoDialogViewModel InfoDialog => ServiceLocator.Current.GetInstance<InfoDialogViewModel>();
        public SettingsViewModel Settings => ServiceLocator.Current.GetInstance<SettingsViewModel>();
    }
}
