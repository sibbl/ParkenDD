using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Storage;
using Windows.UI.Xaml.Controls.Maps;
using GalaSoft.MvvmLight;
using ParkenDD.Models;

namespace ParkenDD.Services
{
    public class SettingsService
    {
        #region Generic methods

        private readonly ApplicationDataContainer _settings;

        public SettingsService()
        {
            _settings = ApplicationData.Current.RoamingSettings;
        }

        /// <summary>
        ///     Update a setting value for our application. If the setting does not
        ///     exist, then add the setting.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddOrUpdateValue(string key, Object value)
        {
            if (ViewModelBase.IsInDesignModeStatic) return false;
            bool valueChanged = false;

            // If the key exists
            if (_settings.Values.ContainsKey(key))
            {
                // If the value has changed
                if (_settings.Values[key] != value)
                {
                    // Store the new value
                    _settings.Values[key] = value;
                    valueChanged = true;
                }
            }
            // Otherwise create the key.
            else
            {
                _settings.Values.Add(key, value);
                valueChanged = true;
            }
            return valueChanged;
        }

        /// <summary>
        ///     Get the current value of the setting, or if it is not found, set the setting to the default setting.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValueOrDefault<T>(string key, T defaultValue)
        {
            if (ViewModelBase.IsInDesignModeStatic) return defaultValue;
            T value;

            // If the key exists, retrieve the value.
            if (_settings.Values.ContainsKey(key))
            {
                value = (T)_settings.Values[key];
            }
            // Otherwise, use the default value.
            else
            {
                value = defaultValue;
            }
            return value;
        }

        #endregion

        #region Getter and setter of settings

        public string SelectedCityId
        {
            get
            {
                if (ViewModelBase.IsInDesignModeStatic) return string.Empty;
                return GetValueOrDefault(nameof(SelectedCityId), string.Empty);
            }
            set { AddOrUpdateValue(nameof(SelectedCityId), value); }
        }
        public string SelectedParkingLotId
        {
            get
            {
                if (ViewModelBase.IsInDesignModeStatic) return string.Empty;
                return GetValueOrDefault(nameof(SelectedParkingLotId), string.Empty);
            }
            set { AddOrUpdateValue(nameof(SelectedParkingLotId), value); }
        }

        public MapCamera MapCamera
        {
            get
            {
                if (ViewModelBase.IsInDesignModeStatic) return null;

                var fieldOfView = GetValueOrDefault(nameof(MapCamera.FieldOfView), 0d);
                var heading = GetValueOrDefault(nameof(MapCamera.Heading), 0d);
                var pitch = GetValueOrDefault(nameof(MapCamera.Pitch), 0d);
                var roll = GetValueOrDefault(nameof(MapCamera.Roll), 0d);
                var lat = GetValueOrDefault(nameof(MapCamera.Location.Position.Latitude), double.NaN);
                var lng = GetValueOrDefault(nameof(MapCamera.Location.Position.Longitude), double.NaN);

                if (!Double.IsNaN(lat) && !Double.IsNaN(lng))
                {
                    return new MapCamera(new Geopoint(new BasicGeoposition
                    {
                        Latitude = lat,
                        Longitude = lng,
                    }), heading, pitch, roll, fieldOfView);
                }
                return null;
            }
            set
            {
                AddOrUpdateValue(nameof(MapCamera.FieldOfView), value.FieldOfView);
                AddOrUpdateValue(nameof(MapCamera.Heading), value.Heading);
                AddOrUpdateValue(nameof(MapCamera.Pitch), value.Pitch);
                AddOrUpdateValue(nameof(MapCamera.Roll), value.Roll);
                AddOrUpdateValue(nameof(MapCamera.Location.Position.Latitude), value.Location.Position.Latitude);
                AddOrUpdateValue(nameof(MapCamera.Location.Position.Longitude), value.Location.Position.Longitude);
            }
        }

        public ParkingLotFilterMode ParkingLotFilterMode
        {
            get
            {
                if (ViewModelBase.IsInDesignModeStatic) return ParkingLotFilterMode.Alphabetically;
                var enumStr = GetValueOrDefault<string>(nameof(ParkingLotFilterMode), null);
                ParkingLotFilterMode result;
                if (!string.IsNullOrEmpty(enumStr) && Enum.TryParse(enumStr, true, out result)) return result;
                return ParkingLotFilterMode.Alphabetically;
            }
            set { AddOrUpdateValue(nameof(ParkingLotFilterMode), value.ToString()); }
        }

        public bool ParkingLotFilterIsGrouped
        {
            get
            {
                if (ViewModelBase.IsInDesignModeStatic) return true;
                return GetValueOrDefault(nameof(ParkingLotFilterIsGrouped), true);
            }
            set { AddOrUpdateValue(nameof(ParkingLotFilterIsGrouped), value); }
        }

        public bool ParkingLotFilterAscending
        {
            get
            {
                if (ViewModelBase.IsInDesignModeStatic) return true;
                return GetValueOrDefault(nameof(ParkingLotFilterAscending), true);
            }
            set { AddOrUpdateValue(nameof(ParkingLotFilterAscending), value); }
        }

        #endregion
    }
}
