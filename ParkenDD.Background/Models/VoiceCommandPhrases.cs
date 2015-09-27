using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace ParkenDD.Background.Models
{
    public sealed class VoiceCommandPhrases
    {
        private class VoiceCommandInputComparer : IEqualityComparer<string>
        {
            const string Pattern = @"[^a-zA-Z0-9\-_&\s\/]";

            public static bool UmlautsIgnoringEqual(string a, string b)
            {
                //e.g. Zürich becomes Z�rich, so this is a workaround to compare strings by ignoring umlauts
                a = Regex.Replace(a, Pattern, "");
                b = Regex.Replace(b, Pattern, "");
                return a.Equals(b, StringComparison.OrdinalIgnoreCase);
            }
            public bool Equals(string a, string b)
            {
                //e.g. Zürich becomes Z�rich, so this is a workaround to compare strings by ignoring umlauts
                a = Regex.Replace(a, Pattern, "");
                b = Regex.Replace(b, Pattern, "");
                return UmlautsIgnoringEqual(a, b);
            }

            public int GetHashCode(string obj)
            {
                return obj.GetHashCode();
            }
        }

        public IList<VoiceCommandMetaDataPhrase> Cities { get; set; }

        public VoiceCommandPhrases()
        {
            Cities = new List<VoiceCommandMetaDataPhrase>();
        }
        
        public IEnumerable<string> GetCityPhraseList()
        {
            return Cities.Select(x => x.Name).Distinct(new VoiceCommandInputComparer());
        }

        public IEnumerable<string> GetParkingLotPhraseList()
        {
            var result = new List<string>();
            foreach (var city in Cities)
            {
                result.AddRange(city.ParkingLots.Select(lots => lots.Name));
            }
            return result.Distinct(new VoiceCommandInputComparer());
        }

        public string FindCityIdByName(string name)
        {
            return
                (from city in Cities
                    where VoiceCommandInputComparer.UmlautsIgnoringEqual(city.Name, name)
                    select city.Id)
                .FirstOrDefault();
        }

        public string FindParkingLotIdByNameAndCityId(string cityId, string name)
        {
            var city = Cities.FirstOrDefault(x => x.Id.Equals(cityId));
            return city?.ParkingLots.FirstOrDefault(x => VoiceCommandInputComparer.UmlautsIgnoringEqual(x.Name, name))?.Id;
        }
    }
}
