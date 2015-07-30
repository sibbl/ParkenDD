using System;
using System.Collections.Generic;
using System.Linq;

namespace ParkenDD.Win10.Models
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

        public static IEnumerable<ParkingLotListGroup> CreateGroups(IEnumerable<ParkingLot> items, bool orderAscending = true)
        {
            var result = new List<ParkingLotListGroup>();
            var orderedItems = orderAscending ? items.OrderBy(x => x.Region) : items.OrderByDescending(x => x.Region);
            foreach (var i in orderedItems)
            {
                var header = i.Region ?? "Weitere"; //TODO: localize
                var existingGroup = result.FirstOrDefault(x => x.Header.Equals(header));
                if (existingGroup == null)
                {
                    var newGroup = new ParkingLotListGroup(header, i);
                    result.Add(newGroup);
                }
                else
                {
                    existingGroup.ParkingLots.Add(i);
                }
            }
            if (result.Count == 1)
            {
                result[0].Header = "Alle Parkplätze"; //TODO: localize
            }
            return result;
        }
    }
}
