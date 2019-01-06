namespace TVProgViewer.TVProgApp.Controls
{
    partial class OtherSettings
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OtherSettings));
            this.label1 = new System.Windows.Forms.Label();
            this.dtpBeginEnd = new System.Windows.Forms.DateTimePicker();
            this.chbNotTwoCopy = new System.Windows.Forms.CheckBox();
            this.gbShceduler = new System.Windows.Forms.GroupBox();
            this.tlpRemRestriction = new System.Windows.Forms.TableLayoutPanel();
            this.chbMonday = new System.Windows.Forms.CheckBox();
            this.dtpMon = new System.Windows.Forms.DateTimePicker();
            this.chbTuesday = new System.Windows.Forms.CheckBox();
            this.dtpTue = new System.Windows.Forms.DateTimePicker();
            this.chbWensday = new System.Windows.Forms.CheckBox();
            this.dtpFri = new System.Windows.Forms.DateTimePicker();
            this.dtpSun = new System.Windows.Forms.DateTimePicker();
            this.chbSunday = new System.Windows.Forms.CheckBox();
            this.dtpWen = new System.Windows.Forms.DateTimePicker();
            this.chbThursday = new System.Windows.Forms.CheckBox();
            this.dtpThu = new System.Windows.Forms.DateTimePicker();
            this.chbFriday = new System.Windows.Forms.CheckBox();
            this.chbSaturday = new System.Windows.Forms.CheckBox();
            this.dtpSat = new System.Windows.Forms.DateTimePicker();
            this.pUrls = new System.Windows.Forms.Panel();
            this.tbUrlNewProgInterTV = new System.Windows.Forms.TextBox();
            this.tbUrlOldProgInterTV = new System.Windows.Forms.TextBox();
            this.tbUrlNewProgXMLTV = new System.Windows.Forms.TextBox();
            this.tbUrlOldProgXMLTV = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.gbShceduler.SuspendLayout();
            this.tlpRemRestriction.SuspendLayout();
            this.pUrls.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // dtpBeginEnd
            // 
            resources.ApplyResources(this.dtpBeginEnd, "dtpBeginEnd");
            this.dtpBeginEnd.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpBeginEnd.Name = "dtpBeginEnd";
            this.dtpBeginEnd.ShowUpDown = true;
            this.dtpBeginEnd.Value = new System.DateTime(2012, 1, 31, 5, 45, 0, 0);
            this.dtpBeginEnd.ValueChanged += new System.EventHandler(this.dtpBeginEnd_ValueChanged);
            // 
            // chbNotTwoCopy
            // 
            resources.ApplyResources(this.chbNotTwoCopy, "chbNotTwoCopy");
            this.chbNotTwoCopy.Checked = true;
            this.chbNotTwoCopy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbNotTwoCopy.Name = "chbNotTwoCopy";
            this.chbNotTwoCopy.UseVisualStyleBackColor = true;
            this.chbNotTwoCopy.CheckedChanged += new System.EventHandler(this.chbNotTwoCopy_CheckedChanged);
            // 
            // gbShceduler
            // 
            this.gbShceduler.Controls.Add(this.tlpRemRestriction);
            resources.ApplyResources(this.gbShceduler, "gbShceduler");
            this.gbShceduler.Name = "gbShceduler";
            this.gbShceduler.TabStop = false;
            // 
            // tlpRemRestriction
            // 
            resources.ApplyResources(this.tlpRemRestriction, "tlpRemRestriction");
            this.tlpRemRestriction.Controls.Add(this.chbMonday, 0, 0);
            this.tlpRemRestriction.Controls.Add(this.dtpMon, 1, 0);
            this.tlpRemRestriction.Controls.Add(this.chbTuesday, 0, 1);
            this.tlpRemRestriction.Controls.Add(this.dtpTue, 1, 1);
            this.tlpRemRestriction.Controls.Add(this.chbWensday, 0, 2);
            this.tlpRemRestriction.Controls.Add(this.dtpFri, 1, 4);
            this.tlpRemRestriction.Controls.Add(this.dtpSun, 1, 6);
            this.tlpRemRestriction.Controls.Add(this.chbSunday, 0, 6);
            this.tlpRemRestriction.Controls.Add(this.dtpWen, 1, 2);
            this.tlpRemRestriction.Controls.Add(this.chbThursday, 0, 3);
            this.tlpRemRestriction.Controls.Add(this.dtpThu, 1, 3);
            this.tlpRemRestriction.Controls.Add(this.chbFriday, 0, 4);
            this.tlpRemRestriction.Controls.Add(this.chbSaturday, 0, 5);
            this.tlpRemRestriction.Controls.Add(this.dtpSat, 1, 5);
            this.tlpRemRestriction.Name = "tlpRemRestriction";
            // 
            // chbMonday
            // 
            resources.ApplyResources(this.chbMonday, "chbMonday");
            this.chbMonday.Name = "chbMonday";
            this.chbMonday.UseVisualStyleBackColor = true;
            this.chbMonday.CheckedChanged += new System.EventHandler(this.chbMonday_CheckedChanged);
            // 
            // dtpMon
            // 
            resources.ApplyResources(this.dtpMon, "dtpMon");
            this.dtpMon.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpMon.Name = "dtpMon";
            this.dtpMon.ShowUpDown = true;
            this.dtpMon.ValueChanged += new System.EventHandler(this.dtpMon_ValueChanged);
            // 
            // chbTuesday
            // 
            resources.ApplyResources(this.chbTuesday, "chbTuesday");
            this.chbTuesday.Name = "chbTuesday";
            this.chbTuesday.UseVisualStyleBackColor = true;
            this.chbTuesday.CheckedChanged += new System.EventHandler(this.chbTuesday_CheckedChanged);
            // 
            // dtpTue
            // 
            resources.ApplyResources(this.dtpTue, "dtpTue");
            this.dtpTue.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpTue.Name = "dtpTue";
            this.dtpTue.ShowUpDown = true;
            this.dtpTue.ValueChanged += new System.EventHandler(this.dtpTue_ValueChanged);
            // 
            // chbWensday
            // 
            resources.ApplyResources(this.chbWensday, "chbWensday");
            this.chbWensday.Name = "chbWensday";
            this.chbWensday.UseVisualStyleBackColor = true;
            this.chbWensday.CheckedChanged += new System.EventHandler(this.chbWensday_CheckedChanged);
            // 
            // dtpFri
            // 
            resources.ApplyResources(this.dtpFri, "dtpFri");
            this.dtpFri.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpFri.Name = "dtpFri";
            this.dtpFri.ShowUpDown = true;
            this.dtpFri.ValueChanged += new System.EventHandler(this.dtpFri_ValueChanged);
            // 
            // dtpSun
            // 
            resources.ApplyResources(this.dtpSun, "dtpSun");
            this.dtpSun.CalendarForeColor = System.Drawing.Color.Maroon;
            this.dtpSun.CalendarTitleForeColor = System.Drawing.Color.Maroon;
            this.dtpSun.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpSun.Name = "dtpSun";
            this.dtpSun.ShowUpDown = true;
            this.dtpSun.ValueChanged += new System.EventHandler(this.dtpSun_ValueChanged);
            // 
            // chbSunday
            // 
            resources.ApplyResources(this.chbSunday, "chbSunday");
            this.chbSunday.ForeColor = System.Drawing.Color.Maroon;
            this.chbSunday.Name = "chbSunday";
            this.chbSunday.UseVisualStyleBackColor = true;
            this.chbSunday.CheckedChanged += new System.EventHandler(this.chbSunday_CheckedChanged);
            // 
            // dtpWen
            // 
            resources.ApplyResources(this.dtpWen, "dtpWen");
            this.dtpWen.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpWen.Name = "dtpWen";
            this.dtpWen.ShowUpDown = true;
            this.dtpWen.ValueChanged += new System.EventHandler(this.dtpWen_ValueChanged);
            // 
            // chbThursday
            // 
            resources.ApplyResources(this.chbThursday, "chbThursday");
            this.chbThursday.Name = "chbThursday";
            this.chbThursday.UseVisualStyleBackColor = true;
            this.chbThursday.CheckedChanged += new System.EventHandler(this.chbThursday_CheckedChanged);
            // 
            // dtpThu
            // 
            resources.ApplyResources(this.dtpThu, "dtpThu");
            this.dtpThu.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpThu.Name = "dtpThu";
            this.dtpThu.ShowUpDown = true;
            this.dtpThu.ValueChanged += new System.EventHandler(this.dtpThu_ValueChanged);
            // 
            // chbFriday
            // 
            resources.ApplyResources(this.chbFriday, "chbFriday");
            this.chbFriday.Name = "chbFriday";
            this.chbFriday.UseVisualStyleBackColor = true;
            this.chbFriday.CheckedChanged += new System.EventHandler(this.chbFriday_CheckedChanged);
            // 
            // chbSaturday
            // 
            resources.ApplyResources(this.chbSaturday, "chbSaturday");
            this.chbSaturday.ForeColor = System.Drawing.Color.Maroon;
            this.chbSaturday.Name = "chbSaturday";
            this.chbSaturday.UseVisualStyleBackColor = true;
            this.chbSaturday.CheckedChanged += new System.EventHandler(this.chbSaturday_CheckedChanged);
            // 
            // dtpSat
            // 
            resources.ApplyResources(this.dtpSat, "dtpSat");
            this.dtpSat.CalendarForeColor = System.Drawing.Color.Maroon;
            this.dtpSat.CalendarTitleForeColor = System.Drawing.Color.Maroon;
            this.dtpSat.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.dtpSat.Name = "dtpSat";
            this.dtpSat.ShowUpDown = true;
            this.dtpSat.ValueChanged += new System.EventHandler(this.dtpSat_ValueChanged);
            // 
            // pUrls
            // 
            this.pUrls.Controls.Add(this.tbUrlNewProgInterTV);
            this.pUrls.Controls.Add(this.tbUrlOldProgInterTV);
            this.pUrls.Controls.Add(this.tbUrlNewProgXMLTV);
            this.pUrls.Controls.Add(this.tbUrlOldProgXMLTV);
            this.pUrls.Controls.Add(this.label5);
            this.pUrls.Controls.Add(this.label4);
            this.pUrls.Controls.Add(this.label3);
            this.pUrls.Controls.Add(this.label2);
            resources.ApplyResources(this.pUrls, "pUrls");
            this.pUrls.Name = "pUrls";
            // 
            // tbUrlNewProgInterTV
            // 
            resources.ApplyResources(this.tbUrlNewProgInterTV, "tbUrlNewProgInterTV");
            this.tbUrlNewProgInterTV.Name = "tbUrlNewProgInterTV";
            this.tbUrlNewProgInterTV.TextChanged += new System.EventHandler(this.tbUrlNewProgInterTV_TextChanged);
            // 
            // tbUrlOldProgInterTV
            // 
            resources.ApplyResources(this.tbUrlOldProgInterTV, "tbUrlOldProgInterTV");
            this.tbUrlOldProgInterTV.Name = "tbUrlOldProgInterTV";
            this.tbUrlOldProgInterTV.TextChanged += new System.EventHandler(this.tbUrlOldProgInterTV_TextChanged);
            // 
            // tbUrlNewProgXMLTV
            // 
            resources.ApplyResources(this.tbUrlNewProgXMLTV, "tbUrlNewProgXMLTV");
            this.tbUrlNewProgXMLTV.Name = "tbUrlNewProgXMLTV";
            this.tbUrlNewProgXMLTV.TextChanged += new System.EventHandler(this.tbUrlNewProg_TextChanged);
            // 
            // tbUrlOldProgXMLTV
            // 
            resources.ApplyResources(this.tbUrlOldProgXMLTV, "tbUrlOldProgXMLTV");
            this.tbUrlOldProgXMLTV.Name = "tbUrlOldProgXMLTV";
            this.tbUrlOldProgXMLTV.TextChanged += new System.EventHandler(this.tbUrlOldProg_TextChanged);
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // OtherSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbShceduler);
            this.Controls.Add(this.pUrls);
            this.Controls.Add(this.chbNotTwoCopy);
            this.Controls.Add(this.dtpBeginEnd);
            this.Controls.Add(this.label1);
            this.Name = "OtherSettings";
            this.Load += new System.EventHandler(this.OtherSettings_Load);
            this.gbShceduler.ResumeLayout(false);
            this.tlpRemRestriction.ResumeLayout(false);
            this.tlpRemRestriction.PerformLayout();
            this.pUrls.ResumeLayout(false);
            this.pUrls.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dtpBeginEnd;
        private System.Windows.Forms.CheckBox chbNotTwoCopy;
        private System.Windows.Forms.GroupBox gbShceduler;
        private System.Windows.Forms.TableLayoutPanel tlpRemRestriction;
        private System.Windows.Forms.CheckBox chbMonday;
        private System.Windows.Forms.CheckBox chbTuesday;
        private System.Windows.Forms.CheckBox chbWensday;
        private System.Windows.Forms.CheckBox chbFriday;
        private System.Windows.Forms.CheckBox chbThursday;
        private System.Windows.Forms.CheckBox chbSaturday;
        private System.Windows.Forms.CheckBox chbSunday;
        private System.Windows.Forms.DateTimePicker dtpMon;
        private System.Windows.Forms.DateTimePicker dtpTue;
        private System.Windows.Forms.DateTimePicker dtpWen;
        private System.Windows.Forms.DateTimePicker dtpThu;
        private System.Windows.Forms.DateTimePicker dtpFri;
        private System.Windows.Forms.DateTimePicker dtpSat;
        private System.Windows.Forms.DateTimePicker dtpSun;
        private System.Windows.Forms.Panel pUrls;
        private System.Windows.Forms.TextBox tbUrlOldProgXMLTV;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbUrlNewProgXMLTV;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbUrlNewProgInterTV;
        private System.Windows.Forms.TextBox tbUrlOldProgInterTV;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
    }
}
