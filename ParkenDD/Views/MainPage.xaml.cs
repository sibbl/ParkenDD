using System;
using System.ComponentModel;
using System.Linq;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;
using ParkenDD.Messages;
using ParkenDD.ViewModels;
using Microsoft.Practices.ServiceLocation;
using ParkenDD.Controls;
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
                UpdateParkingLotFilter();
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

            Messenger.Default.Register(this, async (ShowSearchResultOnMapMessage msg) =>
            {
                await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    DrawingService.DrawSearchResult(Map, msg.Result);
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
                    bool isParkingLotInView;
                    Map.IsLocationInView(selectedParkingLotPoint, out isParkingLotInView);
                    if (!isParkingLotInView)
                    {
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
    }
}
