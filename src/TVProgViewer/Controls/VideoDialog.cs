using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using DShowNET;
using DirectX.Capture;
using TVProgViewer.TVProgApp.Classes;
using TVProgViewer.TVProgApp.Dialogs;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp.Controls
{
    public partial class VideoDialog : Form
    {
        private Capture _capture = null;
        private static DataTable _channels = null;
        private int _lastTvFreq, _defaultTvFreq = 0;
        private string _captureFileName = String.Empty;

        public VideoDialog(Capture capture, DataTable channels, int defaultFreq = 223250000)
        {
            InitializeComponent();
            _capture = capture;
            _channels = channels;
            _defaultTvFreq = defaultFreq;
            BindTaskPriorityCombo(cbChannels);
        }

        private void FindChannelByFreq()
        {
            if (_channels.Columns.Contains("freq"))
            {
                if (_lastTvFreq >= numFreq.Minimum && _lastTvFreq <= numFreq.Maximum)
                {
                    string strChanName =
                        _channels.Select(String.Format("[freq] = '{0}'", _lastTvFreq))[0]["user-name"].ToString();
                    cbChannels.Text = strChanName;
                }
            }
        }

        private void VideoDialog_Load(object sender, EventArgs e)
        {
            InitCombos();
            InitMenues();
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
        }

        private void InitMenues()
        {
            PropertyPage p;
            try
            {
                PropertyPage.Items.Clear();
                for (int c = 0; c < _capture.PropertyPages.Count; c++)
                {
                    p = _capture.PropertyPages[c];
                    PropertyPage.Items.Add(p.Name + "...", null, new EventHandler(mnuPropertyPages_Click));
                }
                btnProperties.Enabled = (_capture.PropertyPages.Count > 0);
            }
            catch
            {
                btnProperties.Enabled = false;
            }
        }

        private void mnuPropertyPages_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripItem m = sender as ToolStripItem;
                if (_capture != null)
                {
                    _capture.PropertyPages = null;
                    _capture.VideoSources = null;
                    _capture.AudioSources = null;
                    _capture.PropertyPages[PropertyPage.Items.IndexOf(m)].Show(this);

                    if ((_capture.Tuner != null) &&
                        (PropertyPage.Items[PropertyPage.Items.IndexOf(m)].Text == Resources.TVTunerText))
                    {
                        TunerSettings.Default.CountryCode = _capture.Tuner.CountryCode;
                        TunerSettings.Default.TuningSpace = _capture.Tuner.TuningSpace;
                        TunerSettings.Default.InputType = (byte) _capture.Tuner.InputType;
                        numFreq.Value = _capture.Tuner.GetVideoFrequency;
                        TunerSettings.Default.Save();
                    }
                }

            }
            catch (Exception ex)
            {
                Statics.ShowDialog(Resources.WarningText,
                                   Resources.UnableShowPropText,
                                   MessageDialog.MessageIcon.Alert,
                                   MessageDialog.MessageButtons.Ok);
                Statics.EL.LogException(ex);
            }
        }

        private void InitCombos()
        {
            // Загрузка размеров фрейма:
            try
            {
                this.cbPreviewSize.SelectedIndexChanged -=
                    new System.EventHandler(this.cbPreviewSize_SelectedIndexChanged);
                cbPreviewSize.Items.Clear();
                Size frameSize = _capture.PreviewFrameSize;
                cbPreviewSize.Items.Add("160 x 120");
                cbPreviewSize.Items.Add("320 x 240");

                // Добавить Pal форматы:
                cbPreviewSize.Items.Add("352 x 288");
                cbPreviewSize.Items.Add("640 x 480");

                // Добавить Ntsc форматы:
                cbPreviewSize.Items.Add("720 x 480");

                // Добавить некоторые Pal-форматы:
                cbPreviewSize.Items.Add("720 x 576");
                cbPreviewSize.Items.Add("768 x 576");

                cbPreviewSize.Text = (frameSize == new Size(160, 120)
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
                cbPreviewSize.Enabled = true;
            }
            catch (Exception)
            {
                cbPreviewSize.Enabled = false;
            }
            finally
            {
                this.cbPreviewSize.SelectedIndexChanged +=
                    new System.EventHandler(this.cbPreviewSize_SelectedIndexChanged);
            }
            try
            {
                if (_capture.PreviewWindow == null)
                {
                    _capture.Tuner.AudioMode = AMTunerModeType.TV;
                    _capture.PreviewWindow = videoPanel;
                    numFreq.Minimum = _capture.Tuner.MinFrequency;
                    numFreq.Maximum = 1000000000;
                    numFreq.Increment = 500000;
                    _capture.Tuner.CountryCode = TunerSettings.Default.CountryCode;
                    numFreq.Value = _defaultTvFreq;
                    FindChannelByFreq();
                }

            }
            catch (Exception ex)
            {
                Statics.EL.LogException(ex);
                Statics.ShowDialog(Resources.WarningText, Resources.UnablePreviewText,
                                   MessageDialog.MessageIcon.Alert, MessageDialog.MessageButtons.Ok);
            }

        }


        private static void BindTaskPriorityCombo(ComboBox priorityCombo)
        {
            priorityCombo.DrawMode = DrawMode.OwnerDrawVariable;
            priorityCombo.DrawItem += new DrawItemEventHandler(priorityCombo_DrawItem);
            priorityCombo.Items.Clear();
            foreach (DataRow drChan in _channels.Rows)
            {
                if ((bool) drChan["visible"])
                    priorityCombo.Items.Add(drChan["user-name"]);
            }
            priorityCombo.SelectedIndex = 0;
        }

        private static void priorityCombo_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index >= 0)
            {
                ComboBox cmbPriority = sender as ComboBox;
                string text = cmbPriority.Items[e.Index].ToString();

                if (text.Length > 0)
                {
                    string priority = text;
                    Image img = GetTaskPriorityImage(priority);

                    if (img != null)
                    {
                        e.Graphics.DrawImage(img, e.Bounds.X, e.Bounds.Y, 16, 16);
                    }
                }

                e.Graphics.DrawString(text, cmbPriority.Font, System.Drawing.Brushes.Black,
                                      new RectangleF(e.Bounds.X + 15, e.Bounds.Y,
                                                     e.Bounds.Width, e.Bounds.Height));

                e.DrawFocusRectangle();
            }
        }

        private static Image GetTaskPriorityImage(string priority)
        {
            foreach (DataRow drChan in _channels.Rows)
            {
                if (priority == drChan["user-name"].ToString())
                {
                    return (Image) drChan["icon"];
                }
            }
            return null;
        }

        private void VideoDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            _capture.PreviewWindow = null;
        }

        private void numFreq_ValueChanged(object sender, EventArgs e)
        {
            if (_capture != null)
            {
                if (_capture.Tuner != null)
                {
                    _capture.VideoSources = null;
                    _capture.AudioSources = null;
                    _lastTvFreq = (int) numFreq.Value;
                    int res = _capture.Tuner.SetFrequency(_lastTvFreq);
                }
            }
        }

        private void cbPreviewSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                // Отключить предпросмотр во измежания появления вспышек (опционально)
                bool preview = (_capture.PreviewWindow != null);
                _capture.PreviewWindow = null;

                // Обновить размер фрейма:
                string[] s = cbPreviewSize.Text.Split('x');
                Size size = new Size(int.Parse(s[0]), int.Parse(s[1]));
                TunerSettings.Default.PreviewSize = _capture.PreviewFrameSize = size;
                TunerSettings.Default.Save();
                // Восстановить первоначальные настройки предпросмотра:
                _capture.PreviewWindow = (preview ? videoPanel : null);

                InitCombos();

            }
            catch (Exception ex)
            {
                Statics.ShowDialog(Resources.WarningText,
                                   Resources.UnableSizeFrameText +
                                   ex.Message + "\n\n" + ex.ToString(), MessageDialog.MessageIcon.Alert,
                                   MessageDialog.MessageButtons.Ok);
                Statics.EL.LogException(ex);
            }
        }

        private void btnProperties_Click(object sender, EventArgs e)
        {
            btnProperties.ContextMenuStrip.Show(MousePosition);
        }

        private void btnAccept_Click(object sender, EventArgs e)
        {
            try
            {
                if (!_channels.Columns.Contains("freq"))
                {
                    _channels.Columns.Add("freq", typeof (long));
                }
                _channels.Select(String.Format("[user-name] = '{0}'", cbChannels.Text))[0]["freq"] = numFreq.Value;
                string xmlOptChannelFile = Path.Combine(Application.StartupPath, Preferences.xmlOptChannelFile);
                if (!File.Exists(xmlOptChannelFile))
                {
                    XmlTextWriter tr = new XmlTextWriter(xmlOptChannelFile, null);
                    tr.WriteStartDocument();
                    tr.WriteStartElement("Root");
                    foreach (DataRow dr in _channels.Rows)
                    {
                        tr.WriteStartElement("channel");
                        tr.WriteAttributeString("id", dr["id"].ToString());
                        tr.WriteAttributeString("visible", dr["visible"].ToString());
                        tr.WriteAttributeString("user-name", dr["user-name"].ToString());
                        tr.WriteAttributeString("number", dr["number"].ToString());
                        tr.WriteAttributeString("diff", dr["diff"].ToString());
                        tr.WriteAttributeString("freq", dr["freq"].ToString());
                        tr.WriteEndElement();

                    }
                    tr.WriteEndElement();
                    tr.WriteEndDocument();
                    tr.Flush();
                    tr.Close();
                }
                else
                {
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.Load(xmlOptChannelFile);
                    XPathNavigator xNav = ((IXPathNavigable) xdoc).CreateNavigator();
                    foreach (DataRow dr in _channels.Rows)
                    {
                        XPathNodeIterator itr = xNav.Select("/Root/channel[@id='" + dr["id"].ToString() + "']");
                        while (itr.MoveNext())
                        {
                            if (itr.Current.MoveToAttribute("freq", String.Empty))
                            {
                                itr.Current.SetValue(dr["freq"].ToString() == String.Empty ? "0" : dr["freq"].ToString());
                            }
                            else
                                itr.Current.CreateAttribute(String.Empty, "freq", String.Empty,
                                                            dr["freq"].ToString() == String.Empty
                                                                ? "0"
                                                                : dr["freq"].ToString());
                        }
                    }
                    xdoc.Save(xmlOptChannelFile);
                }
                Statics.ShowDialog(Resources.InformationText, Resources.FreqComparedSuccessText,
                                   MessageDialog.MessageIcon.Info,
                                   MessageDialog.MessageButtons.Ok);
            }
            catch (Exception ex)
            {
                Statics.EL.LogException(ex);
                Statics.ShowDialog(Resources.InformationText, Resources.FreqCompareNotSuccessText,
                                   MessageDialog.MessageIcon.Info,
                                   MessageDialog.MessageButtons.Ok);
            }
        }

        private void cbChannels_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_channels.Columns.Contains("freq"))
            {
                string strTextChannel = cbChannels.Text.Contains("'")
                                            ? cbChannels.Text.Replace("'", "''")
                                            : cbChannels.Text;
                long freqChan = (long) _channels.Select(String.Format("[user-name] = '{0}'", strTextChannel))[0]["freq"];
                numFreq.Value = (freqChan >= numFreq.Minimum && freqChan <= numFreq.Maximum) ? freqChan : numFreq.Value;
            }
        }


        private string TransformFileName(string pattern)
        {
            pattern = pattern.Replace(Resources.DateText, DateTime.Today.ToString("ddMMyyyy"));
            pattern = pattern.Replace(Resources.TimeText, DateTime.Now.ToString("HHmm"));
            pattern = pattern.Replace(Resources.ProgramText, cbChannels.Text);
            pattern = pattern.Replace(Resources.TelecastText, "");
            return pattern;
        }

        private void btnRec_Click(object sender, EventArgs e)
        {
            try
            {
                if (_capture == null)
                    throw new ApplicationException(Resources.PleaseSetVADeviceText);
                if (!_capture.Cued)
                {
                    _capture.Filename = TunerSettings.Default.CaptureDir + @"\" +
                                        TransformFileName(TunerSettings.Default.PatternCaptureFileName);
                    _capture.RecFileMode = (DirectX.Capture.Capture.RecFileModeType)
                                           Enum.Parse(typeof (DirectX.Capture.Capture.RecFileModeType),
                                                      TunerSettings.Default.RecordMode.ToString());
                }
                _capture.Cue();
                _capture.Start();
                btnRec.Enabled = false;
                btnStop.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.ToString());
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (_capture == null)
                    throw new ApplicationException(Resources.PleaseSetVADeviceText);
                _capture.Stop();
                btnRec.Enabled = true;
                btnStop.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n\n" + ex.ToString());
            }
        }

      
        private void dSleep()
        {
            Thread.Sleep(2500);
            try
            {
                Invoke((MethodInvoker) delegate { pTools.Visible = false; });
            }
            catch(Exception ex)
            {
               Statics.EL.LogException(ex);   
            }
        }

        private void pTools_MouseHover(object sender, EventArgs e)
        {
            pTools.Visible = true;
        }

        private void splitter1_MouseMove(object sender, MouseEventArgs e)
        {
            if (pTools.Visible)
            {
                Thread thr = new Thread(dSleep);
                thr.Start();
            }
            else{ pTools.Visible = true;}
        }
    }
}
