using System;
using System.Collections.Generic;
using Windows.UI.Notifications;
using NotificationsExtensions.Toasts;
using ParkenDD.Api.Models;
using ParkenDD.Api.Models.Exceptions;

namespace ParkenDD.Services
{
    public class ExceptionService
    {
        private const string ContactEmail = "me@sibbl.net";
        private const string EmailFormat = "mailto:{0}?subject={1}&body={2}";
        private readonly TrackingService _tracking;
        private readonly ResourceService _res;
        private DateTime? _lastException;

        public ExceptionService(TrackingService tracking, ResourceService res)
        {
            _tracking = tracking;
            _res = res;
        }

        private void ShowToast(ToastContent content)
        {
            var now = DateTime.Now;
            //show only one exception every 5 sec (minimum value for toast notifications to be visible)
            if (_lastException == null || now - _lastException.Value > TimeSpan.FromSeconds(5))
            {
                _lastException = DateTime.Now;
                var notifier = ToastNotificationManager.CreateToastNotifier();
                var notification = new ToastNotification(content.GetXml());
                notifier.Show(notification);
            }
        }

        public void HandleApiExceptionForMetaData(ApiException e)
        {
            _tracking.TrackException(e, new Dictionary<string, string>()
            {
                { "handled", "true"},
                { "type", "metadata"},
            });
            var mailStr = string.Format(EmailFormat,
                ContactEmail,
                Uri.EscapeDataString(_res.ExceptionMailMetaDataSubject),
                Uri.EscapeDataString(string.Format(_res.ExceptionMailMetaDataBody, e.Message))
                );
            var content = new ToastContent
            {
                Launch = "handledMetaDataApiException",
                Visual = new ToastVisual
                {
                    TitleText = new ToastText
                    {
                        Text = _res.ExceptionToastTitle
                    },
                    BodyTextLine1 = new ToastText
                    {
                        Text = _res.ExceptionToastMetaDataContent
                    }
                },
                Actions = new ToastActionsCustom
                {
                    Buttons =
                    {
                        new ToastButton(_res.ExceptionToastVisitParkenDdButton, "http://www.parkendd.de")
                        {
                            ActivationType = ToastActivationType.Protocol,
                        },
                        new ToastButton(_res.ExceptionToastContactDevButton, mailStr)
                        {
                            ActivationType = ToastActivationType.Protocol,
                        }
                    }
                }
            };
            ShowToast(content);
        }
        public void HandleApiExceptionForCityData(ApiException e, MetaDataCityRow city)
        {
            _tracking.TrackException(e, new Dictionary<string, string>()
            {
                { "handled", "true"},
                { "type", "city"},
                { "city", city?.Id }
            });
            var mailStr = string.Format(EmailFormat,
                ContactEmail,
                Uri.EscapeDataString(string.Format(_res.ExceptionMailCitySubject, city?.Name)),
                Uri.EscapeDataString(string.Format(_res.ExceptionMailCityBody, city?.Name, e.Message))
                );
            var content = new ToastContent
            {
                Launch = "handledCityDataApiException",
                Visual = new ToastVisual
                {
                    TitleText = new ToastText
                    {
                        Text = _res.ExceptionToastTitle
                    },

                    BodyTextLine1 = new ToastText
                    {
                        Text = string.Format(_res.ExceptionToastCityContent, city?.Name)
                    }
                },
                Actions = new ToastActionsCustom
                {
                    Buttons =
                    {
                        new ToastButton(_res.ExceptionToastShowInBrowserButton, city?.Url.ToString())
                        {
                            ActivationType = ToastActivationType.Protocol,
                        },
                        new ToastButton(_res.ExceptionToastContactDevButton, mailStr)
                        {
                            ActivationType = ToastActivationType.Protocol,
                        }
                    }
                }
            };
            ShowToast(content);
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
            var mailStr = string.Format(EmailFormat,
                ContactEmail,
                Uri.EscapeDataString(string.Format(_res.ExceptionMailForecastSubject, lot?.Name, city?.Name)),
                Uri.EscapeDataString(string.Format(_res.ExceptionMailForecastBody, lot?.Name, city?.Name, e.Message))
                );
            var content = new ToastContent
            {
                Launch = "handledForecastApiException",
                Visual = new ToastVisual
                {
                    TitleText = new ToastText
                    {
                        Text = _res.ExceptionToastTitle
                    },

                    BodyTextLine1 = new ToastText
                    {
                        Text = string.Format(_res.ExceptionToastForecastContent, lot?.Name, city?.Name)
                    }
                },
                Actions = new ToastActionsCustom
                {
                    Buttons =
                    {
                        new ToastButton(_res.ExceptionToastShowInBrowserButton, city?.Url.ToString())
                        {
                            ActivationType = ToastActivationType.Protocol,
                        },
                        new ToastButton(_res.ExceptionToastContactDevButton, mailStr)
                        {
                            ActivationType = ToastActivationType.Protocol,
                        }
                    }
                }
            };
            ShowToast(content);
        }

        public void HandleException(Exception e, string type = "unknown")
        {
            _tracking.TrackException(e, new Dictionary<string, string>()
            {
                { "handled", "true"},
                { "type", type},
            });
        }

        public void OnUnhandledException(Exception e, ref bool handled)
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
