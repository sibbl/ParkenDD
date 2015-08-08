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
        public async Task<IEnumerable<ParkingLotListGroup>> CreateGroups(IEnumerable<ParkingLot> items)
        {
            var mainVm = SimpleIoc.Default.GetInstance<MainViewModel>();
            var filter = mainVm.ParkingLotFilterMode;
            var orderAsc = mainVm.ParkingLotFilterAscending;
            var result = new List<ParkingLotListGroup>();
            var orderedItems = orderAsc ? items.OrderBy(x => x.Region) : items.OrderByDescending(x => x.Region);
            var alphabeticalSortingFunc = new Func<ParkingLot, string>(x => x.Name);
            switch (filter)
            {
                case ParkingLotFilterMode.Alphabetically:
                    orderedItems = orderAsc ? orderedItems.ThenBy(alphabeticalSortingFunc) : orderedItems.ThenByDescending(alphabeticalSortingFunc);
                    break;
                case ParkingLotFilterMode.Availability:
                    var availabilitySortingFunc = new Func<ParkingLot, double>(x => ((double)x.TotalLots / (double)x.FreeLots));
                    orderedItems = orderAsc ? orderedItems.ThenBy(availabilitySortingFunc).ThenBy(alphabeticalSortingFunc) : orderedItems.ThenByDescending(availabilitySortingFunc).ThenBy(alphabeticalSortingFunc);
                    break;
                case ParkingLotFilterMode.Distance:
                    //TODO: get distance here
                    var locationService = SimpleIoc.Default.GetInstance<GeolocationService>();

                    var userPos = await locationService.GetUserLocation();
                    if (userPos == null)
                    {
                        mainVm.ParkingLotFilterMode = ParkingLotFilterMode.Alphabetically;
                        return await CreateGroups(items);
                    }

                    var distanceSortingFunc = new Func<ParkingLot, double>(x => x?.Coordinates?.Point == null ? double.MaxValue : userPos.Coordinate.Point.GetDistanceTo(x.Coordinates.Point));
                    orderedItems = orderAsc ? orderedItems.ThenBy(distanceSortingFunc).ThenBy(alphabeticalSortingFunc) : orderedItems.ThenByDescending(distanceSortingFunc).ThenBy(alphabeticalSortingFunc);
                    break;
                default:
                    orderedItems = orderAsc ? orderedItems.ThenBy(x => x.Name) : orderedItems.ThenByDescending(x => x.Name);
                    break;
            }
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
