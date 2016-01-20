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
            ///TODO: Load config data
        }

        public static PluginConfig Instance {
            get
            {
                if (_instance == null)
                {
                    _instance = new PluginConfig();
                }

                return _instance;
            }
        }
    }
}
