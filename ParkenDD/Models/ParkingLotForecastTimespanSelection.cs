using System;
using ParkenDD.Services;

namespace ParkenDD.Models
{
    public class ParkingLotForecastTimespanSelection
    {
        public ParkingLotForecastTimespanEnum Mode { get; private set; }
        public TimeSpan TimeSpan { get; private set; }
        public string Title { get; private set; }

        public ParkingLotForecastTimespanSelection(ParkingLotForecastTimespanEnum mode)
        {
            Mode = mode;
            switch (mode)
            {
                case ParkingLotForecastTimespanEnum.Days7:
                    Title = ResourceService.Instance.ParkingLotForecastTimespan7Days;
                    TimeSpan = TimeSpan.FromDays(7);
                    break;
                case ParkingLotForecastTimespanEnum.Hours24:
                    Title = ResourceService.Instance.ParkingLotForecastTimespan24Hrs;
                    TimeSpan = TimeSpan.FromHours(24);
                    break;
                case ParkingLotForecastTimespanEnum.Hours6:
                    Title = ResourceService.Instance.ParkingLotForecastTimespan6Hrs;
                    TimeSpan = TimeSpan.FromHours(6);
                    break;
            }
        }
    }
}
