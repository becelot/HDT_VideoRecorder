using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Hearthstone_Deck_Tracker.Enums;
using HDT_GameRecorder.Utils;
using System.Diagnostics;

namespace HDT_GameRecorder
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        Dictionary<CheckBox, GameMode> checkBoxToModeDict;

        public SettingsControl()
        {
            InitializeComponent();
            SetRecordedGameModes();
            SetOBSProfileNames();
            SetOBSStatusNotification();
        }

        private void SetOBSStatusNotification()
        {


            if (OBSUtils.isObsInstalled())
            {
                //Layout
                Label testLabel = new Label();
                testLabel.Margin = new Thickness(5, 5, 5, 0);
                testLabel.HorizontalAlignment = HorizontalAlignment.Center;
                testLabel.Content = "OBS installation was found";
                obsStackPanel.Children.Add(testLabel);
            }
            else {
                //DockPanel
                DockPanel dp = new DockPanel();
                dp.Margin = new Thickness(5, 5, 5, 0);
                dp.HorizontalAlignment = HorizontalAlignment.Stretch;

                //Label
                Label statusLabel = new Label();
                statusLabel.Content = "OBS is not installed";
                dp.Children.Add(statusLabel);

                //Button
                Button downloadButton = new Button();
                downloadButton.Margin = new Thickness(5, 0, 0, 0);
                downloadButton.Content = "Download OBS";
                downloadButton.Click += DownloadButton_Click;
                dp.Children.Add(downloadButton);

                obsStackPanel.Children.Add(dp);
            }

            
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://google.de");
        }

        private void SetOBSProfileNames()
        {
            foreach (String p in OBSUtils.getProfiles())
            {
                profileSettings.Items.Add(p);
            }
            profileSettings.SelectedItem = PluginConfig.Instance.profileName;
        }

        private void SetRecordedGameModes()
        {
            checkBoxToModeDict = new Dictionary<CheckBox, GameMode>()
            {
                { CheckboxRecordRanked, GameMode.Ranked },
                { CheckboxRecordArena, GameMode.Arena},
                { CheckboxRecordBrawl, GameMode.Brawl },
                { CheckboxRecordCasual, GameMode.Casual },
                { CheckboxRecordFriendly, GameMode.Friendly },
                { CheckboxRecordPractice, GameMode.Practice },
            };

            foreach (CheckBox cb in checkBoxToModeDict.Keys)
            {
                cb.IsChecked = PluginConfig.Instance.recordedGameModes.Contains(checkBoxToModeDict[cb]);
            }
        }

        private void CheckboxChecked(object sender, RoutedEventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            GameMode gm = checkBoxToModeDict[cb];
            if (cb.IsChecked == true)
            {
                PluginConfig.Instance.recordedGameModes.Add(gm);
            } else
            {
                PluginConfig.Instance.recordedGameModes.Remove(gm);
            }

            PluginConfig.Instance.Save();
        }

        private void profileSettings_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PluginConfig.Instance.profileName = profileSettings.SelectedItem.ToString();
            PluginConfig.Instance.Save();
        }
    }
}
