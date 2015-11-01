using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;
using ParkenDD.Api.Models;
using ParkenDD.Messages;
using ParkenDD.Models;
using ParkenDD.Services;
using ParkenDD.Utils;
using ParkenDD.ViewModels;

namespace ParkenDD.Controls
{
    public sealed partial class DistanceTextBlock : UserControl
    {
        private readonly MainViewModel _mainVm;
        private readonly SettingsService _settings;
        private readonly LocalizationService _localization;
        public DistanceTextBlock()
        {
            InitializeComponent();
            DataContextChanged += (sender, args) =>
            {
                UpdateDistance();
            };
            _mainVm = ServiceLocator.Current.GetInstance<MainViewModel>();
            _settings = ServiceLocator.Current.GetInstance<SettingsService>();
            _localization = ServiceLocator.Current.GetInstance<LocalizationService>();
            _mainVm.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == nameof(_mainVm.UserLocation))
                {
                    UpdateDistance();
                }
            };
            Messenger.Default.Register(this, (SettingChangedMessage msg) =>
            {
                if (msg.IsSetting(nameof(_settings.DistanceUnit)) || msg.IsSetting(nameof(_settings.CurrentLocale)))
                {
                    UpdateDistance();
                }
            });
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
                var unit = _settings.DistanceUnit;
                var distance = pos.Coordinate.Point.GetDistanceTo(coord.Point, unit);
                var culture = _localization.GetCulture(_settings.CurrentLocale);
                switch (unit)
                {
                    case DistanceUnitEnum.Kilometers:
                        DistanceText.Text = distance > 1
                            ? string.Format(culture, "{0:0.#} km", distance)
                            : string.Format(culture, "{0:0} m", distance * 1000);
                        break;
                    case DistanceUnitEnum.Miles:
                        DistanceText.Text = distance > 1
                            ? string.Format(culture, "{0:0.#} mi", distance)
                            : string.Format(culture, "{0:0} yd", distance * 1760);
                        break;
                }
            }
        }
    }
}
