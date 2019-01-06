using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DShowNET;
using DirectX.Capture;
using TVProgViewer.TVProgApp.Classes;
using TVProgViewer.TVProgApp.Controls;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp.Dialogs
{
    public partial class OptionsDialog : Form
    {
        private readonly AudioSettings audioSettings;
        private readonly VideoSettings videoSettings;
        private readonly CountryDep countryDep;
        private readonly AppierenceSettings appearance = new AppierenceSettings(); 
        private readonly VisibleSettings view = new VisibleSettings();
        private readonly OnRunningSettings onRunning = new OnRunningSettings();
        private readonly ReminderSettings reminder = new ReminderSettings();
        private readonly OtherSettings other = new OtherSettings();
        public bool AudioViaPCI { get; set; }
        public event EventHandler<CaptureMessage> SendComplete;
        protected virtual void OnSendComplete (bool result)
        {
            if (SendComplete != null)
            {
                SendComplete(this, new CaptureMessage(result));
            }
        }
       
        public OptionsDialog(Capture capture)
        {
            InitializeComponent();
            audioSettings = new AudioSettings(capture);
            videoSettings = new VideoSettings(capture);
            countryDep = new CountryDep(capture);
            videoSettings.SendComplete += new EventHandler<CaptureMessage>(tvTunerDevice_SendComplete);
        }
        

        void tvTunerDevice_SendComplete(object sender, CaptureMessage e)
        {
            if (this.InvokeRequired)
            {
                Invoke(new EventHandler<CaptureMessage>(tvTunerDevice_SendComplete), sender, e);
            }
            else OnSendComplete(e.Complete);
        }

        private void AddControlToPanel(Control ctrl)
        {
            panel.Controls.Add(ctrl);
            ctrl.Dock = DockStyle.Fill;
        }

        private void tvpvOptions_AfterSelect(object sender, TreeViewEventArgs e)
        {
            panel.Controls.Clear();
            
            if (e.Node == tvpvOptions.Nodes["nPublicAppearance"])
            {
                AddControlToPanel(appearance);
            }
            if (e.Node == tvpvOptions.Nodes["nView"])
            {
                AddControlToPanel(view);
            }
            if (e.Node == tvpvOptions.Nodes["nOnRunning"])
            {
                AddControlToPanel(onRunning);
            }
            if (e.Node == tvpvOptions.Nodes["nReminder"])
            {
                AddControlToPanel(reminder);
            }
            // Настройки ТВ-тюнера
            // Региональные настройки:
            if (e.Node == tvpvOptions.Nodes["tvTuner"].Nodes["nCountryDep"])
            {
                AddControlToPanel(countryDep);
            }
            // Настройки видео:
            if(e.Node ==  tvpvOptions.Nodes["tvTuner"].Nodes["nVideo"])
            {
                AddControlToPanel(videoSettings);
            }
            // Настройки аудио:
            if (e.Node == tvpvOptions.Nodes["tvTuner"].Nodes["nAudio"])
            {
                AddControlToPanel(audioSettings);
            }
            // Другие настройки:
            if (e.Node == tvpvOptions.Nodes["nOther"])
            {
                AddControlToPanel(other);
            }
            this.Text = "Общие настройки: " + e.Node.Text;
        }
    }
}
