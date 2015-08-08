using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;
using ParkenDD.Models;

namespace ParkenDD.Converters
{
    public class IsCollectionViewSourceGroupedConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return value is List<ParkingLotListGroup>;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
