using Hearthstone_Deck_Tracker.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace HDT_GameRecorder.Utils
{
    [Serializable]
    class PluginConfig
    {


        public List<GameMode> recordedGameModes { get; set; }

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
                    _instance.loadFromFile(_instance.AppDataPath + @"\" + STORAGE_FILE_NAME);
                }

                return _instance;
            }
        }

        [XmlIgnore]
        public string AppDataPath
        {
            get
            {
                return Hearthstone_Deck_Tracker.Config.AppDataPath + @"\VideoRecorder";
            }
        }

        private void loadFromFile(string file)
        {
            //Check if directory exists
            if (!Directory.Exists(AppDataPath))
            {
                Directory.CreateDirectory(AppDataPath);
            }


            if (File.Exists(file))
            {

            } else //require init
            {

            }
        }
    }
}
