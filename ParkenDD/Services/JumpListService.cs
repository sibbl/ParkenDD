using System;
using System.Threading.Tasks;
using Windows.UI.StartScreen;
using ParkenDD.Api.Models;

namespace ParkenDD.Services
{
    public class JumpListService
    {
        private const string ArgumentFormat = "city={0}";

        public async void UpdateCityList(MetaData metaData)
        {
            await UpdateCityListAsync(metaData);
        }
        public async Task UpdateCityListAsync(MetaData metaData)
        {
            if (JumpList.IsSupported())
            {
                var jl = await JumpList.LoadCurrentAsync();
                jl.SystemGroupKind = JumpListSystemGroupKind.None;

                while (jl.Items.Count > 0)
                {
                    jl.Items.RemoveAt(0);
                }

                foreach (var city in metaData.Cities)
                {
                    var item = JumpListItem.CreateWithArguments(string.Format(ArgumentFormat, city.Id), city.Name);
                    item.GroupName = "Städte";  //localize...
                    item.Logo = new Uri("ms-appx:///Assets/ParkingIcon.png");
                    jl.Items.Add(item);
                }
                await jl.SaveAsync();
            }
        }
    }
}
