using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp.Controls
{
    public partial class OnRunningSettings : UserControl
    {
        public OnRunningSettings()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Инициализация настроек
        /// </summary>
        private void InitSettings()
        {
            chbDontShowLogo.Checked = Settings.Default.FlagDontShowLogo;
            Preferences.MainTabLoad modeMainTabLoad = (Preferences.MainTabLoad) Enum.Parse(typeof (Preferences.MainTabLoad), Settings.Default.MainTabLoad);
            byte modeByte = (byte) modeMainTabLoad;
            switch (modeByte)
            {
                case 0:
                    rbNow.Checked = true;
                    break;
                case 1:
                    rbNext.Checked = true;
                    break;
                case 2:
                    rbDays.Checked = true;
                    break;
                case 3:
                    rbChannels.Checked = true;
                    break;
            }
            Preferences.WindowMode winMode =
                (Preferences.WindowMode) Enum.Parse(typeof (Preferences.WindowMode), Settings.Default.WindowMode);
            switch (winMode)
            {
                case Preferences.WindowMode.Minimize:
                    rbMinimize.Checked = true;
                    break;
                case Preferences.WindowMode.Restore:
                    rbRestore.Checked = true;
                    break;
                case Preferences.WindowMode.Maximize:
                    rbMaximize.Checked = true;
                    break;
            }
            chbAutorun.Checked = Settings.Default.Autorun;
        }
        /// <summary>
        /// При изменении режима появления заставки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chbDontShowLogo_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.FlagDontShowLogo = chbDontShowLogo.Checked;
        }

        private void OnRunningSettings_Load(object sender, EventArgs e)
        {
            InitSettings();
        }

        private void OnCheckedMainTabMode (object sender, EventArgs e)
        {
            if (sender is RadioButton)
            {
                if ((sender as RadioButton).Checked)
                {
                    Preferences.mainTabLoad = (Preferences.MainTabLoad) byte.Parse((sender as RadioButton).Tag.ToString());
                    Settings.Default.MainTabLoad = Preferences.mainTabLoad.ToString();
                }
            }
        }

        private void OnCheckedWindowMode (object sender, EventArgs e)
        {
            if (sender is RadioButton)
            {
                if ((sender as RadioButton).Checked)
                {
                    Settings.Default.WindowMode =
                        ((Preferences.WindowMode) byte.Parse((sender as RadioButton).Tag.ToString())).ToString();
                }
            }
        }

        private void chbAutorun_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.Autorun = chbAutorun.Checked;
        }
    }
}
