using System;
using System.Threading.Tasks;
using Windows.Storage;
using Cimbalino.Toolkit.Extensions;
using Newtonsoft.Json;
using ParkenDD.Api.Models;

namespace ParkenDD.Services
{
    public class StorageService
    {
        private const string MetaDataFilename = "meta.json";
        private const string SelectedCityFilename = "city_{0}.json";
        private readonly StorageFolder _tempFolder = ApplicationData.Current.TemporaryFolder;

        private async Task SaveAsync<T>(string filename, T data)
        {
            var file = await _tempFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(data));
        }

        private async Task<T> ReadAsync<T>(string filename)
        {
            try
            {
                var file = await _tempFolder.GetFileAsync(filename);
                return JsonConvert.DeserializeObject<T>(await FileIO.ReadTextAsync(file));
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public async void SaveMetaData(MetaData data)
        {
            await SaveMetaDataAsync(data);
        }

        public async Task SaveMetaDataAsync(MetaData data)
        {
            await SaveAsync(MetaDataFilename, data);
        }

        public async Task<MetaData> ReadMetaDataAsync()
        {
            return await ReadAsync<MetaData>(MetaDataFilename);
        }
        public async void SaveCityData(string cityId, City data)
        {
            await SaveCityDataAsync(cityId, data);
        }

        public async Task SaveCityDataAsync(string cityId, City data)
        {
            await SaveAsync(string.Format(SelectedCityFilename, cityId), data);
        }

        public async Task<City> ReadCityDataAsync(string cityId)
        {
            return await ReadAsync<City>(string.Format(SelectedCityFilename, cityId));
        }
    }
}
