using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using ParkenDD.Win10.Services.DesignTime;
using ParkenDD.Win10.Services.Interfaces;
using ParkenDD.Win10.ViewModels;

namespace ParkenDD.Win10.Services
{
    public class ViewModelLocatorService
    {
        static ViewModelLocatorService()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<INavigationService, NavigationService>();
            SimpleIoc.Default.Register<VoiceCommandService>();

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IApiService, DesignApiService>();
            }
            else
            {
                SimpleIoc.Default.Register<IApiService, ApiService>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
        }
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
    }
}
