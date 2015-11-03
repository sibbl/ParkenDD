using System;
using System.Linq;
using Windows.UI.Xaml.Controls;
using GalaSoft.MvvmLight.Threading;

namespace ParkenDD.Services
{
    public class ReviewAppService
    {
        private readonly SettingsService _settings;
        private readonly ResourceService _res;
        private readonly long[] _appRatingsThresholds = {3, 15, 25, 50};
        public ReviewAppService(SettingsService settings, ResourceService res)
        {
            _settings = settings;
            _res = res;
        }
        public void AppLaunched()
        {
            _settings.AppLaunchCount++;
            if (!_settings.UserReviewedApp && _appRatingsThresholds.Contains(_settings.AppLaunchCount))
            {
                DispatcherHelper.CheckBeginInvokeOnUI(async () =>
                {
                    var contentDialog1 = new ContentDialog
                    {
                        Title = _res.ReviewAppDialog1Title,
                        PrimaryButtonText = _res.ReviewAppDialog1YesButton,
                        SecondaryButtonText = _res.ReviewAppDialog1NoButton,
                        Content = _res.ReviewAppDialog1Content
                    };
                    if (await contentDialog1.ShowAsync() == ContentDialogResult.Primary)
                    {
                        await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9nblggh1p0sr"));
                        _settings.UserReviewedApp = true;
                    }
                    else
                    {
                        var contentDialog2 = new ContentDialog
                        {
                            Title = _res.ReviewAppDialog2Title,
                            PrimaryButtonText = _res.ReviewAppDialog2FeedbackButton,
                            SecondaryButtonText = _res.ReviewAppDialog2NoButton,
                            Content = _res.ReviewAppDialog2Content,
                        };
                        if (await contentDialog2.ShowAsync() == ContentDialogResult.Primary)
                        {
                            var subject = _res.ReviewAppFeedbackMailTitle;
                            var body = _res.ReviewAppFeedbackMailBody;
                            await Windows.System.Launcher.LaunchUriAsync(new Uri(
                                string.Format("mailto:me@sibbl.net?subject={0}&body={1}", subject, body)
                            ));
                        }
                    }
                });
            }
        }
    }
}
