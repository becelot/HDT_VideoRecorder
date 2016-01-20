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
        public List<GameMode> recordedGameModes;

        private const string STORAGE_FILE_NAME = "config.xml";

        private PluginConfig()
        {

        }
    }
}
