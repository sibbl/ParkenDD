using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
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
        private readonly GeolocationService _geo;
        private readonly TrackingService _tracking;
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
                _tracking.TrackSelectParkingLotEvent(SelectedCity, SelectedParkingLot?.ParkingLot);
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

        #region SearchText

        private string _searchText;

        public string SearchText
        {
            get { return _searchText; }
            set
            {
                Set(() => SearchText, ref _searchText, value);
            }
        }
        #endregion

        #region SearchSuggestions

        private List<AddressSearchSuggestionItem> _searchSuggestions;
        public List<AddressSearchSuggestionItem> SearchSuggestions
        {
            get { return _searchSuggestions; }
            set { Set(() => SearchSuggestions, ref _searchSuggestions, value); }
        }

        #endregion

        #region MapCenter

        private Geopoint _mapCenter;

        public Geopoint MapCenter
        {
            get { return _mapCenter; }
            set { Set(() => MapCenter, ref _mapCenter, value); }
        }

        #endregion

        #endregion

        #region CONSTRUCTOR

        public MainViewModel(IParkenDdClient client,
            VoiceCommandService voiceCommandService,
            ParkingLotListFilterService filterService,
            SettingsService settings,
            LifecycleService lifecycle,
            StorageService storage,
            GeolocationService geo,
            TrackingService tracking)
        {
            _client = client;
            _voiceCommands = voiceCommandService;
            _filterService = filterService;
            _settings = settings;
            _storage = storage;
            _geo = geo;
            _tracking = tracking;
            lifecycle.Register(this);
            OnResume();
        }

        #endregion

        #region LOGIC

        private async void InitData()
        {
            Debug.WriteLine("[MainVm] InitData started");
            LoadingMetaData = true;
            MetaData = await GetMetaData();
            LoadingMetaData = false;
            SelectedCity = FindCityByName(MetaData, "Dresden"); //TODO: select nearest or recently selected city
            TryLoadOnlineMetaData();
            TryLoadOnlineCityData();
            Debug.WriteLine("[MainVm] InitData finished");
        }

        private async Task<MetaData> GetOfflineMetaData()
        {
            Debug.WriteLine("[MainVm] GetOfflineMetaData");
            return await _storage.ReadMetaDataAsync();
        }

        private async Task<City> GetOfflineCityData(string cityId)
        {
            Debug.WriteLine("[MainVm] GetOfflineCityData for {0}", cityId, null);
            return await _storage.ReadCityDataAsync(cityId);
        }

        private async Task<MetaData> GetMetaData(bool forceServerRefresh = false)
        {
            Debug.WriteLine("[MainVm] GetMetaData (forcerefresh={0})", forceServerRefresh);
            if (!forceServerRefresh)
            {
                var offlineMetaData = await GetOfflineMetaData();
                if (offlineMetaData != null)
                {
                    Debug.WriteLine("[MainVm] GetMetaData (forcerefresh={0}): found offline data", forceServerRefresh);
                    return offlineMetaData;
                }
            }
            Debug.WriteLine("[MainVm] GetMetaData (forcerefresh={0}): perform request", forceServerRefresh);
            var metaData = await _client.GetMetaDataAsync();
            Debug.WriteLine("[MainVm] GetMetaData (forcerefresh={0}): got response", forceServerRefresh);
            _metaDataIsOnlineData = true;
            _storage.SaveMetaData(metaData);
            _voiceCommands.UpdateCityList(metaData);
            return metaData;
        }

        private async Task<City> GetCity(string cityId, bool forceServerRefresh = false)
        {
            Debug.WriteLine("[MainVm] GetCity for {0} (forcerefresh={1})", cityId, forceServerRefresh);
            if (!forceServerRefresh)
            {
                var offlineCity = await GetOfflineCityData(cityId);
                if (offlineCity != null)
                {
                    Debug.WriteLine("[MainVm] GetCity for {0} (forcerefresh={1}): found offline data", cityId, forceServerRefresh);
                    return offlineCity;
                }
            }
            Debug.WriteLine("[MainVm] GetCity for {0} (forcerefresh={1}): perform request", cityId, forceServerRefresh);
            var city = await _client.GetCityAsync(cityId);
            Debug.WriteLine("[MainVm] GetCity for {0} (forcerefresh={1}): got response", cityId, forceServerRefresh);
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
            Debug.WriteLine("[MainVm] TrySelectCityById for {0}: find city by id", id, null);
            if (FindCityById(MetaData, id) == null)
            {
                Debug.WriteLine("[MainVm] TrySelectCityById for {0}: needs loading meta data", id, null);
                MetaData = await GetMetaData() ?? await GetMetaData(true);
            }
            var city = FindCityById(MetaData, id);
            if (city != null)
            {
                Debug.WriteLine("[MainVm] TrySelectCityById for {0}: success", id, null);
                SelectedCity = city;
            }
        }

        public async Task TrySelectCityByName(string name)
        {
            Debug.WriteLine("[MainVm] TrySelectCityByName for {0}: find city by name", name, null);
            if (FindCityByName(MetaData, name) == null)
            {
                Debug.WriteLine("[MainVm] TrySelectCityByName for {0}: needs loading meta data", name, null);
                MetaData = await GetMetaData() ?? await GetMetaData(true);
            }
            var city = FindCityByName(MetaData, name);
            if (city != null)
            {
                Debug.WriteLine("[MainVm] TrySelectCityByName for {0}: success", name, null);
                SelectedCity = city;
            }
        }

        public async Task TrySelectParkingLotByName(string cityName, string parkingLotName)
        {
            Debug.WriteLine("[MainVm] TrySelectParkingLotByName for {0} / {1}: find city by name", cityName, parkingLotName);
            if (FindCityByName(MetaData, cityName) == null)
            {
                Debug.WriteLine("[MainVm] TrySelectParkingLotByName for {0} / {1}: needs loading meta data", cityName, parkingLotName);
                MetaData = await GetMetaData() ?? await GetMetaData(true);
            }
            var city = FindCityByName(MetaData, cityName);
            if (city != null)
            {
                Debug.WriteLine("[MainVm] TrySelectParkingLotByName for {0} / {1}: found city, load city details now", cityName, parkingLotName);
                var cityDetails = await LoadCity(city.Name);
                if (FindParkingLotByName(cityDetails, parkingLotName) == null)
                {
                    Debug.WriteLine("[MainVm] TrySelectParkingLotByName for {0} / {1}: force reload city details", cityName, parkingLotName);
                    cityDetails = await LoadCity(city.Name, true);
                }
                SelectedCity = city;
                SelectedCityData = cityDetails;
                Debug.WriteLine("[MainVm] TrySelectParkingLotByName for {0} / {1}: try find parking lot by name", cityName, parkingLotName);
                var parkingLot = FindParkingLotByName(cityDetails, parkingLotName);
                if (parkingLot != null)
                {
                    Debug.WriteLine("[MainVm] TrySelectParkingLotByName for {0} / {1}: success", cityName, parkingLotName);
                    SelectedParkingLot = new SelectableParkingLot(parkingLot);
                }
            }
        }

        private async Task TrySelectParkingLotById(string cityId, string parkingLotId)
        {
            Debug.WriteLine("[MainVm] TrySelectParkingLotById for {0} / {1}", cityId, parkingLotId);
            var taskCity = TrySelectCityById(cityId);
            ParkingLot parkingLot = null;
            var taskParkingLot = Task.Run(async () =>
            {
                var city = FindCityById(MetaData, cityId);
                if (city != null)
                {
                    Debug.WriteLine("[MainVm] TrySelectParkingLotById for {0} / {1}: load city", cityId, parkingLotId);
                    var cityDetails = await LoadCity(city.Name);
                    if (FindParkingLotById(cityDetails, parkingLotId) == null)
                    {
                        Debug.WriteLine("[MainVm] TrySelectParkingLotById for {0} / {1}: force refresh city details", cityId, parkingLotId);
                        cityDetails = await LoadCity(city.Name, true);
                    }
                    SelectedCity = city;
                    SelectedCityData = cityDetails;
                    Debug.WriteLine("[MainVm] TrySelectParkingLotById for {0} / {1}: set city + city data. find parking lot now", cityId, parkingLotId);
                    parkingLot = FindParkingLotById(cityDetails, parkingLotId);
                }
            });
            await Task.WhenAll(taskCity, taskParkingLot);
            if (parkingLot != null)
            {
                Debug.WriteLine("[MainVm] TrySelectParkingLotById for {0} / {1}: success", cityId, parkingLotId);
                SelectedParkingLot = new SelectableParkingLot(parkingLot);
            }
        }

        private async void LoadCityAndSelectCity()
        {
            _tracking.TrackSelectCityEvent(SelectedCity);
            Debug.WriteLine("[MainVm] LoadCityAndSelectCity");
            Debug.WriteLine("[MainVm] LoadCityAndSelectCity: found selected city, reset selection");
            SelectedCityData = null;
            LoadingCity = true;
            Debug.WriteLine("[MainVm] LoadCityAndSelectCity: load selected city");
            SelectedCityData = await LoadCity(SelectedCity.Id);
            LoadingCity = false;
            SelectedParkingLot = null;
            UpdateMapBounds();
        }

        private void UpdateMapBounds()
        {
            if (SelectedCityData == null)
                return;
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
            Debug.WriteLine("[MainVm] LoadCity: {0} (forceRefresh={1})", cityId, forceRefresh);
            if (_cities.ContainsKey(cityId))
            {
                if (forceRefresh)
                {
                    var newCityData = await GetCity(cityId, true);
                    _cities[cityId].Merge(newCityData, ParkingLots);
                    UpdateParkingLotListFilter();
                    return _cities[cityId];
                }
                return _cities[cityId];
            }
            Debug.WriteLine("[MainVm] LoadCity: {0} not in dict", cityId, null);
            _cities[cityId] = await GetCity(cityId, forceRefresh);
            Debug.WriteLine("[MainVm] LoadCity: loaded {0}", cityId, null);
            return _cities[cityId];
        }

        private async void UpdateParkingLotListFilter()
        {
            Debug.WriteLine("[MainVm] UpdateParkingLotListFilter");
            if (ParkingLots == null)
            {
                Debug.WriteLine("[MainVm] UpdateParkingLotListFilter: ParkingLots is null");
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
                _tracking.TrackParkingLotFilterEvent(ParkingLotFilterMode, ParkingLotFilterAscending, ParkingLotFilterIsGrouped);
                var selectedParkingLot = SelectedParkingLot;
                if (ParkingLotFilterIsGrouped)
                {
                    Debug.WriteLine("[MainVm] UpdateParkingLotListFilter: create groups");
                    ParkingLotsGroupedCollectionViewSource = await _filterService.CreateGroups(ParkingLots);
                }
                else
                {
                    Debug.WriteLine("[MainVm] UpdateParkingLotListFilter: create list");
                    ParkingLotsListCollectionViewSource = await _filterService.CreateList(ParkingLots);
                }
                SelectedParkingLot = selectedParkingLot; //need to do this as the CVS uses the first one again...
                if (SelectedParkingLot != null)
                {
                    SelectedParkingLot.IsSelected = true;
                }
                LoadingCity = false;
            }
        }

        private async void TryLoadOnlineCityData()
        {
            Debug.WriteLine("[MainVm] TryLoadOnlineCityData");
            if (SelectedCity != null)
            {
                if (!_cityHasOnlineData.ContainsKey(SelectedCity.Id) || _cityHasOnlineData[SelectedCity.Id] != true)
                {
                    Debug.WriteLine("[MainVm] TryLoadOnlineCityData: not loaded yet, do request");
                    await LoadCity(SelectedCity.Id, true);
                }
            }
        }

        private async void TryLoadOnlineMetaData()
        {
            Debug.WriteLine("[MainVm] TryLoadOnlineMetaData");
            if (_metaDataIsOnlineData != true)
            {
                Debug.WriteLine("[MainVm] TryLoadOnlineMetaData: not loaded yet, do request");
                var newData = await GetMetaData(true);
                if (MetaData == null)
                {
                    Debug.WriteLine("[MainVm] TryLoadOnlineMetaData: set new MetaData");
                    MetaData = newData;
                }
                else
                {
                    Debug.WriteLine("[MainVm] TryLoadOnlineMetaData: merge with MetaData");
                    MetaData.Merge(newData);
                }
            }
        }

        public async void TryGetUserPosition()
        {
            Debug.WriteLine("[MainVm] TryGetUserPosition: request location");
            await _geo.GetUserLocation();
            Debug.WriteLine("[MainVm] TryGetUserPosition: got answer");
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
            _tracking.TrackNavigateToParkingLotEvent(SelectedCity, lot);
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
            TryGetUserPosition();
            if (SelectedCity != null)
            {
                _tracking.TrackReloadCityEvent(SelectedCity);
                LoadingCity = true;
                await LoadCity(SelectedCity.Id, true);
                LoadingCity = false;
            }
        }
        #endregion

        #region SearchTextChangedCommand

        private RelayCommand<AutoSuggestBoxTextChangedEventArgs> _searchTextChangedCommand;
        public RelayCommand<AutoSuggestBoxTextChangedEventArgs> SearchTextChangedCommand => _searchTextChangedCommand ?? (_searchTextChangedCommand = new RelayCommand<AutoSuggestBoxTextChangedEventArgs>(SearchTextChanged));

        private async void SearchTextChanged(AutoSuggestBoxTextChangedEventArgs args)
        {
            var isUserInput = args.Reason == AutoSuggestionBoxTextChangeReason.UserInput;
            if (isUserInput && !string.IsNullOrEmpty(SearchText))
            {
                //TODO: handle concurrent searches
                var result = await MapLocationFinder.FindLocationsAsync(
                    SearchText,
                    MapCenter,
                    10);
                if (result.Status == MapLocationFinderStatus.Success)
                {
                    SearchSuggestions = result.Locations.Select(x => new AddressSearchSuggestionItem(x)).Where(x => !string.IsNullOrEmpty(x.ToString())).ToList();
                }
            }

            if (string.IsNullOrWhiteSpace(SearchText))
            {
                Messenger.Default.Send(new HideSearchResultOnMapMessage());
                SearchSuggestions = null;
            }
        }

        #endregion

        #region SearchResultChosenCommand

        private RelayCommand<AutoSuggestBoxQuerySubmittedEventArgs> _searchResultChosenCommand;
        public RelayCommand<AutoSuggestBoxQuerySubmittedEventArgs> SearchResultChosenCommand => _searchResultChosenCommand ?? (_searchResultChosenCommand = new RelayCommand<AutoSuggestBoxQuerySubmittedEventArgs>(SearchResultChosen));

        private void SearchResultChosen(AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            var result = args.ChosenSuggestion as AddressSearchSuggestionItem;
            if (result != null)
            {
                _tracking.TrackSelectSearchResultEvent();
                Messenger.Default.Send(new ShowSearchResultOnMapMessage(result));
            }
        }

        #endregion

        #endregion

        private async void LoadLastState(string selectedCityId)
        {
            var selectedParkingLotId = _settings.SelectedParkingLotId;
            if (string.IsNullOrEmpty(selectedParkingLotId))
            {
                await TrySelectCityById(selectedCityId);
            }
            else
            {
                await TrySelectParkingLotById(selectedCityId, selectedParkingLotId);
            }
            TryLoadOnlineMetaData();
            TryLoadOnlineCityData();
        }

        public void OnResume()
        {
            ParkingLotFilterMode = _settings.ParkingLotFilterMode;
            ParkingLotFilterIsGrouped = _settings.ParkingLotFilterIsGrouped;
            ParkingLotFilterAscending = _settings.ParkingLotFilterAscending;
            var selectedCityId = _settings.SelectedCityId;
            if (!string.IsNullOrEmpty(selectedCityId))
            {
                LoadLastState(selectedCityId);
            }
            else
            {
                InitData();
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
