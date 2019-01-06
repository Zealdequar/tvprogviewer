using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TVProgViewer.TVProgApp.Controls
{
    public partial class ReminderSettings : UserControl
    {
        public ReminderSettings()
        {
            InitializeComponent();
            InitSettings();
        }

        private void InitSettings()
        {
            chbOn.Checked = Reminder.Default.On;
            dtpRemFrom1.Enabled = dtpRemTo1.Enabled = chbMonday.Checked = Reminder.Default.Monday;
            dtpRemFrom2.Enabled = dtpRemTo2.Enabled = chbTuesday.Checked = Reminder.Default.Tuesday;
            dtpRemFrom3.Enabled = dtpRemTo3.Enabled = chbWensday.Checked = Reminder.Default.Wednesday;
            dtpRemFrom4.Enabled = dtpRemTo4.Enabled = chbThursday.Checked = Reminder.Default.Thursday;
            dtpRemFrom5.Enabled = dtpRemTo5.Enabled = chbFriday.Checked = Reminder.Default.Friday;
            dtpRemFrom6.Enabled = dtpRemTo6.Enabled = chbSaturday.Checked = Reminder.Default.Saturday;
            dtpRemFrom7.Enabled = dtpRemTo7.Enabled = chbSunday.Checked = Reminder.Default.Sunday;
            
            dtpRemFrom1.ValueChanged -= new EventHandler(dtpRemFrom1_ValueChanged);
            dtpRemTo1.ValueChanged -= new EventHandler(dtpRemTo1_ValueChanged);
            dtpRemFrom2.ValueChanged -= new EventHandler(dtpRemFrom2_ValueChanged);
            dtpRemTo2.ValueChanged -= new EventHandler(dtpRemTo2_ValueChanged);
            dtpRemFrom3.ValueChanged -= new EventHandler(dtpRemFrom3_ValueChanged);
            dtpRemTo3.ValueChanged -= new EventHandler(dtpRemTo3_ValueChanged);
            dtpRemFrom4.ValueChanged -= new EventHandler(dtpRemFrom4_ValueChanged);
            dtpRemTo4.ValueChanged -= new EventHandler(dtpRemTo4_ValueChanged);
            dtpRemFrom5.ValueChanged -= new EventHandler(dtpRemFrom5_ValueChanged);
            dtpRemTo5.ValueChanged -= new EventHandler(dtpRemTo5_ValueChanged);
            dtpRemFrom6.ValueChanged -= new EventHandler(dtpRemFrom6_ValueChanged);
            dtpRemTo6.ValueChanged -= new EventHandler(dtpRemTo6_ValueChanged);
            dtpRemFrom7.ValueChanged -= new EventHandler(dtpRemFrom7_ValueChanged);
            dtpRemTo7.ValueChanged -= new EventHandler(dtpRemTo7_ValueChanged);

            dtpRemFrom1.Value = DataTimeHelper(dtpRemFrom1, Reminder.Default.MonFrom);
            dtpRemTo1.Value = DataTimeHelper(dtpRemTo1, Reminder.Default.MonTo);
            dtpRemFrom2.Value = DataTimeHelper(dtpRemFrom2, Reminder.Default.TueFrom);
            dtpRemTo2.Value = DataTimeHelper(dtpRemTo2, Reminder.Default.TueTo);
            dtpRemFrom3.Value = DataTimeHelper(dtpRemFrom3, Reminder.Default.WenFrom);
            dtpRemTo3.Value = DataTimeHelper(dtpRemTo3, Reminder.Default.WenTo);
            dtpRemFrom4.Value = DataTimeHelper(dtpRemFrom4, Reminder.Default.ThsFrom);
            dtpRemTo4.Value = DataTimeHelper(dtpRemTo4, Reminder.Default.ThsTo);
            dtpRemFrom5.Value = DataTimeHelper(dtpRemFrom5, Reminder.Default.FriFrom);
            dtpRemTo5.Value = DataTimeHelper(dtpRemTo5, Reminder.Default.FriTo);
            dtpRemFrom6.Value = DataTimeHelper(dtpRemFrom6, Reminder.Default.SatFrom);
            dtpRemTo6.Value = DataTimeHelper(dtpRemTo6, Reminder.Default.SatTo);
            dtpRemFrom7.Value = DataTimeHelper(dtpRemFrom7, Reminder.Default.SunFrom);
            dtpRemTo7.Value = DataTimeHelper(dtpRemTo7, Reminder.Default.SunTo);

            dtpRemTo1.ValueChanged +=new EventHandler(dtpRemTo1_ValueChanged);
            dtpRemFrom1.ValueChanged +=new EventHandler(dtpRemFrom1_ValueChanged);
            dtpRemFrom2.ValueChanged +=new EventHandler(dtpRemFrom2_ValueChanged);
            dtpRemTo2.ValueChanged +=new EventHandler(dtpRemTo2_ValueChanged);
            dtpRemFrom3.ValueChanged += new EventHandler(dtpRemFrom3_ValueChanged);
            dtpRemTo3.ValueChanged += new EventHandler(dtpRemTo3_ValueChanged);
            dtpRemFrom4.ValueChanged +=new EventHandler(dtpRemFrom4_ValueChanged);
            dtpRemTo4.ValueChanged += new EventHandler(dtpRemTo4_ValueChanged);
            dtpRemFrom5.ValueChanged +=new EventHandler(dtpRemFrom5_ValueChanged);
            dtpRemTo5.ValueChanged +=new EventHandler(dtpRemTo5_ValueChanged);
            dtpRemFrom6.ValueChanged +=new EventHandler(dtpRemFrom6_ValueChanged);
            dtpRemTo6.ValueChanged +=new EventHandler(dtpRemTo6_ValueChanged);
            dtpRemFrom7.ValueChanged +=new EventHandler(dtpRemFrom7_ValueChanged);
            dtpRemTo7.ValueChanged +=new EventHandler(dtpRemTo7_ValueChanged);
            
            chbMessage.Checked = Reminder.Default.Message;
            numOpacity.ValueChanged -= new EventHandler(numOpacity_ValueChanged);

            numOpacity.Value = Reminder.Default.Opacity;

            numOpacity.ValueChanged +=new EventHandler(numOpacity_ValueChanged);
            chbFullScreen.Checked = Reminder.Default.FullScreenMain;
            rbAfterTelecast.Checked = Reminder.Default.AfterTelecast;
            numAfterSeconds.Enabled = rbAfterSeconds.Checked = Reminder.Default.AfterSeconds;
            numAfterSeconds.ValueChanged -= new EventHandler(numAfterSeconds_ValueChanged);
            numAfterSeconds.Value = Reminder.Default.QtySeconds;
            numAfterSeconds.ValueChanged +=new EventHandler(numAfterSeconds_ValueChanged);
            chbOnSound.Checked = Reminder.Default.OnSound;
            textBox1.Text = Reminder.Default.FileNameSound;
            Preferences.PlaySound modeSound =
                (Preferences.PlaySound) Enum.Parse(typeof (Preferences.PlaySound), Reminder.Default.SoundMode);
            switch (modeSound)
            {
                case Preferences.PlaySound.DynamicSound:
                    rbDynamic.Checked = true;
                    break;
                case Preferences.PlaySound.SystemSound:
                    rbSystem.Checked = true;
                    break;
               case Preferences.PlaySound.UserSound:
                    rbFile.Checked = true;
                    break;
            }
            chbRemindLater.Checked = numMinutesLater.Enabled = Reminder.Default.RemindLater;
            numMinutesLater.ValueChanged -= new EventHandler(numMinutesLater_ValueChanged);
            numMinutesLater.Value = Reminder.Default.MinutesLater;
            numMinutesLater.ValueChanged += new EventHandler(numMinutesLater_ValueChanged);
        }

        private DateTime DataTimeHelper(DateTimePicker val, TimeSpan ts)
        {
            return new DateTime(val.MinDate.Year, val.MinDate.Month, val.MinDate.Day, ts.Hours, ts.Minutes, ts.Seconds);
        }
        /// <summary>
        /// Вкл./выкл. напоминатель
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chbOn_CheckedChanged(object sender, EventArgs e)
        {
            Reminder.Default.On = chbOn.Checked;
        }

        private void chbMonday_CheckedChanged(object sender, EventArgs e)
        {
            dtpRemFrom1.Enabled = dtpRemTo1.Enabled = Reminder.Default.Monday = chbMonday.Checked;
        }

        private void chbTuesday_CheckedChanged(object sender, EventArgs e)
        {
            dtpRemFrom2.Enabled = dtpRemTo2.Enabled = Reminder.Default.Tuesday = chbTuesday.Checked;
        }

        private void chbWensday_CheckedChanged(object sender, EventArgs e)
        {
            dtpRemFrom3.Enabled = dtpRemTo3.Enabled = Reminder.Default.Wednesday = chbWensday.Checked;
        }

        private void chbThursday_CheckedChanged(object sender, EventArgs e)
        {
            dtpRemFrom4.Enabled = dtpRemTo4.Enabled = Reminder.Default.Thursday = chbThursday.Checked;
        }

        private void chbFriday_CheckedChanged(object sender, EventArgs e)
        {
            dtpRemFrom5.Enabled = dtpRemTo5.Enabled = Reminder.Default.Friday = chbFriday.Checked;
        }

        private void chbSaturday_CheckedChanged(object sender, EventArgs e)
        {
            dtpRemFrom6.Enabled = dtpRemTo6.Enabled = Reminder.Default.Saturday = chbSaturday.Checked;
        }

        private void chbSunday_CheckedChanged(object sender, EventArgs e)
        {
            dtpRemFrom7.Enabled = dtpRemTo7.Enabled = Reminder.Default.Sunday = chbSunday.Checked;
        }

        private void ReminderSettings_Load(object sender, EventArgs e)
        {
            InitSettings();
        }
        private TimeSpan TimeSpanHelper(DateTimePicker dtp)
        {
            return new TimeSpan(0, dtp.Value.Hour, dtp.Value.Minute, dtp.Value.Second);
        }

        private void dtpRemFrom1_ValueChanged(object sender, EventArgs e)
        {
            Reminder.Default.MonFrom = TimeSpanHelper(dtpRemFrom1);
        }

        private void dtpRemTo1_ValueChanged(object sender, EventArgs e)
        {
            Reminder.Default.MonTo = TimeSpanHelper(dtpRemTo1);
        }

        private void dtpRemFrom2_ValueChanged(object sender, EventArgs e)
        {
            Reminder.Default.TueFrom = TimeSpanHelper(dtpRemFrom2);
        }

        private void dtpRemTo2_ValueChanged(object sender, EventArgs e)
        {
            Reminder.Default.TueTo = TimeSpanHelper(dtpRemTo2);
        }

        private void dtpRemFrom3_ValueChanged(object sender, EventArgs e)
        {
            Reminder.Default.WenFrom = TimeSpanHelper(dtpRemFrom3);
        }

        private void dtpRemTo3_ValueChanged(object sender, EventArgs e)
        {
            Reminder.Default.WenTo = TimeSpanHelper(dtpRemTo3);
        }

        private void dtpRemFrom4_ValueChanged(object sender, EventArgs e)
        {
            Reminder.Default.ThsFrom = TimeSpanHelper(dtpRemFrom4);
        }

        private void dtpRemTo4_ValueChanged(object sender, EventArgs e)
        {
            Reminder.Default.ThsTo = TimeSpanHelper(dtpRemTo4);
        }

        private void dtpRemFrom5_ValueChanged(object sender, EventArgs e)
        {
            Reminder.Default.FriFrom = TimeSpanHelper(dtpRemFrom5);
        }

        private void dtpRemTo5_ValueChanged(object sender, EventArgs e)
        {
            Reminder.Default.FriTo = TimeSpanHelper(dtpRemTo5);
        }

        private void dtpRemFrom6_ValueChanged(object sender, EventArgs e)
        {
            Reminder.Default.SatFrom = TimeSpanHelper(dtpRemFrom6);
        }

        private void dtpRemTo6_ValueChanged(object sender, EventArgs e)
        {
            Reminder.Default.SatTo = TimeSpanHelper(dtpRemTo6);
        }

        private void dtpRemFrom7_ValueChanged(object sender, EventArgs e)
        {
            Reminder.Default.SunFrom = TimeSpanHelper(dtpRemFrom7);
        }

        private void dtpRemTo7_ValueChanged(object sender, EventArgs e)
        {
            Reminder.Default.SunTo = TimeSpanHelper(dtpRemTo7);
        }

        private void chbMessage_CheckedChanged(object sender, EventArgs e)
        {
            Reminder.Default.Message = chbMessage.Checked;
        }

        private void numOpacity_ValueChanged(object sender, EventArgs e)
        {
            Reminder.Default.Opacity = Byte.Parse(numOpacity.Value.ToString());
        }

        private void chbFullScreen_CheckedChanged(object sender, EventArgs e)
        {
            Reminder.Default.FullScreenMain = chbFullScreen.Checked;
        }

        private void numAfterSeconds_ValueChanged(object sender, EventArgs e)
        {
            Reminder.Default.QtySeconds = (int)numAfterSeconds.Value;
        }

        private void rbAfterTelecast_CheckedChanged(object sender, EventArgs e)
        {
            Reminder.Default.AfterTelecast = rbAfterTelecast.Checked;
        }

        private void rbAfterSeconds_CheckedChanged(object sender, EventArgs e)
        {
            Reminder.Default.AfterSeconds = numAfterSeconds.Enabled = rbAfterSeconds.Checked;
        }

        private void btnChangeFile_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                Reminder.Default.FileNameSound = textBox1.Text = openFileDialog1.FileName;
            }
        }

        private void rbDynamic_CheckedChanged(object sender, EventArgs e)
        {
            if (rbDynamic.Checked) Reminder.Default.SoundMode = "DynamicSound";
        }

        private void rbSystem_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSystem.Checked) Reminder.Default.SoundMode = "SystemSound";
        }

        private void rbFile_CheckedChanged(object sender, EventArgs e)
        {
            if (rbFile.Checked) Reminder.Default.SoundMode = "UserSound";
        }

        private void numMinutesLater_ValueChanged(object sender, EventArgs e)
        {
            Reminder.Default.MinutesLater = (int) numMinutesLater.Value;
        }

        private void chbRemindLater_CheckedChanged(object sender, EventArgs e)
        {
            Reminder.Default.RemindLater = numMinutesLater.Enabled = chbRemindLater.Checked;
        }


    }
}
