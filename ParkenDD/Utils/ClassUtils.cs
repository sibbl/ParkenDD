using System.Collections.ObjectModel;
using System.Linq;
using ParkenDD.Api.Models;
using ParkenDD.Models;

namespace ParkenDD.Utils
{
    public static class ClassUtils
    {
        public static void Merge(this MetaData metaData, MetaData newData)
        {
            metaData.ApiVersion = newData.ApiVersion;
            metaData.Reference = newData.Reference;
            metaData.ServerVersion = newData.ServerVersion;
            foreach (var city in newData.Cities)
            {
                var existingCity = metaData.Cities.Where(x => x.Key == city.Key).Select(x => x.Value).FirstOrDefault();
                if (existingCity == null)
                {
                    metaData.Cities.Add(city.Key, city.Value);
                }
                else
                {
                    existingCity.Coordinates = city.Value.Coordinates;
                    existingCity.Name = city.Value.Name;
                    existingCity.Source = city.Value.Source;
                    existingCity.Url = city.Value.Url;
                }
            }
            foreach (var existingLot in metaData.Cities.ToList().Where(existingCity => newData.Cities.Where(x => x.Key == existingCity.Key).Select(x => x.Value).FirstOrDefault() == null))
            {
                metaData.Cities.Remove(existingLot.Key);
            }
        }

        public static void Merge(this City city, City newData, ObservableCollection<SelectableParkingLot> parkingLotCollection)
        {
            city.DataSource = newData.DataSource;
            city.LastDownloaded = newData.LastDownloaded;
            city.LastUpdated = newData.LastUpdated;
            foreach (var newLot in newData.Lots)
            {
                var existingLot = city.Lots.FirstOrDefault(x => x.Id == newLot.Id);
                if (existingLot == null)
                {
                    city.Lots.Add(newLot);
                    parkingLotCollection.Add(new SelectableParkingLot(newLot));
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

                    var selectableParkingLot = parkingLotCollection.FirstOrDefault(x => x.ParkingLot.Id == existingLot.Id);
                    selectableParkingLot?.RaiseParkingLotPropertyChanged();
                }
            }
            foreach (var existingLot in city.Lots.ToList().Where(existingLot => newData.Lots.FirstOrDefault(newLot => newLot.Id == existingLot.Id) == null))
            {
                city.Lots.Remove(existingLot);
                var selectableParkingLot = parkingLotCollection.FirstOrDefault(x => x.ParkingLot.Id == existingLot.Id);
                if (selectableParkingLot != null)
                {
                    parkingLotCollection.Remove(selectableParkingLot);
                }
            }
        }
    }
}
