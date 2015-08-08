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
