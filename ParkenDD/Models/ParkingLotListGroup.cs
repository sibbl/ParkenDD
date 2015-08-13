using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using GalaSoft.MvvmLight.Ioc;
using ParkenDD.Api.Models;
using ParkenDD.Services;
using ParkenDD.Utils;
using ParkenDD.ViewModels;

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
        public ParkingLotListGroup(string header, ParkingLot parkingLot)
        {
            Header = header;
            ParkingLots = new List<ParkingLot> { parkingLot };
        }
    }
}
