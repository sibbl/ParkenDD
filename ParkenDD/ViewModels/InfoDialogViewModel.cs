using System;
using System.Collections.Generic;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using ParkenDD.Messages;
using ParkenDD.Models;

namespace ParkenDD.ViewModels
{
    public class InfoDialogViewModel : ViewModelBase
    {
        public List<LicenseInformationItem> Licenses => new List<LicenseInformationItem>
        {
            new LicenseInformationItem
            {
                Name = "Application Insight Tools",
                Copyright = "Microsoft",
                License = "Application Insights Tools and Software Developer Kit",
                Link = new Uri("https://www.visualstudio.com/support/dn614805")
            },
            new LicenseInformationItem
            {
                Name = "Json.NET",
                Copyright = "2007 James Newton-King",
                License = "MIT",
                Link = new Uri("https://raw.githubusercontent.com/JamesNK/Newtonsoft.Json/master/LICENSE.md")
            }
        };

        private RelayCommand _hideDialogCommand;
        public RelayCommand HideDialogCommand => _hideDialogCommand ?? (_hideDialogCommand = new RelayCommand(HideDialog));

        private void HideDialog()
        {
            Messenger.Default.Send(new InfoDialogToggleVisibilityMessage(false));
        }
    }
}
