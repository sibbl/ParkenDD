using System;
using Windows.UI.Xaml.Data;

namespace ParkenDD.Converters
{
    public class TimeSpanToDateTimeAxisLabelFormatConverter : IValueConverter
    {
        private const string HourFormat = "{}{0:H:mm}";
        private const string DayFormat = "{}{0:dd.MM.}";
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is TimeSpan))
            {
                return HourFormat;
            }
            var ts = (TimeSpan) value;
            if (ts.TotalDays > 1)
            {
                return DayFormat;
            }
            else
            {
                return HourFormat;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
