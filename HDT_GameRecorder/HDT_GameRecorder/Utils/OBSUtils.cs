using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Windows.Forms;
using Keyboard;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;

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
            if (OBSUtils.isObsRunning())
                getProcessInformation().process.Kill();
        }

        public static void createStandardScene(string sceneName)
        {
            String json;
            using (StreamReader sr = new StreamReader(OBSUtils.getConfigPath() + @"\sceneCollection\scenes.xconfig"))
            {
                json = sr.ReadToEnd();
            }
            String scenes = json.Substring(0, json.IndexOf("global sources"));
            if (!scenes.Contains(" " + sceneName + " "))
            {
                String[] split = scenes.Split('\n');
                scenes = split[0] + '\n';
                scenes += String.Format(readFromResourceStream("HDT_GameRecorder.Resources.StandardScene.txt"), sceneName, Screen.PrimaryScreen.Bounds.Width.ToString(), Screen.PrimaryScreen.Bounds.Height.ToString()) + '\n';
                foreach (String s in split.Skip(1))
                {
                    if (s == "") continue;
                    scenes += s + '\n';
                }
            }


            String sources = json.Substring(json.IndexOf("global sources"), json.Length - json.IndexOf("global sources"));
            if (!sources.Contains(" " + sceneName + " "))
            {
                String[] split = sources.Split('\n');
                sources = split[0] + '\n';
                sources += String.Format(readFromResourceStream("HDT_GameRecorder.Resources.StandardSource.txt"), sceneName, Screen.PrimaryScreen.Bounds.Width.ToString(), Screen.PrimaryScreen.Bounds.Height.ToString()) + '\n';
                foreach (String s in split.Skip(1))
                {
                    if (s == "") continue;
                    sources += s + '\n';
                }
            }


            Hearthstone_Deck_Tracker.Logger.WriteLine(scenes + sources);
            using (StreamWriter sw = new StreamWriter(getConfigPath() + @"\sceneCollection\scenes.xconfig"))
            {
                sw.WriteLine(scenes + sources);
            }
        }

        private static string readFromResourceStream(string resourceName)
        {
            try
            {
                using (Stream stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                {
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        string result = sr.ReadToEnd();
                        return result;
                    }
                }
            }
            catch (Exception)
            {

            }
            return "";
        }

        public async static Task<Boolean> createStandardProfile(string profileName)
        {
            //Check for existing profile
            if (File.Exists(getConfigPath() + @"\profiles\" + profileName + ".ini"))
            {
                //Ask to overwrite profile
                var overwrite = await DialogManager.ShowMessageAsync(Hearthstone_Deck_Tracker.API.Core.MainWindow, "Profile does exist", "A profile with the same name does already exist. Overwrite?", MessageDialogStyle.AffirmativeAndNegative);
                if (overwrite == MessageDialogResult.Negative) 
                {
                    return false;
                }
            }
            FileStream fs = File.Create(getConfigPath() + @"\profiles\" + profileName + ".ini");
            StreamWriter sw = new StreamWriter(fs);

            string result = readFromResourceStream("HDT_GameRecorder.Resources.StandardProfile.txt");

            sw.Write(result);

            sw.Close();


            IniFile ini = new IniFile(getConfigPath() + @"\profiles\" + profileName + ".ini");

            ini.IniWriteValue("Video", "BaseHeight", Screen.PrimaryScreen.Bounds.Height.ToString());
            ini.IniWriteValue("Video", "BaseWidth", Screen.PrimaryScreen.Bounds.Width.ToString());
            ini.IniWriteValue("Publish", "SavePath", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)+ @"\Videos\Recorded\$T.mp4");

            OBSUtils.kill();
            ini = new IniFile(getConfigPath() + @"\global.ini");
            ini.IniWriteValue("General", "Profile", profileName);
            await DialogManager.ShowMessageAsync(Hearthstone_Deck_Tracker.API.Core.MainWindow, "Created profile", "Profile creation sucessfull!", MessageDialogStyle.Affirmative);

            return true;
        }
    }
}
