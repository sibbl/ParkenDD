using System;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace ParkenDD.Api.Models
{
    public sealed class MetaDataCityRow : ViewModelBase
    {
        /// <summary>
        ///     Unique ID of city
        /// </summary>
        private string _id;
        public string Id
        {
            get { return _id; }
            set { Set(ref _id, value); }
        }

        /// <summary>
        ///     Name of city
        /// </summary>
        [JsonProperty("name")]
        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }
        private string _name;

        /// <summary>
        ///     Source URI of data
        /// </summary>
        [JsonProperty("source")]
        public Uri Source
        {
            get { return _source; }
            set { Set(ref _source, value); }
        }
        private Uri _source;

        /// <summary>
        ///     URI for data
        /// </summary>
        [JsonProperty("url")]
        public Uri Url
        {
            get { return _url; }
            set { Set(ref _url, value); }
        }
        private Uri _url;

        /// <summary>
        ///     Coordinates of the parking lot
        /// </summary>
        [JsonProperty("coords")]
        public Coordinate Coordinates
        {
            get { return _coordinates; }
            set { Set(ref _coordinates, value); }
        }
        private Coordinate _coordinates;

        /// <summary>
        ///     Is parking lot under active support?
        /// </summary>
        [JsonProperty("active_support")]
        public bool ActiveSupport
        {
            get { return _activeSupport; }
            set { Set(ref _activeSupport, value); }
        }
        private bool _activeSupport;



        public override string ToString()
        {
            return Name;
        }
    }
}
