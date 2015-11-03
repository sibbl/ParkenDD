using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using ParkenDD.Api.Models;
using ParkenDD.Models;
using ParkenDD.Utils;
using ParkenDD.ViewModels;

namespace ParkenDD.Services
{
    public class ParkingLotListFilterService
    {
        public async Task<IEnumerable<ParkingLot>> CreateList(IEnumerable<ParkingLot> items)
        {
            var mainVm = ServiceLocator.Current.GetInstance<MainViewModel>();
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
                    var userPos = await ServiceLocator.Current.GetInstance<GeolocationService>().GetUserLocation();
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
            var orderedItems = await CreateList(items);
            var result = new List<ParkingLotListGroup>();
            var res = ResourceService.Instance;
            //create groups in the order which the server returned
            foreach (var i in items)
            {
                var header = i.Region ?? res.ParkingLotListGroupHeaderOther;
                if (!result.Any(x => x.Header.Equals(header)))
                {
                    result.Add(new ParkingLotListGroup(header));
                }
            }
            //then add ordered items
            foreach (var i in orderedItems)
            {
                var header = i.Region ?? res.ParkingLotListGroupHeaderOther;
                result.FirstOrDefault(x => x.Header.Equals(header)).ParkingLots.Add(i);
            }
            if (result.Count == 1)
            {
                result[0].Header = res.ParkingLotListGroupHeaderAll;
            }
            return result;
        }
    }
}
