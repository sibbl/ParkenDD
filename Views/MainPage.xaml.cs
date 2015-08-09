using System;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;
using ParkenDD.Messages;
using ParkenDD.Api.Models;
using ParkenDD.ViewModels;
using GalaSoft.MvvmLight.Ioc;
using ParkenDD.Models;
using ParkenDD.Services;
using ParkenDD.Utils;

namespace ParkenDD.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel Vm => (MainViewModel)DataContext;
        private MapDrawingService DrawingService => SimpleIoc.Default.GetInstance<MapDrawingService>();
        private ParkingLot _selectedLot;

        public MainPage()
        {
            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Required;

            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

            Messenger.Default.Register(this, async (ZoomMapToBoundsMessage msg) => await Map.TrySetViewBoundsAsync(msg.BoundingBox, null, MapAnimationKind.Bow));
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = new Color()
            {
                A = 0,
                B = 255,
                G = 0,
                R = 0
            };
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = new Color()
            {
                A = 255,
                B = 140,
                G = 84,
                R = 20
            };
            Vm.PropertyChanged += async (sender, args) =>
            {
                if (args.PropertyName == nameof(Vm.ParkingLots))
                {
                    DrawingService.DrawParkingLots(Map, BackgroundDrawingContainer);
                    if (Vm.ParkingLots != null)
                    {
                        Vm.ParkingLots.CollectionChanged += (o, eventArgs) =>
                        {
                            DrawingService.DrawParkingLots(Map, BackgroundDrawingContainer);
                        };
                    }
                }else if(args.PropertyName == nameof(Vm.SelectedParkingLot))
                {
                    DrawingService.RedrawParkingLot(BackgroundDrawingContainer, Vm.SelectedParkingLot);
                    DrawingService.RedrawParkingLot(BackgroundDrawingContainer, _selectedLot);
                    _selectedLot = Vm.SelectedParkingLot;
                    bool isParkingLotInView;
                    Map.IsLocationInView(_selectedLot.Coordinates.Point, out isParkingLotInView);
                    if (!isParkingLotInView)
                    {
                        await Map.TrySetViewAsync(_selectedLot.Coordinates.Point);
                    }
                }else if (args.PropertyName == nameof(Vm.ParkingLotFilterMode))
                {
                    UpdateParkingLotFilter();
                }
            };
            ParkingLotList.SelectionChanged += (sender, args) =>
            {
                ParkingLotList.ScrollIntoView(ParkingLotList.SelectedItem);
            };
            Map.MapElementClick += (sender, args) =>
            {
                Vm.SelectedParkingLot = DrawingService.GetParkingLotOfIcon(args.MapElements.GetTopmostIcon());
            };
            UpdateParkingLotFilter();
        }

        private void UpdateParkingLotFilter()
        {
            switch (Vm.ParkingLotFilterMode)
            {
                case ParkingLotFilterMode.Alphabetically:
                    ParkingLotFilterMenuItemAlphabetically.IsChecked = true;
                    ParkingLotFilterMenuItemDistance.IsChecked =
                        ParkingLotFilterMenuItemAvailability.IsChecked = false;
                    break;
                case ParkingLotFilterMode.Availability:
                    ParkingLotFilterMenuItemAvailability.IsChecked = true;
                    ParkingLotFilterMenuItemAlphabetically.IsChecked =
                        ParkingLotFilterMenuItemDistance.IsChecked = false;
                    break;
                case ParkingLotFilterMode.Distance:
                    ParkingLotFilterMenuItemDistance.IsChecked = true;
                    ParkingLotFilterMenuItemAlphabetically.IsChecked =
                        ParkingLotFilterMenuItemAvailability.IsChecked = false;
                    break;
            }
        }

        private void ToggleSplitView(object sender, RoutedEventArgs routedEventArgs)
        {
            SplitView.IsPaneOpen = !SplitView.IsPaneOpen;
        }
    }
}
