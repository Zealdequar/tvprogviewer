using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp.Controls
{
    public partial class OtherSettings : UserControl
    {
        public OtherSettings()
        {
            InitializeComponent();
            InitSettings();
        }

        private void InitSettings()
        {
            dtpBeginEnd.ValueChanged -= new EventHandler(dtpBeginEnd_ValueChanged);
            dtpBeginEnd.Value = dtpBeginEnd.MinDate;

            dtpBeginEnd.Value = new DateTime(dtpBeginEnd.MinDate.Year, dtpBeginEnd.MinDate.Month,
                                             dtpBeginEnd.MinDate.Day, Settings.Default.BeginEndTime.Hours,
                                             Settings.Default.BeginEndTime.Minutes,
                                             Settings.Default.BeginEndTime.Seconds);
            chbNotTwoCopy.Checked = Settings.Default.OneCopyApp;
            dtpBeginEnd.ValueChanged += new EventHandler(dtpBeginEnd_ValueChanged);
            dtpMon.ValueChanged -=new EventHandler(dtpMon_ValueChanged);
            dtpTue.ValueChanged -=new EventHandler(dtpTue_ValueChanged);
            dtpWen.ValueChanged -= new EventHandler(dtpWen_ValueChanged);
            dtpThu.ValueChanged -= new EventHandler(dtpThu_ValueChanged);
            dtpFri.ValueChanged -= new EventHandler(dtpFri_ValueChanged);
            dtpSat.ValueChanged -= new EventHandler(dtpSat_ValueChanged);
            dtpSun.ValueChanged += new EventHandler(dtpSun_ValueChanged);

            chbMonday.Checked = dtpMon.Enabled = Settings.Default.Monday;
            dtpMon.Value = DateTimeHelper(dtpMon, Settings.Default.MonTime);
            chbTuesday.Checked = dtpTue.Enabled = Settings.Default.Tuesday;
            dtpTue.Value = DateTimeHelper(dtpTue, Settings.Default.TueTime);
            chbWensday.Checked = dtpWen.Enabled = Settings.Default.Wednesday;
            dtpWen.Value = DateTimeHelper(dtpWen, Settings.Default.WenTime);
            chbThursday.Checked = dtpThu.Enabled = Settings.Default.Thursday;
            dtpThu.Value = DateTimeHelper(dtpThu, Settings.Default.ThsTime);
            chbFriday.Checked = dtpFri.Enabled = Settings.Default.Friday;
            dtpFri.Value = DateTimeHelper(dtpFri, Settings.Default.FriTime);
            chbSaturday.Checked = dtpSat.Enabled = Settings.Default.Saturday;
            dtpSat.Value = DateTimeHelper(dtpSat, Settings.Default.SatTime);
            chbSunday.Checked = dtpSun.Enabled = Settings.Default.Sunday;
            dtpSun.Value = DateTimeHelper(dtpSun, Settings.Default.SunTime);

            dtpSun.ValueChanged += new EventHandler(dtpSun_ValueChanged);
            dtpSat.ValueChanged += new EventHandler(dtpSat_ValueChanged);
            dtpFri.ValueChanged += new EventHandler(dtpFri_ValueChanged);
            dtpThu.ValueChanged += new EventHandler(dtpThu_ValueChanged);
            dtpWen.ValueChanged += new EventHandler(dtpWen_ValueChanged);
            dtpTue.ValueChanged += new EventHandler(dtpTue_ValueChanged);
            dtpMon.ValueChanged += new EventHandler(dtpMon_ValueChanged);

            tbUrlOldProgXMLTV.TextChanged -= new EventHandler(tbUrlOldProg_TextChanged);
            tbUrlNewProgXMLTV.TextChanged -= new EventHandler(tbUrlNewProg_TextChanged);
            tbUrlOldProgInterTV.TextChanged -=new EventHandler(tbUrlOldProgInterTV_TextChanged);
            tbUrlNewProgInterTV.TextChanged -=new EventHandler(tbUrlNewProgInterTV_TextChanged);
            tbUrlNewProgXMLTV.Text = Settings.Default.UrlNewProgXMLTV;
            tbUrlOldProgXMLTV.Text = Settings.Default.UrlOldProgXMLTV;
            tbUrlNewProgInterTV.Text = Settings.Default.UrlNewProgInterTV;
            tbUrlOldProgInterTV.Text = Settings.Default.UrlOldProgInterTV;
            tbUrlNewProgInterTV.TextChanged += new EventHandler(tbUrlNewProgInterTV_TextChanged);
            tbUrlOldProgInterTV.TextChanged += new EventHandler(tbUrlOldProgInterTV_TextChanged);
            tbUrlNewProgXMLTV.TextChanged += new EventHandler(tbUrlNewProg_TextChanged);
            tbUrlOldProgXMLTV.TextChanged += new EventHandler(tbUrlOldProg_TextChanged);
        }
        private DateTime DateTimeHelper(DateTimePicker val, TimeSpan ts)
        {
            return new DateTime(val.MinDate.Year, val.MinDate.Month, val.MinDate.Day, ts.Hours, ts.Minutes, ts.Seconds);
        }
        private TimeSpan TimeSpanHelper(DateTimePicker dtp)
        {
            return new TimeSpan(0, dtp.Value.Hour, dtp.Value.Minute, dtp.Value.Second);
        }
        private void OtherSettings_Load(object sender, EventArgs e)
        {
            InitSettings();
        }

        private void dtpBeginEnd_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.BeginEndTime = new TimeSpan(0, dtpBeginEnd.Value.Hour, dtpBeginEnd.Value.Minute, dtpBeginEnd.Value.Second);
        }

        private void chbNotTwoCopy_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.OneCopyApp = chbNotTwoCopy.Checked;
        }

        private void dtpMon_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.MonTime = TimeSpanHelper(dtpMon);
        }

        private void dtpTue_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.TueTime = TimeSpanHelper(dtpTue);
        }

        private void chbWensday_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.Wednesday = dtpWen.Enabled = chbWensday.Checked;
        }

        private void dtpWen_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.WenTime = TimeSpanHelper(dtpWen);
        }

        private void chbMonday_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.Monday = dtpMon.Enabled = chbMonday.Checked;
        }

        private void chbTuesday_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.Tuesday = dtpTue.Enabled = chbTuesday.Checked;
        }

        private void chbThursday_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.Thursday = dtpThu.Enabled = chbThursday.Checked;
        }

        private void dtpThu_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.ThsTime = TimeSpanHelper(dtpThu);
        }

        private void chbFriday_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.Friday =  dtpFri.Enabled = chbFriday.Checked;
        }

        private void dtpFri_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.FriTime = TimeSpanHelper(dtpFri);
        }

        private void chbSaturday_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.Saturday = dtpSat.Enabled = chbSaturday.Checked;
        }

        private void dtpSat_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.SatTime = TimeSpanHelper(dtpSat);
        }

        private void chbSunday_CheckedChanged(object sender, EventArgs e)
        {
            Settings.Default.Sunday = dtpSun.Enabled = chbSunday.Checked;
        }

        private void dtpSun_ValueChanged(object sender, EventArgs e)
        {
            Settings.Default.SunTime = TimeSpanHelper(dtpSun);
        }

        private void tbUrlNewProg_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.UrlNewProgXMLTV = tbUrlNewProgXMLTV.Text;
        }

        private void tbUrlOldProg_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.UrlOldProgXMLTV = tbUrlOldProgXMLTV.Text;
        }

        private void tbUrlOldProgInterTV_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.UrlOldProgInterTV = tbUrlOldProgInterTV.Text;
        }

        private void tbUrlNewProgInterTV_TextChanged(object sender, EventArgs e)
        {
            Settings.Default.UrlNewProgInterTV = tbUrlNewProgInterTV.Text;
        }


    }
}
