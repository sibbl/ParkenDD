using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ParkenDD.Messages;
using ParkenDD.Services;
using ParkenDD.Api.Models;
using ParkenDD.Api.Interfaces;

namespace ParkenDD.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IParkenDdClient _client;
        private readonly VoiceCommandService _voiceCommands;
        private readonly Dictionary<string, City> _cities = new Dictionary<string, City>();

        private MetaData _metaData;
        public MetaData MetaData
        {
            get { return _metaData; }
            set { Set(() => MetaData, ref _metaData, value); }
        }

        private bool _loadingMetaData;
        public bool LoadingMetaData
        {
            get { return _loadingMetaData; }
            set { Set(() => LoadingMetaData, ref _loadingMetaData, value); }
        }

        private MetaDataCityRow _selectedCity;
        public MetaDataCityRow SelectedCity
        {
            get { return _selectedCity; }
            set
            {
                Set(() => SelectedCity, ref _selectedCity, value);
                LoadCityAndSelectCity();
            }
        }

        private City _selectedCityData;
        public City SelectedCityData
        {
            get { return _selectedCityData; }
            set
            {
                Set(() => SelectedCityData, ref _selectedCityData, value); 
                ParkingLots = value == null ? null :  new ObservableCollection<ParkingLot>(value.Lots);
            }
        }

        private ParkingLot _selectedParkingLot;
        public ParkingLot SelectedParkingLot
        {
            get { return _selectedParkingLot; }
            set
            {
                Set(() => SelectedParkingLot, ref _selectedParkingLot, value);
            }
        }

        private bool _loadingCity;
        public bool LoadingCity
        {
            get { return _loadingCity; }
            set { Set(() => LoadingCity, ref _loadingCity, value); }
        }

        private ObservableCollection<ParkingLot> _parkingLots;
        public ObservableCollection<ParkingLot> ParkingLots
        {
            get { return _parkingLots; }
            set { Set(() => ParkingLots, ref _parkingLots, value); }
        }


        public MainViewModel(IParkenDdClient client, VoiceCommandService voiceCommandService)
        {
            _client = client;
            _voiceCommands = voiceCommandService;
            LoadMetaData();
        }

        private async void LoadMetaData()
        {
            LoadingMetaData = true;
            MetaData = await _client.GetMeta();
            LoadingMetaData = false;
            _voiceCommands.UpdateCityList(MetaData);
        }

        private MetaDataCityRow FindCityByName(MetaData metaData, string name)
        {
            return metaData?.Cities?.Select(x => x.Value).FirstOrDefault(x => x.Name.ToLower().Equals(name.ToLower()));
        }

        private ParkingLot FindParkingLotByName(City city, string name)
        {
            return city?.Lots?.FirstOrDefault(x => x.Name.ToLower().Equals(name.ToLower()));
        }

        public async void TrySelectCityByName(string name)
        {
            if (MetaData == null || FindCityByName(MetaData, name) == null)
            {
                MetaData = await _client.GetMeta();
            }
            var city = FindCityByName(MetaData, name);
            if (city != null)
            {
                SelectedCity = city;
            }
        }

        public async void TrySelectParkingLotByName(string cityName, string parkingLotName)
        {
            if (MetaData == null || FindCityByName(MetaData, cityName) == null)
            {
                MetaData = await _client.GetMeta();
            }
            var city = FindCityByName(MetaData, cityName);
            if (city != null)
            {
                LoadingCity = true;
                var cityDetails = await LoadCity(cityName);
                LoadingCity = false;
                if (FindParkingLotByName(cityDetails, parkingLotName) == null)
                {
                    //force refresh
                    LoadingCity = true;
                    cityDetails = await LoadCity(cityName, true);
                    LoadingCity = false;
                }
                SelectedCity = city;
                SelectedCityData = cityDetails;
                var parkingLot = FindParkingLotByName(cityDetails, parkingLotName);
                if (parkingLot != null)
                {
                    SelectedParkingLot = parkingLot;
                }
            }
        }

        private async void LoadCityAndSelectCity()
        {
            SelectedCityData = null;
            SelectedParkingLot = null;
            LoadingCity = true;
            SelectedCityData = await LoadCity(SelectedCity.Id);
            LoadingCity = false;
            _voiceCommands.UpdateParkingLotList(SelectedCityData);
            UpdateMapBounds();
        }

        private void UpdateMapBounds()
        {
            var maxLat = Double.NaN;
            var minLat = Double.NaN;
            var maxLng = Double.NaN;
            var minLng = Double.NaN;
            foreach (var i in SelectedCityData.Lots.Where(i => i?.Coordinates != null))
            {
                if (Double.IsNaN(maxLat) || maxLat < i.Coordinates.Latitude)
                {
                    maxLat = i.Coordinates.Latitude;
                }
                if (Double.IsNaN(minLat) || minLat > i.Coordinates.Latitude)
                {
                    minLat = i.Coordinates.Latitude;
                }
                if (Double.IsNaN(maxLng) || maxLng < i.Coordinates.Longitude)
                {
                    maxLng = i.Coordinates.Longitude;
                }
                if (Double.IsNaN(minLng) || minLng > i.Coordinates.Longitude)
                {
                    minLng = i.Coordinates.Longitude;
                }
            }
            if (Double.IsNaN(maxLat) || Double.IsNaN(maxLat) || Double.IsNaN(maxLat) || Double.IsNaN(maxLat))
            {
                return;
            }
            Messenger.Default.Send(
                new ZoomMapToBoundsMessage(
                    new GeoboundingBox(
                        new BasicGeoposition
                        {
                            Latitude = maxLat,
                            Longitude = minLng
                        }, 
                        new BasicGeoposition
                        {
                            Latitude = minLat,
                            Longitude = maxLng
                        }
                    )
                )
            );
        }

        private async Task<City> LoadCity(string cityId, bool forceRefresh = false)
        {
            if (_cities.ContainsKey(cityId) && !forceRefresh)
            {
                return _cities[cityId];
            }
            _cities[cityId] = await _client.GetCity(cityId);
            return _cities[cityId];
        }


        private RelayCommand _reloadSelecedCityCommand;
        public RelayCommand ReloadSelectedCityCommand => _reloadSelecedCityCommand ?? (_reloadSelecedCityCommand = new RelayCommand(ReloadSelectedCity));

        private async void ReloadSelectedCity()
        {
            LoadingCity = true;
            SelectedCityData = await LoadCity(SelectedCity.Id);
            LoadingCity = false;
        }
    }
}
