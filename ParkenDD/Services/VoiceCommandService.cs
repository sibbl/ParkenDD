using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.ApplicationInsights;
using ParkenDD.Api.Models;
using ParkenDD.ViewModels;

namespace ParkenDD.Services
{
    public class VoiceCommandService
    {
        private const string VoiceCommandPath = "VoiceCommands.xml";
        public VoiceCommandService()
        {
            Init();
        }
        public async void Init() {
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
                new TelemetryClient().TrackException(e, new Dictionary<string,string> {{"name", "voiceCommandInit"}, {"handled", "true"}});
            }
        }

        public async void UpdateCityList(MetaData metaData)
        {
            await UpdateCityListAsync(metaData);
        }

        public async Task UpdateCityListAsync(MetaData metaData)
        {
            if (metaData?.Cities != null)
            {
                //TODO: catch exceptions and log them
                Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinition commandSet;

                if (
                    Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.InstalledCommandDefinitions
                        .TryGetValue("ParkenDdCommands_de", out commandSet))
                {
                    await commandSet.SetPhraseListAsync("city", metaData.Cities.Select(x => x.Value.Name));
                }
            }
        }

        public async void UpdateParkingLotList(City city)
        {
            await UpdateParkingLotListAsync(city);
        }

        public async Task UpdateParkingLotListAsync(City city)
        {
            if (city?.Lots != null)
            {
                //TODO: catch exceptions and log them
                Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinition commandSet;

                if (
                    Windows.ApplicationModel.VoiceCommands.VoiceCommandDefinitionManager.InstalledCommandDefinitions
                        .TryGetValue("ParkenDdCommands_de", out commandSet))
                {
                    await commandSet.SetPhraseListAsync("parking_lot", city.Lots.Select(x => x.Name));
                }
            }
        }

        public async void HandleActivation(VoiceCommandActivatedEventArgs args)
        {
            var speechRecognitionResult = args.Result;

            var voiceCommandName = speechRecognitionResult.RulePath[0];

            //TODO: check encoding of properties. Umlauts are currently not supported.
            if (voiceCommandName == "selectCity")
            {
                var cityName = speechRecognitionResult.SemanticInterpretation.Properties["city"][0];
                await SimpleIoc.Default.GetInstance<MainViewModel>().TrySelectCityByName(cityName);
            }
            else if(voiceCommandName == "selectParkingLot")
            {
                var cityName = speechRecognitionResult.SemanticInterpretation.Properties["city"][0];
                var parkingLotName = speechRecognitionResult.SemanticInterpretation.Properties["parking_lot"][0];
                await SimpleIoc.Default.GetInstance<MainViewModel>().TrySelectParkingLotByName(cityName, parkingLotName);
            }
        }
    }
}
