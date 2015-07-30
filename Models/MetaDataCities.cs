using System.Collections.Generic;
using System.Linq;

namespace ParkenDD.Win10.Models
{
    public class MetaDataCities : Dictionary<string, MetaDataCityRow>
    {
        public List<MetaDataCityRow> List => this.Select(x => x.Value).ToList();
    }
}
