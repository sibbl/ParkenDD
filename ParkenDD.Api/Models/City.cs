using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace ParkenDD.Api.Models
{
    public class City : ViewModelBase
    {
        /// <summary>
        ///     Link to the source of the data
        /// </summary>
        [JsonProperty("data_source")]
        public Uri DataSource
        {
            get { return _dataSource; }
            set { Set(ref _dataSource, value); }
        }
        private Uri _dataSource;

        /// <summary>
        ///     Time and date of the last download of the data.
        /// </summary>
        [JsonProperty("last_downloaded")]
        public DateTime LastDownloaded
        {
            get { return _lastDownloaded; }
            set { Set(ref _lastDownloaded, value); }
        }
        private DateTime _lastDownloaded;

        /// <summary>
        ///     Time and date of the last update of the data source
        /// </summary>
        [JsonProperty("last_updated")]
        public DateTime? LastUpdated
        {
            get { return _lastUpdated; }
            set { Set(ref _lastUpdated, value); }
        }
        private DateTime? _lastUpdated;

        /// <summary>
        ///     Array of parking lots in the city.
        /// </summary>
        [JsonProperty("lots")]
        public ObservableCollection<ParkingLot> Lots
        {
            get { return _lots; }
            set { Set(ref _lots, value); }
        }
        private ObservableCollection<ParkingLot> _lots;
    }
}
