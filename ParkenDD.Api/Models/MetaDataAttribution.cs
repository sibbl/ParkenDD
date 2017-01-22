using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace ParkenDD.Api.Models
{
    public class MetaDataAttribution : ViewModelBase
    {
        /// <summary>
        ///     Contributor of the data source
        /// </summary>
        public String Contributor
        {
            get { return _contributor; }
            set { Set(ref _contributor, value); }
        }
        private String _contributor;

        /// <summary>
        ///     License of the data source
        /// </summary>
        public String License
        {
            get { return _license; }
            set { Set(ref _license, value); }
        }
        private String _license;

        /// <summary>
        ///     URL to the data source
        /// </summary>
        public Uri Url
        {
            get { return _url; }
            set { Set(ref _url, value); }
        }
        private Uri _url;
    }
}
