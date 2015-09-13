using System;

namespace ParkenDD.Models
{
    public class ParkingLotForecastDataPoint
    {
        public DateTime Time { get; set; }
        public byte Value { get; set; }

        public ParkingLotForecastDataPoint(byte value, DateTime time)
        {
            Time = time;
            Value = value;
        }
    }
}
