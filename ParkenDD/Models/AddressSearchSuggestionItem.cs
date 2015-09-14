using System.Text;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;

namespace ParkenDD.Models
{
    public class AddressSearchSuggestionItem
    {
        private readonly MapLocation _location;
        public Geopoint Point { get; set; }

        public string Street
        {
            get
            {
                var str = _location.Address.Street;
                if (!string.IsNullOrEmpty(_location.Address.StreetNumber))
                {
                    str += " " + _location.Address.StreetNumber;
                }
                return str;
            }
        }

        public string Neighborhood => _location.Address.Neighborhood;

        public string District => _location.Address.District;
        public string Town => _location.Address.Town;

        public AddressSearchSuggestionItem(MapLocation location)
        {
            Point = location.Point;
            _location = location;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(Street))
            {
                sb.Append(Street);
            }
            if (!string.IsNullOrEmpty(District))
            {
                sb.AppendFormat(sb.Length == 0 ? "{0}" : ", {0}", District);
            }
            if (!string.IsNullOrEmpty(Town))
            {
                sb.AppendFormat(sb.Length == 0 ? "{0}" : ", {0}", Town);
            }
            return sb.ToString();
        }
    }
}
