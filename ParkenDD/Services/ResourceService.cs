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

    }
}
