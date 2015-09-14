using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ParkenDD.Api.Models
{
    public class MetaDataCities : Dictionary<string, MetaDataCityRow>
    {
        public ObservableCollection<MetaDataCityRow> List => new ObservableCollection<MetaDataCityRow>(this.Select(x => x.Value).ToList());

        public MetaDataCities()
        {
        }

        public MetaDataCities(IDictionary<string, MetaDataCityRow> data) : base(data)
        {
        }
    }
}
