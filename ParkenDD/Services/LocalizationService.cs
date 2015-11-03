using System.Collections.Generic;
using System.Globalization;
using Windows.Globalization;
using Windows.System.UserProfile;
using ParkenDD.Models;

namespace ParkenDD.Services
{
    public enum SupportedLocale
    {
        German,
        English
    }
    public class LocalizationService
    {
        private readonly SettingsService _settings;
        private readonly ResourceService _resources;
        public LocalizationService(SettingsService settings, ResourceService resources)
        {
            _settings = settings;
            _resources = resources;
            UpdateCulture();
        }
        public static SupportedLocale GetDeviceLocalization()
        {
            var primaryLanguage = ApplicationLanguages.Languages[0];
            if (primaryLanguage.StartsWith("de"))
                return SupportedLocale.German;
            return SupportedLocale.English;
        }

        public List<SupportedLocale> GetSupportedLocales()
        {
            return new List<SupportedLocale>
            {
                SupportedLocale.German,
                SupportedLocale.English
            };
        }

        public List<DistanceUnitEnum> GetSupportedDistanceUnits()
        {
            return new List<DistanceUnitEnum>
            {
                DistanceUnitEnum.Kilometers,
                DistanceUnitEnum.Miles
            };
        }

        public string GetLocalizationTitle(SupportedLocale locale)
        {
            switch (locale)
            {
                case SupportedLocale.German:
                    return "Deutsch";
                case SupportedLocale.English:
                    return "English";
            }
            return string.Empty;
        }

        public string GetDistanceUnitTitle(DistanceUnitEnum distanceUnit)
        {
            switch (distanceUnit)
            {
                case DistanceUnitEnum.Kilometers:
                    return _resources.DistanceUnitKilometers;
                case DistanceUnitEnum.Miles:
                    return _resources.DistanceUnitMiles;
            }
            return string.Empty;
        }

        public string GetRestartRequiredLabelForNewLanguage(SupportedLocale locale)
        {
            switch (locale)
            {
                case SupportedLocale.German:
                    return "Du musst die App neu starten, damit die Änderung der Sprache sichtbar wird.";
                default:
                    return "You have to restart the app in order to see the language changes.";
            }
        }

        public void UpdateCulture()
        {
            UpdateCulture(_settings.CurrentLocale);
        }

        public void UpdateCulture(SupportedLocale locale)
        {
            var loc = GetCultureString(locale);
            ApplicationLanguages.PrimaryLanguageOverride = loc;
        }

        public CultureInfo GetCulture(SupportedLocale locale)
        {
            return new CultureInfo(GetCultureString(locale));
        }

        public string GetCultureString(SupportedLocale locale)
        {
            switch (locale)
            {
                case SupportedLocale.German:
                    return "de-DE";
                case SupportedLocale.English:
                    return "en-US";
            }
            return GlobalizationPreferences.Languages[0];
        }
    }
}
