using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DirectX.Capture;
using DShowNET;
using TVProgViewer.TVProgApp.Classes;
using TVProgViewer.TVProgApp.Dialogs;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp.Controls
{
    /// <summary>
    /// Настройки видеоустройства
    /// </summary>
    public partial class VideoSettings : UserControl
    {
        private Capture _capture = null;
        
        public event EventHandler<CaptureMessage> SendComplete;
        FileNameDialog fnDlg = new FileNameDialog();
        public VideoSettings(Capture capture)
        {
            InitializeComponent();
            _capture = capture;
            this.cbVideoDevices.SelectedIndexChanged += new System.EventHandler(this.cbVideoDevices_SelectedIndexChanged);
            this.cbVideoCompressors.SelectedIndexChanged += new System.EventHandler(this.cbVideoCompressors_SelectedIndexChanged);
            this.cbVideoSources.SelectedIndexChanged += new System.EventHandler(this.cbVideoSources_SelectedIndexChanged);
            this.cbVideoStandart.SelectedIndexChanged += new System.EventHandler(this.cbVideoStandart_SelectedIndexChanged);
            this.cbFrameSize.SelectedIndexChanged += new System.EventHandler(this.cbFrameSize_SelectedIndexChanged);
            this.cbVideoRates.SelectedIndexChanged += new System.EventHandler(this.cbVideoRates_SelectedIndexChanged);
            this.cbColorSpace.SelectedIndexChanged += new System.EventHandler(this.cbColorSpace_SelectedIndexChanged);
            this.cbAVRecFileModes.SelectedIndexChanged += new System.EventHandler(this.cbAVRecFileModes_SelectedIndexChanged);

            
            if (TunerSettings.Default.CaptureDir == String.Empty)
            {
                tbDirCapture.Text = TunerSettings.Default.CaptureDir = Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
                Settings.Default.Save();
            }
            else
            {
                tbDirCapture.Text = TunerSettings.Default.CaptureDir;
            }
            if (TunerSettings.Default.PatternCaptureFileName == String.Empty)
            {
                tbCaptureFileName.Text = TunerSettings.Default.PatternCaptureFileName = Resources.DateTimeProgramTelecastText;
                Settings.Default.Save();
            }
            else
            {
                tbCaptureFileName.Text = TunerSettings.Default.PatternCaptureFileName;
            }
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
                Filter videoDevice = null;
                this.cbVideoDevices.SelectedIndexChanged -=
                    new System.EventHandler(this.cbVideoDevices_SelectedIndexChanged);
                this.cbVideoCompressors.SelectedIndexChanged -=
                    new System.EventHandler(this.cbVideoCompressors_SelectedIndexChanged);
                this.cbVideoSources.SelectedIndexChanged -=
                    new System.EventHandler(this.cbVideoSources_SelectedIndexChanged);
                this.cbVideoStandart.SelectedIndexChanged -=
                    new System.EventHandler(this.cbVideoStandart_SelectedIndexChanged);
                this.cbFrameSize.SelectedIndexChanged -= new System.EventHandler(this.cbFrameSize_SelectedIndexChanged);
                this.cbVideoRates.SelectedIndexChanged -= new System.EventHandler(this.cbVideoRates_SelectedIndexChanged);
                this.cbColorSpace.SelectedIndexChanged -= new System.EventHandler(this.cbColorSpace_SelectedIndexChanged);
                this.cbAVRecFileModes.SelectedIndexChanged -=
                    new System.EventHandler(this.cbAVRecFileModes_SelectedIndexChanged);

                if (_capture != null)
                {
                    videoDevice = _capture.VideoDevice;
                }
                // Загрузка видеоустройств:
                cbVideoDevices.Items.Clear();
                cbVideoDevices.Items.Add(Resources.NoText);
                for (int i = 0; i < Preferences.filters.VideoInputDevices.Count; i++)
                {
                    f = Preferences.filters.VideoInputDevices[i];
                    int curIndex = cbVideoDevices.Items.Add(f.Name);
                    if (videoDevice != null)
                    {
                        if (videoDevice.Name == f.Name) cbVideoDevices.SelectedIndex = curIndex;
                    }
                }
                if (videoDevice == null) cbVideoDevices.SelectedIndex = 0;
                cbVideoDevices.Enabled = (Preferences.filters.VideoInputDevices.Count > 0);
                if (cbVideoDevices.SelectedIndex > 0)
                {
                    // Загрузка алгоритмов сжатия видео:
                    try
                    {
                        cbVideoCompressors.Items.Clear();
                        cbVideoCompressors.Items.Add(Resources.NoText);
                        for (int i = 0; i < Preferences.filters.VideoCompressors.Count; i++)
                        {
                            f = Preferences.filters.VideoCompressors[i];
                            int curIndex = cbVideoCompressors.Items.Add(f.Name);
                            if (_capture.VideoCompressor != null)
                            {
                                if (_capture.VideoCompressor.Name == f.Name)
                                    cbVideoCompressors.SelectedIndex = curIndex;
                            }
                        }
                        if (_capture.VideoCompressor == null) cbVideoCompressors.SelectedIndex = 0;
                        cbVideoCompressors.Enabled = ((_capture.VideoDevice != null) &&
                                                      (Preferences.filters.VideoCompressors.Count > 0));
                    }
                    catch (Exception ex)
                    {
                        Statics.EL.LogException(ex);
                        cbVideoCompressors.Enabled = false;
                    }

                    // Загрузка видео источников:
                    try
                    {
                        cbVideoSources.Items.Clear();
                        _capture.VideoSources = null;
                        current = _capture.VideoSource;
                        for (int i = 0; i < _capture.VideoSources.Count; i++)
                        {
                            s = _capture.VideoSources[i];
                            cbVideoSources.Items.Add(s.Name);
                        }
                        cbVideoSources.Text = current.Name;
                        cbVideoSources.Enabled = (_capture.VideoSources.Count > 0);
                        if (current != null)
                        {
                            _capture.VideoSource = current;
                        }
                    }
                    catch (Exception ex)
                    {
                        Statics.EL.LogException(ex);
                        cbVideoSources.Enabled = false;
                    }


                    // Загрузка видеостандартов:
                    cbVideoStandart.Items.Clear();

                    if ((_capture != null) &&
                        (_capture.dxUtils != null) && (_capture.dxUtils.VideoDecoderAvail))
                    {
                        try
                        {
                            if (TunerSettings.Default.VideoStandard != String.Empty)
                            {
                                AnalogVideoStandard a =
                                    (AnalogVideoStandard)
                                    Enum.Parse(typeof (AnalogVideoStandard), TunerSettings.Default.VideoStandard);
                                _capture.dxUtils.VideoStandard = a;
                            }
                            AnalogVideoStandard currentStandard = _capture.dxUtils.VideoStandard;
                            AnalogVideoStandard avaliableStandards = _capture.dxUtils.AvailableVideoStandards;
                            int mask = 1;
                            int item = 0;
                            int currItem = -1;
                            while (mask <= (int) AnalogVideoStandard.PAL_N_COMBO)
                            {
                                int avs = mask & (int) avaliableStandards;
                                if (avs != 0)
                                {
                                    cbVideoStandart.Items.Add(((AnalogVideoStandard) avs).ToString());
                                    if (currentStandard == (AnalogVideoStandard) avs)
                                    {
                                        currItem = item;
                                    }
                                    item++;
                                }
                                mask *= 2;
                            }
                            cbVideoStandart.Enabled = (cbVideoStandart.Items.Count > 1);
                            if (currItem >= 0)
                            {
                                cbVideoStandart.SelectedIndex = currItem;
                            }
                            else
                            {
                                if (cbVideoStandart.Items.Count > 0)
                                {
                                    cbVideoStandart.SelectedIndex = 0;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Statics.EL.LogException(ex);
                            cbVideoStandart.Enabled = false;
                        }
                    }

                    // Загрузка размеров фрейма:
                    try
                    {
                        cbFrameSize.Items.Clear();
                        Size frameSize = _capture.FrameSize;
                        cbFrameSize.Items.Add("160 x 120");
                        cbFrameSize.Items.Add("320 x 240");

                        // Добавить Pal форматы:
                        cbFrameSize.Items.Add("352 x 288");
                        cbFrameSize.Items.Add("640 x 480");

                        // Добавить Ntsc форматы:
                        cbFrameSize.Items.Add("720 x 480");

                        // Добавить некоторые Pal-форматы:
                        cbFrameSize.Items.Add("720 x 576");
                        cbFrameSize.Items.Add("768 x 576");

                        cbFrameSize.Text = (frameSize == new Size(160, 120)
                                                ? "160 x 120"
                                                : frameSize == new Size(320, 240)
                                                      ? "320 x 240"
                                                      : frameSize == new Size(352, 288)
                                                            ? "352 x 288"
                                                            : frameSize == new Size(640, 480)
                                                                  ? "640 x 480"
                                                                  : frameSize == new Size(720, 480)
                                                                        ? "720 x 480"
                                                                        : frameSize == new Size(720, 576)
                                                                              ? "720 x 576"
                                                                              : frameSize == new Size(768, 576)
                                                                                    ? "768 x 576"
                                                                                    : null);
                        cbFrameSize.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        Statics.EL.LogException(ex);
                        cbFrameSize.Enabled = false;
                    }

                    // Загрузка частот видеопотока:
                    try
                    {
                        cbVideoRates.Items.Clear();
                        var frameRate = (int) (_capture.FrameRate*1000);
                        cbVideoRates.Items.Add("15 " + Resources.KadrovText);
                        cbVideoRates.Items.Add("24 " + Resources.KadraText + Resources.FilmText);
                        cbVideoRates.Items.Add("25 " + Resources.KadrovText + " (PAL)");
                        cbVideoRates.Items.Add("29.997 " + Resources.KadrovText + " (NTSC)");
                        cbVideoRates.Items.Add("30 " + Resources.KadrovText + " (~NTSC)");
                        cbVideoRates.Items.Add("59.994 " + Resources.KadrovText + " (2xNTSC)");

                        cbVideoRates.Text = (frameRate == 15000
                                                 ? "15 " + Resources.KadrovText
                                                 : (frameRate == 24000
                                                        ? "24 " + Resources.KadraText + Resources.FilmText
                                                        : (frameRate == 25000
                                                               ? "25 " + Resources.KadrovText + " (PAL)"
                                                               : (frameRate == 29997
                                                                      ? "29.997 " + Resources.KadrovText + " (NTSC)"
                                                                      : (frameRate == 30000
                                                                             ? "30 " + Resources.KadrovText + " (~NTSC)"
                                                                             : (frameRate == 59994
                                                                                    ? "59.994 " + Resources.KadrovText +
                                                                                      " (2xNTSC)"
                                                                                    : null))))));
                        cbVideoRates.Enabled = true;
                    }
                    catch (Exception ex)
                    {
                        Statics.EL.LogException(ex);
                        cbVideoRates.Enabled = false;
                    }

                    cbColorSpace.Items.Clear();
                    // Загрузка пространства цветов:
                    if ((_capture != null) && (_capture.dxUtils != null))
                    {
                        try
                        {
                            DxUtils.ColorSpaceEnum currentColorSpace = _capture.ColorSpace;
                            string[] names = _capture.dxUtils.SubTypeList;
                            int item = 0;
                            int currItem = 0;
                            foreach (string name in names)
                            {
                                cbColorSpace.Items.Add(name);
                                if (currentColorSpace ==
                                    (DxUtils.ColorSpaceEnum) Enum.Parse(typeof (DxUtils.ColorSpaceEnum), name))
                                {
                                    currItem = item;
                                }
                                item++;
                            }
                            cbColorSpace.SelectedIndex = currItem;
                            cbColorSpace.Enabled = true;
                        }
                        catch (Exception ex)
                        {
                            Statics.EL.LogException(ex);
                            cbColorSpace.Enabled = false;
                        }
                    }
                    else
                    {
                        cbColorSpace.Enabled = false;
                    }
                    // Загрузка режимов аудио/видео для записи в файл:
                    try
                    {
                        cbAVRecFileModes.Items.Clear();
                        // Заполнение всех режимов записи в файл, используя перечислитель такой как строка (и расширение файла)
                        for (int i = 0; i < 3; i++)
                        {
                            cbAVRecFileModes.Items.Add(((DirectX.Capture.Capture.RecFileModeType) i).ToString());
                            if (_capture.RecFileMode == (DirectX.Capture.Capture.RecFileModeType) i)
                            {
                                cbAVRecFileModes.SelectedIndex = i;
                            }
                            cbAVRecFileModes.Enabled = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        Statics.EL.LogException(ex);
                        cbAVRecFileModes.Enabled = false;
                    }
                }
                this.cbVideoDevices.SelectedIndexChanged +=
                    new System.EventHandler(this.cbVideoDevices_SelectedIndexChanged);
                this.cbVideoCompressors.SelectedIndexChanged +=
                    new EventHandler(this.cbVideoCompressors_SelectedIndexChanged);
                this.cbVideoSources.SelectedIndexChanged +=
                    new System.EventHandler(this.cbVideoSources_SelectedIndexChanged);
                this.cbVideoStandart.SelectedIndexChanged +=
                    new System.EventHandler(this.cbVideoStandart_SelectedIndexChanged);
                this.cbFrameSize.SelectedIndexChanged += new System.EventHandler(this.cbFrameSize_SelectedIndexChanged);
                this.cbVideoRates.SelectedIndexChanged += new System.EventHandler(this.cbVideoRates_SelectedIndexChanged);
                this.cbColorSpace.SelectedIndexChanged += new System.EventHandler(this.cbColorSpace_SelectedIndexChanged);
                this.cbAVRecFileModes.SelectedIndexChanged +=
                    new System.EventHandler(this.cbAVRecFileModes_SelectedIndexChanged);
            }
            catch
            {
            }
        }

        /// <summary>
        /// При изменении выбора видеодевайса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbVideoDevices_SelectedIndexChanged(object sender, EventArgs e)
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
                // Получить новое видеоустройство
                videoDevice = (cbVideoDevices.SelectedIndex > 0)
                                  ? Preferences.filters.VideoInputDevices[cbVideoDevices.SelectedIndex - 1]
                                  : null;

                // Создать объект Capture
                if ((videoDevice != null) || (audioDevice != null))
                {
                    _capture = new Capture(videoDevice, audioDevice, TunerSettings.Default.AudioViaPCI);
                    OnSendComplete(true);
                    TunerSettings.Default.VideoDevice = (videoDevice != null) ? videoDevice.MonikerString : String.Empty;
                }
                UpdateCombos();
            }
            catch (Exception ex)
            {
                Statics.EL.LogException(ex);
                Statics.ShowDialog(Resources.WarningText, Resources.VideoDeviceNotSupportText,
                                   MessageDialog.MessageIcon.Alert, MessageDialog.MessageButtons.Ok);
                OnSendComplete(false);
            }
        }
        /// <summary>
        /// При изменении выбора видеокодека
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbVideoCompressors_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Изменить видеокодек
                if (_capture != null)
                {
                    _capture.VideoCompressor = ((cbVideoCompressors.Items.Count > 0)
                                                               ? Preferences.filters.VideoCompressors[
                                                                   cbVideoCompressors.SelectedIndex - 1]
                                                               : null);
                    TunerSettings.Default.VideoCompressor = (_capture.VideoCompressor != null)
                                                           ? _capture.VideoCompressor.MonikerString
                                                           : String.Empty;
                    UpdateCombos();
                }
            }
            catch (Exception ex)
            {
                Statics.EL.LogException(ex);
                Statics.ShowDialog(Resources.WarningText, Resources.VideoCompressorNotSupportText,
                                   MessageDialog.MessageIcon.Alert, MessageDialog.MessageButtons.Ok);
            }
        }
        
        /// <summary>
        /// При изменении выбора видеоисточника
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbVideoSources_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Выбор видеоисточника
                _capture.VideoSources = null;
                _capture.VideoSource = _capture.VideoSources[cbVideoSources.SelectedIndex];
                TunerSettings.Default.VideoSource = cbVideoSources.SelectedIndex;
                UpdateCombos();
            }
            catch (Exception ex)
            {
                Statics.ShowDialog(Resources.WarningText,
                                   Resources.VideoSourceNotSupportText +
                                   ex.Message + "\n\n" + ex.ToString(), MessageDialog.MessageIcon.Alert,
                                   MessageDialog.MessageButtons.Ok);
                Statics.EL.LogException(ex);
            }
        }
        
        /// <summary>
        /// При изменении выбора видеостандарта
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbVideoStandart_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((_capture == null) || _capture.dxUtils == null)
                return;
            try
            {
                var a = (AnalogVideoStandard) Enum.Parse(typeof (AnalogVideoStandard), cbVideoStandart.Text);
                _capture.dxUtils.VideoStandard = a;
                TunerSettings.Default.VideoStandard = cbVideoStandart.Text;
                UpdateCombos();
            }
            catch (Exception ex)
            {
                _capture.dxUtils.VideoStandard = AnalogVideoStandard.None;
                Statics.ShowDialog(Resources.WarningText,
                                   Resources.UnableVideoStandardText +
                                   ex.Message + "\n\n" + ex.ToString(), MessageDialog.MessageIcon.Alert,
                                   MessageDialog.MessageButtons.Ok);
                Statics.EL.LogException(ex);
            }
        }

        /// <summary>
        /// При изменении выбора размера фрейма
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbFrameSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Отключить предпросмотр во измежания появления вспышек (опционально)
                bool preview = (_capture.PreviewWindow != null);
                _capture.PreviewWindow = null;
                
                // Обновить размер фрейма:
                string[] s = cbFrameSize.Text.Split('x');
                Size size = new Size(int.Parse(s[0]), int.Parse(s[1]));
                TunerSettings.Default.FrameSize = _capture.FrameSize = size;

                // Восстановить первоначальные настройки предпросмотра:
                //_capture.PreviewWindow = (preview ? panelVideo : null);

                UpdateCombos();

            }
            catch (Exception ex)
            {
                Statics.ShowDialog(Resources.WarningText,
                                   Resources.FrameSizeNotSupportText +
                                   ex.Message + "\n\n" + ex.ToString(), MessageDialog.MessageIcon.Alert,
                                   MessageDialog.MessageButtons.Ok);
                Statics.EL.LogException(ex);
            }
        }

        /// <summary>
        /// При изменении выбора частоты видеопотока
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbVideoRates_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string[] s = cbVideoRates.Text.Split(' ');
                TunerSettings.Default.FrameRate = _capture.FrameRate = double.Parse(s[0]);
                UpdateCombos();
            }
            catch (Exception ex)
            {
                Statics.ShowDialog(Resources.WarningText,
                                   Resources.FrameRateNotSupportText +
                                   ex.Message + "\n\n" + ex.ToString(), MessageDialog.MessageIcon.Alert,
                                   MessageDialog.MessageButtons.Ok);
                Statics.EL.LogException(ex);
            }
        }

        /// <summary>
        /// При изменении выбора цветового пространства
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbColorSpace_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                DxUtils.ColorSpaceEnum c =
                    (DxUtils.ColorSpaceEnum)
                    Enum.Parse(typeof (DxUtils.ColorSpaceEnum), TunerSettings.Default.ColorSpace = cbColorSpace.Text);
                _capture.ColorSpace = c;
                UpdateCombos();
            }
            catch (Exception ex)
            {
                Statics.ShowDialog(Resources.WarningText,
                                   Resources.UnableSetColorSpaceText +
                                   ex.Message + "\n\n" + ex.ToString(), MessageDialog.MessageIcon.Alert,
                                   MessageDialog.MessageButtons.Ok);
                Statics.EL.LogException(ex);
            }
        }

        /// <summary>
        /// При изменении выбора аудио/видео режима
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cbAVRecFileModes_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                _capture.RecFileMode =
                    (DirectX.Capture.Capture.RecFileModeType) cbAVRecFileModes.SelectedIndex;
                switch (_capture.RecFileMode)
                {
                    case DirectX.Capture.Capture.RecFileModeType.Wmv:
                        
                    case DirectX.Capture.Capture.RecFileModeType.Wma:
                        btnAudioVideoFormat.Enabled = true;
                        break;
                    case DirectX.Capture.Capture.RecFileModeType.Avi:
                        btnAudioVideoFormat.Enabled = false;
                        break;
                }
                TunerSettings.Default.RecordMode = _capture.RecFileMode.ToString();
                UpdateCombos();
            }
            catch (Exception ex)
            {
                Statics.ShowDialog(Resources.WarningText,
                                   Resources.FormatNotSupportText +
                                   ex.Message + "\n\n" + ex.ToString(), MessageDialog.MessageIcon.Alert,
                                   MessageDialog.MessageButtons.Ok);
                Statics.EL.LogException(ex);
            }
        }
        private void btnAudioVideoFormat_Click(object sender, EventArgs e)
        {
            if (_capture != null)
            {
                //Показать форму только для Windows Media форматов
                switch (_capture.RecFileMode)
                {
                    case DirectX.Capture.Capture.RecFileModeType.Wma:
                        case DirectX.Capture.Capture.RecFileModeType.Wmv:
                        AsfForm asfForm = new AsfForm(_capture);
                        asfForm.ShowDialog(this);
                        UpdateCombos();
                        break;
                    default:
                        break;
                }
            }
        }
        /// <summary>
        /// Изменение имени файла для записи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChangeFileName_Click(object sender, EventArgs e)
        {
            if (fnDlg.ShowDialog() == DialogResult.OK)
            {
                TunerSettings.Default.PatternCaptureFileName = tbCaptureFileName.Text = fnDlg.CaptureFilename;
                Settings.Default.Save();
                FileNameSettings.Default.Date = fnDlg.Date;
                FileNameSettings.Default.Time = fnDlg.Time;
                FileNameSettings.Default.Programm = fnDlg.Program;
                FileNameSettings.Default.Telecast = fnDlg.Telecast;
                FileNameSettings.Default.UserBool = fnDlg.UserBool;
                FileNameSettings.Default.UserText = fnDlg.UserText;
                FileNameSettings.Default.Divider = fnDlg.DividerText;
                FileNameSettings.Default.Save();
            }
        }
        
        /// <summary>
        /// Изменение директории для имени файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnChangeDir_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                TunerSettings.Default.CaptureDir = tbDirCapture.Text = folderBrowserDialog.SelectedPath;
                Settings.Default.Save();
            }
        }

        /// <summary>
        /// Включить/выключить видеорежим рендеринга 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chbVMR9_CheckedChanged(object sender, EventArgs e)
        {
            if (_capture != null)
            {
                _capture.UseVMR9 = TunerSettings.Default.VMR9 = chbVMR9.Checked;
            }
        }
       /// <summary>
       /// Вкл./выкл.деинтерлейс
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void chbDeInterlace_CheckedChanged(object sender, EventArgs e)
       {
           initValues();
       }

        /// <summary>
        /// Инициализация некоторых значений
        /// </summary>
        private void initValues()
        {
            if (_capture != null)
            {
                _capture.UseVMR9 = chbVMR9.Checked;
                chbDeInterlace.Checked = this.FindDeinterlaceFilter(chbDeInterlace.Checked);
            }
        }
        
        /// <summary>
        /// Найти деинтерлейсный фильтр
        /// </summary>
        /// <param name="addFilter"></param>
        /// <returns></returns>
        private bool FindDeinterlaceFilter(bool addFilter)
        {
            if (addFilter)
            {
                string filterName = "Deinterlace";

                for (int i = 0; i < Preferences.filters.LegacyFilters.Count; i++)
                {
                    if (Preferences.filters.LegacyFilters[i].Name.Contains(filterName))
                    {
                        _capture.DeInterlace = Preferences.filters.LegacyFilters[i];
                        return true;
                    }
                }
            }

            // Или нет необходимого деинтерлейсного фильтра 
            // или не нашлось деинтерлейсного фильтра
            _capture.DeInterlace = null;
            return false;
        }

        private void btnVideoCaps_Click(object sender, EventArgs e)
        {
            try
            {
                string s;
                s = String.Format(
                    @Resources.VideoCapabilitiesText +
                    "--------------------------------\n\n" +
                    @Resources.VideoInputSizeText +
                    "\n" +
                    @Resources.MinFrameSizeText +
                    @Resources.MaxFrameSizeText +
                    @Resources.DetailsFrameSizeText +
                    @Resources.DetailsFrameSizeYText +
                    "\n" +
                    @Resources.MinRateVideoStreamText +
                    @Resources.MaxRateVideoStreamText +
                    @Resources.VideoModesText,
                    _capture.VideoCaps.InputSize.Width, _capture.VideoCaps.InputSize.Height,
                    _capture.VideoCaps.MinFrameSize.Width, _capture.VideoCaps.MinFrameSize.Height,
                    _capture.VideoCaps.MaxFrameSize.Width, _capture.VideoCaps.MaxFrameSize.Height,
                    _capture.VideoCaps.FrameSizeGranularityX,
                    _capture.VideoCaps.FrameSizeGranularityY,
                    _capture.VideoCaps.MinFrameRate,
                    _capture.VideoCaps.MaxFrameRate,
                    _capture.VideoCaps.AnalogVideoStandard.ToString());
                MessageBox.Show(s);
            }
            catch(Exception ex)
            {
                Statics.ShowDialog(Resources.WarningText,
                                   Resources.UnableShowVideoCapText + ex.Message + "\n\n" + ex.ToString(),
                                   MessageDialog.MessageIcon.Alert, MessageDialog.MessageButtons.Ok);
                Statics.EL.LogException(ex);
            }
        }

        private void VideoSettings_Load(object sender, EventArgs e)
        {
            initValues();
            UpdateCombos();
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
        }
    }
}
