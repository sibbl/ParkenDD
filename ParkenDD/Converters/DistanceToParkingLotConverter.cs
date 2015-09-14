using System;
using Windows.UI.Xaml.Data;
using Microsoft.Practices.ServiceLocation;
using ParkenDD.Api.Models;
using ParkenDD.Utils;
using ParkenDD.ViewModels;

namespace ParkenDD.Converters
{
    public class DistanceToParkingLotConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var lot = value as ParkingLot;
            var location = ServiceLocator.Current.GetInstance<MainViewModel>()?.UserLocation;
            if (lot?.Coordinates?.Point == null || location == null)
            {
                return string.Empty;
            }
            var dist = location.Coordinate.Point.GetDistanceTo(lot.Coordinates.Point);
            return dist > 1 ? string.Format("{0:0.#} km", dist) : string.Format("{0} m", dist * 1000);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
