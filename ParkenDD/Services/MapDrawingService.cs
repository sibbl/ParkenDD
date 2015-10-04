using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media.Imaging;
using GalaSoft.MvvmLight.Threading;
using ParkenDD.Api.Models;
using ParkenDD.Controls;
using ParkenDD.Models;
using ParkenDD.ViewModels;

namespace ParkenDD.Services
{
    public class MapDrawingService
    {
        private const int ZindexTopmostParkingLot = 100001; //100% * 1000 + 1 to be on top
        private const int ZindexSearchResult = 100002;
        private const int ZindexUserPosition = 100003;

        private readonly MainViewModel _mainVm;
        private Dictionary<string, MapIcon> _mapIconParkingLotDict;
        private MapIcon _searchResultIcon;
        private MapIcon _userPositionIcon;
        public MapDrawingService(MainViewModel mainVm)
        {
            _mainVm = mainVm;
        }

        public async void RedrawParkingLot(Grid drawingContainer, SelectableParkingLot lot)
        {
            if (lot != null && _mapIconParkingLotDict != null && _mapIconParkingLotDict.ContainsKey(lot.ParkingLot.Id))
            {
                var icon = _mapIconParkingLotDict[lot.ParkingLot.Id];
                icon.Image = await GetMapIconDonutImage(drawingContainer, lot.ParkingLot);
                icon.ZIndex = GetZIndexForParkingLot(lot.ParkingLot);
            }
        }

        private int GetZIndexForParkingLot(ParkingLot lot)
        {
            if (_mainVm.SelectedParkingLot?.ParkingLot == lot)
            {
                return ZindexTopmostParkingLot; //100% * 1000 + 1 to be on top
            }
            var zIndex = ((double)lot.FreeLots / (double)lot.TotalLots);
            if (double.IsNaN(zIndex) || double.IsInfinity(zIndex))
            {
                zIndex = 0;
            }
            else if (zIndex > 100)
            {
                zIndex = 100;
            }
            return (int)Math.Round(zIndex * 1000);
        }

        private async void DrawParkingLot(MapControl map, Grid drawingContainer, SelectableParkingLot lot)
        {
            if (lot?.ParkingLot?.Coordinates != null)
            {
                var icon = new MapIcon
                {
                    Location = lot.ParkingLot.Coordinates.Point,
                    NormalizedAnchorPoint = new Point(0.5, 0.5),
                    Title = lot.ParkingLot.Name,
                    CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible,
                    Image = await GetMapIconDonutImage(drawingContainer, lot.ParkingLot),
                    ZIndex = GetZIndexForParkingLot(lot.ParkingLot),
                };
                if (_mapIconParkingLotDict.ContainsKey(lot.ParkingLot.Id))
                {
                    //TODO: optimize this code. Don't redraw the whole icon if maybe only location or title changed
                    map.MapElements.Remove(_mapIconParkingLotDict[lot.ParkingLot.Id]);
                    _mapIconParkingLotDict.Remove(lot.ParkingLot.Id);
                }
                _mapIconParkingLotDict.Add(lot.ParkingLot.Id, icon);
                map.MapElements.Add(icon);
            }
        }

        public void RemoveSearchResult(MapControl map)
        {
            if (_searchResultIcon != null)
            {
                map.MapElements.Remove(_searchResultIcon);
                _searchResultIcon = null;
            }
        }

        public void DrawSearchResult(MapControl map, AddressSearchSuggestionItem result)
        {
            if (_searchResultIcon == null)
            {
                _searchResultIcon = new MapIcon();
                map.MapElements.Add(_searchResultIcon);
                _searchResultIcon.ZIndex = ZindexSearchResult;
                _searchResultIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/SearchMapIcon.png"));
                _searchResultIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
                _searchResultIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
            }
            _searchResultIcon.Location = result.Point;
            _searchResultIcon.Title = result.ToString();
        }

        public void DrawUserPosition(MapControl map, Geoposition point)
        {
            if (_userPositionIcon == null)
            {
                _userPositionIcon = new MapIcon();
                map.MapElements.Add(_userPositionIcon);
                _userPositionIcon.Title = ResourceService.Instance.MapCurrentLocationLabel;
                _userPositionIcon.ZIndex = ZindexUserPosition;
                _userPositionIcon.Image = RandomAccessStreamReference.CreateFromUri(new Uri("ms-appx:///Assets/UserPositionMapIcon.png"));
                _userPositionIcon.NormalizedAnchorPoint = new Point(0.5, 0.5);
                _userPositionIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
            }
            _userPositionIcon.Location = point.Coordinate.Point;
        }

        public void DrawParkingLots(MapControl map, Grid drawingContainer)
        {
            var lots = _mainVm.ParkingLots;
            if (_mapIconParkingLotDict != null)
            {
                foreach (var icon in _mapIconParkingLotDict)
                {
                    map.MapElements.Remove(icon.Value);
                }
            }
            _mapIconParkingLotDict = new Dictionary<string, MapIcon>();
            if (lots != null)
            {
                foreach (var lot in lots)
                {
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        DrawParkingLot
                            (map,
                                drawingContainer,
                                lot);
                    });
                }
            }
        }

        private async Task<IRandomAccessStreamReference> GetMapIconDonutImage(Grid drawingContainer, ParkingLot lot)
        {
            var drawDonut = new ParkingLotLoadDonut
            {
                ParkingLot = lot,
                Style = Application.Current.Resources["ParkingLotMapIconDonutStyle"] as Style
            };
            drawingContainer.Children.Add(drawDonut);

            var rtb = new RenderTargetBitmap();
            await rtb.RenderAsync(drawDonut);

            drawingContainer.Children.Remove(drawDonut);

            //TODO: take care of possible scaling issues

            var pixels = (await rtb.GetPixelsAsync()).ToArray();
            var stream = new InMemoryRandomAccessStream();
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.BmpEncoderId, stream);
            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, (uint)rtb.PixelWidth, (uint)rtb.PixelHeight, 96, 96, pixels);
            await encoder.FlushAsync();
            stream.Seek(0);
            return RandomAccessStreamReference.CreateFromStream(stream.AsStream().AsRandomAccessStream());
        }

        public SelectableParkingLot GetParkingLotOfIcon(MapIcon icon)
        {
            if (icon != null && _mapIconParkingLotDict != null && _mapIconParkingLotDict.ContainsValue(icon))
            {
                var id = _mapIconParkingLotDict.FirstOrDefault(x => x.Value == icon).Key;
                return _mainVm.ParkingLots.FirstOrDefault(x => x.ParkingLot.Id == id);
            }
            return null;
        }
    }
}
