using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;


namespace HDT_GameRecorder.Utils
{
    class OBSUtils
    {
        private static String lastExecutablePath = "";
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

        public static string getInstallDirectory()
        {
            if (lastExecutablePath == "")
            {
                IniFile ini = new IniFile(getConfigPath() + @"\global.ini");
                lastExecutablePath = ini.IniReadValue("General", "LastAppDirectory");
            }
            return lastExecutablePath;
        }

        public static string getExecutablePath()
        {
            return getInstallDirectory() + @"\OBS.exe";
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

        public static Boolean isObsRunning()
        {
            string fileNameToFilter = Path.GetFullPath(getExecutablePath());
            foreach (Process p in Process.GetProcesses())
            {
                string fileName = Path.GetFullPath(p.MainModule.FileName);

                if (string.Compare(fileNameToFilter, fileName, true) == 0)
                {
                    return true;
                }
            }
            return false;
        }

        public static void startObs()
        {
            if (OBSUtils.isObsRunning())
            {
                return;
            }

            //Start OBS
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
