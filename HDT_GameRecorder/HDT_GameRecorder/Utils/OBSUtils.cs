using System;
using System.Collections.Generic;
using System.IO;


namespace HDT_GameRecorder.Utils
{
    class OBSUtils
    {
        private OBSUtils() { }

        public static string getConfigPath()
        {
            //Get standard configuration path of OBS
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OBS"; 
            if (!Directory.Exists(path))
            {
                throw new ObsNotInstalledException("OBS is not installed on this system");
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OBS";
        }

        public static List<string> getProfiles()
        {
            string path = getConfigPath() + @"\profiles";

            string[] profiles = Directory.GetFiles(path);

            List<string> result = new List<string>();

            foreach (string profile in profiles)
            {
                string tmp = profile;
                tmp = tmp.Remove(0, path.Length + 1);
                tmp = tmp.Remove(tmp.Length - 4, 4);

                result.Add(tmp);
            }

            return result;
        }

        public static Boolean isObsInstalled()
        {
            //Get standard configuration path of OBS
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OBS";
            if (!Directory.Exists(path))
            {
                return false;
            }

            return true;
        }

        public static void startRecording()
        {

        }

        public static void stopRecording()
        {

        }
    }
}
