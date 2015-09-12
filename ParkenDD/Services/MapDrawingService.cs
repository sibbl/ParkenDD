using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;
using Windows.UI.Xaml.Media.Imaging;
using ParkenDD.Api.Models;
using ParkenDD.Controls;
using ParkenDD.Models;
using ParkenDD.ViewModels;

namespace ParkenDD.Services
{
    public class MapDrawingService
    {
        private readonly MainViewModel _mainVm;
        private Dictionary<SelectableParkingLot, MapIcon> _mapIconParkingLotDict;
        public MapDrawingService(MainViewModel mainVm)
        {
            _mainVm = mainVm;
        }

        public async void RedrawParkingLot(Grid drawingContainer, SelectableParkingLot lot)
        {
            if (lot != null && _mapIconParkingLotDict != null && _mapIconParkingLotDict.ContainsKey(lot))
            {
                var icon = _mapIconParkingLotDict[lot];
                icon.Image = await GetMapIconDonutImage(drawingContainer, lot.ParkingLot);
                icon.ZIndex = GetZIndexForParkingLot(lot.ParkingLot);
            }
        }

        private int GetZIndexForParkingLot(ParkingLot lot)
        {
            if (_mainVm.SelectedParkingLot?.ParkingLot == lot)
            {
                return 100001; //100% * 1000 + 1 to be on top
            }
            var zIndex = ((double)lot.FreeLots / (double)lot.TotalLots);
            if (Double.IsNaN(zIndex) || Double.IsInfinity(zIndex))
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
                if (_mapIconParkingLotDict.ContainsKey(lot))
                {
                    //TODO: optimize this code. Don't redraw the whole icon if maybe only location or title changed
                    map.MapElements.Remove(_mapIconParkingLotDict[lot]);
                    _mapIconParkingLotDict.Remove(lot);
                }
                _mapIconParkingLotDict.Add(lot, icon);
                map.MapElements.Add(icon);
            }
        }

        public void DrawParkingLots(MapControl map, Grid drawingContainer)
        {
            var lots = _mainVm.ParkingLots;
            map.MapElements.Clear();
            _mapIconParkingLotDict = new Dictionary<SelectableParkingLot, MapIcon>();
            if (lots != null)
            {
                foreach (var lot in lots)
                {
                    DrawParkingLot(map, drawingContainer, lot);
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
                return _mapIconParkingLotDict.FirstOrDefault(x => x.Value == icon).Key;
            }
            return null;
        }
    }
}
