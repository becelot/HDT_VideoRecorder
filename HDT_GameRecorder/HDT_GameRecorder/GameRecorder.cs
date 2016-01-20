using System;

using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Enums;

using HDT_GameRecorder.Utils;

namespace HDT_GameRecorder
{
    class GameRecorder
    {

        private static Boolean gameOngoing = false;

        public static void Load()
        {
            //Add callbacks
            GameEvents.OnGameStart.Add(onGameStart);
            GameEvents.OnGameEnd.Add(onGameEnd);

            //Currently ongoing game?
            gameOngoing = !Core.Game.IsInMenu;

            if (gameOngoing)
            {
                onGameStart();

            }
        }

        private static void onGameStart()
        {
            GameMode currentGameMode = Core.Game.CurrentGameMode;
            if (PluginConfig.Instance.recordedGameModes.Contains(currentGameMode))
            {
                OBSUtils.startRecording();
            }
        }

        private static void onGameEnd()
        {

        }
    }
}
