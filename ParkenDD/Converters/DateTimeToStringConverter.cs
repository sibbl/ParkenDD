using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using ParkenDD.Services;

namespace ParkenDD.Converters
{
    public class DateTimeToStringConverter : DependencyObject, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var res = ResourceService.Instance;
            if (!(value is DateTime))
            {
                return value;
            }
            var dt = (DateTime) value;
            var now = DateTime.Now;
            if (now.Date.Equals(dt.Date))
            {
                return dt.ToString(res.ParkingLotLastRefreshHourFormat);
            }
            var days = (now.Date - dt.Date).Days;
            if (days < 2)
            {
                return string.Format(res.ParkingLotLastRefreshYesterdayAt, dt.ToString(res.ParkingLotLastRefreshHourFormat));
            }
            return string.Format(res.ParkingLotLastRefreshDaysAgo , days);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
