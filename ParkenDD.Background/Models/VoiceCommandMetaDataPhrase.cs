using System.Collections.Generic;

namespace ParkenDD.Background.Models
{
    public sealed class VoiceCommandMetaDataPhrase
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public IList<VoiceCommandCityPhrase> ParkingLots { get; set; }
        public VoiceCommandMetaDataPhrase()
        {
            ParkingLots = new List<VoiceCommandCityPhrase>();
        }
    }
}
