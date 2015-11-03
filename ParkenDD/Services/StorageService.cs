using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Newtonsoft.Json;
using ParkenDD.Api.Models;
using ParkenDD.Background.Models;

namespace ParkenDD.Services
{
    public class StorageService
    {
        private const string MetaDataFilename = "meta.json";
        private const string SelectedCityFilename = "city_{0}.json";
        private const string VoiceCommandPhrasesFilename = "phrases.json";
        private readonly StorageFolder _tempFolder = ApplicationData.Current.TemporaryFolder;
        private readonly SemaphoreSlim _metaMutex = new SemaphoreSlim(1);
        private Dictionary<string, SemaphoreSlim> _cityMutexes = new Dictionary<string, SemaphoreSlim>();
        private readonly SemaphoreSlim _voiceCommandPhraseMutex = new SemaphoreSlim(1);

        private async Task SaveAsync<T>(string filename, T data, SemaphoreSlim mutex = null)
        {
            if (mutex != null)
            {
                await mutex.WaitAsync();
            }
            var file = await _tempFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            try
            {
                await FileIO.WriteTextAsync(file, JsonConvert.SerializeObject(data));
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                mutex?.Release();
            }
        }

        private async Task<T> ReadAsync<T>(string filename, SemaphoreSlim mutex = null)
        {
            if (mutex != null)
            {
                await mutex.WaitAsync();
            }
            try
            {
                var file = await _tempFolder.GetFileAsync(filename);
                return JsonConvert.DeserializeObject<T>(await FileIO.ReadTextAsync(file));
            }
            catch (Exception)
            {
                return default(T);
            }
            finally
            {
                mutex?.Release();
            }
        }

        public async void SaveMetaData(MetaData data)
        {
            await SaveMetaDataAsync(data);
        }

        public async Task SaveMetaDataAsync(MetaData data)
        {
            await SaveAsync(MetaDataFilename, data, _metaMutex);
        }

        public async Task<MetaData> ReadMetaDataAsync()
        {
            return await ReadAsync<MetaData>(MetaDataFilename, _metaMutex);
        }

        public async void SaveCityData(string cityId, City data)
        {
            await SaveCityDataAsync(cityId, data);
        }

        public async Task SaveCityDataAsync(string cityId, City data)
        {
            var mutex = _cityMutexes.ContainsKey(cityId) ? _cityMutexes[cityId] : new SemaphoreSlim(1);
            await SaveAsync(string.Format(SelectedCityFilename, cityId), data, mutex);
        }

        public async Task<City> ReadCityDataAsync(string cityId)
        {
            var mutex = _cityMutexes.ContainsKey(cityId) ? _cityMutexes[cityId] : new SemaphoreSlim(1);
            return await ReadAsync<City>(string.Format(SelectedCityFilename, cityId), mutex);
        }

        public async void SaveVoiceCommandPhrases(VoiceCommandPhrases data)
        {
            await SaveVoiceCommandPhrasesAsync(data);
        }

        public async Task SaveVoiceCommandPhrasesAsync(VoiceCommandPhrases data)
        {
            await SaveAsync(VoiceCommandPhrasesFilename, data, _voiceCommandPhraseMutex);
        }

        public async Task<VoiceCommandPhrases> ReadVoiceCommandPhrasesAsync()
        {
            var dict = await ReadAsync<VoiceCommandPhrases>(VoiceCommandPhrasesFilename, _voiceCommandPhraseMutex);
            return dict ?? new VoiceCommandPhrases();
        }
    }
}
