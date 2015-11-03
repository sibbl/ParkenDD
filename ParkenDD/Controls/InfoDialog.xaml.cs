using Windows.UI.Xaml.Controls;
using ParkenDD.ViewModels;

namespace ParkenDD.Controls
{
    public sealed partial class InfoDialog : UserControl
    {
        public InfoDialog()
        {
            InitializeComponent();
        }

        public InfoDialogViewModel Vm => (InfoDialogViewModel) DataContext;
        public SettingsViewModel SettingsVm => (SettingsViewModel) SettingsPivotItem.DataContext;
        public MainViewModel MainVm => (MainViewModel) DataPivotItem.DataContext;
    }
}
