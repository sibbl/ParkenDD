using System.Collections.Generic;
using ParkenDD.Api.Models;

namespace ParkenDD.Models
{
    public class ParkingLotListGroup
    {
        public string Header { get; set; }
        public List<ParkingLot> ParkingLots { get; set; }
        public ParkingLotListGroup()
        {
            ParkingLots = new List<ParkingLot>();
        }
        public ParkingLotListGroup(string header)
        {
            Header = header;
            ParkingLots = new List<ParkingLot>();
        }
        public ParkingLotListGroup(string header, ParkingLot parkingLot)
        {
            Header = header;
            ParkingLots = new List<ParkingLot> { parkingLot };
        }
    }
}
