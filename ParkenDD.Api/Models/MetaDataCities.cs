using System.Collections.Generic;
using System.Linq;

namespace ParkenDD.Api.Models
{
    public class MetaDataCities : Dictionary<string, MetaDataCityRow>
    {
        public List<MetaDataCityRow> List => this.Select(x => x.Value).ToList();

        public MetaDataCities()
        {
        }

        public MetaDataCities(IDictionary<string, MetaDataCityRow> data) : base(data)
        {
        }
    }
}
