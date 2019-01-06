using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DShowNET;
using DirectX.Capture;
using TVProgViewer.TVProgApp.Classes;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp.Controls
{
    public partial class CountryDep : UserControl
    {
        private Capture _capture = null;
        public CountryDep(Capture capture)
        {
            InitializeComponent();
            _capture = capture;
            LoadSettings();
            this.cbInputType.SelectedIndexChanged += new System.EventHandler(this.cbInputType_SelectedIndexChanged);
            this.cbVideoStandart.SelectedIndexChanged += new System.EventHandler(this.cbVideoStandart_SelectedIndexChanged);
            this.tbCountryCode.TextChanged += new System.EventHandler(this.tbCountryCode_TextChanged);
            this.tbTuningSpace.TextChanged += new System.EventHandler(this.tbTuningSpace_TextChanged);
        }

        private void LoadSettings()
        {
            cbInputType.Enabled = false;
            cbVideoStandart.Enabled = false;

            tbCountryCode.Enabled = true;
            tbTuningSpace.Enabled = true;

            if (_capture == null)
            {
                return;
            }

            if ((_capture.dxUtils == null) && (_capture.Tuner == null))
            {
                return;
            }
            try
            {

                cbVideoStandart.Items.Clear();
                if (_capture.dxUtils != null)
                {
                    if (TunerSettings.Default.VideoStandard != String.Empty)
                    {
                        AnalogVideoStandard a =
                            (AnalogVideoStandard)Enum.Parse(typeof(AnalogVideoStandard), TunerSettings.Default.VideoStandard);
                        _capture.dxUtils.VideoStandard = a;
                    }
                    AnalogVideoStandard currentStandard = _capture.dxUtils.VideoStandard;
                    AnalogVideoStandard avaliableStandards = _capture.dxUtils.AvailableVideoStandards;
                    int mask = 1;
                    int item = 0;
                    int currItem = -1;
                    while (mask <= (int)AnalogVideoStandard.PAL_N_COMBO)
                    {
                        int avs = mask & (int)avaliableStandards;
                        if (avs != 0)
                        {
                            cbVideoStandart.Items.Add(((AnalogVideoStandard)avs).ToString());
                            if (currentStandard == (AnalogVideoStandard)avs)
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

                cbInputType.Items.Clear();
                cbInputType.Items.Add(Resources.CableText);
                cbInputType.Items.Add(Resources.AntennaText);
                if (_capture.Tuner != null)
                {
                    _capture.Tuner.InputType = TunerSettings.Default.TuningSpace == 0
                                                              ? TunerInputType.Cable
                                                              : TunerInputType.Antenna;
                    cbInputType.SelectedIndex = _capture.Tuner.InputType == TunerInputType.Cable ? 0 : 1;
                    cbInputType.Enabled = true;
                    try
                    {
                        _capture.Tuner.CountryCode = TunerSettings.Default.CountryCode;
                        tbCountryCode.Text = TunerSettings.Default.CountryCode.ToString();
                        _capture.Tuner.TuningSpace = TunerSettings.Default.TuningSpace;
                        tbTuningSpace.Text = TunerSettings.Default.TuningSpace.ToString();
                    }
                    catch (Exception ex)
                    {
                        Statics.EL.LogException(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Statics.EL.LogException(ex);
            }
        }

        private void cbInputType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_capture != null)
            {
                if (_capture.Tuner != null)
                {
                    if (cbInputType.SelectedIndex == 0)
                    {
                        _capture.Tuner.InputType = TunerInputType.Cable;
                        TunerSettings.Default.InputType = 0;
                    }
                    else
                    {
                        _capture.Tuner.InputType = TunerInputType.Antenna;
                        TunerSettings.Default.InputType = 1;
                    }
                }
            }
        }

        private void cbVideoStandart_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_capture != null)
            {
                if (_capture.dxUtils != null)
                {
                    if (_capture.dxUtils.VideoDecoderAvail)
                    {
                        try
                        {
                            AnalogVideoStandard a =
                                (AnalogVideoStandard) Enum.Parse(typeof (AnalogVideoStandard), cbVideoStandart.Text);
                            _capture.dxUtils.VideoStandard = a;
                            TunerSettings.Default.VideoStandard = cbVideoStandart.Text;
                        }
                        catch (Exception)
                        {
                            _capture.dxUtils.VideoStandard = AnalogVideoStandard.None;
                        }
                    }
                    else
                    {
                        _capture.dxUtils.VideoStandard = AnalogVideoStandard.None;
                    }
                }
            }
        }

        private void tbCountryCode_TextChanged(object sender, EventArgs e)
        {
            if (_capture != null)
            {
                if (_capture.Tuner != null)
                {
                    try
                    {
                        TunerSettings.Default.CountryCode = _capture.Tuner.CountryCode = Convert.ToInt32(tbCountryCode.Text, 10);
                    }
                    catch (Exception ex)
                    {
                        Statics.EL.LogException(ex);
                        TunerSettings.Default.CountryCode = _capture.Tuner.CountryCode = 7; 
                        tbCountryCode.Text = "7";
                    }

                }
            }
        }

        private void tbTuningSpace_TextChanged(object sender, EventArgs e)
        {
            if (_capture != null)
            {
                if (_capture.Tuner != null)
                {
                    try
                    {
                        TunerSettings.Default.TuningSpace = _capture.Tuner.TuningSpace = Convert.ToInt32(tbTuningSpace.Text, 10);
                    }
                    catch (Exception ex)
                    {
                        Statics.EL.LogException(ex);
                        TunerSettings.Default.TuningSpace = _capture.Tuner.TuningSpace = 0;
                        tbTuningSpace.Text = "0";
                    }
                }
            }
        }

        private void CountryDep_Load(object sender, EventArgs e)
        {
            this.cbInputType.SelectedIndexChanged -= new System.EventHandler(this.cbInputType_SelectedIndexChanged);
            this.cbVideoStandart.SelectedIndexChanged -= new System.EventHandler(this.cbVideoStandart_SelectedIndexChanged);
            this.tbCountryCode.TextChanged -= new System.EventHandler(this.tbCountryCode_TextChanged);
            this.tbTuningSpace.TextChanged -= new System.EventHandler(this.tbTuningSpace_TextChanged);
            LoadSettings();
            this.cbInputType.SelectedIndexChanged += new System.EventHandler(this.cbInputType_SelectedIndexChanged);
            this.cbVideoStandart.SelectedIndexChanged += new System.EventHandler(this.cbVideoStandart_SelectedIndexChanged);
            this.tbCountryCode.TextChanged += new System.EventHandler(this.tbCountryCode_TextChanged);
            this.tbTuningSpace.TextChanged += new System.EventHandler(this.tbTuningSpace_TextChanged);
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
        }
    }
}
