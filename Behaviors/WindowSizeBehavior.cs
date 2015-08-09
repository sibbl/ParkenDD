using Windows.Foundation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Microsoft.Xaml.Interactivity;

namespace ParkenDD.Behaviors
{
    public class WindowSizeBehavior : DependencyObject, IBehavior
    {
        public void Attach(DependencyObject associatedObject)
        {
        }

        public void Detach()
        {
        }

        public DependencyObject AssociatedObject { get; }

        public Size PreferredMinSize
        {
            get { return (Size)GetValue(PreferredMinSizeProperty); }
            set { SetValue(PreferredMinSizeProperty, value); }
        }

        public static readonly DependencyProperty PreferredMinSizeProperty =
            DependencyProperty.Register("PreferredMinSize",
            typeof(Size),
            typeof(WindowSizeBehavior),
            new PropertyMetadata(null, OnPreferredMinSizeChanged));

        private static void OnPreferredMinSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ApplicationView.GetForCurrentView().SetPreferredMinSize((Size)e.NewValue);
        }
    }
}
