using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp
{
    public partial class OptChannelsDialog : Form
    {

        public bool UnshowUnchekedChannels
        {
            get { return rbDontShow.Checked; }
        }

        public string Diff
        {
            get { return mtbDiff.Text; }
        }

        /// <summary>
        /// Применение изменнений немедленно
        /// </summary>
        public bool AcceptChangesImmidiatly
        {
            get { return rbImmidiatly.Checked; }
            set { rbImmidiatly.Checked = value; }
        }

        private DataSet dsConfig = new DataSet("Config");
        public OptChannelsDialog()
        {
            InitializeComponent();
            GetDataFromConfig();
        }

        /// <summary>
        /// Подтверждение изменений
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            string xmlExpOptChannelFileName = Path.Combine(Application.StartupPath, Preferences.xmlExpChanOptionsFile);
            if (dsConfig.Tables["ChanSettings"] != null)
            {
                DataTable confTable = dsConfig.Tables["ChanSettings"];

                DataRow row = confTable.Rows[0];
                if (rbDelete.Checked) row["UncheckedChannels"] = "Delete";
                else if (rbDontShow.Checked) row["UncheckedChannels"] = "DontShow";

                row["Diff"] = mtbDiff.Text;

                if (rbImmidiatly.Checked) row["AcceptChanges"] = "Immidiatly";
                else if (rbAfterDataLoad.Checked) row["AcceptChanges"] = "AfterDataLoad";
                dsConfig.WriteXml(xmlExpOptChannelFileName);
            }
            else
            {
                DataTable chanSettings = new DataTable("ChanSettings");
                chanSettings.Columns.Add("UncheckedChannels", typeof(string));
                chanSettings.Columns.Add("Diff", typeof(string));
                chanSettings.Columns.Add("AcceptChanges", typeof(string));
                chanSettings.Rows.Add("DontShow", "+04:00", "Immidiatly");
                rbDontShow.Checked = rbImmidiatly.Checked = true;
                mtbDiff.Text = "+04:00";
                dsConfig.Tables.Add(chanSettings);
                dsConfig.WriteXml(xmlExpOptChannelFileName);
            }
        }


        private void GetDataFromConfig()
        {
            string xmlExpOptChannelFileName = Path.Combine(Application.StartupPath, Preferences.xmlExpChanOptionsFile);
            if (File.Exists(xmlExpOptChannelFileName))
            {
                dsConfig.ReadXml(xmlExpOptChannelFileName, XmlReadMode.Auto);
                if (dsConfig.Tables["ChanSettings"] != null)
                {
                    DataRow dr = dsConfig.Tables["ChanSettings"].Rows[0];
                    if (dr != null)
                    {
                        if (dr["UncheckedChannels"].ToString() == "Delete")
                        {
                            rbDelete.Checked = true;
                        }
                        else if (dr["UncheckedChannels"].ToString() == "DontShow")
                        {
                            rbDontShow.Checked = true;
                        }
                        mtbDiff.Text = dr["Diff"].ToString();
                        if (dr["AcceptChanges"].ToString() == "Immidiatly")
                        {
                            rbImmidiatly.Checked = true;
                        }
                        else if (dr["AcceptChanges"].ToString() == "AfterDataLoad")
                        {
                            rbAfterDataLoad.Checked = true;
                        }
                    }
                }
            }
            else // Установка значений по умолчанию
            {
                DataTable chanSettings = new DataTable("ChanSettings");
                chanSettings.Columns.Add("UncheckedChannels", typeof(string));
                chanSettings.Columns.Add("Diff", typeof(string));
                chanSettings.Columns.Add("AcceptChanges", typeof(string));
                chanSettings.Rows.Add("DontShow", "+04:00", "Immidiatly");
                rbDontShow.Checked = rbImmidiatly.Checked = true;
                mtbDiff.Text = "+04:00";
                dsConfig.Tables.Add(chanSettings);
                dsConfig.WriteXml(xmlExpOptChannelFileName);
            }
        }
        private void OptChannelsForm_Load(object sender, EventArgs e)
        {
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
        }
    }
}
