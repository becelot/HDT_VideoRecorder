using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Keyboard;
using System.Threading;

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

        public struct ProcessInformation
        {
            public Process process;
            public String path;
            public String commandLine; 
        }

        private static ProcessInformation getProcessInformation()
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
                    string fileNameProcess = Path.GetFullPath(item.Path);
                    if (string.Compare(fileNameProcess, fileName, true) == 0)
                    {
                        return new ProcessInformation { process = item.Process, path = item.Path, commandLine = item.CommandLine };
                    }
                }
            }

            return new ProcessInformation { process = null, path = null, commandLine = null};
        }

        public static String getRunningWindowTitle()
        {
            ProcessInformation pi = getProcessInformation();
            if (pi.process != null)
            {
                return pi.process.MainWindowTitle;
            }
            return "";
        }

        public static Boolean isObsRunning()
        {
            ProcessInformation pi = getProcessInformation();

            if (pi.process != null)
            {
                return true;
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
            Process p = Process.Start(getExecutablePath());
            p.WaitForInputIdle();
        }

        public static Boolean isObsInstalled()
        {
            //Get standard configuration path of OBS
            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\OBS";

            //If %appdata%/OBS does not exist
            if (!Directory.Exists(path))
            {
                return false;
            }

            //if OBS.exe does not exist
            if (!File.Exists(getExecutablePath()))
            {
                return false;
            }

            return true;
        }

        public static OBSKey getStartRecordingKey()
        {
            IniFile ini = new IniFile(getConfigPath() + @"\profiles\" + PluginConfig.Instance.profileName + ".ini");
            int keyValue;
            if (Int32.TryParse( ini.IniReadValue("Publish", "StartRecordingHotkey"), out keyValue))
            {
                Hearthstone_Deck_Tracker.Logger.WriteLine("VideoRecorder: Send " + keyValue.ToString());
                return new OBSKey(keyValue);
            }
            return new OBSKey();
        }

        public static OBSKey getStopRecordingKey()
        {
            IniFile ini = new IniFile(getConfigPath() + @"\profiles\" + PluginConfig.Instance.profileName + ".ini");
            int keyValue;
            if (Int32.TryParse(ini.IniReadValue("Publish", "StopRecordingHotkey"), out keyValue))
            {
                return new OBSKey(keyValue);
            }
            return new OBSKey();
        }

        public static void startRecording()
        {
            //startObs();
            IntPtr foregroundApplication = Messaging.GetForegroundWindow();
            IntPtr ptr = getProcessInformation().process.MainWindowHandle;
            Key key = getStartRecordingKey();

            bool test = key.PressForeground(ptr);

            //SendKeys.SendWait("{F11}");
            Messaging.SetForegroundWindow(foregroundApplication);
        }

        public static void stopRecording()
        {
            IntPtr foregroundApplication = Messaging.GetForegroundWindow();
            IntPtr ptr = getProcessInformation().process.MainWindowHandle;

            Key key = getStopRecordingKey();

            key.PressForeground(ptr);

            Messaging.SetForegroundWindow(foregroundApplication);
        }

        public static void kill()
        {
            getProcessInformation().process.Kill();
        }

        public static void createStandardProfile(string profileName)
        {
            //Don't overwrite existing configuration
            if (File.Exists(getConfigPath() + @"\profiles\" + profileName + ".ini"))
            {
                return;
            }
            FileStream fs = File.Create(getConfigPath() + @"\profiles\" + profileName + ".ini");
            StreamWriter sw = new StreamWriter(fs);

            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var resourceName = "HDT_GameRecorder.StandardProfile.txt";

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    string result = sr.ReadToEnd();
                    sw.Write(result);
                }
            }

            

        }
    }
}
