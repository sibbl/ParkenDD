using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace ParkenDD.Converters
{
    public class IntGreaterZeroToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (!(value is int))
            {
                return false;
            }
            var num = (int) value;
            return num > 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
