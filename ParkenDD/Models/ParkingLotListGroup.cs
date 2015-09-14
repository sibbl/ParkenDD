using System.Collections.Generic;

namespace ParkenDD.Models
{
    public class ParkingLotListGroup
    {
        public string Header { get; set; }
        public List<SelectableParkingLot> ParkingLots { get; set; }
        public ParkingLotListGroup()
        {
            ParkingLots = new List<SelectableParkingLot>();
        }
        public ParkingLotListGroup(string header)
        {
            Header = header;
            ParkingLots = new List<SelectableParkingLot>();
        }
        public ParkingLotListGroup(string header, SelectableParkingLot parkingLot)
        {
            Header = header;
            ParkingLots = new List<SelectableParkingLot> { parkingLot };
        }
    }
}
