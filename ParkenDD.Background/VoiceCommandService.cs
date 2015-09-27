using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
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

            switch (voiceCommand.CommandName)
            {
                case "getParkingLotData":
                    var phrases = await ReadAsync<VoiceCommandPhrases>(VoiceCommandPhrasesFilename);
                    if (phrases != null)
                    {
                        var cityName = voiceCommand.Properties["city"][0];
                        var parkingLotName = voiceCommand.Properties["parking_lot"][0];

                        ParkingLot lot = null;

                        var cityId = phrases.FindCityIdByName(cityName);
                        if (cityId != null)
                        {
                            var parkingLotId = phrases.FindParkingLotIdByNameAndCityId(cityId, parkingLotName);
                            if (parkingLotId != null)
                            {
                                var api = new ParkenDdClient();
                                //TODO: localize
                                var waitMsg = new VoiceCommandUserMessage
                                {
                                    DisplayMessage = "Moment, ich fahre kurz vorbei...",
                                    SpokenMessage = "Einen Moment, ich fahre kurz vorbei..."
                                };
                                var waitResponse = VoiceCommandResponse.CreateResponse(waitMsg);
                                await voiceServiceConnection.ReportProgressAsync(waitResponse);

                                var city = await api.GetCityAsync(cityId);
                                lot = city?.Lots?.FirstOrDefault(x => x.Id.Equals(parkingLotId));
                            }
                        }
                        if (lot == null)
                        {
                            //TODO: localize
                            var errorMsg = new VoiceCommandUserMessage
                            {
                                DisplayMessage = "Parkplatz nicht gefunden...",
                                SpokenMessage = "Der Parkplatz wurde leider nicht gefunden..."
                            };
                            var errorResp = VoiceCommandResponse.CreateResponseForPrompt(errorMsg, errorMsg);
                            await voiceServiceConnection.RequestConfirmationAsync(errorResp);
                        }
                        else
                        {
                            var percent = Math.Round((double)lot.FreeLots / (double)lot.TotalLots * 100);
                            var responseMsg = new VoiceCommandUserMessage
                            {
                                //TODO: localize
                                DisplayMessage =
                                    string.Format(
                                        "Der Parkplatz {0} hat noch {1} von {2} Parkplätzen frei ({3}%)",
                                        lot.Name,
                                        lot.FreeLots,
                                        lot.TotalLots,
                                        percent),
                                SpokenMessage = string.Format("{0} von {1} Parkplätzen sind aktuell frei",
                                    lot.FreeLots,
                                    lot.TotalLots)
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
