using System;
using System.Collections.Generic;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using GoogleAnalytics;
using GoogleAnalytics.Core;
using Microsoft.ApplicationInsights;
using ParkenDD.Api.Models;
using ParkenDD.Models;

namespace ParkenDD.Services
{
    public class TrackingService
    {
        private readonly TelemetryClient _client;
        private readonly Tracker _tracker;

        public TrackingService()
        {
            WindowsAppInitializer.InitializeAsync();
            _tracker = EasyTracker.GetTracker();

            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            _tracker.AppScreen = new Dimensions((int)(bounds.Width * scaleFactor), (int)(bounds.Height * scaleFactor));
            _tracker.UserAgentOverride = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;
            _tracker.SendEvent("device_information", "resolution", (bounds.Width * scaleFactor) + "x" + (bounds.Height * scaleFactor), 0);
            _tracker.SendEvent("device_information", "platform", Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily, 0);

            _client = new TelemetryClient();
        }

        public void TrackSelectParkingLotEvent(MetaDataCityRow city, ParkingLot parkingLot)
        {
            var properties = new Dictionary<string, string>
            {
                { "city", city.Id },
                { "parkingLot", parkingLot?.Id }
            };
            _client.TrackEvent("Select parking lot", properties);
            _tracker.SendEvent("ui_action", "select_parking_lot", city.Id + " > " + parkingLot?.Id, 0);
        }

        public void TrackSelectCityEvent(MetaDataCityRow city)
        {
            var properties = new Dictionary<string, string>
            {
                { "city", city.Id },
            };
            _client.TrackEvent("Select city", properties);
            _tracker.SendEvent("ui_action", "select_city", city.Id, 0);
        }

        public void TrackReloadCityEvent(MetaDataCityRow city)
        {
            var properties = new Dictionary<string, string>
            {
                { "city", city.Id },
            };
            _client.TrackEvent("Reload city", properties);
            _tracker.SendEvent("ui_action", "reload_city", city.Id, 0);
        }


        public void TrackParkingLotFilterEvent(ParkingLotFilterMode mode, bool ascending, bool group)
        {
            var properties = new Dictionary<string, string>
            {
                {"mode", mode.ToString()},
                {"ascending", ascending.ToString()},
                {"group", group.ToString()},
            };
            _client.TrackEvent("Change parking lot filter", properties);
            _tracker.SendEvent("ui_action", "set_filter", mode.ToString() + " / " + ascending.ToString() + " / " + group.ToString(), 0);
            _tracker.SendEvent("ui_action", "set_filter_mode", mode.ToString(), 0);
            _tracker.SendEvent("ui_action", "set_filter_ascending", ascending.ToString(), 0);
            _tracker.SendEvent("ui_action", "set_filter_group", group.ToString(), 0);
        }

        public void TrackForecastRangeEvent(ParkingLot parkingLot, ParkingLotForecastTimespanEnum? mode)
        {
            if (parkingLot == null)
                return;
            var properties = new Dictionary<string, string>
            {
                { "parkingLot", parkingLot.Id },
                { "mode", mode?.ToString() },
            };
            _client.TrackEvent("Change forecast mode", properties);
            _tracker.SendEvent("ui_action", "change_forecast_mode", mode?.ToString() + " > " +  parkingLot.Id, 0);
        }

        public void TrackSelectSearchResultEvent()
        {
            _tracker.SendEvent("ui_action", "select_search_result", null, 0);
        }

        public void TrackNavigateToParkingLotEvent(MetaDataCityRow city, ParkingLot parkingLot)
        {
            var properties = new Dictionary<string, string>
            {
                { "city", city.Id },
                { "parkingLot", parkingLot.Id }
            };
            _client.TrackEvent("Navigate to parking lot", properties);
            _tracker.SendEvent("ui_action", "navigate_to_parking_lot", city.Id + " > " + parkingLot.Id, 0);
        }

        public void TrackVoiceCommandEvent(string id)
        {
            var properties = new Dictionary<string, string>
            {
                { "id", id }
            };
            _client.TrackEvent("Voice command triggered", properties);
            _tracker.SendEvent("ui_action", "voice_command_triggered", id, 0);
        }

        public void TrackException(Exception e, Dictionary<string, string> properties = null)
        {
            _client.TrackException(e, properties);
            _tracker.SendException(e.Message, false);
        }
    }
}
