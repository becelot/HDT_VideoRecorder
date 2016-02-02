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
            GameEvents.OnGameEnd.Add(GameRecorder.onGameEnd);
            
            if (OBSUtils.isObsRunning())
            {
                OBSUtils.startRecording();
                Thread.Sleep(1000);
                OBSUtils.stopRecording();

                ///TODO: Delete recording
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
            }
        }

        public static void onGameEnd()
        {
            Thread.Sleep(PluginConfig.Instance.recorderActiveAfterGameEnd);
            OBSUtils.stopRecording();
        }
    }
}
