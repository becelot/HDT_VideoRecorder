using System;
using System.Windows.Controls;
using System.Collections.Generic;
using Hearthstone_Deck_Tracker.Plugins;

using HDT_GameRecorder.Utils;
using Hearthstone_Deck_Tracker.API;

namespace HDT_GameRecorder
{
    public class PluginContainer : IPlugin
    {
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
            List<string> profiles = OBSUtils.getProfiles();

            foreach(string s in profiles)
            {
                Hearthstone_Deck_Tracker.Logger.WriteLine(s);
                Hearthstone_Deck_Tracker.Logger.WriteLine(OBSUtils.isObsRunning().ToString());
            }
        }

        public void OnLoad()
        {
            GameRecorder.Load();
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
