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
using ParkenDD.Models;

namespace ParkenDD.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        #region PRIVATE PROPERTIES
        private readonly IParkenDdClient _client;
        private readonly VoiceCommandService _voiceCommands;
        private readonly ParkingLotListFilterService _filterService;
        private readonly Dictionary<string, City> _cities = new Dictionary<string, City>();
        #endregion

        #region PUBLIC PROPERTIES

        #region MetaData
        private MetaData _metaData;
        public MetaData MetaData
        {
            get { return _metaData; }
            set { Set(() => MetaData, ref _metaData, value); }
        }
        #endregion

        #region LoadingMetaData
        private bool _loadingMetaData;
        public bool LoadingMetaData
        {
            get { return _loadingMetaData; }
            set { Set(() => LoadingMetaData, ref _loadingMetaData, value); }
        }
        #endregion

        #region SelectedCity
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
        #endregion

        #region SelectedCityData
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
        #endregion

        #region SelectedParkingLot
        private ParkingLot _selectedParkingLot;
        public ParkingLot SelectedParkingLot
        {
            get { return _selectedParkingLot; }
            set
            {
                Set(() => SelectedParkingLot, ref _selectedParkingLot, value);
            }
        }
        #endregion

        #region LoadingCity
        private bool _loadingCity;
        public bool LoadingCity
        {
            get { return _loadingCity; }
            set { Set(() => LoadingCity, ref _loadingCity, value); }
        }
        #endregion

        #region ParkingLots
        private ObservableCollection<ParkingLot> _parkingLots;
        public ObservableCollection<ParkingLot> ParkingLots
        {
            get { return _parkingLots; }
            set
            {
                Set(() => ParkingLots, ref _parkingLots, value);
                UpdateParkingLotListFilter();
            }
        }
        #endregion

        #region ParkingLotsCollectionViewSource
        private object _parkingLotsCollectionViewSource;
        public object ParkingLotsCollectionViewSource
        {
            get { return _parkingLotsCollectionViewSource; }
            set
            {
                Set(() => ParkingLotsCollectionViewSource, ref _parkingLotsCollectionViewSource, value);
            }
        }
        #endregion

        #region ParkingLotFilterMode
        private ParkingLotFilterMode _parkingLotFilterMode;
        public ParkingLotFilterMode ParkingLotFilterMode
        {
            get { return _parkingLotFilterMode; }
            set { Set(() => ParkingLotFilterMode, ref _parkingLotFilterMode, value); }
        }
        #endregion

        #region ParkingLotFilterIsGrouped
        private bool _parkingLotFilterIsGrouped;
        public bool ParkingLotFilterIsGrouped
        {
            get { return _parkingLotFilterIsGrouped; }
            set
            {
                Set(() => ParkingLotFilterIsGrouped, ref _parkingLotFilterIsGrouped, value);
                UpdateParkingLotListFilter();
            }
        }
        #endregion

        #region ParkingLotFilterAscending
        private bool _parkingLotFilterAscending;
        public bool ParkingLotFilterAscending
        {
            get { return _parkingLotFilterAscending; }
            set
            {
                Set(() => ParkingLotFilterAscending, ref _parkingLotFilterAscending, value);
                UpdateParkingLotListFilter();
            }
        }
        #endregion

        #endregion

        #region CONSTRUCTOR

        public MainViewModel(IParkenDdClient client, VoiceCommandService voiceCommandService, ParkingLotListFilterService filterService)
        {
            _client = client;
            _voiceCommands = voiceCommandService;
            _filterService = filterService;
            ParkingLotFilterMode = ParkingLotFilterMode.Alphabetically;
            ParkingLotFilterIsGrouped = true;
            ParkingLotFilterAscending = true;
            LoadMetaData();
        }

        #endregion

        #region LOGIC

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

        private async void UpdateParkingLotListFilter()
        {
            if (ParkingLots == null)
            {
                ParkingLotsCollectionViewSource = null;
            }
            else
            {
                LoadingCity = true;
                if (ParkingLotFilterIsGrouped)
                {
                    ParkingLotsCollectionViewSource = await _filterService.CreateGroups(ParkingLots);
                }
                else
                {
                    ParkingLotsCollectionViewSource = await _filterService.CreateList(ParkingLots);
                }
                LoadingCity = false;
            }
        }

        #endregion

        #region COMMANDS

        #region ReloadSelectedCityCommand
        private RelayCommand _reloadSelecedCityCommand;
        public RelayCommand ReloadSelectedCityCommand => _reloadSelecedCityCommand ?? (_reloadSelecedCityCommand = new RelayCommand(ReloadSelectedCity));

        private async void ReloadSelectedCity()
        {
            LoadingCity = true;
            SelectedCityData = await LoadCity(SelectedCity.Id);
            LoadingCity = false;
        }
        #endregion

        #region SetParkingLotFilterToAlphabeticallyCommand
        private RelayCommand _setParkingLotFilterToAlphabeticallyCommand;
        public RelayCommand SetParkingLotFilterToAlphabeticallyCommand => _setParkingLotFilterToAlphabeticallyCommand ?? (_setParkingLotFilterToAlphabeticallyCommand = new RelayCommand(SetParkingLotFilterToAlphabetically));

        private void SetParkingLotFilterToAlphabetically()
        {
            ParkingLotFilterMode = ParkingLotFilterMode.Alphabetically;
            UpdateParkingLotListFilter();
        }
        #endregion

        #region SetParkingLotFilterToAlphabeticallyCommand
        private RelayCommand _setParkingLotFilterToDistanceCommand;
        public RelayCommand SetParkingLotFilterToDistanceCommand => _setParkingLotFilterToDistanceCommand ?? (_setParkingLotFilterToDistanceCommand = new RelayCommand(SetParkingLotFilterToDistance));

        private void SetParkingLotFilterToDistance()
        {
            ParkingLotFilterMode = ParkingLotFilterMode.Distance;
            UpdateParkingLotListFilter();
        }
        #endregion

        #region SetParkingLotFilterToAvailabilityCommand
        private RelayCommand _setParkingLotFilterToAvailabilityCommand;
        public RelayCommand SetParkingLotFilterToAvailabilityCommand => _setParkingLotFilterToAvailabilityCommand ?? (_setParkingLotFilterToAvailabilityCommand = new RelayCommand(SetParkingLotFilterToAvailability));

        private void SetParkingLotFilterToAvailability()
        {
            ParkingLotFilterMode = ParkingLotFilterMode.Availability;
            UpdateParkingLotListFilter();
        }
        #endregion

        #endregion
    }
}
