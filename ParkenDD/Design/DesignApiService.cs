using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ParkenDD.Api.Models;
using ParkenDD.Api.Interfaces;

namespace ParkenDD.Design
{
    public class DesignParkenDdClient : IParkenDdClient
    {
        public async Task<MetaData> GetMetaDataAsync(CancellationToken? ct = null)
        {
            return new MetaData
            {
                ApiVersion = "0.0.1",
                Reference = new Uri("https://github.com/offenesdresden/ParkAPI"),
                ServerVersion = "0.0.1",
                Cities = new MetaDataCities
                {
                    {
                        "dresden-id", 
                        new MetaDataCityRow
                        {
                            Id = "dresden-id",
                            Name = "Dresden"
                        }
                    },
                    {
                        "ingolstadt_id",
                        new MetaDataCityRow
                        {
                            Id = "ingolstadt_id",
                            Name = "Ingolstadt"
                        }
                    },
                    {
                        "bla",
                        new MetaDataCityRow
                        {
                            Id = "bla",
                            Name = "Eine Test-Stadt mit äüö und ganz viel Text"
                        }
                    }
                }
            };
        }

        public async Task<City> GetCityAsync(string cityId, CancellationToken? ct = null)
        {
            return new City
            {
                DataSource = new Uri("https://www.dresden.de/parken"),
                LastDownloaded = DateTime.Now.AddMinutes(-29),
                LastUpdated = DateTime.Now.AddMinutes(-37),
                Lots = new List<ParkingLot>
                {
                    new ParkingLot
                    {
                        Id = "dresdenaltmarkt",
                        Coordinates = new Coordinate(51.05031, 13.73754),
                        Forecast = null,
                        HasForecast = true,
                        HasLongForecast = false,
                        FreeLots = 87,
                        TotalLots = 400,
                        LotType = "Tiefgarage",
                        Name = "Altmarkt",
                        Region = "Innere Altstadt",
                        Address = "Wilsdruffer Straße",
                        State = ParkingLotState.Open
                    },
                    new ParkingLot
                    {
                        Id = "dresdenanderfrauenkirche",
                        Coordinates = new Coordinate(51.05165, 13.7439),
                        Forecast = null,
                        HasForecast = true,
                        HasLongForecast = false,
                        FreeLots = 39,
                        TotalLots = 120,
                        LotType = "Tiefgarage",
                        Name = "An der Frauenkirche",
                        Region = "Innere Altstadt",
                        Address = "An der Frauenkirche 12a",
                        State = ParkingLotState.Open
                    },
                    new ParkingLot
                    {
                        Id = "test",
                        Coordinates = new Coordinate(51.04785, 13.7459),
                        Forecast = null,
                        HasForecast = false,
                        HasLongForecast = false,
                        FreeLots = 120,
                        TotalLots = 120,
                        LotType = "Irgendeine andere Art",
                        Name = "Irgendein Name der viel zu lang ist",
                        Region = "Eine Region die auch viel zu lang ist Altstadt",
                        Address = "Ein ewig langer Straßennahme 12a",
                        State = ParkingLotState.Closed
                    },
                }
            };
        }

        public async Task<Forecast> GetForecastAsync(string cityId, string parkingLotId, DateTime @from, DateTime to, CancellationToken? ct = null)
        {
            return new Forecast();
        }
    }
}
