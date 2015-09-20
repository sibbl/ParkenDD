using Windows.UI.Xaml;

namespace ParkenDD.Messages
{
    public class InfoDialogToggleVisibilityMessage
    {
        public bool IsVisible { get; }

        public Visibility Visibility => IsVisible ? Visibility.Visible : Visibility.Collapsed;

        public InfoDialogToggleVisibilityMessage(bool isVisible)
        {
            IsVisible = isVisible;
        }
    }
}
