using System;

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
            //TODO: localize
            switch (mode)
            {
                case ParkingLotForecastTimespanEnum.Days7:
                    Title = "Vorhersage für die nächsten 7 Tage";
                    TimeSpan = TimeSpan.FromDays(7);
                    break;
                case ParkingLotForecastTimespanEnum.Hours24:
                    Title = "Vorhersage für die nächsten 24 Stunden";
                    TimeSpan = TimeSpan.FromHours(24);
                    break;
                case ParkingLotForecastTimespanEnum.Hours6:
                    Title = "Vorhersage für die nächsten 6 Stunden";
                    TimeSpan = TimeSpan.FromHours(6);
                    break;
            }
        }
    }
}
