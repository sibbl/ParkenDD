using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.VoiceCommands;
using ParkenDD.Api;

namespace ParkenDD.Background
{
    public sealed class VoiceCommandService : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var serviceDeferral = taskInstance.GetDeferral();
            var triggerDetails = taskInstance.TriggerDetails as AppServiceTriggerDetails;

            var voiceServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);
            var voiceCommand = await voiceServiceConnection.GetVoiceCommandAsync();

            switch (voiceCommand.CommandName)
            {
                case "getParkingLotData":
                    var cityStr = voiceCommand.Properties["city"][0];
                    var lotName = voiceCommand.Properties["parking_lot"][0];

                    var api = new ParkenDdClient();
                    //TODO: localize
                    var waitMsg = new VoiceCommandUserMessage
                    {
                        DisplayMessage = "Moment, ich fahre kurz vorbei...",
                        SpokenMessage = "Einen Moment, ich fahre kurz vorbei..."
                    };
                    var waitResponse = VoiceCommandResponse.CreateResponse(waitMsg);
                    await voiceServiceConnection.ReportProgressAsync(waitResponse);
                    var getCityTask = api.GetCityAsync(cityStr);
                    var getMetaTask = api.GetMetaDataAsync();
                    await Task.WhenAll(getCityTask, getMetaTask);

                    var meta = getMetaTask.Result;
                    var city = getCityTask.Result;
                    var lot = city?.Lots?.FirstOrDefault(x => x.Name.ToLower().Equals(lotName.ToLower()));
                    if (lot == null)
                    {
                        var cityMeta = meta?.Cities?.FirstOrDefault(x => x.Value.Name.ToLower().Equals(cityStr.ToLower()));
                        if (cityMeta != null)
                        {
                            city = await api.GetCityAsync(cityMeta.Value.Value.Id);
                            lot = city?.Lots?.FirstOrDefault(x => x.Name.ToLower().Equals(lotName.ToLower()));
                        }
                    }
                    if (lot == null)
                    {
                        //TODO: localize
                        var errorMsg = new VoiceCommandUserMessage { 
                            DisplayMessage = "Parkplatz nicht gefunden...",
                            SpokenMessage = "Der Parkplatz wurde leider nicht gefunden..."
                        };
                        var errorResp = VoiceCommandResponse.CreateResponseForPrompt(errorMsg, errorMsg);
                        await voiceServiceConnection.RequestConfirmationAsync(errorResp);
                    }
                    else
                    {
                        var percent = Math.Round((double) lot.FreeLots/(double) lot.TotalLots * 100);
                        var responseMsg = new VoiceCommandUserMessage
                        {
                            //TODO: localize
                            DisplayMessage =
                                string.Format("Der Parkplatz {0} hat noch {1} von {2} Parkplätzen frei ({3}%)",
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
                    break;
            }

            serviceDeferral?.Complete();
        }
    }
}
