using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.System;
using Cimbalino.Toolkit.Extensions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ParkenDD.Messages;
using ParkenDD.Services;
using ParkenDD.Api.Models;
using ParkenDD.Api.Interfaces;
using ParkenDD.Interfaces;
using ParkenDD.Models;
using ParkenDD.Utils;

namespace ParkenDD.ViewModels
{
    public class MainViewModel : ViewModelBase, ICanResume, ICanSuspend
    {
        #region PRIVATE PROPERTIES
        private readonly IParkenDdClient _client;
        private readonly VoiceCommandService _voiceCommands;
        private readonly ParkingLotListFilterService _filterService;
        private readonly SettingsService _settings;
        private readonly StorageService _storage;
        private readonly Dictionary<string, City> _cities = new Dictionary<string, City>();
        private readonly Dictionary<string, bool> _cityHasOnlineData = new Dictionary<string, bool>();
        private bool _metaDataIsOnlineData = false;
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
                TryLoadOnlineCityData();
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
                ParkingLots = value == null ? null :  new ObservableCollection<SelectableParkingLot>(value.Lots.Select(x => new SelectableParkingLot(x)));
            }
        }
        #endregion

        #region SelectedParkingLot
        private SelectableParkingLot _selectedParkingLot;
        public SelectableParkingLot SelectedParkingLot
        {
            get { return _selectedParkingLot; }
            set
            {
                if (value != null)
                {
                    value.IsSelected = true;
                }
                if (_selectedParkingLot != null)
                {
                    _selectedParkingLot.IsSelected = false;
                }
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
        private ObservableCollection<SelectableParkingLot> _parkingLots;
        public ObservableCollection<SelectableParkingLot> ParkingLots
        {
            get { return _parkingLots; }
            set
            {
                Set(() => ParkingLots, ref _parkingLots, value);
                UpdateParkingLotListFilter();
            }
        }
        #endregion

        #region ParkingLotsGroupedCollectionViewSource
        private IEnumerable<ParkingLotListGroup> _parkingLotsGroupedCollectionViewSource;
        public IEnumerable<ParkingLotListGroup> ParkingLotsGroupedCollectionViewSource
        {
            get { return _parkingLotsGroupedCollectionViewSource; }
            set
            {
                Set(() => ParkingLotsGroupedCollectionViewSource, ref _parkingLotsGroupedCollectionViewSource, value);
            }
        }
        #endregion

        #region ParkingLotsListCollectionViewSource
        private IEnumerable<SelectableParkingLot> _parkingLotsListCollectionViewSource;
        public IEnumerable<SelectableParkingLot> ParkingLotsListCollectionViewSource
        {
            get { return _parkingLotsListCollectionViewSource; }
            set
            {
                Set(() => ParkingLotsListCollectionViewSource, ref _parkingLotsListCollectionViewSource, value);
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

        #region UserLocation
        private Geoposition _userLocation;
        public Geoposition UserLocation
        {
            get { return _userLocation; }
            set { Set(() => UserLocation, ref _userLocation, value); }
        }
        #endregion

        #endregion

        #region CONSTRUCTOR

        public MainViewModel(IParkenDdClient client,
            VoiceCommandService voiceCommandService,
            ParkingLotListFilterService filterService,
            SettingsService settings,
            LifecycleService lifecycle,
            StorageService storage)
        {
            _client = client;
            _voiceCommands = voiceCommandService;
            _filterService = filterService;
            _settings = settings;
            _storage = storage;
            lifecycle.Register(this);
            OnResume();
        }

        #endregion

        #region LOGIC

        private async Task InitData()
        {
            LoadingMetaData = true;
            MetaData = await GetMetaData();
            LoadingMetaData = false;
            SelectedCity = FindCityByName(MetaData, "Dresden"); //TODO: select nearest or recently selected city
            TryLoadOnlineMetaData();
            TryLoadOnlineCityData();
        }

        private async Task<MetaData> GetOfflineMetaData()
        {
            return await _storage.ReadMetaDataAsync();
        }

        private async Task<City> GetOfflineCityData(string cityId)
        {
            return await _storage.ReadCityDataAsync(cityId);
        }

        private async Task<MetaData> GetMetaData(bool forceServerRefresh = false)
        {
            if (!forceServerRefresh)
            {
                var offlineMetaData = await GetOfflineMetaData();
                if (offlineMetaData != null)
                {
                    return offlineMetaData;
                }
            }
            var metaData = await _client.GetMetaDataAsync();
            _metaDataIsOnlineData = true;
            _storage.SaveMetaData(metaData);
            _voiceCommands.UpdateCityList(metaData);
            return metaData;
        }

        private async Task<City> GetCity(string cityId, bool forceServerRefresh = false)
        {
            if (!forceServerRefresh)
            {
                var offlineCity = await GetOfflineCityData(cityId);
                if (offlineCity != null)
                {
                    return offlineCity;
                }
            }
            var city = await _client.GetCityAsync(cityId);
            _cityHasOnlineData[cityId] = true;
            _storage.SaveCityData(cityId, city);
            _voiceCommands.UpdateParkingLotList(city);
            return city;
        }

        private MetaDataCityRow FindCityByName(MetaData metaData, string name)
        {
            return metaData?.Cities?.Select(x => x.Value).FirstOrDefault(x => x.Name.ToLower().Equals(name.ToLower()));
        }

        private MetaDataCityRow FindCityById(MetaData metaData, string id)
        {
            if (metaData?.Cities == null || !metaData.Cities.ContainsKey(id))
            {
                return null;
            }
            return metaData.Cities[id];
        }

        private ParkingLot FindParkingLotByName(City city, string name)
        {
            return city?.Lots?.FirstOrDefault(x => x.Name.ToLower().Equals(name.ToLower()));
        }

        private ParkingLot FindParkingLotById(City city, string id)
        {
            return city?.Lots?.FirstOrDefault(x => x.Id.Equals(id));
        }

        private async Task TrySelectCityById(string id)
        {
            if (FindCityById(MetaData, id) == null)
            {
                MetaData = await GetMetaData() ?? await GetMetaData(true);
            }
            var city = FindCityById(MetaData, id);
            if (city != null)
            {
                SelectedCity = city;
            }
            TryLoadOnlineMetaData();
        }

        public async Task TrySelectCityByName(string name)
        {
            if (FindCityByName(MetaData, name) == null)
            {
                MetaData = await GetMetaData() ?? await GetMetaData(true);
            }
            var city = FindCityByName(MetaData, name);
            if (city != null)
            {
                SelectedCity = city;
            }
            TryLoadOnlineMetaData();
        }

        public async Task TrySelectParkingLotByName(string cityName, string parkingLotName)
        {
            if (FindCityByName(MetaData, cityName) == null)
            {
                MetaData = await GetMetaData() ?? await GetMetaData(true);
            }
            var city = FindCityByName(MetaData, cityName);
            if (city != null)
            {
                var cityDetails = await LoadCity(city.Name);
                if (FindParkingLotByName(cityDetails, parkingLotName) == null)
                {
                    cityDetails = await LoadCity(city.Name, true);
                }
                SelectedCity = city;
                SelectedCityData = cityDetails;
                var parkingLot = FindParkingLotByName(cityDetails, parkingLotName);
                if (parkingLot != null)
                {
                    SelectedParkingLot = new SelectableParkingLot(parkingLot);
                }
            }
            TryLoadOnlineMetaData();
            TryLoadOnlineCityData();
        }

        private async Task TrySelectParkingLotById(string cityId, string parkingLotId)
        {
            var taskCity = TrySelectCityById(cityId);
            ParkingLot parkingLot = null;
            var taskParkingLot = Task.Run(async () =>
            {
                var city = FindCityById(MetaData, cityId);
                if (city != null)
                {
                    var cityDetails = await LoadCity(city.Name);
                    if (FindParkingLotById(cityDetails, parkingLotId) == null)
                    {
                        cityDetails = await LoadCity(city.Name, true);
                    }
                    SelectedCity = city;
                    SelectedCityData = cityDetails;
                    parkingLot = FindParkingLotById(cityDetails, parkingLotId);
                }
            });
            await Task.WhenAll(taskCity, taskParkingLot);
            if (parkingLot != null)
            {
                SelectedParkingLot = new SelectableParkingLot(parkingLot);
            }
            TryLoadOnlineMetaData();
            TryLoadOnlineCityData();
        }

        private async void LoadCityAndSelectCity()
        {
            SelectedCityData = null;
            LoadingCity = true;
            SelectedCityData = await LoadCity(SelectedCity.Id);
            LoadingCity = false;
            SelectedParkingLot = null;
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
                var cityPoint = SelectedCity.Coordinates?.Point;
                if (cityPoint != null)
                {
                    Messenger.Default.Send(new ZoomMapToCoordinateMessage(SelectedCity.Coordinates.Point));
                }
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
            if (_cities.ContainsKey(cityId))
            {
                if (forceRefresh)
                {
                    var newCityData = await GetCity(cityId, true);
                    bool addedOrRemovedItems;
                    _cities[cityId].Merge(newCityData, ParkingLots, out addedOrRemovedItems);
                    if (addedOrRemovedItems)
                    {
                        UpdateParkingLotListFilter();
                    }
                    return _cities[cityId];
                }
                return _cities[cityId];
            }
            _cities[cityId] = await GetCity(cityId, forceRefresh);
            return _cities[cityId];
        }

        private async void UpdateParkingLotListFilter()
        {
            if (ParkingLots == null)
            {
                if (ParkingLotFilterIsGrouped)
                {
                    ParkingLotsGroupedCollectionViewSource = null;
                }
                else
                {
                    ParkingLotsListCollectionViewSource = null;
                }
            }
            else
            {
                LoadingCity = true;
                var selectedParkingLot = SelectedParkingLot;
                if (ParkingLotFilterIsGrouped)
                {
                    ParkingLotsGroupedCollectionViewSource = await _filterService.CreateGroups(ParkingLots);
                }
                else
                {
                    ParkingLotsListCollectionViewSource = await _filterService.CreateList(ParkingLots);
                }
                SelectedParkingLot = selectedParkingLot; //need to do this as the CVS uses the first one again...
                LoadingCity = false;
            }
        }

        private async void TryLoadOnlineCityData()
        {
            if (SelectedCity != null)
            {
                if (!_cityHasOnlineData.ContainsKey(SelectedCity.Id) || _cityHasOnlineData[SelectedCity.Id] != true)
                {
                    await LoadCity(SelectedCity.Id, true);
                }
            }
        }

        private async void TryLoadOnlineMetaData()
        {
            if (_metaDataIsOnlineData != true)
            {
                MetaData.Merge(await GetMetaData(true));
            }
        }

        #endregion

        #region COMMANDS

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

        #region NavigateToParkingLotCommand
        private RelayCommand<ParkingLot> _navigateToParkingLotCommand;
        public RelayCommand<ParkingLot> NavigateToParkingLotCommand => _navigateToParkingLotCommand ?? (_navigateToParkingLotCommand = new RelayCommand<ParkingLot>(NavigateToParkingLot));

        private async void NavigateToParkingLot(ParkingLot lot)
        {
            const string launcherString = "bingmaps:?rtp=~{0}";
            const string launcherPosString = "pos.{0}_{1}_{2}";
            const string launcherAdrString = "adr.{0}";
            if (lot == null)
            {
                return;
            }
            Uri launcherUri = null;
            if (lot.Coordinates != null)
            {
                launcherUri = new Uri(
                    String.Format(
                        launcherString,
                        String.Format(
                            launcherPosString,
                            lot.Coordinates.Latitude.ToString(CultureInfo.InvariantCulture),
                            lot.Coordinates.Longitude.ToString(CultureInfo.InvariantCulture),
                            String.Format("Parkplatz {0}", lot.Name) //TODO: localize
                        )
                    )
                );
            }else if (!String.IsNullOrEmpty(lot.Address))
            {
                launcherUri = new Uri(
                    String.Format(
                        launcherString,
                        String.Format(
                            launcherAdrString,
                            Uri.EscapeDataString(lot.Address + ", " + SelectedCity.Name)
                            )
                        )
                    );
            }
            else
            {
                launcherUri = new Uri(
                    String.Format(
                        launcherString,
                        String.Format(
                            launcherAdrString,
                            Uri.EscapeDataString(lot.Name + ", " + SelectedCity.Name)
                            )
                        )
                    );
            }
            if (launcherUri != null)
            {
                var launcherOptions = new LauncherOptions
                {
                    TargetApplicationPackageFamilyName = "Microsoft.WindowsMaps_8wekyb3d8bbwe"
                };
                await Launcher.LaunchUriAsync(launcherUri, launcherOptions);
            }
        }
        #endregion

        #region RefreshCityDetailsCommand
        private RelayCommand _refreshCityDetailsCommand;
        public RelayCommand RefreshCityDetailsCommand => _refreshCityDetailsCommand ?? (_refreshCityDetailsCommand = new RelayCommand(RefreshCityDetails));

        private async void RefreshCityDetails()
        {
            if (SelectedCity != null)
            {
                LoadingCity = true;
                await LoadCity(SelectedCity.Id, true);
                LoadingCity = false;
            }
        }
        #endregion

        #endregion

        public async void OnResume()
        {
            ParkingLotFilterMode = _settings.ParkingLotFilterMode;
            ParkingLotFilterIsGrouped = _settings.ParkingLotFilterIsGrouped;
            ParkingLotFilterAscending = _settings.ParkingLotFilterAscending;
            var selectedCityId = _settings.SelectedCityId;
            if (!String.IsNullOrEmpty(selectedCityId))
            {
                var selectedParkingLotId = _settings.SelectedParkingLotId;
                if (String.IsNullOrEmpty(selectedParkingLotId))
                {
                    await TrySelectCityById(selectedCityId);
                }
                else
                {
                    await TrySelectParkingLotById(selectedCityId, selectedParkingLotId);
                }
            }
            else
            {
                await InitData();
            }
        }

        public Task OnSuspend()
        {
            return Task.Run(() =>
            {
                _settings.ParkingLotFilterMode = ParkingLotFilterMode;
                _settings.ParkingLotFilterAscending = ParkingLotFilterAscending;
                _settings.ParkingLotFilterIsGrouped = ParkingLotFilterIsGrouped;
                _settings.SelectedCityId = SelectedCity?.Id;
                _settings.SelectedParkingLotId = SelectedParkingLot?.ParkingLot?.Id;
            });
        }
    }
}
