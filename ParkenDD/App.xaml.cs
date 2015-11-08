using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.ServiceLocation;
using ParkenDD.Services;
using ParkenDD.ViewModels;
using ParkenDD.Views;

namespace ParkenDD
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
            UnhandledException += OnUnhandledException;
            Resuming += (sender, o) =>
            {
                ServiceLocator.Current.GetInstance<MainViewModel>().RefreshCityDetails(true);
            };
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            var handled = false;
            ServiceLocator.Current.GetInstance<ExceptionService>().OnUnhandledException(unhandledExceptionEventArgs.Exception, ref handled);
            unhandledExceptionEventArgs.Handled = handled;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            DispatcherHelper.Initialize();
            var rootFrame = Window.Current.Content as Frame;
            ServiceLocator.Current.GetInstance<LocalizationService>().UpdateCulture();

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                e.SplashScreen.Dismissed += (sender, args) =>
                {
                    var loadState = (e.PreviousExecutionState != ApplicationExecutionState.Terminated);
                    Task.Factory.StartNew(() =>
                    {
                        ServiceLocator.Current
                            .GetInstance<MainViewModel>().Initialize(loadState);
                    }, TaskCreationOptions.LongRunning);
                };
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(MainPage), e.Arguments);
            }

            LauncherService.ParseArguments(e.Arguments);

            // Ensure the current window is active
            Window.Current.Activate();

            if (e.PreviousExecutionState == ApplicationExecutionState.NotRunning || e.PreviousExecutionState == ApplicationExecutionState.ClosedByUser)
            {
                ServiceLocator.Current.GetInstance<ReviewAppService>().AppLaunched();
            }
        }

        protected override void OnActivated(IActivatedEventArgs e)
        {
            if (e.Kind == ActivationKind.VoiceCommand)
            {
                DispatcherHelper.CheckBeginInvokeOnUI(() =>
                {
                    ServiceLocator.Current.GetInstance<VoiceCommandService>()
                        .HandleActivation(e as VoiceCommandActivatedEventArgs);
                });
            }

        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }
    }
}
