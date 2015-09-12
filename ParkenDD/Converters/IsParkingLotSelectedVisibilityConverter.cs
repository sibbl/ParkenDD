using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using Microsoft.Practices.ServiceLocation;
using ParkenDD.Api.Models;
using ParkenDD.ViewModels;

namespace ParkenDD.Converters
{
    public class IsParkingLotSelectedVisibilityConverter : DependencyObject, IValueConverter
    {
        public ParkingLot SelectedParkingLot
        {
            get { return (ParkingLot)GetValue(SelectedParkingLotProperty); }
            set { SetValue(SelectedParkingLotProperty, value); }
        }

        public static readonly DependencyProperty SelectedParkingLotProperty =
            DependencyProperty.Register("SelectedParkingLot",
                                        typeof(ParkingLot),
                                        typeof(IsParkingLotSelectedVisibilityConverter),
                                        new PropertyMetadata(null));

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var parkingLot = value as ParkingLot;
            if (parkingLot != null)
            {
                return parkingLot == ServiceLocator.Current.GetInstance<MainViewModel>()?.SelectedParkingLot?.ParkingLot
                    ? Visibility.Visible
                    : Visibility.Collapsed;
            }
            return  Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
