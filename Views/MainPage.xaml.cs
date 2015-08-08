using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;
using ParkenDD.Messages;
using ParkenDD.Api.Models;
using ParkenDD.ViewModels;
using System.Linq;
using ParkenDD.Controls;

namespace ParkenDD.Views
{
    public sealed partial class MainPage : Page
    {
        private Dictionary<ParkingLot, MapIcon> _mapIconParkingLotDict; 
        public MainViewModel Vm => (MainViewModel)DataContext;

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
            Vm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(Vm.ParkingLots))
                {
                    DrawLotsOnMap();
                    if (Vm.ParkingLots != null)
                    {
                        Vm.ParkingLots.CollectionChanged += (o, eventArgs) => DrawLotsOnMap();
                    }
                }
            };
            ParkingLotList.SelectionChanged += (sender, args) =>
            {
                ParkingLotList.ScrollIntoView(ParkingLotList.SelectedItem);
            };
            Map.MapElementClick += (sender, args) =>
            {
                MapIcon iconOnTop = null;
                foreach (var element in args.MapElements)
                {
                    if (element is MapIcon && (iconOnTop == null || iconOnTop.ZIndex < element.ZIndex))
                    {
                        iconOnTop = (MapIcon)element;
                    }
                }
                if (iconOnTop != null && _mapIconParkingLotDict != null && _mapIconParkingLotDict.ContainsValue(iconOnTop))
                {
                    var oldSelectedParkingLot = Vm.SelectedParkingLot;
                    Vm.SelectedParkingLot = _mapIconParkingLotDict.FirstOrDefault(x => x.Value == iconOnTop).Key;
                    RedrawLot(oldSelectedParkingLot);
                    RedrawLot(Vm.SelectedParkingLot);
                }
            };
        }

        private void ToggleSplitView(object sender, RoutedEventArgs routedEventArgs)
        {
            SplitView.IsPaneOpen = !SplitView.IsPaneOpen;
        }

        private async void RedrawLot(ParkingLot lot)
        {
            if(_mapIconParkingLotDict.ContainsKey(lot))
            {
                var icon = _mapIconParkingLotDict[lot];
                icon.Image = await GetMapIconDonutImage(lot);
                icon.ZIndex = getZindexForLot(lot);
            }
        }

        private int getZindexForLot(ParkingLot lot)
        {
            var zIndex = ((double)lot.FreeLots / (double)lot.TotalLots);
            if (Double.IsNaN(zIndex) || Double.IsInfinity(zIndex))
            {
                zIndex = 0;
            }
            return (int)Math.Round(zIndex * 1000);
        }

        private async void DrawLotOnMap(ParkingLot lot)
        {
            if (lot?.Coordinates != null)
            {
                var icon = new MapIcon
                {
                    Location = lot.Coordinates.Point,
                    NormalizedAnchorPoint = new Point(0.5, 0.5),
                    Title = lot.Name,
                    CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible,
                    Image = await GetMapIconDonutImage(lot),
                    ZIndex = getZindexForLot(lot),
                };
                _mapIconParkingLotDict.Add(lot, icon);
                Map.MapElements.Add(icon);
            }
        }

        private void DrawLotsOnMap()
        {
            var lots = Vm.ParkingLots;
            Map.MapElements.Clear();
            _mapIconParkingLotDict = new Dictionary<ParkingLot, MapIcon>();
            if (lots != null)
            {
                foreach (var lot in lots)
                {
                    DrawLotOnMap(lot);
                }
            }
        }

        public async Task<IRandomAccessStreamReference> GetMapIconDonutImage(ParkingLot lot)
        {
            var drawDonut = new ParkingLotLoadDonut();
            drawDonut.ParkingLot = lot;
            drawDonut.Style = Application.Current.Resources["ParkingLotMapIconDonutStyle"] as Style;
            BackgroundDrawingContainer.Children.Add(drawDonut);

            var rtb = new RenderTargetBitmap();
            await rtb.RenderAsync(drawDonut);

            BackgroundDrawingContainer.Children.Remove(drawDonut);

            var pixels = (await rtb.GetPixelsAsync()).ToArray();
            var stream = new InMemoryRandomAccessStream();
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)rtb.PixelWidth, (uint)rtb.PixelHeight, 96, 96, pixels);
            await encoder.FlushAsync();
            stream.Seek(0);
            return RandomAccessStreamReference.CreateFromStream(stream.AsStream().AsRandomAccessStream());
        }
    }
}
