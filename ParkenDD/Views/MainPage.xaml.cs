using System;
using System.ComponentModel;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;
using ParkenDD.Messages;
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
        private SelectableParkingLot _selectedLot;
        private GeoboundingBox _initialMapBbox;
        private Geopoint _initialCoordinates;

        public MainPage()
        {
            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Required;

            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

            Map.Loaded += async (sender, args) =>
            {
                if (_initialMapBbox != null)
                {
                    await Map.TrySetViewBoundsAsync(_initialMapBbox, null, MapAnimationKind.None);
                }
                if (_initialCoordinates != null)
                {
                    await Map.TrySetViewAsync(_initialCoordinates, null, null, null, MapAnimationKind.None);
                }
            };

            Messenger.Default.Register(this, async (ZoomMapToBoundsMessage msg) =>
            {
                await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    _initialMapBbox = msg.BoundingBox; //set initial bbox as the following won't work while splash screen is still visible
                    await Map.TrySetViewBoundsAsync(msg.BoundingBox, null, MapAnimationKind.Bow);
                });
            });

            Messenger.Default.Register(this, async (ZoomMapToCoordinateMessage msg) =>
            {
                await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    _initialCoordinates = msg.Point; //set initial coordinates as the following won't work while splash screen is still visible
                    await Map.TrySetViewAsync(msg.Point, null, null, null, MapAnimationKind.Bow);
                });
            });

            //TODO: set other colors as well
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
                        foreach (var selectableParkingLot in Vm.ParkingLots)
                        {
                            selectableParkingLot.ParkingLot.PropertyChanged += (sender1, changedEventArgs) =>
                            {
                                DrawingService.RedrawParkingLot(BackgroundDrawingContainer, selectableParkingLot);
                            };
                        }
                        Vm.ParkingLots.CollectionChanged += (o, eventArgs) =>
                        {
                            DrawingService.DrawParkingLots(Map, BackgroundDrawingContainer);
                            if (eventArgs.NewItems != null)
                            {
                                foreach (var selectableParkingLot in eventArgs.NewItems.OfType<SelectableParkingLot>())
                                {
                                    selectableParkingLot.ParkingLot.PropertyChanged += (sender1, changedEventArgs) =>
                                    {
                                        DrawingService.RedrawParkingLot(BackgroundDrawingContainer, selectableParkingLot);
                                    };
                                }
                            }
                        };
                    }
                }else if(args.PropertyName == nameof(Vm.SelectedParkingLot))
                {
                    DrawingService.RedrawParkingLot(BackgroundDrawingContainer, Vm.SelectedParkingLot);
                    DrawingService.RedrawParkingLot(BackgroundDrawingContainer, _selectedLot);
                    _selectedLot = Vm.SelectedParkingLot;
                    var selectedParkingLotPoint = _selectedLot?.ParkingLot?.Coordinates?.Point;
                    if (selectedParkingLotPoint != null)
                    {
                        bool isParkingLotInView;
                        Map.IsLocationInView(selectedParkingLotPoint, out isParkingLotInView);
                        if (!isParkingLotInView)
                        {
                            await Map.TrySetViewAsync(selectedParkingLotPoint);
                        }
                    }
                }else if (args.PropertyName == nameof(Vm.ParkingLotFilterMode))
                {
                    UpdateParkingLotFilter();
                }else if (args.PropertyName == nameof(Vm.ParkingLotsGroupedCollectionViewSource))
                {
                    var cvs = Resources["SelectedCityData"] as CollectionViewSource;
                    cvs.IsSourceGrouped = true;
                    cvs.Source = Vm.ParkingLotsGroupedCollectionViewSource;
                }
                else if (args.PropertyName == nameof(Vm.ParkingLotsListCollectionViewSource))
                {
                    var cvs = Resources["SelectedCityData"] as CollectionViewSource;
                    cvs.IsSourceGrouped = false;
                    cvs.Source = Vm.ParkingLotsListCollectionViewSource;
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

        private void SplitViewItemSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SplitView.DisplayMode != SplitViewDisplayMode.Inline)
            {
                SplitView.IsPaneOpen = false;
            }
        }
    }
}
