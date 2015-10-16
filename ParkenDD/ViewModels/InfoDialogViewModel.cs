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
                Name = "Cimbalino Toolkit",
                Copyright = "2011 Pedro Lamas",
                License = "MIT",
                Link = new Uri("https://raw.githubusercontent.com/Cimbalino/Cimbalino-Toolkit/master/LICENSE.txt")
            },
            new LicenseInformationItem
            {
                Name = "Google Analytics SDK for Windows and Windows Phone",
                Copyright = "Tim Greenfield",
                License = "Microsoft Public License",
                Link = new Uri("https://googleanalyticssdk.codeplex.com/license")
            },
            new LicenseInformationItem
            {
                Name = "Json.NET",
                Copyright = "2007 James Newton-King",
                License = "MIT",
                Link = new Uri("https://raw.githubusercontent.com/JamesNK/Newtonsoft.Json/master/LICENSE.md")
            },
            new LicenseInformationItem
            {
                Name = "MvvmLight",
                Copyright = "2009 - 2014 Laurent Bugnion",
                License = "MIT",
                Link = new Uri("https://mvvmlight.codeplex.com/license")
            },
            new LicenseInformationItem
            {
                Name = "WindowsStateTriggers",
                Copyright = "Morten Nielsen",
                License = "MIT",
                Link = new Uri("https://raw.githubusercontent.com/dotMorten/WindowsStateTriggers/master/LICENSE")
            },
            new LicenseInformationItem
            {
                Name = "WinRT XAML Toolkit",
                Copyright = "2012 Filip Skakun",
                License = "MIT",
                Link = new Uri("https://winrtxamltoolkit.codeplex.com/license")
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
