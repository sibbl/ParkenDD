using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using ParkenDD.Messages;
using ParkenDD.Models;
using ParkenDD.Services;

namespace ParkenDD.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly SettingsService _settings;
        private readonly LocalizationService _localization;

        #region ShowExperimentalCities
        private bool _showExperimentalCities;

        public bool ShowExperimentalCities
        {
            get { return _showExperimentalCities; }
            set {
                if (Set(ref _showExperimentalCities, value))
                {
                    _settings.ShowExperimentalCities = value;
                    Messenger.Default.Send(new SettingChangedMessage(nameof(ShowExperimentalCities)));
                }
            }
        }
        #endregion

        #region Distance unit

        public Dictionary<DistanceUnitEnum, string> DistanceUnitValues
        {
            get
            {
                return _localization.GetSupportedDistanceUnits().ToDictionary(loc => loc, _localization.GetDistanceUnitTitle);
            }
        }

        public int DistanceUnitDefaultIndex
        {
            get
            {
                var dict = DistanceUnitValues;
                var loc = _settings.DistanceUnit;
                for (var i = 0; i < dict.Count; i++)
                    if (loc == dict.Keys.ElementAt(i)) return i;
                return 0;
            }
        }

        private DistanceUnitEnum _distanceUnit;
        public DistanceUnitEnum DistanceUnit
        {
            get { return _distanceUnit; }
            set
            {
                if (Set(ref _distanceUnit, value))
                {
                    _settings.DistanceUnit = value;
                    Messenger.Default.Send(new SettingChangedMessage(nameof(DistanceUnit)));
                }
            }
        }

        #endregion


        #region Language
        private readonly SupportedLocale _displayedLocale;

        public Dictionary<SupportedLocale, string> LocaleValues
        {
            get
            {
                return _localization.GetSupportedLocales().ToDictionary(loc => loc, _localization.GetLocalizationTitle);
            }
        }

        public int LocaleDefaultIndex
        {
            get
            {
                var dict = LocaleValues;
                var loc = _settings.CurrentLocale;
                for (var i = 0; i < dict.Count; i++)
                    if (loc == dict.Keys.ElementAt(i)) return i;
                return 0;
            }
        }

        private SupportedLocale _locale;

        public SupportedLocale Locale
        {
            get { return _locale; }
            set
            {
                if (Set(ref _locale, value))
                {
                    _localization.UpdateCulture(value);
                    RaisePropertyChanged(() => ChangeLanguageString);
                    RaisePropertyChanged(() => LanguageChanged);
                    _settings.CurrentLocale = value;
                    Messenger.Default.Send(new SettingChangedMessage(nameof(Locale)));
                }
            }
        }
        public string ChangeLanguageString => _localization.GetRestartRequiredLabelForNewLanguage(Locale);

        public bool LanguageChanged => !_displayedLocale.Equals(Locale);

        #endregion


        public SettingsViewModel(SettingsService settings, LocalizationService localization)
        {
            _settings = settings;
            _localization = localization;

            _distanceUnit = settings.DistanceUnit;
            _displayedLocale = settings.CurrentLocale;
            _locale = settings.CurrentLocale;
            _showExperimentalCities = settings.ShowExperimentalCities;
            RaisePropertyChanged(() => Locale);
            RaisePropertyChanged(() => LocaleDefaultIndex);
            RaisePropertyChanged(() => LanguageChanged);
            RaisePropertyChanged(() => ChangeLanguageString);
        }
    }
}
