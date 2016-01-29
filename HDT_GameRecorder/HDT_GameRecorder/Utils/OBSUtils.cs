using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Management;


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
            string fileName = Path.GetFullPath(getExecutablePath());

            var wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
            using (var searcher = new ManagementObjectSearcher(wmiQueryString))
            using (var results = searcher.Get())
            {
                var query = from p in Process.GetProcesses()
                            join mo in results.Cast<ManagementObject>()
                            on p.Id equals (int)(uint)mo["ProcessId"]
                            select new
                            {
                                Process = p,
                                Path = (string)mo["ExecutablePath"],
                                CommandLine = (string)mo["CommandLine"],
                            };
                foreach (var item in query)
                {
                    // Do what you want with the Process, Path, and CommandLine
                    if (item.Path == null) continue;
                    string fileNameProcess = Path.GetFullPath( item.Path);
                    if (string.Compare(fileNameProcess, fileName, true) == 0)
                    {
                        return true;
                    }
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
