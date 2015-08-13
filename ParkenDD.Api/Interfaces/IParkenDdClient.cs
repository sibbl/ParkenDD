using System.Threading;
using System.Threading.Tasks;
using ParkenDD.Api.Models;

namespace ParkenDD.Api.Interfaces
{
    public interface IParkenDdClient
    {
        /// <summary>
        ///     Get meta data from server
        /// </summary>
        /// <param name="ct">cancellation token</param>
        /// <returns>parsed server response</returns>
        Task<MetaData> GetMetaDataAsync(CancellationToken? ct = null);

        /// <summary>
        ///     Get city details from server
        /// </summary>
        /// <param name="cityId">ID of city</param>
        /// <param name="ct">cancellation token</param>
        /// <returns>parsed server response</returns>
        Task<City> GetCityAsync(string cityId, CancellationToken? ct = null);
    }
}
