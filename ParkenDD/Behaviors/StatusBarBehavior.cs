using System;
using Windows.UI;
using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace ParkenDD.Behaviors
{
    public class StatusBarBehavior : DependencyObject, IBehavior
    {
        public void Attach(DependencyObject associatedObject)
        {
        }
        public void Detach()
        {
        }
        public DependencyObject AssociatedObject { get; }

        #region BackgroundColor

        public Color BackgroundColor
        {
            get { return (Color)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty BackgroundColorProperty =
            DependencyProperty.Register("BackgroundColor",
                typeof(Color),
                typeof(StatusBarBehavior),
                new PropertyMetadata(null, OnBackgroundColorChanged));

        private static void OnBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                Windows.UI.ViewManagement.StatusBar.GetForCurrentView().BackgroundColor = (Color) e.NewValue;
            }
        }
        #endregion

        #region ForegroundColor

        public Color ForegroundColor
        {
            get { return (Color)GetValue(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }

        public static readonly DependencyProperty ForegroundColorProperty =
            DependencyProperty.Register("ForegroundColor",
                typeof(Color),
                typeof(StatusBarBehavior),
                new PropertyMetadata(null, OnForegroundColorChanged));

        private static void OnForegroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ForegroundColor = (Color) e.NewValue;
            }
        }
        #endregion

        #region BackgroundOpacity

        public double BackgroundOpacity
        {
            get { return (double) GetValue(BackgroundOpacityProperty); }
            set { SetValue(BackgroundOpacityProperty, value); }
        }

        public static readonly DependencyProperty BackgroundOpacityProperty =
            DependencyProperty.Register("BackgroundOpacity",
                typeof (double),
                typeof (StatusBarBehavior),
                new PropertyMetadata(null, OnBackgroundOpacityChanged));

        private static void OnBackgroundOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                Windows.UI.ViewManagement.StatusBar.GetForCurrentView().BackgroundOpacity = (double) e.NewValue;
            }
        }
        #endregion

        #region ProgressIndicatorVisible

        public bool ProgressIndicatorVisible
        {
            get { return (bool) GetValue(ProgressIndicatorVisibleProperty); }
            set { SetValue(ProgressIndicatorVisibleProperty, value); }
        }

        public static readonly DependencyProperty ProgressIndicatorVisibleProperty =
            DependencyProperty.Register("ProgressIndicatorVisible",
                typeof (bool),
                typeof (StatusBarBehavior),
                new PropertyMetadata(null, OnProgressIndicatorVisibleChanged));

        private static async void OnProgressIndicatorVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                if ((bool) e.NewValue)
                {
                    await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync();
                }
                else
                {
                    await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator.HideAsync();
                }
            }
        }
        #endregion

        #region ProgressIndicatorText

        public string ProgressIndicatorText
        {
            get { return (string) GetValue(ProgressIndicatorTextProperty); }
            set { SetValue(ProgressIndicatorTextProperty, value); }
        }

        public static readonly DependencyProperty ProgressIndicatorTextProperty =
            DependencyProperty.Register("ProgressIndicatorText",
                typeof (string),
                typeof (StatusBarBehavior),
                new PropertyMetadata(null, OnProgressIndicatorTextChanged));

        private static async void OnProgressIndicatorTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator.Text = (string) e.NewValue;
                await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync();
            }
        }
        #endregion

        #region ProgressIndicatorValue

        public double ProgressIndicatorValue
        {
            get { return (double) GetValue(ProgressIndicatorValueProperty); }
            set { SetValue(ProgressIndicatorValueProperty, value); }
        }

        public static readonly DependencyProperty ProgressIndicatorValueProperty =
            DependencyProperty.Register("ProgressIndicatorValue",
                typeof (double),
                typeof (StatusBarBehavior),
                new PropertyMetadata(null, OnProgressIndicatorValueChanged));

        private static async void OnProgressIndicatorValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator.ProgressValue =
                    (double) e.NewValue;
                await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator.ShowAsync();
            }
        }
        #endregion
    }
}
