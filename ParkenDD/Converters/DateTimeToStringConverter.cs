using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ParkenDD.Converters
{
    public class DateTimeToStringConverter : DependencyObject, IValueConverter
    {
        private const string HourFormat = "H:mm";
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is DateTime))
            {
                return value;
            }
            var dt = (DateTime) value;
            var now = DateTime.Now;
            if (now.Date.Equals(dt.Date))
            {
                return dt.ToString(HourFormat);
            }
            var days = (now.Date - dt.Date).Days;
            if (days < 2)
            {
                //TODO: localize
                return string.Format("gestern um {0}", dt.ToString(HourFormat));
            }
            //TODO: localize
            return string.Format("vor {0} Tagen", days);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
