using Hearthstone_Deck_Tracker.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

using Hearthstone_Deck_Tracker.Utility;
using System.ComponentModel;

namespace HDT_GameRecorder.Utils
{
    public class PluginConfig
    {
        [XmlArray(ElementName = "RecordedGameMode")]
        [XmlArrayItem(ElementName ="GameMode")]
        public List<GameMode> recordedGameModes { get; set; }

        [DefaultValue("Hearthstone")]
        [XmlElement(ElementName = "profileName")]
        public String profileName { get; set; }

        [DefaultValue(1)]
        public int recorderActiveAfterGameEnd { get; set; }

        [XmlIgnore]
        private const string STORAGE_FILE_NAME = "config.xml";

        //Singleton pattern
        [XmlIgnore]
        private static PluginConfig _instance = null;

        private PluginConfig()
        {
        }

        public static PluginConfig Instance {
            get
            {
                if (_instance == null)
                {
                    _instance = new PluginConfig();
                    Hearthstone_Deck_Tracker.Logger.WriteLine("Load recorder settings from config.xml: " + _instance.ConfigFile);
                    loadFromFile(_instance.ConfigFile);
                }

                return _instance;
            }
        }

        [XmlIgnore]
        public static string AppDataPath
        {
            get
            {
                return Hearthstone_Deck_Tracker.Config.AppDataPath + @"\VideoRecorder";
            }
        }

        [XmlIgnore]
        public string ConfigFile
        {
            get
            {
                return AppDataPath + @"\" + STORAGE_FILE_NAME;
            }
        }

        public void Save()
        {
            Hearthstone_Deck_Tracker.XmlManager<PluginConfig>.Save(Instance.ConfigFile, Instance);
        }

        private static void loadFromFile(string file)
        {
            //Check if directory exists
            if (!Directory.Exists(AppDataPath))
            {
                Directory.CreateDirectory(AppDataPath);
            }


            if (File.Exists(file))
            {
                _instance =  Hearthstone_Deck_Tracker.XmlManager<PluginConfig>.Load(file);
            } else //require init
            {
                Hearthstone_Deck_Tracker.Logger.WriteLine("Not found, create new instance manually");
                List<GameMode> recorded = new List<GameMode>();
                recorded.Add(GameMode.Practice);
                recorded.Add(GameMode.Arena);
                recorded.Add(GameMode.Brawl);
                recorded.Add(GameMode.Casual);
                recorded.Add(GameMode.Friendly);
                recorded.Add(GameMode.Ranked);
                recorded.Add(GameMode.Spectator);
                recorded.Add(GameMode.None);
                _instance = new PluginConfig();
                _instance.recordedGameModes = recorded;
                _instance.profileName = "Hearthstone";
                _instance.Save();
            }
        }
    }
}
