using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Threading;
using ParkenDD.Api.Models;

namespace ParkenDD.Utils
{
    public static class ClassUtils
    {
        public static void Merge(this MetaData metaData, MetaData newData)
        {
            if (newData == null)
            {
                return;
            }
            Task.Run(async () =>
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    metaData.ApiVersion = newData.ApiVersion;
                    metaData.Reference = newData.Reference;
                    metaData.ServerVersion = newData.ServerVersion;
                });
            });
            Parallel.ForEach(newData.Cities, async (city) =>
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    var existingCity = metaData.Cities.FirstOrDefault(x => x.Id == city.Id);
                    if (existingCity == null)
                    {
                        metaData.Cities.Add(city);
                    }
                    else
                    {
                        existingCity.Coordinates = city.Coordinates;
                        existingCity.Name = city.Name;
                        existingCity.Source = city.Source;
                        existingCity.Url = city.Url;
                    }
                });
            });
            Parallel.ForEach(metaData.Cities.Where(existingCity => newData.Cities.FirstOrDefault(x => x.Id == existingCity.Id) == null).ToList(), async (existingLot) =>
            {
                await DispatcherHelper.RunAsync(() => {
                    metaData.Cities.Remove(existingLot);
                });
            });
        }

        public static void Merge(this City city, City newData, ObservableCollection<ParkingLot> parkingLotCollection)
        {
            if (newData == null || parkingLotCollection == null)
            {
                return;
            }
            Task.Run(async () =>
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    city.DataSource = newData.DataSource;
                    city.LastDownloaded = newData.LastDownloaded;
                    city.LastUpdated = newData.LastUpdated;
                });
            });
            Parallel.ForEach(newData.Lots, async (newLot) =>
            {
                await DispatcherHelper.RunAsync(() =>
                {
                    var existingLot = city.Lots.FirstOrDefault(x => x.Id == newLot.Id);
                    if (existingLot == null)
                    {
                        city.Lots.Add(newLot);
                        parkingLotCollection.Add(newLot);
                    }
                    else
                    {
                        existingLot.TotalLots = newLot.TotalLots;
                        existingLot.FreeLots = newLot.FreeLots;
                        existingLot.Address = newLot.Address;
                        existingLot.HasForecast = newLot.HasForecast;
                        existingLot.Coordinates = newLot.Coordinates;
                        existingLot.LotType = newLot.LotType;
                        existingLot.Forecast = newLot.Forecast;
                        existingLot.HasLongForecast = newLot.HasLongForecast;
                        existingLot.Name = newLot.Name;
                        existingLot.Region = newLot.Region;
                        existingLot.State = newLot.State;
                    }
                });
            });
            Parallel.ForEach(
                city.Lots.Where(
                    existingLot => newData.Lots.FirstOrDefault(newLot => newLot.Id == existingLot.Id) == null).ToList(),
                async (existingLot) =>
                {
                    await DispatcherHelper.RunAsync(() =>
                    {
                        city.Lots.Remove(existingLot);
                        var selectableParkingLot = parkingLotCollection.FirstOrDefault(x => x.Id == existingLot.Id);
                        if (selectableParkingLot != null)
                        {
                            parkingLotCollection.Remove(selectableParkingLot);
                        }
                    });
                });
        }
    }
}
