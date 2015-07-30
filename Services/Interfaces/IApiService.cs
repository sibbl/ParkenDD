using System.Threading;
using System.Threading.Tasks;
using ParkenDD.Win10.Models;

namespace ParkenDD.Win10.Services.Interfaces
{
    public interface IApiService
    {
        /// <summary>
        ///     Get meta data from server
        /// </summary>
        /// <param name="ct">cancellation token</param>
        /// <returns>parsed server response</returns>
        Task<MetaData> GetMeta(CancellationToken? ct = null);

        /// <summary>
        ///     Get city details from server
        /// </summary>
        /// <param name="cityId">ID of city</param>
        /// <param name="ct">cancellation token</param>
        /// <returns>parsed server response</returns>
        Task<City> GetCity(string cityId, CancellationToken? ct = null);
    }
}
