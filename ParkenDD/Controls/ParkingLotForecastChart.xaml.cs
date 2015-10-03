using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Threading;
using Microsoft.Practices.ServiceLocation;
using ParkenDD.Api.Interfaces;
using ParkenDD.Api.Models;
using ParkenDD.Models;
using ParkenDD.Services;
using ParkenDD.ViewModels;
using WinRTXamlToolkit.Controls.DataVisualization.Charting;

namespace ParkenDD.Controls
{
    public sealed partial class ParkingLotForecastChart : UserControl
    {
        private List<ParkingLotForecastTimespanSelection> ComboBoxValues { get; } = new List<ParkingLotForecastTimespanSelection>
        {
            new ParkingLotForecastTimespanSelection(ParkingLotForecastTimespanEnum.Hours6),
            new ParkingLotForecastTimespanSelection(ParkingLotForecastTimespanEnum.Hours24),
            new ParkingLotForecastTimespanSelection(ParkingLotForecastTimespanEnum.Days7),
        };

        private bool _initialized;
        private DateTime? _cachedForecastEndDate;
        private readonly List<ParkingLotForecastDataPoint> _cachedForecast = new List<ParkingLotForecastDataPoint>();


        public bool IsSelected
        {
            get { return (bool) GetValue(IsSelectedProperty); }
            set
            {
                SetValue(IsSelectedProperty, value);
                if (value)
                {
                    InitForecast();
                }
            }
        }

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register("IsSelected",
                typeof (bool),
                typeof (ParkingLotForecastChart),
                new PropertyMetadata(false));

        public ParkingLotForecastChart()
        {
            InitializeComponent();
        }

        private async void UpdateChart()
        {
            var parkingLot = DataContext as ParkingLot;
            if (parkingLot != null && parkingLot.HasForecast)
            {
                //TODO: cache data somewhere
                //TODO: cancel running updating tasks
                if (_initialized)
                {
                    ForecastChart.Series.Clear();
                }
                var timeSpanSelection = SelectionComboBox.SelectedItem as ParkingLotForecastTimespanSelection;
                var timeSpan = timeSpanSelection?.TimeSpan;
                if (timeSpan.HasValue)
                {
                    ForecastChart.Opacity = 0.2;
                    LoadingProgressRing.Visibility = Visibility.Visible;
                    //TODO: play some fancy animation!
                    await Task.Run(async () =>
                    {
                        var api = ServiceLocator.Current.GetInstance<IParkenDdClient>();
                        var mainVm = ServiceLocator.Current.GetInstance<MainViewModel>();
                        var now = DateTime.Now;
                        var startDate = _cachedForecastEndDate?.AddMinutes(30) ?? now;
                        var endDate = now.Add(timeSpan.Value);
                        if (endDate > startDate)
                        {
                            var forecast = await
                                api.GetForecastAsync(mainVm.SelectedCity.Id, parkingLot.Id, startDate, endDate);
                            _cachedForecast.AddRange(forecast.Data.Select(
                                    item => new ParkingLotForecastDataPoint(item.Value, item.Key)));
                            if (_cachedForecastEndDate.HasValue)
                            {
                                if (_cachedForecastEndDate.Value < endDate)
                                {
                                    _cachedForecastEndDate = endDate;
                                }
                            }
                            else
                            {
                                _cachedForecastEndDate = endDate;
                            }
                        }
                        var points = _cachedForecast.Where(x => x.Time >= now && x.Time <= now.Add(timeSpan.Value));
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            var series = new AreaSeries
                            {
                                ItemsSource = points,
                                IndependentValuePath = "Time",
                                DependentValuePath = "Value",
                                Style = Application.Current.Resources["ParkingLotForecastChartAreaSeriesStyle"] as Style
                            };
                            ForecastChart.Series.Add(series);
                            ForecastChart.Opacity = 1;
                            LoadingProgressRing.Visibility = Visibility.Collapsed;
                        });
                        if (!_initialized)
                        {
                            ServiceLocator.Current.GetInstance<TrackingService>()?
                                .TrackForecastRangeEvent(parkingLot, timeSpanSelection?.Mode);
                        }
                        _initialized = true;
                    });
                }
            }
        }

        private void InitForecast()
        {
            if (!_initialized)
            {
                FindName(nameof(ForecastContainer));

                ValueAxis.Minimum = 0;
                ValueAxis.Maximum = 100;
                ValueAxis.Interval = 10;

                SelectionComboBox.ItemsSource = ComboBoxValues;
                SelectionComboBox.SelectedItem = ComboBoxValues[0];

                SelectionComboBox.SelectionChanged += (sender, args) => UpdateChart();

                UpdateChart();
            }
        }
    }
}
