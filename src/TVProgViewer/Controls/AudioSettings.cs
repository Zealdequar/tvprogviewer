using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DirectX.Capture;
using TVProgViewer.TVProgApp.Classes;
using TVProgViewer.TVProgApp.Dialogs;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp.Controls
{
    // - TODO Дополнить настройки
    // - TODO Сохранять настройки в файл
   
    public partial class AudioSettings : UserControl
    {
        public event EventHandler<CaptureMessage> SendComplete;
        private Capture _capture = null;
        public AudioSettings(Capture capture)
        {
            InitializeComponent();

            chbAudioViaPci.Checked = TunerSettings.Default.AudioViaPCI;

            _capture = capture;
            /* this.cbAudioDevices.SelectedIndexChanged += new System.EventHandler(this.cbAudioDevices_SelectedIndexChanged);
            this.cbAudioCompressors.SelectedIndexChanged += new System.EventHandler(this.cbAudioCompressors_SelectedIndexChanged);
            this.cbAudioSources.SelectedIndexChanged += new System.EventHandler(this.cbAudioSources_SelectedIndexChanged);
            this.cbAudioChannels.SelectedIndexChanged +=
                new System.EventHandler(this.cbAudioChannels_SelectedIndexChanged);
            this.cbAudioRates.SelectedIndexChanged += new System.EventHandler(this.cbAudioRates_SelectedIndexChanged);
            this.cbAudioSize.SelectedIndexChanged += new System.EventHandler(this.cbAudioSize_SelectedIndexChanged);*/
        }

        protected virtual void OnSendComplete(bool result)
        {
            if (SendComplete != null)
            {
                SendComplete(this, new CaptureMessage(result));
            }
        }

        private void UpdateCombos()
        {
            try
            {
                Filter f;
                Source current, s;
                Filter audioDevice = null;
                this.cbAudioDevices.SelectedIndexChanged -=
                    new System.EventHandler(this.cbAudioDevices_SelectedIndexChanged);
                this.cbAudioCompressors.SelectedIndexChanged -= this.cbAudioCompressors_SelectedIndexChanged;
                this.cbAudioSources.SelectedIndexChanged -=
                    new System.EventHandler(this.cbAudioSources_SelectedIndexChanged);
                this.cbAudioChannels.SelectedIndexChanged -=
                    new System.EventHandler(this.cbAudioChannels_SelectedIndexChanged);
                this.cbAudioRates.SelectedIndexChanged -= new System.EventHandler(this.cbAudioRates_SelectedIndexChanged);
                this.cbAudioSize.SelectedIndexChanged -= new System.EventHandler(this.cbAudioSize_SelectedIndexChanged);
                if (_capture != null)
                {
                    audioDevice = _capture.AudioDevice;
                }

                // Загрузка аудиоустройств:
                cbAudioDevices.Items.Clear();
                cbAudioDevices.Items.Add(Resources.NoText);
                for (int i = 0; i < Preferences.filters.AudioInputDevices.Count; i++)
                {
                    f = Preferences.filters.AudioInputDevices[i];
                    int curIndex = cbAudioDevices.Items.Add(f.Name);
                    if (audioDevice != null)
                    {
                        if (audioDevice.Name == f.Name) cbAudioDevices.SelectedIndex = curIndex;
                    }
                }
                if (audioDevice == null) cbAudioDevices.SelectedIndex = 0;
                cbAudioDevices.Enabled = (Preferences.filters.AudioInputDevices.Count > 0);

                // Загрузка алгоритмов сжатия аудио:
                try
                {
                    cbAudioCompressors.Items.Clear();
                    cbAudioCompressors.Items.Add(Resources.NoText);
                    for (int i = 0; i < Preferences.filters.AudioCompressors.Count; i++)
                    {
                        f = Preferences.filters.AudioCompressors[i];
                        int curIndex = cbAudioCompressors.Items.Add(f.Name);
                        if (_capture.AudioCompressor != null)
                        {
                            if (_capture.AudioCompressor.Name == f.Name)
                                cbAudioCompressors.SelectedIndex = curIndex;
                        }
                    }
                    if (_capture.AudioCompressor == null) cbAudioCompressors.SelectedIndex = 0;
                    cbAudioCompressors.Enabled = ((_capture.AudioDevice != null) &&
                                                  (Preferences.filters.AudioCompressors.Count > 0));
                }
                catch (Exception)
                {
                    cbAudioCompressors.Enabled = false;
                }

                // Загрузка аудио источников:
                try
                {
                    cbAudioSources.Items.Clear();
                    _capture.AudioSources = null;
                    current = _capture.AudioSource;
                    for (int i = 0; i < _capture.AudioSources.Count; i++)
                    {
                        s = _capture.AudioSources[i];
                        cbAudioSources.Items.Add(s.Name);
                    }
                    cbAudioSources.Text = current.Name;
                    cbAudioSources.Enabled = (_capture.AudioSources.Count > 0);
                    if (current != null)
                    {
                        _capture.AudioSource = current;
                    }
                }
                catch (Exception)
                {
                    cbAudioSources.Enabled = false;
                }

                // Загрузка аудио каналов:
                try
                {
                    cbAudioChannels.Items.Clear();
                    short audioChannels = _capture.AudioChannels;
                    cbAudioChannels.Items.Add(Resources.MonoText);
                    cbAudioChannels.Items.Add(Resources.StereoText);
                    cbAudioChannels.SelectedIndex = (audioChannels == 1) ? 0 : 1;
                    cbAudioChannels.Enabled = true;
                }
                catch
                {
                    cbAudioChannels.Enabled = false;
                }

                // Загрузка аудио-частот:
                try
                {
                    cbAudioRates.Items.Clear();
                    int samplingRate = _capture.AudioSamplingRate;
                    cbAudioRates.Items.Add("8 " + Resources.KhzText);
                    cbAudioRates.Items.Add("11,025 " + Resources.KhzText);
                    cbAudioRates.Items.Add("22,05 " + Resources.KhzText);
                    cbAudioRates.Items.Add("32 " + Resources.KhzText);
                    cbAudioRates.Items.Add("44,1 " + Resources.KhzText);
                    cbAudioRates.Items.Add("48 " + Resources.KhzText);
                    cbAudioRates.Text = (samplingRate == 8000
                                             ? "8 " + Resources.KhzText
                                             : (samplingRate == 11025
                                                    ? "11,025 " + Resources.KhzText
                                                    : (samplingRate == 22050
                                                           ? "22,05 " + Resources.KhzText
                                                           : (samplingRate == 32000
                                                                  ? "32 " + Resources.KhzText
                                                                  : (samplingRate == 44100
                                                                         ? "44,1 " + Resources.KhzText
                                                                         : (samplingRate == 48000
                                                                                ? "48 " + Resources.KhzText
                                                                                : null))))));
                    cbAudioRates.Enabled = true;
                }
                catch (Exception)
                {
                    cbAudioRates.Enabled = false;
                }

                // Загрузка размеров аудио:
                try
                {
                    cbAudioSize.Items.Clear();
                    short sampleSize = _capture.AudioSampleSize;
                    cbAudioSize.Items.Add("8 " + Resources.BitText);
                    cbAudioSize.Items.Add("16 " + Resources.BitText);
                    cbAudioSize.Text = (sampleSize == 8
                                            ? "8 " + Resources.BitText
                                            : (sampleSize == 16 ? "16 " + Resources.BitText : null));
                    cbAudioSize.Enabled = true;
                }
                catch (Exception ex)
                {
                    Statics.EL.LogException(ex);
                    cbAudioSize.Enabled = false;
                }
                this.cbAudioDevices.SelectedIndexChanged +=
                    new System.EventHandler(this.cbAudioDevices_SelectedIndexChanged);
                this.cbAudioCompressors.SelectedIndexChanged +=
                    new System.EventHandler(this.cbAudioCompressors_SelectedIndexChanged);
                this.cbAudioSources.SelectedIndexChanged +=
                    new System.EventHandler(this.cbAudioSources_SelectedIndexChanged);
                this.cbAudioChannels.SelectedIndexChanged +=
                    new System.EventHandler(this.cbAudioChannels_SelectedIndexChanged);
                this.cbAudioRates.SelectedIndexChanged += new System.EventHandler(this.cbAudioRates_SelectedIndexChanged);
                this.cbAudioSize.SelectedIndexChanged += new System.EventHandler(this.cbAudioSize_SelectedIndexChanged);
            }
            catch (Exception ex)
            {
            }
        }
        
        /// <summary>
        /// При изменении выбора аудиодевайса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbAudioDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Получить текущие устройства и уничтожить объект capture
                // потому что видео и аудио устройства могут быть
                // измененными при создании нового объекта Capture
                Filter videoDevice = null;
                Filter audioDevice = null;
                if (_capture != null)
                {
                    videoDevice = _capture.VideoDevice;
                    audioDevice = _capture.AudioDevice;
                    _capture.Dispose();
                    _capture = null;
                }
                // Получить новое аудиоустройство
                audioDevice = (cbAudioDevices.SelectedIndex > 0)
                                  ? Preferences.filters.AudioInputDevices[cbAudioDevices.SelectedIndex - 1]
                                  : null;

                // Создать объект Capture
                if ((videoDevice != null) || (audioDevice != null))
                {
                    _capture = new Capture(videoDevice, audioDevice, TunerSettings.Default.AudioViaPCI);
                    OnSendComplete(true);
                    _capture.CaptureComplete += new EventHandler(capture_CaptureComplete);
                    TunerSettings.Default.AudioDevice = (audioDevice != null) ? audioDevice.MonikerString : String.Empty;
                }
                UpdateCombos();
            }
            catch (Exception ex)
            {
                Statics.EL.LogException(ex);
                Statics.ShowDialog(Resources.WarningText, Resources.AudioDeviceNotSupportText,
                                   MessageDialog.MessageIcon.Alert, MessageDialog.MessageButtons.Ok);
                OnSendComplete(false);
            }
        }

        /// <summary>
        /// При изменении выбора аудиокодека
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbAudioCompressors_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Изменить аудиокодек:
                if (_capture != null)
                {
                    _capture.AudioCompressor = ((cbAudioCompressors.Items.Count > 0)
                                                               ? Preferences.filters.AudioCompressors[
                                                                   cbAudioCompressors.SelectedIndex - 1]
                                                               : null);
                    TunerSettings.Default.AudioCompressor = (_capture.AudioCompressor != null)
                                                           ? _capture.AudioCompressor.MonikerString
                                                           : String.Empty;
                    UpdateCombos();
                }
            }
            catch (Exception ex)
            {
                Statics.EL.LogException(ex);
                Statics.ShowDialog(Resources.WarningText, Resources.AudioCodecNotSupportText,
                                   MessageDialog.MessageIcon.Alert, MessageDialog.MessageButtons.Ok);
            }
        }
        
        /// <summary>
        /// При изменении выбора аудиоисточника
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbAudioSources_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Выбор аудиоисточника
                _capture.AudioSources = null;
                _capture.AudioSource = _capture.AudioSources[cbAudioSources.SelectedIndex];
                TunerSettings.Default.AudioSource = cbAudioSources.SelectedIndex;
                UpdateCombos();
            }
            catch (Exception ex)
            {
                Statics.EL.LogException(ex);
                Statics.ShowDialog(Resources.WarningText,
                                   Resources.AudioSourceNotSupportText +
                                   ex.Message + "\n\n" + ex.ToString(), MessageDialog.MessageIcon.Alert,
                                   MessageDialog.MessageButtons.Ok);
            }
        }
        
        /// <summary>
        /// При изменении выбора аудио канала
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbAudioChannels_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                TunerSettings.Default.AudioChannel =
                    _capture.AudioChannels = (short) Math.Pow(2, cbAudioChannels.SelectedIndex);
                UpdateCombos();
            }
            catch (Exception ex)
            {
                Statics.EL.LogException(ex);
                Statics.ShowDialog(Resources.WarningText,
                                   Resources.AudioChannelsNotSupportText + ex.Message + "\n\n" + ex.ToString(),
                                   MessageDialog.MessageIcon.Alert, MessageDialog.MessageButtons.Ok);
            }
        }
        
        /// <summary>
        /// При изменении выбора аудио-частоты
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbAudioRates_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string[] s = cbAudioRates.Text.Split(' ');
                // Парсинг зависит от региональных настроек: в странах Европы числовой
                // разделитель (целых и дробных) отделяется запятой, в Объединенном Королевстве это точка:
                // 44,1 кГц в Европе и 44.1 кГц в Соединенном Королевстве
                int samplingRate = (int) (double.Parse(s[0])*1000);
                TunerSettings.Default.AudioRate = _capture.AudioSamplingRate = samplingRate;
                UpdateCombos();
            }
            catch(Exception ex)
            {
                Statics.EL.LogException(ex);
                Statics.ShowDialog(Resources.WarningText,
                                   Resources.AudioRateNotSupportText + ex.Message + "\n\n" + ex.ToString(),
                                   MessageDialog.MessageIcon.Alert, MessageDialog.MessageButtons.Ok);
            }
        }
        
        /// <summary>
        /// При изменении выбора размера линии аудио
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbAudioSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string[] s = cbAudioSize.Text.Split(' ');
                short sampleSize = short.Parse(s[0]);
                TunerSettings.Default.AudioSize = _capture.AudioSampleSize = sampleSize;
                UpdateCombos();
            }
            catch (Exception ex)
            {
                Statics.EL.LogException(ex);
                Statics.ShowDialog(Resources.WarningText,
                                   Resources.AudioSizeNotSupportText + ex.Message + "\n\n" + ex.ToString(),
                                   MessageDialog.MessageIcon.Alert, MessageDialog.MessageButtons.Ok);
            }
        }
        
       

        /// <summary>
        /// Демонстрация успешного завершения создания объекта захвата
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void capture_CaptureComplete(object sender, EventArgs e)
        {
            Debug.WriteLine("Настройка Capture успешно завершена");
        }

        /// <summary>
        /// Включить/выключить аудио через карту PCI
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chbAudioViaPci_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                // Получить текущие устройства и уничтожить объект capture
                // потому что видео и аудио устройства могут быть
                // измененными при создании нового объекта Capture
                Filter videoDevice = null;
                Filter audioDevice = null;
                if (_capture != null)
                {
                    videoDevice = _capture.VideoDevice;
                    audioDevice = _capture.AudioDevice;
                    _capture.Dispose();
                    _capture = null;
                }
                // Создать объект Capture
                if ((videoDevice != null) || (audioDevice != null))
                {
                    _capture = new Capture(videoDevice, audioDevice, TunerSettings.Default.AudioViaPCI = chbAudioViaPci.Checked);
                    OnSendComplete(true);
                    _capture.CaptureComplete += new EventHandler(capture_CaptureComplete);
                }
                UpdateCombos();
            }
            catch (Exception ex)
            {
                Statics.EL.LogException(ex);
                Statics.ShowDialog(Resources.WarningText, Resources.AudioDeviceNotSupportText,
                                   MessageDialog.MessageIcon.Alert, MessageDialog.MessageButtons.Ok);
                OnSendComplete(false);
            }
        }

        /// <summary>
        /// Возможности аудио
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAudioCaps_Click(object sender, EventArgs e)
        {
            try
            {
                string s;
                s = String.Format(
                    @Resources.AudioCapabilitiesText +
                    "--------------------------------\n\n" +
                    @Resources.MinNumberChannelsText +
                    @Resources.MaxNumberChannelsText +
                    @Resources.ChannelsGranularityText +
                    "\n" +
                    @Resources.AudioMinSizeLinesText +
                    @Resources.AudioMaxSizeLinesText +
                    @Resources.SizeGranularityText +
                    "\n" +
                    @Resources.MinSamplingRateText +
                    @Resources.MaxSamplingRateText +
                    @Resources.SamplingRateGranularityText,
                    _capture.AudioCaps.MinimumChannels,
                    _capture.AudioCaps.MaximumChannels,
                    _capture.AudioCaps.ChannelsGranularity,
                    _capture.AudioCaps.MinimumSampleSize,
                    _capture.AudioCaps.MaximumSampleSize,
                    _capture.AudioCaps.SampleSizeGranularity,
                    _capture.AudioCaps.MinimumSamplingRate,
                    _capture.AudioCaps.MaximumSamplingRate,
                    _capture.AudioCaps.SamplingRateGranularity);
                MessageBox.Show(s);
            }
            catch (Exception ex)
            {
                Statics.EL.LogException(ex);
                Statics.ShowDialog(Resources.WarningText,
                                   Resources.UnableDisplayAudioCapText + ex.Message + "\n\n" + ex.ToString(),
                                   MessageDialog.MessageIcon.Alert, MessageDialog.MessageButtons.Ok);
            }
        }

        private void AudioSettings_Load(object sender, EventArgs e)
        {
            UpdateCombos();
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
        }
    }
    
    public class CaptureMessage : EventArgs
    {
        public bool Complete { get; private set; }
        public CaptureMessage(bool complete)
        {
            this.Complete = complete;
        }
    }
}
