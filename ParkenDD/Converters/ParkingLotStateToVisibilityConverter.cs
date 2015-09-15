using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using ParkenDD.Api.Models;

namespace ParkenDD.Converters
{
    public class ParkingLotStateToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is ParkingLotState))
            {
                return Visibility.Collapsed;
            }
            var state = (ParkingLotState)value;
            if (state == ParkingLotState.NoData)
                return Visibility.Collapsed;
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
