using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using ParkenDD.Models;
using ParkenDD.Utils;
using ParkenDD.ViewModels;

namespace ParkenDD.Services
{
    public class ParkingLotListFilterService
    {
        public async Task<IEnumerable<SelectableParkingLot>> CreateList(IEnumerable<SelectableParkingLot> items)
        {
            var mainVm = SimpleIoc.Default.GetInstance<MainViewModel>();
            var filter = mainVm.ParkingLotFilterMode;
            var orderAsc = mainVm.ParkingLotFilterAscending;
            var alphabeticalSortingFunc = new Func<SelectableParkingLot, string>(x => x.ParkingLot.Name);
            switch (filter)
            {
                case ParkingLotFilterMode.Alphabetically:
                    return orderAsc ? items.OrderBy(alphabeticalSortingFunc) : items.OrderByDescending(alphabeticalSortingFunc);
                case ParkingLotFilterMode.Availability:
                    var availabilitySortingFunc = new Func<SelectableParkingLot, double>(x => ((double)x.ParkingLot.TotalLots / (double)x.ParkingLot.FreeLots));
                    return orderAsc ? items.OrderBy(availabilitySortingFunc) : items.OrderByDescending(availabilitySortingFunc);
                case ParkingLotFilterMode.Distance:
                    var userPos = await SimpleIoc.Default.GetInstance<GeolocationService>().GetUserLocation();
                    if (userPos == null)
                    {
                        mainVm.ParkingLotFilterMode = ParkingLotFilterMode.Alphabetically;
                        return await CreateList(items);
                    }

                    var distanceSortingFunc = new Func<SelectableParkingLot, double>(x => x?.ParkingLot?.Coordinates?.Point == null ? double.MaxValue : userPos.Coordinate.Point.GetDistanceTo(x.ParkingLot.Coordinates.Point));
                    return orderAsc ? items.OrderBy(distanceSortingFunc).ThenBy(alphabeticalSortingFunc) : items.OrderByDescending(distanceSortingFunc).ThenBy(alphabeticalSortingFunc);
                default:
                    return orderAsc ? items.OrderBy(alphabeticalSortingFunc) : items.OrderByDescending(alphabeticalSortingFunc);
            }
        }
        public async Task<IEnumerable<ParkingLotListGroup>> CreateGroups(IEnumerable<SelectableParkingLot> items)
        {
            var mainVm = SimpleIoc.Default.GetInstance<MainViewModel>();
            var orderAsc = mainVm.ParkingLotFilterAscending;
            var orderedItems = await CreateList(orderAsc ? items.OrderBy(x => x.ParkingLot.Region) : items.OrderByDescending(x => x.ParkingLot.Region));
            var result = new List<ParkingLotListGroup>();
            foreach (var i in orderedItems)
            {
                var header = i.ParkingLot.Region ?? "Weitere"; //TODO: localize
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
