using System.Collections.Generic;
using System.Linq;
using ParkenDD.Api.Models;
using ParkenDD.Background.Models;
using ParkenDD.Models;

namespace ParkenDD.Utils
{
    public static class VoiceCommandPhraseUtils
    {
        public static void UpdateCities(this VoiceCommandPhrases dict, IEnumerable<MetaDataCityRow> cities)
        {
            if (cities == null)
            {
                return;
            }
            foreach (var city in cities)
            {
                dict.UpdateCity(city);
            }
        }

        public static void UpdateCity(this VoiceCommandPhrases phrase, MetaDataCityRow city)
        {
            var item = phrase.Cities.FirstOrDefault(x => x.Id == city.Id);
            if (item == null)
            {
                item = new VoiceCommandMetaDataPhrase
                {
                    Id = city.Id
                };
                phrase.Cities.Add(item);
            }
            item.Name = city.Name;
        }


        public static void UpdateParkingLots(this VoiceCommandPhrases phrase, MetaDataCityRow city, IEnumerable<ParkingLot> lots)
        {
            var item = phrase.Cities.FirstOrDefault(x => x.Id == city.Id);
            if (item != null)
            {
                item.ParkingLots = lots.Select(x => new VoiceCommandCityPhrase
                {
                    Id = x.Id,
                    Name = x.Name
                }).ToList();
            }
        }
    }
}
