using System;

using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Enums;

using HDT_GameRecorder.Utils;
using System.Threading;

namespace HDT_GameRecorder
{
    public static class GameRecorder
    {

        private static Boolean gameOngoing = false;

        public static void Load()
        {
            PluginConfig.Instance.Save();

            //Currently ongoing game?
            gameOngoing = !Core.Game.IsInMenu;


            //Add callbacks
            GameEvents.OnGameStart.Add(GameRecorder.onGameStart);
            GameEvents.OnInMenu.Add(GameRecorder.onInMenu);

            //Temporary workaround
            if (OBSUtils.isObsRunning())
            {
                OBSUtils.kill();
            }
            

            if (gameOngoing)
            {
                onGameStart();
            }
        }

        public static void onGameStart()
        {
            Hearthstone_Deck_Tracker.Logger.WriteLine("VideoGameRecorder: Test start game!");
            OBSUtils.startObs();
            
            GameMode currentGameMode = Core.Game.CurrentGameMode;
            if (PluginConfig.Instance.recordedGameModes.Contains(currentGameMode))
            {
                Hearthstone_Deck_Tracker.Logger.WriteLine("VideoRecorder: Start recording game!");
                OBSUtils.startRecording();

                gameOngoing = true;
            }
        }

        public static void onInMenu()
        {
            if (gameOngoing)
            {
                OBSUtils.stopRecording();
            }
            gameOngoing = false;
        }
    }
}
