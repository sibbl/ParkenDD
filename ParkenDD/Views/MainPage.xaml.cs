using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using ParkenDD.Messages;
using ParkenDD.ViewModels;
using Microsoft.Practices.ServiceLocation;
using ParkenDD.Models;
using ParkenDD.Services;
using ParkenDD.Utils;

namespace ParkenDD.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel Vm => (MainViewModel)DataContext;
        private static MapDrawingService DrawingService => ServiceLocator.Current.GetInstance<MapDrawingService>();
        private SelectableParkingLot _selectedLot;
        private GeoboundingBox _initialMapBbox;
        private Geopoint _initialCoordinates;
        private bool _infoDialogVisible;
        private bool _zoomedToInitialView;
        private bool _mapLoaded;

        public MainPage()
        {
            InitializeComponent();

            NavigationCacheMode = NavigationCacheMode.Required;

            CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;

            Map.Loaded += async (sender, args) =>
            {
                Debug.WriteLine("Mainpage map: map loaded");
                if (_initialMapBbox != null)
                {
                    Debug.WriteLine("Mainpage map: map loaded, set initial bounds");
                    await Map.TrySetViewBoundsAsync(_initialMapBbox, null, MapAnimationKind.None);
                    _zoomedToInitialView = true;
                }
                else if(_initialCoordinates != null)
                {
                    Debug.WriteLine("Mainpage map: map loaded, set initial coords");
                    await Map.TrySetViewAsync(_initialCoordinates, null, null, null, MapAnimationKind.None);
                    _zoomedToInitialView = true;
                }
                _mapLoaded = true;
            };

            Messenger.Default.Register(this, (ZoomMapToBoundsMessage msg) =>
            {
                if (!_mapLoaded)
                {
                    _initialMapBbox = msg.BoundingBox;
                        //set initial bbox as the following won't work while splash screen is still visible
                }
                else
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(async () =>
                    {
                        Debug.WriteLine("Mainpage map: zoom map to bounds msg");
                        await Map.TrySetViewBoundsAsync(msg.BoundingBox, null, MapAnimationKind.Bow);
                        _zoomedToInitialView = _mapLoaded;
                    });
                }
            });

            Messenger.Default.Register(this, (ZoomMapToCoordinateMessage msg) =>
            {
                if (!_mapLoaded)
                {
                    _initialCoordinates = msg.Point;
                        //set initial coordinates as the following won't work while splash screen is still visible
                }
                else
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(async () =>
                    {
                        Debug.WriteLine("Mainpage map: zoom map to coordinates msg");
                        await
                            Map.TrySetViewAsync(msg.Point, null, null, null, MapAnimationKind.Bow);
                        _zoomedToInitialView = _mapLoaded;
                    });
                }
            });

            Messenger.Default.Register(this, (ShowSearchResultOnMapMessage msg) =>
            {
                DispatcherHelper.CheckBeginInvokeOnUI(async () =>
                {
                    DrawingService.DrawSearchResult(Map, msg.Result);
                    Debug.WriteLine("Mainpage map: show search result - wait until initial view was zoomed to");
                    await WaitForInitialMapZoom();
                    Debug.WriteLine("Mainpage map: show search result");
                    await Map.TrySetViewAsync(msg.Result.Point, null, null, null, MapAnimationKind.Bow);
                });
            });

            Messenger.Default.Register(this, (HideSearchResultOnMapMessage msg) =>
            {
                DrawingService.RemoveSearchResult(Map);
            });

            Messenger.Default.Register(this, (InfoDialogToggleVisibilityMessage msg) =>
            {
                FindName(nameof(InfoDialog));
                _infoDialogVisible = msg.IsVisible;
                InfoDialog.Visibility = msg.Visibility;
            });

            Vm.PropertyChanged += OnViewModelPropertyChanged;

            ParkingLotList.SelectionChanged += (sender, args) =>
            {
                ParkingLotList.ScrollIntoView(ParkingLotList.SelectedItem);
            };

            Map.MapElementClick += (sender, args) =>
            {
                Vm.SelectedParkingLot = DrawingService.GetParkingLotOfIcon(args.MapElements.GetTopmostIcon());
            };

            SystemNavigationManager.GetForCurrentView().BackRequested += (sender, args) =>
            {
                if (_infoDialogVisible)
                {
                    Messenger.Default.Send(new InfoDialogToggleVisibilityMessage(false));
                    args.Handled = true;
                }
            };

            UpdateParkingLotFilter();
        }

        private async void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(Vm.ParkingLots))
            {
                FindName(nameof(BackgroundDrawingContainer));
                DrawingService.DrawParkingLots(Map, BackgroundDrawingContainer);
                if (Vm.ParkingLots != null)
                {
                    foreach (var selectableParkingLot in Vm.ParkingLots)
                    {
                        selectableParkingLot.ParkingLot.PropertyChanged += (sender1, changedEventArgs) => { DrawingService.RedrawParkingLot(BackgroundDrawingContainer, selectableParkingLot); };
                    }
                    Vm.ParkingLots.CollectionChanged += (o, eventArgs) =>
                    {
                        DrawingService.DrawParkingLots(Map, BackgroundDrawingContainer);
                        if (eventArgs.NewItems != null)
                        {
                            foreach (var selectableParkingLot in eventArgs.NewItems.OfType<SelectableParkingLot>())
                            {
                                selectableParkingLot.ParkingLot.PropertyChanged += (sender1, changedEventArgs) => { DrawingService.RedrawParkingLot(BackgroundDrawingContainer, selectableParkingLot); };
                            }
                        }
                    };
                }
            }
            else if (args.PropertyName == nameof(Vm.SelectedParkingLot))
            {
                DrawingService.RedrawParkingLot(BackgroundDrawingContainer, Vm.SelectedParkingLot);
                DrawingService.RedrawParkingLot(BackgroundDrawingContainer, _selectedLot);
                _selectedLot = Vm.SelectedParkingLot;
                var selectedParkingLotPoint = _selectedLot?.ParkingLot?.Coordinates?.Point;
                if (selectedParkingLotPoint != null)
                {
                    Debug.WriteLine("Mainpage map: selected parking lot - wait until initial view was zoomed to");
                    await WaitForInitialMapZoom();
                    bool isParkingLotInView;
                    Map.IsLocationInView(selectedParkingLotPoint, out isParkingLotInView);
                    Debug.WriteLine("Mainpage map: selected parking lot - check");
                    if (Map.ZoomLevel < 14)
                    {
                        Debug.WriteLine("Mainpage map: selected parking lot, zoom level too high");
                        await Map.TrySetViewAsync(selectedParkingLotPoint, 14);
                    }
                    else if (!isParkingLotInView)
                    {
                        Debug.WriteLine("Mainpage map: selected parking lot, parking lot not in view");
                        await Map.TrySetViewAsync(selectedParkingLotPoint);
                    }
                }
            }
            else if (args.PropertyName == nameof(Vm.ParkingLotFilterMode))
            {
                UpdateParkingLotFilter();
            }
            else if (args.PropertyName == nameof(Vm.ParkingLotsGroupedCollectionViewSource))
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
            else if (args.PropertyName == nameof(Vm.UserLocation))
            {
                DrawingService.DrawUserPosition(Map, Vm.UserLocation);
            }
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

        private void HideSplitViewPaneIfNotInline()
        {
            if (SplitView.DisplayMode != SplitViewDisplayMode.Inline)
            {
                SplitView.IsPaneOpen = false;
            }
        }

        private void SplitViewItemSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HideSplitViewPaneIfNotInline();
        }

        private void ShowInfoDialogButtonClick(object sender, RoutedEventArgs e)
        {
            HideSplitViewPaneIfNotInline();
        }

        private async Task WaitForInitialMapZoom()
        {
            var counter = 0;
            while (!_zoomedToInitialView && counter < 10) //stop waiting when done or after 10 * 200ms = 2sec
            {
                Debug.WriteLine("Mainpage map: wait...");
                await Task.Delay(200);
                counter++;
            }
        }

        private void UIElement_OnTapped(object sender, TappedRoutedEventArgs e)
        {
        }
    }
}
