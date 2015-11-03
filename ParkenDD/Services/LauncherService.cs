using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Microsoft.Practices.ServiceLocation;
using ParkenDD.ViewModels;

namespace ParkenDD.Services
{
    public class LauncherService
    {
        public static void ParseArguments(string arguments)
        {
            if (string.IsNullOrEmpty(arguments))
            {
                return;
            }
            var decoder = new WwwFormUrlDecoder(arguments);

            var city = decoder.FirstOrDefault(x => x.Name.Equals("city"));
            var parkingLot = decoder.FirstOrDefault(x => x.Name.Equals("parkingLot"));
            if (city != null && parkingLot != null)
            {
                Task.Run(async () =>
                {
                    await ServiceLocator.Current.GetInstance<MainViewModel>().TrySelectParkingLotById(city.Value, parkingLot.Value);
                });
            }else if (city != null)
            {
                Task.Run(async () =>
                {
                    await ServiceLocator.Current.GetInstance<MainViewModel>().TrySelectCityById(city.Value);
                });
            }
        }
    }
}
