using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Ioc;
using ParkenDD.Api.Models;
using ParkenDD.Models;
using ParkenDD.Utils;
using ParkenDD.ViewModels;

namespace ParkenDD.Services
{
    public class ParkingLotListFilterService
    {
       // var orderedItems = orderAsc ? items.OrderBy(x => x.Region) : items.OrderByDescending(x => x.Region);
        public async Task<IEnumerable<ParkingLot>> CreateList(IEnumerable<ParkingLot> items)
        {
            var mainVm = SimpleIoc.Default.GetInstance<MainViewModel>();
            var filter = mainVm.ParkingLotFilterMode;
            var orderAsc = mainVm.ParkingLotFilterAscending;
            var alphabeticalSortingFunc = new Func<ParkingLot, string>(x => x.Name);
            switch (filter)
            {
                case ParkingLotFilterMode.Alphabetically:
                    return orderAsc ? items.OrderBy(alphabeticalSortingFunc) : items.OrderByDescending(alphabeticalSortingFunc);
                case ParkingLotFilterMode.Availability:
                    var availabilitySortingFunc = new Func<ParkingLot, double>(x => ((double)x.TotalLots / (double)x.FreeLots));
                    return orderAsc ? items.OrderBy(availabilitySortingFunc) : items.OrderByDescending(availabilitySortingFunc);
                case ParkingLotFilterMode.Distance:
                    //TODO: get distance here
                    var locationService = SimpleIoc.Default.GetInstance<GeolocationService>();

                    var userPos = await locationService.GetUserLocation();
                    if (userPos == null)
                    {
                        mainVm.ParkingLotFilterMode = ParkingLotFilterMode.Alphabetically;
                        return await CreateList(items);
                    }

                    var distanceSortingFunc = new Func<ParkingLot, double>(x => x?.Coordinates?.Point == null ? double.MaxValue : userPos.Coordinate.Point.GetDistanceTo(x.Coordinates.Point));
                    return orderAsc ? items.OrderBy(distanceSortingFunc).ThenBy(alphabeticalSortingFunc) : items.OrderByDescending(distanceSortingFunc).ThenBy(alphabeticalSortingFunc);
                default:
                    return orderAsc ? items.OrderBy(alphabeticalSortingFunc) : items.OrderByDescending(alphabeticalSortingFunc);
            }
        }
        public async Task<IEnumerable<ParkingLotListGroup>> CreateGroups(IEnumerable<ParkingLot> items)
        {
            var mainVm = SimpleIoc.Default.GetInstance<MainViewModel>();
            var orderAsc = mainVm.ParkingLotFilterAscending;
            var orderedItems = await CreateList(orderAsc ? items.OrderBy(x => x.Region) : items.OrderByDescending(x => x.Region));
            var result = new List<ParkingLotListGroup>();
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
