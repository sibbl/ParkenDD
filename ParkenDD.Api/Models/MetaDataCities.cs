using System.Collections.ObjectModel;
using System.Linq;

namespace ParkenDD.Api.Models
{
    public sealed class MetaDataCities : ObservableCollection<MetaDataCityRow>
    {
        public MetaDataCityRow Get(string cityId)
        {
            return this.FirstOrDefault(x => x.Id == cityId);
        }
    }
}
