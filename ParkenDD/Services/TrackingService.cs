using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using ParkenDD.Api.Models;
using ParkenDD.Models;

namespace ParkenDD.Services
{
    public class TrackingService
    {
        private readonly TelemetryClient _client;

        public TrackingService()
        {
            WindowsAppInitializer.InitializeAsync();
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
        }

        public void TrackSelectCityEvent(MetaDataCityRow city)
        {
            var properties = new Dictionary<string, string>
            {
                { "city", city.Id },
            };
            _client.TrackEvent("Select city", properties);
        }

        public void TrackReloadCityEvent(MetaDataCityRow city)
        {
            var properties = new Dictionary<string, string>
            {
                { "city", city.Id },
            };
            _client.TrackEvent("Reload city", properties);
        }

        private ParkingLotFilterMode _oldParkingLotFilterEvent;
        private bool _oldParkingLotFilterAsc;
        private bool _oldParkingLotFilterGrouping;
        private bool _parkingLotFilterFirstCall = true;

        public void TrackParkingLotFilterEvent(ParkingLotFilterMode mode, bool ascending, bool group)
        {
            if (_parkingLotFilterFirstCall)
            {
                _oldParkingLotFilterEvent = mode;
                _oldParkingLotFilterAsc = ascending;
                _oldParkingLotFilterGrouping = group;
                _parkingLotFilterFirstCall = false;
                return;
            }
            //only track if at least one of the properties was changed
            if (_oldParkingLotFilterEvent != mode ||
                _oldParkingLotFilterAsc != ascending ||
                _oldParkingLotFilterGrouping != group)
            {
                var properties = new Dictionary<string, string>
                {
                    {"mode", mode.ToString()},
                    {"ascending", ascending.ToString()},
                    {"group", ascending.ToString()},
                };
                _client.TrackEvent("Change parking lot filter", properties);
            }
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
        }

        public void TrackSelectSearchResultEvent()
        {
            _client.TrackEvent("Select search result");
        }

        public void TrackNavigateToParkingLotEvent(MetaDataCityRow city, ParkingLot parkingLot)
        {
            var properties = new Dictionary<string, string>
            {
                { "city", city.Id },
                { "parkingLot", parkingLot.Id }
            };
            _client.TrackEvent("Navigate to parking lot", properties);
        }

        public void TrackVoiceCommandEvent(string id)
        {
            var properties = new Dictionary<string, string>
            {
                { "id", id }
            };
            _client.TrackEvent("Voice command triggered", properties);
        }

        public void TrackException(Exception e, Dictionary<string, string> properties = null)
        {
            _client.TrackException(e, properties);
        }
    }
}
