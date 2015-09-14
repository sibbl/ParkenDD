using System;
using Windows.Storage;
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
