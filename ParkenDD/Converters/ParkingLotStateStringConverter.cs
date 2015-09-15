using System;
using Windows.UI.Xaml.Data;
using ParkenDD.Api.Models;

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
                //TODO: localize
                case ParkingLotState.Closed:
                    return "geschlossen";
                case ParkingLotState.Open:
                    return "geöffnet";
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
