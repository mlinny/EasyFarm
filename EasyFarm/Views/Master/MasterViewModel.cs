﻿
/*///////////////////////////////////////////////////////////////////
<EasyFarm, general farming utility for FFXI.>
Copyright (C) <2013>  <Zerolimits>

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
*/
///////////////////////////////////////////////////////////////////

using System.Windows;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Windows.Input;
using System.Linq;
using EasyFarm.UserSettings;
using System.Collections.ObjectModel;
using EasyFarm.Logging;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.ServiceLocation;
using EasyFarm.Views;
using System.Xml.Serialization;

namespace EasyFarm.ViewModels
{
    public class MasterViewModel : ViewModelBase
    {
        public bool IsSettingsShown { get; set; }

        public MasterViewModel()
        {
            this.IsSettingsShown = false;

            // Get events from view models to update the status bar's text.
            EventAggregator.GetEvent<StatusBarUpdateEvent>().Subscribe(a => { StatusBarText = a; });

            // Tell the user the program has loaded the player's data
            InformUser("Bot Loaded: " + FFACE.Player.Name);

            // Set the main window's title to the player's name. 
            MainWindowTitle = "EasyFarm - " + FFACE.Player.Name;

            // Create start command handler.
            StartCommand = new DelegateCommand(Start);

            ExitCommand = new DelegateCommand(Exit);

            SaveCommand = new DelegateCommand(Save);

            SettingsCommand = new DelegateCommand(ShowSettings);
        }

        private void ShowSettings() 
        {
            IRegionManager regionManager =
                ServiceLocator.Current.GetInstance<IRegionManager>();         

            var region = regionManager.Regions["MainRegion"];

            if (!IsSettingsShown)
            {
                SettingsHeader = "_Main...";
                region.Activate(region.Views.OfType<SettingsView>().First());
                IsSettingsShown = true;
            }
            else
            {
                SettingsHeader = "_Settings...";
                region.Activate(region.Views.OfType<MainView>().First());
                IsSettingsShown = false;
            }
        }

        /// <summary>
        /// Bind for the title bar's text. 
        /// </summary>
        public String MainWindowTitle 
        {
            get { return Config.Instance.MainWindowTitle; }
            set { SetProperty(ref Config.Instance.MainWindowTitle, value); }
        }

        /// <summary>
        /// Binding for the status bar's text. 
        /// </summary>
        public String StatusBarText
        {
            get { return Config.Instance.StatusBarText; }
            set { SetProperty(ref Config.Instance.StatusBarText, value); }
        }

        /// <summary>
        /// Tells whether the bot is working or not. 
        /// </summary>
        public bool IsWorking 
        {
            get { return GameEngine.IsWorking; }
            set { SetProperty(ref GameEngine.IsWorking, value); }
        }

        private string _startStopHeader = "St_art";
        /// <summary>
        /// Binding for File -> Start/Pause text.
        /// </summary>
        public string StartPauseHeader
        {
            get
            {
                return _startStopHeader;
            }
            set
            {
                SetProperty(ref _startStopHeader, value);
            }
        }

        private string _settingsHeader = "_Settings...";

        public string SettingsHeader
        {
            get { return _settingsHeader; }
            set { SetProperty(ref _settingsHeader, value); }
        }
       
        /// <summary>
        /// Command to start the bot. 
        /// </summary>
        public ICommand StartCommand { get; set; }

        /// <summary>
        /// Command to shut down the program. 
        /// </summary>
        public ICommand ExitCommand { get; set; }

        /// <summary>
        /// Command to save the user's settings. 
        /// </summary>
        public DelegateCommand SaveCommand { get; set; }

        /// <summary>
        /// Load the settings window. 
        /// </summary>
        public DelegateCommand SettingsCommand { get; set; }

        /// <summary>
        /// Tells the program to start farming. 
        /// </summary>
        public void Start()
        {
            if (GameEngine.IsWorking)
            {
                Logger.Write.BotStop("Bot now paused");
                InformUser("Program paused.");                
                GameEngine.Stop();
                StartPauseHeader = "St_art";
            }
            else
            {
                Logger.Write.BotStart("Bot now running");
                InformUser("Program running.");
                GameEngine.Start();
                StartPauseHeader = "P_ause";
            }
        }

        /// <summary>
        /// Saves the player's data to file. 
        /// </summary>
        private void Save()
        {
            Logger.Write.SaveSettings("Settings saved");
            Config.Instance.SaveSettings();
        }

        /// <summary>
        /// Shutsdown the program. 
        /// </summary>
        private void Exit()
        {
            Application.Current.Shutdown();
        }        
    }
}

namespace EasyFarm.UserSettings
{
    public partial class Config
    {
        /// <summary>
        /// The text dislayed at the bottom of the screen
        /// </summary>
        [XmlIgnore]
        public String StatusBarText;

        /// <summary>
        /// The window's name: player's name. 
        /// </summary>
        [XmlIgnore]
        public string MainWindowTitle;
    }
}