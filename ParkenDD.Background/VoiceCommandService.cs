using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;
using Newtonsoft.Json;
using ParkenDD.Api;
using ParkenDD.Api.Models;
using ParkenDD.Background.Models;

namespace ParkenDD.Background
{
    public sealed class VoiceCommandService : IBackgroundTask
    {
        private const string VoiceCommandPhrasesFilename = "phrases.json";
        private readonly StorageFolder _tempFolder = ApplicationData.Current.TemporaryFolder;

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

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var serviceDeferral = taskInstance.GetDeferral();
            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            var voiceServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);
            var voiceCommand = await voiceServiceConnection.GetVoiceCommandAsync();

            var res = ResourceLoader.GetForViewIndependentUse("Resources");

            switch (voiceCommand.CommandName)
            {
                case "GetParkingLotData":
                    var waitMsg = new VoiceCommandUserMessage
                    {
                        DisplayMessage = res.GetString("VoiceCommandParkingStateWaitDisplayMsg"),
                        SpokenMessage = res.GetString("VoiceCommandParkingStateWaitSpokenMsg")
                    };
                    var waitResponse = VoiceCommandResponse.CreateResponse(waitMsg);
                    await voiceServiceConnection.ReportProgressAsync(waitResponse);

                    var phrases = await ReadAsync<VoiceCommandPhrases>(VoiceCommandPhrasesFilename);
                    if (phrases != null)
                    {
                        var cityName = voiceCommand.Properties["city"][0];
                        var parkingLotName = voiceCommand.Properties["parking_lot"][0];

                        ParkingLot lot = null;
                        var now = DateTime.Now;
                        var lastUpdated = now;

                        var cityId = phrases.FindCityIdByName(cityName);
                        if (cityId != null)
                        {
                            var parkingLotId = phrases.FindParkingLotIdByNameAndCityId(cityId, parkingLotName);
                            if (parkingLotId != null)
                            {
                                var api = new ParkenDdClient();

                                var city = await api.GetCityAsync(cityId);
                                lastUpdated = city.LastUpdated;
                                lot = city?.Lots?.FirstOrDefault(x => x.Id.Equals(parkingLotId));
                            }
                        }
                        if (lot == null)
                        {
                            var errorMsg = new VoiceCommandUserMessage
                            {
                                DisplayMessage = res.GetString("VoiceCommandParkingStateParkingLotNotFoundDisplayMsg"),
                                SpokenMessage = res.GetString("VoiceCommandParkingStateParkingLotNotFoundSpokenMsg")
                            };
                            var errorResp = VoiceCommandResponse.CreateResponseForPrompt(errorMsg, errorMsg);
                            await voiceServiceConnection.ReportFailureAsync(errorResp);
                        }
                        else
                        {
                            var percent = Math.Round((double)lot.FreeLots / (double)lot.TotalLots * 100);
                            var age = now - lastUpdated;
                            var ageNumber = 0;
                            string spokenMessageFormat, displayMessageFormat;
                            if (age <= TimeSpan.FromMinutes(5))
                            {
                                spokenMessageFormat = res.GetString("VoiceCommandParkingStateUpToDateSpokenMsg");
                                displayMessageFormat = res.GetString("VoiceCommandParkingStateUpToDateDisplayMsg");
                            }else if (age <= TimeSpan.FromHours(2))
                            {
                                spokenMessageFormat = res.GetString("VoiceCommandParkingStateLessThan2HrsSpokenMsg");
                                displayMessageFormat = res.GetString("VoiceCommandParkingStateLessThan2HrsDisplayMsg");
                                ageNumber = age.Minutes;
                            }else if (age <= TimeSpan.FromDays(2))
                            {
                                spokenMessageFormat = res.GetString("VoiceCommandParkingStateLessThan2DaysSpokenMsg");
                                displayMessageFormat = res.GetString("VoiceCommandParkingStateLessThan2DaysDisplayMsg");
                                ageNumber = (int)Math.Ceiling(age.TotalHours);
                            }
                            else
                            {
                                spokenMessageFormat = res.GetString("VoiceCommandParkingStateVeryOldSpokenMsg");
                                displayMessageFormat = res.GetString("VoiceCommandParkingStateVeryOldDisplayMsg");
                            }
                            var responseMsg = new VoiceCommandUserMessage
                            {
                                DisplayMessage =
                                    string.Format(
                                        displayMessageFormat,
                                        lot.Name, //0
                                        lot.FreeLots, //1
                                        lot.TotalLots, //2
                                        percent, //3
                                        ageNumber //4
                                    ),
                                SpokenMessage = string.Format(spokenMessageFormat,
                                    lot.FreeLots, //0
                                    lot.TotalLots, //1
                                    ageNumber //2
                                    )
                            };

                            var response = VoiceCommandResponse.CreateResponse(responseMsg);
                            await voiceServiceConnection.ReportSuccessAsync(response);
                        }
                    }
                    break;
            }

            serviceDeferral?.Complete();
        }
    }
}
