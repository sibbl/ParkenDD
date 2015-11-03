using Windows.ApplicationModel.Resources;
using Microsoft.Practices.ServiceLocation;

namespace ParkenDD.Services
{
    public class ResourceService
    {
        private readonly ResourceLoader _resLoader;

        public ResourceService()
        {
            _resLoader = ResourceLoader.GetForViewIndependentUse();
        }

        public static ResourceService Instance => ServiceLocator.Current.GetInstance<ResourceService>();

        public string ParkingLotForecastTimespan7Days => _resLoader.GetString(nameof(ParkingLotForecastTimespan7Days));
        public string ParkingLotForecastTimespan24Hrs => _resLoader.GetString(nameof(ParkingLotForecastTimespan24Hrs));
        public string ParkingLotForecastTimespan6Hrs => _resLoader.GetString(nameof(ParkingLotForecastTimespan6Hrs));
        public string ParkingLotLastRefreshHourFormat => _resLoader.GetString(nameof(ParkingLotLastRefreshHourFormat));
        public string ParkingLotLastRefreshYesterdayAt => _resLoader.GetString(nameof(ParkingLotLastRefreshYesterdayAt));
        public string ParkingLotLastRefreshDaysAgo => _resLoader.GetString(nameof(ParkingLotLastRefreshDaysAgo));
        public string ParkingLotStateClosed => _resLoader.GetString(nameof(ParkingLotStateClosed));
        public string ParkingLotStateOpen => _resLoader.GetString(nameof(ParkingLotStateOpen));
        public string MapCurrentLocationLabel => _resLoader.GetString(nameof(MapCurrentLocationLabel));
        public string ParkingLotListGroupHeaderAll => _resLoader.GetString(nameof(ParkingLotListGroupHeaderAll));
        public string ParkingLotListGroupHeaderOther => _resLoader.GetString(nameof(ParkingLotListGroupHeaderOther));
        public string DirectionsParkingLotLabel => _resLoader.GetString(nameof(DirectionsParkingLotLabel));
        public string ExceptionMailMetaDataSubject => _resLoader.GetString(nameof(ExceptionMailMetaDataSubject));
        public string ExceptionMailMetaDataBody => _resLoader.GetString(nameof(ExceptionMailMetaDataBody));
        public string ExceptionToastTitle => _resLoader.GetString(nameof(ExceptionToastTitle));
        public string ExceptionToastVisitParkenDdButton => _resLoader.GetString(nameof(ExceptionToastVisitParkenDdButton));
        public string ExceptionToastContactDevButton => _resLoader.GetString(nameof(ExceptionToastContactDevButton));
        public string ExceptionToastShowInBrowserButton => _resLoader.GetString(nameof(ExceptionToastShowInBrowserButton));
        public string ExceptionToastMetaDataContent => _resLoader.GetString(nameof(ExceptionToastMetaDataContent));
        public string ExceptionMailCitySubject => _resLoader.GetString(nameof(ExceptionMailCitySubject));
        public string ExceptionMailCityBody => _resLoader.GetString(nameof(ExceptionMailCityBody));
        public string ExceptionToastCityContent => _resLoader.GetString(nameof(ExceptionToastCityContent));
        public string ExceptionMailForecastSubject => _resLoader.GetString(nameof(ExceptionMailForecastSubject));
        public string ExceptionMailForecastBody => _resLoader.GetString(nameof(ExceptionMailForecastBody));
        public string ExceptionToastForecastContent => _resLoader.GetString(nameof(ExceptionToastForecastContent));
        public string DistanceUnitKilometers => _resLoader.GetString(nameof(DistanceUnitKilometers));
        public string DistanceUnitMiles => _resLoader.GetString(nameof(DistanceUnitMiles));
        public string SettingsLanguageRestartRequiredMessage => _resLoader.GetString(nameof(SettingsLanguageRestartRequiredMessage));
        public string LanguageTitle => _resLoader.GetString(nameof(LanguageTitle));
        public string JumpListCitiesHeader => _resLoader.GetString(nameof(JumpListCitiesHeader));
        public string ReviewAppDialog1Title => _resLoader.GetString(nameof(ReviewAppDialog1Title));
        public string ReviewAppDialog1YesButton => _resLoader.GetString(nameof(ReviewAppDialog1YesButton));
        public string ReviewAppDialog1NoButton => _resLoader.GetString(nameof(ReviewAppDialog1NoButton));
        public string ReviewAppDialog1Content => _resLoader.GetString(nameof(ReviewAppDialog1Content));
        public string ReviewAppDialog2Title => _resLoader.GetString(nameof(ReviewAppDialog2Title));
        public string ReviewAppDialog2FeedbackButton => _resLoader.GetString(nameof(ReviewAppDialog2FeedbackButton));
        public string ReviewAppDialog2NoButton => _resLoader.GetString(nameof(ReviewAppDialog2NoButton));
        public string ReviewAppDialog2Content => _resLoader.GetString(nameof(ReviewAppDialog2Content));
        public string ReviewAppFeedbackMailTitle => _resLoader.GetString(nameof(ReviewAppFeedbackMailTitle));
        public string ReviewAppFeedbackMailBody => _resLoader.GetString(nameof(ReviewAppFeedbackMailBody));
    }
}
