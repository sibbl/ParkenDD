using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace ParkenDD.Behaviors
{
    public class TitleBarBehavior : DependencyObject, IBehavior
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
                typeof(TitleBarBehavior),
                new PropertyMetadata(null, OnBackgroundColorChanged));

        private static void OnBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ApplicationView.GetForCurrentView().TitleBar.BackgroundColor = (Color)e.NewValue;
        }
        #endregion
        #region ButtonBackgroundColor
        public Color ButtonBackgroundColor
        {
            get { return (Color)GetValue(ButtonBackgroundColorProperty); }
            set { SetValue(ButtonBackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty ButtonBackgroundColorProperty =
            DependencyProperty.Register("ButtonBackgroundColor",
                typeof(Color),
                typeof(TitleBarBehavior),
                new PropertyMetadata(null, OnButtonBackgroundColorChanged));

        private static void OnButtonBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ApplicationView.GetForCurrentView().TitleBar.ButtonBackgroundColor = (Color)e.NewValue;
        }
        #endregion
        #region ButtonForegroundColor
        public Color ButtonForegroundColor
        {
            get { return (Color)GetValue(ButtonForegroundColorProperty); }
            set { SetValue(ButtonForegroundColorProperty, value); }
        }

        public static readonly DependencyProperty ButtonForegroundColorProperty =
            DependencyProperty.Register("ButtonForegroundColor",
                typeof(Color),
                typeof(TitleBarBehavior),
                new PropertyMetadata(null, OnButtonForegroundColorChanged));

        private static void OnButtonForegroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ApplicationView.GetForCurrentView().TitleBar.ButtonForegroundColor = (Color)e.NewValue;
        }
        #endregion
        #region ButtonHoverBackgroundColor
        public Color ButtonHoverBackgroundColor
        {
            get { return (Color)GetValue(ButtonHoverBackgroundColorProperty); }
            set { SetValue(ButtonHoverBackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty ButtonHoverBackgroundColorProperty =
            DependencyProperty.Register("ButtonHoverBackgroundColor",
                typeof(Color),
                typeof(TitleBarBehavior),
                new PropertyMetadata(null, OnButtonHoverBackgroundColorChanged));

        private static void OnButtonHoverBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ApplicationView.GetForCurrentView().TitleBar.ButtonHoverBackgroundColor = (Color)e.NewValue;
        }
        #endregion
        #region ButtonHoverForegroundColor
        public Color ButtonHoverForegroundColor
        {
            get { return (Color)GetValue(ButtonHoverForegroundColorProperty); }
            set { SetValue(ButtonHoverForegroundColorProperty, value); }
        }

        public static readonly DependencyProperty ButtonHoverForegroundColorProperty =
            DependencyProperty.Register("ButtonHoverForegroundColor",
                typeof(Color),
                typeof(TitleBarBehavior),
                new PropertyMetadata(null, OnButtonHoverForegroundColorChanged));

        private static void OnButtonHoverForegroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ApplicationView.GetForCurrentView().TitleBar.ButtonHoverForegroundColor = (Color)e.NewValue;
        }
        #endregion
        #region ButtonInactiveBackgroundColor
        public Color ButtonInactiveBackgroundColor
        {
            get { return (Color)GetValue(ButtonInactiveBackgroundColorProperty); }
            set { SetValue(ButtonInactiveBackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty ButtonInactiveBackgroundColorProperty =
            DependencyProperty.Register("ButtonInactiveBackgroundColor",
                typeof(Color),
                typeof(TitleBarBehavior),
                new PropertyMetadata(null, OnButtonInactiveBackgroundColorChanged));

        private static void OnButtonInactiveBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ApplicationView.GetForCurrentView().TitleBar.ButtonInactiveBackgroundColor = (Color)e.NewValue;
        }
        #endregion
        #region ButtonInactiveForegroundColor
        public Color ButtonInactiveForegroundColor
        {
            get { return (Color)GetValue(ButtonInactiveForegroundColorProperty); }
            set { SetValue(ButtonInactiveForegroundColorProperty, value); }
        }

        public static readonly DependencyProperty ButtonInactiveForegroundColorProperty =
            DependencyProperty.Register("ButtonInactiveForegroundColor",
                typeof(Color),
                typeof(TitleBarBehavior),
                new PropertyMetadata(null, OnButtonInactiveForegroundColorChanged));

        private static void OnButtonInactiveForegroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ApplicationView.GetForCurrentView().TitleBar.ButtonInactiveForegroundColor = (Color)e.NewValue;
        }
        #endregion
        #region ButtonPressedBackgroundColor
        public Color ButtonPressedBackgroundColor
        {
            get { return (Color)GetValue(ButtonPressedBackgroundColorProperty); }
            set { SetValue(ButtonPressedBackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty ButtonPressedBackgroundColorProperty =
            DependencyProperty.Register("ButtonPressedBackgroundColor",
                typeof(Color),
                typeof(TitleBarBehavior),
                new PropertyMetadata(null, OnButtonPressedBackgroundColorChanged));

        private static void OnButtonPressedBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ApplicationView.GetForCurrentView().TitleBar.ButtonPressedBackgroundColor = (Color)e.NewValue;
        }
        #endregion
        #region ButtonPressedForegroundColor
        public Color ButtonPressedForegroundColor
        {
            get { return (Color)GetValue(ButtonPressedForegroundColorProperty); }
            set { SetValue(ButtonPressedForegroundColorProperty, value); }
        }

        public static readonly DependencyProperty ButtonPressedForegroundColorProperty =
            DependencyProperty.Register("ButtonPressedForegroundColor",
                typeof(Color),
                typeof(TitleBarBehavior),
                new PropertyMetadata(null, OnButtonPressedForegroundColorChanged));

        private static void OnButtonPressedForegroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ApplicationView.GetForCurrentView().TitleBar.ButtonPressedForegroundColor = (Color)e.NewValue;
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
                typeof(TitleBarBehavior),
                new PropertyMetadata(null, OnForegroundColorChanged));

        private static void OnForegroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ApplicationView.GetForCurrentView().TitleBar.ForegroundColor = (Color)e.NewValue;
        }
        #endregion
        #region InactiveBackgroundColor
        public Color InactiveBackgroundColor
        {
            get { return (Color)GetValue(InactiveBackgroundColorProperty); }
            set { SetValue(InactiveBackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty InactiveBackgroundColorProperty =
            DependencyProperty.Register("InactiveBackgroundColor",
                typeof(Color),
                typeof(TitleBarBehavior),
                new PropertyMetadata(null, OnInactiveBackgroundColorChanged));

        private static void OnInactiveBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ApplicationView.GetForCurrentView().TitleBar.InactiveBackgroundColor = (Color)e.NewValue;
        }
        #endregion
        #region InactiveForegroundColor
        public Color InactiveForegroundColor
        {
            get { return (Color)GetValue(InactiveForegroundColorProperty); }
            set { SetValue(InactiveForegroundColorProperty, value); }
        }

        public static readonly DependencyProperty InactiveForegroundColorProperty =
            DependencyProperty.Register("InactiveForegroundColor",
                typeof(Color),
                typeof(TitleBarBehavior),
                new PropertyMetadata(null, OnInactiveForegroundColorChanged));

        private static void OnInactiveForegroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ApplicationView.GetForCurrentView().TitleBar.InactiveForegroundColor = (Color)e.NewValue;
        }
        #endregion
    }
}
