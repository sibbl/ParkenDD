using Windows.UI.Xaml.Controls;
using Microsoft.Practices.ServiceLocation;
using ParkenDD.Api.Models;
using ParkenDD.Utils;
using ParkenDD.ViewModels;

namespace ParkenDD.Controls
{
    public sealed partial class DistanceTextBlock : UserControl
    {
        private readonly MainViewModel _mainVm;
        public DistanceTextBlock()
        {
            InitializeComponent();
            DataContextChanged += (sender, args) =>
            {
                UpdateDistance();
            };
            _mainVm = ServiceLocator.Current.GetInstance<MainViewModel>();
            _mainVm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_mainVm.UserLocation))
                {
                    UpdateDistance();
                }
            };
        }

        private void UpdateDistance()
        {
            var coord = DataContext as Coordinate;
            if (coord == null)
            {
                return;
            }
            var pos = _mainVm.UserLocation;

            if (coord.Point == null || pos == null)
            {
                DistanceText.Text = string.Empty;
            }
            else
            {
                var dist = pos.Coordinate.Point.GetDistanceTo(coord.Point);
                DistanceText.Text = dist > 1 ? string.Format("{0:0.#} km", dist) : string.Format("{0:0} m", dist*1000);
            }
        }
    }
}
