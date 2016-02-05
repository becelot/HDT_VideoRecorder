using System;
using System.Windows.Controls;
using System.Collections.Generic;
using Hearthstone_Deck_Tracker.Plugins;

using HDT_GameRecorder.Utils;
using Hearthstone_Deck_Tracker.API;
using MahApps.Metro.Controls;

namespace HDT_GameRecorder
{
    public class PluginContainer : IPlugin
    {
        private Flyout _settingsFlyout;


        public string Author
        {
            get
            {
                return "Becelot";
            }
        }

        public string ButtonText
        {
            get
            {
                return "Settings";
            }
        }

        public string Description
        {
            get
            {
                return "A video recording software based on OBS that automatically captures your games. To save disk space, the plugin asks (if chosen) to save the video after every match. Otherwise, the plugin saves the video to the desired location.";
            }
        }

        public MenuItem MenuItem
        {
            get
            {
                return null;
            }
        }

        public string Name
        {
            get
            {
                return "Video Game Recorder";
            }
        }

        public Version Version
        {
            get
            {
                return new Version(0, 0, 1);
            }
        }

        public void OnButtonPress()
        {
            if (_settingsFlyout != null)
            {
                _settingsFlyout.IsOpen = true;
            }
            
        }

        public void OnLoad()
        {
            GameRecorder.Load();
            SetSettingsFlyoutControl();
        }

        public void SetSettingsFlyoutControl()
        {
            Flyout flyout = new Flyout();
            SettingsControl settingsControl = new SettingsControl();
            flyout.Header = "Video Game Recorder";
            flyout.Content = settingsControl;
            flyout.Position = Position.Left;
            Panel.SetZIndex(flyout, 100);

            Hearthstone_Deck_Tracker.API.Core.MainWindow.Flyouts.Items.Add(flyout);

            _settingsFlyout = flyout;
        }

        public void OnUnload()
        {
            PluginConfig.Instance.Save();
        }

        public void OnUpdate()
        {

        }
    }
}
