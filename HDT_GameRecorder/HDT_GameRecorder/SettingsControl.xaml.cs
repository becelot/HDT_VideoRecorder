﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Hearthstone_Deck_Tracker.Enums;
using HDT_GameRecorder.Utils;
using System.Diagnostics;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace HDT_GameRecorder
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
    public partial class SettingsControl : UserControl
    {
        private DockPanel obsInstallStatusPanel;


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
            if (obsInstallStatusPanel != null)
            {
                obsStackPanel.Children.Remove(obsInstallStatusPanel);
                obsInstallStatusPanel.Children.Clear();
            }

            obsInstallStatusPanel = new DockPanel();
            obsInstallStatusPanel.Margin = new Thickness(5, 5, 5, 0);
            obsInstallStatusPanel.HorizontalAlignment = HorizontalAlignment.Center;


            if (OBSUtils.isObsInstalled())
            {
                //Layout
                Label testLabel = new Label();
                testLabel.Margin = new Thickness(5, 5, 5, 0);
                testLabel.HorizontalAlignment = HorizontalAlignment.Center;
                testLabel.Content = "OBS installation was found";

                createProfileButton.IsEnabled = true;
                createSceneButton.IsEnabled = true;

                obsInstallStatusPanel.Children.Add(testLabel);
            }
            else {
                //Label
                Label statusLabel = new Label();
                statusLabel.Content = "OBS is not installed";
                obsInstallStatusPanel.Children.Add(statusLabel);

                createProfileButton.IsEnabled = false;
                createSceneButton.IsEnabled = false;

                //Button
                Button downloadButton = new Button();
                downloadButton.Margin = new Thickness(5, 0, 0, 0);
                downloadButton.Content = "Download OBS";
                downloadButton.Click += (sender, e) => Process.Start("https://obsproject.com/");
                obsInstallStatusPanel.Children.Add(downloadButton);
            }

            obsStackPanel.Children.Add(obsInstallStatusPanel);
        }

        private void SetOBSProfileNames()
        {
            profileSettings.Items.Clear();
            foreach (String p in OBSUtils.getProfiles())
            {
                profileSettings.Items.Add(p);
            }
            try
            {
                profileSettings.SelectedItem = PluginConfig.Instance.profileName;
            } catch (Exception)
            {
                profileSettings.SelectedIndex = 0;
            }
            
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
            if (profileSettings.SelectedItem != null)
            {
                PluginConfig.Instance.profileName = profileSettings.SelectedItem.ToString();
                PluginConfig.Instance.Save();
            }

        }

        private void reloadButton_Click(object sender, RoutedEventArgs e)
        {
            SetRecordedGameModes();
            SetOBSProfileNames();
            SetOBSStatusNotification();
        }

        private async void createProfileButton_Click(object sender, RoutedEventArgs e)
        {
            MetroWindow window = Hearthstone_Deck_Tracker.API.Core.MainWindow;

            var result = await DialogManager.ShowInputAsync(window, "Profile name!", "Give a new name to the profile:");

            if (result != null && !result.Equals(""))
            {
                await OBSUtils.createStandardProfile(result);
                reloadButton_Click(null, null);
            }
        }

        private async void createSceneButton_Click(object sender, RoutedEventArgs e)
        {
            MetroWindow window = Hearthstone_Deck_Tracker.API.Core.MainWindow;

            var result = await DialogManager.ShowInputAsync(window, "Scene name!", "Give a new name to the scene:");

            if (result != null && !result.Equals(""))
            {
                OBSUtils.createStandardScene(result);
                reloadButton_Click(null, null);
            }
        }
    }
}
