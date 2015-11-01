using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Globalization;
using Microsoft.Practices.ServiceLocation;
using ParkenDD.Api.Models;
using ParkenDD.Background.Models;
using ParkenDD.Utils;
using ParkenDD.ViewModels;

namespace ParkenDD.Services
{
    public class VoiceCommandService
    {
        private const string VoiceCommandPath = "VoiceCommands.xml";
        private const string VoiceCommandSetNameFormat = "ParkenDdCommands_{0}";

        private readonly TrackingService _tracking;
        private readonly StorageService _storage;
        private Task<VoiceCommandPhrases> _loadPhrasesTask;
        private VoiceCommandPhrases _phrases;

        public VoiceCommandService(TrackingService tracking, StorageService storage)
        {
            _tracking = tracking;
            _storage = storage;
            InstallVoiceCommandDefinitions();
            LoadPhrases();
        }

        private async void LoadPhrases()
        {
            _loadPhrasesTask = _storage.ReadVoiceCommandPhrasesAsync();
            _phrases = await _loadPhrasesTask;
        }

        private async void InstallVoiceCommandDefinitions() {
            //TODO: catch exceptions and log them
            try
            {
                var storageFile =
                    await
                        Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(
                            new Uri("ms-appx:///" + VoiceCommandPath));
                await
                    Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager
                        .InstallCommandDefinitionsFromStorageFileAsync(storageFile);
            }
            catch (Exception e)
            {
               _tracking.TrackException(e, new Dictionary<string,string> {{"name", "voiceCommandInit"}, {"handled", "true"}});
            }
        }

        private VoiceCommandDefinition GetCurrentCommandSet()
        {
            VoiceCommandDefinition commandSet;
            var languages = ApplicationLanguages.Languages;
            var lang = languages.FirstOrDefault();
            if (string.IsNullOrEmpty(lang))
            {
                lang = "en"; //fallback to english as default
            }
            if (VoiceCommandDefinitionManager.InstalledCommandDefinitions.TryGetValue(string.Format(VoiceCommandSetNameFormat, lang), out commandSet))
            {
                return commandSet;
            }
            var langParts = lang.Split('-');
            if (langParts.Length > 1)
            {
                if (VoiceCommandDefinitionManager.InstalledCommandDefinitions.TryGetValue(
                        string.Format(VoiceCommandSetNameFormat, langParts[0]), out commandSet))
                {
                    return commandSet;
                }
            }
            return null;
        }

        public async void UpdateCityList(IEnumerable<MetaDataCityRow> metaData)
        {
            await UpdateCityListAsync(metaData);
        }

        public async Task UpdateCityListAsync(IEnumerable<MetaDataCityRow> metaData)
        {
            if (_phrases == null)
            {
                _phrases = await _loadPhrasesTask;
            }
            if (metaData != null)
            {
                try
                {
                    var commandSet = GetCurrentCommandSet();
                    if (commandSet != null)
                    {
                        _phrases.UpdateCities(metaData);
                        await commandSet.SetPhraseListAsync("city", _phrases.GetCityPhraseList());
                        _storage.SaveVoiceCommandPhrases(_phrases);
                    }
                }
                catch (Exception e)
                {
                    _tracking.TrackException(e, new Dictionary<string, string>
                    {
                        {"type", "update_city_list"},
                        {"handled", "true"}
                    });
                }
            }
        }

        public async void UpdateParkingLotList(MetaDataCityRow city, City data)
        {
            await UpdateParkingLotListAsync(city, data);
        }

        public async Task UpdateParkingLotListAsync(MetaDataCityRow city, City data)
        {
            if (_phrases == null)
            {
                _phrases = await _loadPhrasesTask;
            }
            if (data?.Lots != null)
            {
                try
                {
                    var commandSet = GetCurrentCommandSet();
                    if (commandSet != null)
                    {
                        _phrases.UpdateParkingLots(city, data.Lots);
                        await commandSet.SetPhraseListAsync("parking_lot", _phrases.GetParkingLotPhraseList());
                        _storage.SaveVoiceCommandPhrases(_phrases);
                    }
                }
                catch (Exception e)
                {
                    _tracking.TrackException(e, new Dictionary<string, string>
                    {
                        {"type", "update_parkinglot_list"},
                        {"handled", "true"}
                    });
                }
            }
        }

        public async void HandleActivation(VoiceCommandActivatedEventArgs args)
        {
            var speechRecognitionResult = args.Result;

            var voiceCommandName = speechRecognitionResult.RulePath[0];

            _tracking.TrackVoiceCommandEvent(voiceCommandName);
            
            if (voiceCommandName == "SelectCity")
            {
                var cityName = speechRecognitionResult.SemanticInterpretation.Properties["city"][0];
                var cityId = _phrases.FindCityIdByName(cityName);
                if (cityId != null)
                {
                    await ServiceLocator.Current.GetInstance<MainViewModel>().TrySelectCityById(cityId);
                }
            }
            else if(voiceCommandName == "SelectParkingLot")
            {
                var cityName = speechRecognitionResult.SemanticInterpretation.Properties["city"][0];
                var cityId = _phrases.FindCityIdByName(cityName);
                if (cityId != null)
                {
                    var parkingLotName = speechRecognitionResult.SemanticInterpretation.Properties["parking_lot"][0];
                    var parkingLotId = _phrases.FindParkingLotIdByNameAndCityId(cityId, parkingLotName);
                    await ServiceLocator.Current.GetInstance<MainViewModel>().TrySelectParkingLotById(cityId, parkingLotId);
                }
            }
        }
    }
}
