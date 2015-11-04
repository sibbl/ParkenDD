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
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.ServiceLocation;
using ParkenDD.Api.Models;
using ParkenDD.Messages;
using ParkenDD.Models;
using ParkenDD.Services;
using ParkenDD.Utils;
using ParkenDD.ViewModels;

namespace ParkenDD.Views
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel Vm => (MainViewModel)DataContext;
        private static MapDrawingService DrawingService => ServiceLocator.Current.GetInstance<MapDrawingService>();
        private ParkingLot _selectedLot;
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
                Debug.WriteLine("[MainView] map: map loaded");
                if (_initialMapBbox != null)
                {
                    Debug.WriteLine("[MainView] map: map loaded, set initial bounds");
                    await Map.TrySetViewBoundsAsync(_initialMapBbox, null, MapAnimationKind.None);
                    _zoomedToInitialView = true;
                }
                else if(_initialCoordinates != null)
                {
                    Debug.WriteLine("[MainView] map: map loaded, set initial coords");
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
                        Debug.WriteLine("[MainView] map: zoom map to bounds msg");
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
                        Debug.WriteLine("[MainView] map: zoom map to coordinates msg");
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
                    Debug.WriteLine("[MainView] map: show search result - wait until initial view was zoomed to");
                    await WaitForInitialMapZoom();
                    Debug.WriteLine("[MainView] map: show search result");
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

            Messenger.Default.Register(this, (UpdateParkingLotListSelectionMessage msg) =>
            {
                Debug.WriteLine("[MainView] parking lot list: message received");
                SetParkingLotListSelectionVisualState();
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
                        var lot = selectableParkingLot;
                        lot.PropertyChanged += (sender1, changedEventArgs) => RedrawOnPropertyChanged(lot, changedEventArgs);
                    }
                    Vm.ParkingLots.CollectionChanged += (o, eventArgs) =>
                    {
                        DrawingService.DrawParkingLots(Map, BackgroundDrawingContainer);
                        if (eventArgs.NewItems != null)
                        {
                            foreach (var selectableParkingLot in eventArgs.NewItems.OfType<ParkingLot>())
                            {
                                var lot = selectableParkingLot;
                                lot.PropertyChanged += (sender1, changedEventArgs) => RedrawOnPropertyChanged(lot, changedEventArgs);
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
                var selectedParkingLotPoint = _selectedLot?.Coordinates?.Point;
                if (selectedParkingLotPoint != null)
                {
                    Debug.WriteLine("[MainView] map: selected parking lot - wait until initial view was zoomed to (" + _selectedLot?.Id + ")");
                    await WaitForInitialMapZoom();
                    bool isParkingLotInView;
                    Map.IsLocationInView(selectedParkingLotPoint, out isParkingLotInView);
                    Debug.WriteLine("[MainView] map: selected parking lot - check (" + _selectedLot?.Id + ")");
                    if (Map.ZoomLevel < 14)
                    {
                        Debug.WriteLine("[MainView] map: selected parking lot, zoom level too high (" + _selectedLot?.Id + ")");
                        await Map.TrySetViewAsync(selectedParkingLotPoint, 14);
                    }
                    else if (!isParkingLotInView)
                    {
                        Debug.WriteLine("[MainView] map: selected parking lot, parking lot not in view (" + _selectedLot?.Id + ")");
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

        private void RedrawOnPropertyChanged(ParkingLot lot, PropertyChangedEventArgs args)
        {
            if (lot != null)
            {
                if (args.PropertyName == nameof(lot.FreeLots) ||
                    args.PropertyName == nameof(lot.TotalLots))
                {
                    DrawingService.RedrawParkingLot(BackgroundDrawingContainer, lot);
                }
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
                Debug.WriteLine("[MainView] map: wait...");
                await Task.Delay(200);
                counter++;
            }
        }

        private void ParkingLotListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var oldItem = e.RemovedItems.FirstOrDefault();
            if (oldItem != null)
            {
                var oldItemContainer = ParkingLotList.ContainerFromItem(oldItem);
                var oldListViewItem = oldItemContainer as ListViewItem;
                if (oldListViewItem != null)
                {
                    Debug.WriteLine("[MainView] parking lot list: remove selected visual state for " + (oldItem as ParkingLot).Name);
                    VisualStateManager.GoToState(oldListViewItem.ContentTemplateRoot as Control, "Unselected", false);
                }
            }
            var newItem = e.AddedItems.FirstOrDefault();
            if (newItem != null)
            {
                var newItemContainer = ParkingLotList.ContainerFromItem(newItem);
                var newListViewItem = newItemContainer as ListViewItem;
                if (newListViewItem != null)
                {
                    Debug.WriteLine("[MainView] parking lot list: add selected visual state for " + (newItem as ParkingLot).Name);
                    VisualStateManager.GoToState(newListViewItem.ContentTemplateRoot as Control, "Selected", false);
                }
            }
        }

        private async void SetParkingLotListSelectionVisualState(ParkingLot selectedItem = null)
        {
            if (selectedItem == null)
            {
                selectedItem = Vm.SelectedParkingLot;
            }
            if (selectedItem != null)
            {
                ListViewItem listViewItem = null;
                var count = 0;
                while (listViewItem == null && count < 10)
                {
                    var itemContainer = ParkingLotList.ContainerFromItem(selectedItem);
                    listViewItem = itemContainer as ListViewItem;
                    await Task.Delay(200);
                    count++;
                }

                if (listViewItem != null)
                {
                    Debug.WriteLine("[MainView] parking lot list: set selected visual state for " + selectedItem.Name);
                    VisualStateManager.GoToState(listViewItem.ContentTemplateRoot as Control, "Selected", false);
                }
                else
                {
                    Debug.WriteLine("[MainVm] parking lot list: list view item is null :/");
                }
            }
            else
            {
                Debug.WriteLine("[MainView] parking lot list: set selected index to -1");
                ParkingLotList.SelectedIndex = -1;
            }
        }
    }
}
