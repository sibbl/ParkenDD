using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using NotificationsExtensions.Toasts;
using ParkenDD.Api.Models;
using ParkenDD.Api.Models.Exceptions;

namespace ParkenDD.Services
{
    public class ExceptionService
    {
        private readonly TrackingService _tracking;

        public ExceptionService(TrackingService tracking)
        {
            _tracking = tracking;
        }
        public void HandleApiExceptionForMetaData(ApiException e)
        {
            _tracking.TrackException(e, new Dictionary<string, string>()
            {
                { "handled", "true"},
                { "type", "metadata"},
            });
            //TODO: localization
            var mailSubject = "ParkenDD: cities could not be loaded";
            var mailBody = "Unfortunately I couldn't load cities and I though I might tell you about this.";
            var mailAdress = "me@sibbl.net";
            var mailStr = String.Format("mailto:{0}?body={1}&subject={2}",
                mailAdress,
                Uri.EscapeDataString(mailBody),
                Uri.EscapeDataString(mailSubject)
                );
            var content = new ToastContent
            {
                Launch = "handledMetaDataApiException",
                Visual = new ToastVisual()
                {
                    TitleText = new ToastText
                    {
                        Text = "Oh no!"
                    },

                    BodyTextLine1 = new ToastText
                    {
                        Text = "Unfortunately the server is not reachable and the cities cannot be loaded."
                    }
                },
                Actions = new ToastActionsCustom
                {
                    Buttons =
                    {
                        new ToastButton("visit parkendd.de", "http://www.parkendd.de")
                        {
                            ActivationType = ToastActivationType.Protocol,
                        },
                        new ToastButton("contact developer", mailStr)
                        {
                            ActivationType = ToastActivationType.Protocol,
                        }
                    }
                }
            };
            var notifier = ToastNotificationManager.CreateToastNotifier();
            var notification = new ToastNotification(content.GetXml());
            notifier.Show(notification);
        }
        public void HandleApiExceptionForCityData(ApiException e, MetaDataCityRow city)
        {
            _tracking.TrackException(e, new Dictionary<string, string>()
            {
                { "handled", "true"},
                { "type", "city"},
                { "city", city?.Id }
            });
            //TODO: localization
            var mailSubject = "ParkenDD: {0} could not be loaded";
            var mailBody = "Unfortunately I couldn't load the data of {0} and I though I might tell you about this.";
            var mailAdress = "me@sibbl.net";
            var mailStr = String.Format("mailto:{0}?body={1}&subject={2}",
                mailAdress,
                Uri.EscapeDataString(String.Format(mailBody, city?.Name)),
                Uri.EscapeDataString(String.Format(mailSubject, city?.Name))
                );
            var content = new ToastContent
            {
                Launch = "handledCityDataApiException",
                Visual = new ToastVisual()
                {
                    TitleText = new ToastText
                    {
                        Text = "Oh no!"
                    },

                    BodyTextLine1 = new ToastText
                    {
                        Text = "Unfortunately the server is not reachable and real time parking lot data cannot be loaded."
                    }
                },
                Actions = new ToastActionsCustom
                {
                    Buttons =
                    {
                        new ToastButton("view in browser", city?.Url.ToString())
                        {
                            ActivationType = ToastActivationType.Protocol,
                        },
                        new ToastButton("contact developer", mailStr)
                        {
                            ActivationType = ToastActivationType.Protocol,
                        }
                    }
                }
            };
            var notifier = ToastNotificationManager.CreateToastNotifier();
            var notification = new ToastNotification(content.GetXml());
            notifier.Show(notification);
        }
        public void HandleApiExceptionForForecastData(ApiException e, MetaDataCityRow city, ParkingLot lot)
        {
            _tracking.TrackException(e, new Dictionary<string, string>()
            {
                { "handled", "true"},
                { "type", "forecast"},
                { "city", city?.Id },
                { "lot", lot?.Id }
            });
            //TODO: localization
            var mailSubject = "ParkenDD: forecast data of {0} in {1} could not be loaded";
            var mailBody = "Unfortunately I couldn't load the forecast for {0} in {1} and I though I might tell you about this.";
            var mailAdress = "me@sibbl.net";
            var mailStr = String.Format("mailto:{0}?body={1}&subject={2}",
                mailAdress,
                Uri.EscapeDataString(String.Format(mailBody, lot?.Name, city?.Name)),
                Uri.EscapeDataString(String.Format(mailSubject, lot?.Name, city?.Name))
                );
            var content = new ToastContent
            {
                Launch = "handledForecastApiException",
                Visual = new ToastVisual()
                {
                    TitleText = new ToastText
                    {
                        Text = "Oh no!"
                    },

                    BodyTextLine1 = new ToastText
                    {
                        Text = "Unfortunately the server is not reachable and forecast data cannot be loaded."
                    }
                },
                Actions = new ToastActionsCustom
                {
                    Buttons =
                    {
                        new ToastButton("view in browser", city?.Url.ToString())
                        {
                            ActivationType = ToastActivationType.Protocol,
                        },
                        new ToastButton("contact developer", mailStr)
                        {
                            ActivationType = ToastActivationType.Protocol,
                        }
                    }
                }
            };
            var notifier = ToastNotificationManager.CreateToastNotifier();
            var notification = new ToastNotification(content.GetXml());
            notifier.Show(notification);
        }

        public void HandleException(Exception e, ref bool handled)
        {
            _tracking.TrackException(e, new Dictionary<string, string>()
            {
                { "handled", "false"},
                { "type", "forecast"},
            });
            //TODO: maybe catch and handle more unhandled exceptions here
            //handled = true;
        }
    }
}
