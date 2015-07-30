using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;
using ParkenDD.Win10.Models;

namespace ParkenDD.Win10.Converter.Value
{
    public class ParkingLotListGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var list = value as IEnumerable<ParkingLot>;
            if (list != null)
            {
                var orderAscending = true;
                if (parameter is bool)
                {
                    orderAscending = (bool) parameter;
                }
                return ParkingLotListGroup.CreateGroups(list, orderAscending);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
