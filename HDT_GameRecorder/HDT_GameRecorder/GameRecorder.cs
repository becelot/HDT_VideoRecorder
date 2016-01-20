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
                GameMode currentGameMode = Core.Game.CurrentGameMode;
                OBSUtils.startRecording();
            }
        }

        private static void onGameStart()
        {

        }

        private static void onGameEnd()
        {

        }
    }
}
