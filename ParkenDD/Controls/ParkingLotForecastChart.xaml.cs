using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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
        //TODO: localize
        private List<ParkingLotForecastTimespanSelection> ComboBoxValues { get; } = new List<ParkingLotForecastTimespanSelection>
        {
            new ParkingLotForecastTimespanSelection(ParkingLotForecastTimespanEnum.Hours6),
            new ParkingLotForecastTimespanSelection(ParkingLotForecastTimespanEnum.Hours24),
            new ParkingLotForecastTimespanSelection(ParkingLotForecastTimespanEnum.Days7),
        };

        private bool _initialized;

        public ParkingLotForecastChart()
        {
            InitializeComponent();

            ValueAxis.Minimum = 0;
            ValueAxis.Maximum = 100;
            ValueAxis.Interval = 10;

            SelectionComboBox.ItemsSource = ComboBoxValues;
            SelectionComboBox.SelectedItem = ComboBoxValues[0];

            SelectionComboBox.SelectionChanged += (sender, args) => UpdateChart();

            DataContextChanged += (sender, args) => UpdateChart();
        }

        private async void UpdateChart()
        {
            //TODO: cache data somewhere
            //TODO: cancel running updating tasks
            ForecastChart.Series.Clear();
            var parkingLot = DataContext as ParkingLot;
            var timeSpanSelection = SelectionComboBox.SelectedItem as ParkingLotForecastTimespanSelection;
            var timeSpan = timeSpanSelection?.TimeSpan;
            if (parkingLot != null && parkingLot.HasForecast && timeSpan.HasValue)
            {
                //TODO: add some fancy animation!
                ForecastChart.Opacity = 0.2;
                LoadingProgressRing.Visibility = Visibility.Visible;
                var api = ServiceLocator.Current.GetInstance<IParkenDdClient>();
                var mainVm = ServiceLocator.Current.GetInstance<MainViewModel>();
                var forecast = await
                    api.GetForecastAsync(mainVm.SelectedCity.Id, parkingLot.Id, DateTime.Now,
                        DateTime.Now.Add(timeSpan.Value));
                var points =
                    forecast.Data.Select(
                        item => new ParkingLotForecastDataPoint(item.Value, item.Key)).ToList();
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
                if (_initialized)
                {
                    ServiceLocator.Current.GetInstance<TrackingService>()?
                        .TrackForecastRangeEvent(parkingLot, timeSpanSelection?.Mode);
                }
                _initialized = false;
            }
        }
    }
}
