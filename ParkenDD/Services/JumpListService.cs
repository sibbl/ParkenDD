using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.StartScreen;
using ParkenDD.Api.Models;

namespace ParkenDD.Services
{
    public class JumpListService
    {
        private readonly ResourceService _resources;
        private const string ArgumentFormat = "city={0}";

        public JumpListService(ResourceService resources)
        {
            _resources = resources;
        }

        public async void UpdateCityList(IEnumerable<MetaDataCityRow> metaData)
        {
            await UpdateCityListAsync(metaData);
        }
        public async Task UpdateCityListAsync(IEnumerable<MetaDataCityRow> cities)
        {
            if (JumpList.IsSupported())
            {
                var jl = await JumpList.LoadCurrentAsync();
                jl.SystemGroupKind = JumpListSystemGroupKind.None;

                while (jl.Items.Count > 0)
                {
                    jl.Items.RemoveAt(0);
                }

                foreach (var city in cities)
                {
                    var item = JumpListItem.CreateWithArguments(string.Format(ArgumentFormat, city.Id), city.Name);
                    item.GroupName = _resources.JumpListCitiesHeader;
                    item.Logo = new Uri("ms-appx:///Assets/ParkingIcon.png");
                    jl.Items.Add(item);
                }
                await jl.SaveAsync();
            }
        }
    }
}
