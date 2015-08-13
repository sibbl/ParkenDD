using System;
using System.Linq;
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

            // get the voiceCommandServiceConnection from the tigger details
            var voiceServiceConnection = VoiceCommandServiceConnection.FromAppServiceTriggerDetails(triggerDetails);
            var voiceCommand = await voiceServiceConnection.GetVoiceCommandAsync();

            // switch statement to handle different commands
            switch (voiceCommand.CommandName)
            {
                case "getParkingLotData":
                    //TODO: localization
                    //TODO: load the requested parking lot
                    var api = new ParkenDdClient();
                    var waitMsg = new VoiceCommandUserMessage
                    {
                        DisplayMessage = "Moment, ich fahre kurz vorbei...",
                        SpokenMessage = "Einen Moment, ich fahre kurz vorbei..."
                    };
                    var waitResponse = VoiceCommandResponse.CreateResponse(waitMsg);
                    await voiceServiceConnection.ReportProgressAsync(waitResponse);
                    var city = await api.GetCityAsync("Dresden");
                    var lot = city.Lots.FirstOrDefault(x => x.Id.Equals("dresdentaschenbergpalais"));
                    // get the message the user has spoken
                    //var message = voiceCommand.Properties["message"][0];

                    var percent = Math.Round((double) lot.FreeLots/(double) lot.TotalLots);
                    // create response messages for Cortana to respond
                    var responseMsg = new VoiceCommandUserMessage
                    {
                        DisplayMessage = string.Format("Im Parkplatz {0} sind noch {1} von {2}  Parkplätzen frei ({3}%)", 
                            lot.Name,
                            lot.FreeLots,
                            lot.TotalLots,
                            percent),
                        SpokenMessage = string.Format("Im Parkplatz {0} sind noch {1} von {2} Parkplätzen frei",
                            lot.Name,
                            lot.FreeLots,
                            lot.TotalLots)
                    };

                    // create a response and ask Cortana to respond with success
                    var response = VoiceCommandResponse.CreateResponse(responseMsg);
                    await voiceServiceConnection.ReportSuccessAsync(response);

                    break;
            }

            //Complete the service deferral
            serviceDeferral?.Complete();
        }
    }
}
