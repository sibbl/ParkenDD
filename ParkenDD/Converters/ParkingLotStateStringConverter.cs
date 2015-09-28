using System;
using Windows.UI.Xaml.Data;
using ParkenDD.Api.Models;
using ParkenDD.Services;

namespace ParkenDD.Converters
{
    public class ParkingLotStateStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is ParkingLotState))
            {
                return string.Empty;
            }
            var state = (ParkingLotState) value;
            switch (state)
            {
                case ParkingLotState.Closed:
                    return ResourceService.Instance.ParkingLotStateClosed;
                case ParkingLotState.Open:
                    return ResourceService.Instance.ParkingLotStateOpen;
                default:
                    return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
