using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TVProgViewer.TVProgApp.Controls;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp
{
    public partial class RemindForm : Form
    {
        public DataTable dtRemind { get; set; }
        public int ID { get; set; }
        private TVProgClass _tvProg;
        public RemindForm(TVProgClass tvProg)
        {
            InitializeComponent();
            _tvProg = tvProg;
            this.Opacity = (100 - Reminder.Default.Opacity)/100;
            trackBarOpacity.Value = (int)Math.Round((double) (100 - Reminder.Default.Opacity));
            pProgram.BackColor = Preferences.CellsColor1;
            timerCloser.Enabled = true;
        }

        private void trackBarOpacity_MouseHover(object sender, EventArgs e)
        {
            trackBarOpacity.Visible = true;
        }

        private void trackBarOpacity_MouseLeave(object sender, EventArgs e)
        {
            trackBarOpacity.Visible = false;
        }
        /// <summary>
        /// Изменение прозрачности формы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void trackBarOpacity_ValueChanged(object sender, EventArgs e)
        {
            this.Opacity = trackBarOpacity.Value/100.0 ;
            Reminder.Default.Opacity = (byte) (100 - trackBarOpacity.Value);
            toolTip1.ToolTipTitle = String.Format(Resources.OpacityProcText,  100 - trackBarOpacity.Value);
            Reminder.Default.Save();
        }

        private void RemindForm_Shown(object sender, EventArgs e)
        {
            if (dtRemind != null)
            {
                btnShowChannel.Visible = (Owner as MainForm).PropCapture != null;
                cbWait.Text = String.Empty;
                if (Reminder.Default.AfterSeconds)
                {
                    timerCloser.Interval = Reminder.Default.QtySeconds*1000;
                    timerCloser.Enabled = true;
                }
                else
                dgRemind.AutoGenerateColumns = false;
                dgRemind.Visible = dtRemind.Rows.Count > 1 ? true : false;
                dgRemind.DataSource = dtRemind;
                dgRemind.Columns["colRating"].Width = 26;
                dgRemind.Columns["colChannelImage"].Width = 26;
                dgRemind.Columns["colChannelName"].Width = 150;
                dgRemind.Columns["colFrom"].Width = 55;
                dgRemind.Columns["colGenre"].Width = 26;
                dgRemind.Columns["colTitle"].Width = 250;
                dgRemind.Height = dgRemind.Rows.Count*26;
                if (dgRemind.Rows.Count > 0)
                {
                    if (dgRemind.CurrentCell != null) dgRemind.CurrentCell.Selected = false;
                    dgRemind.CurrentCell = dgRemind.Rows[dgRemind.Rows.Count - 1].Cells["colTitle"];
                    dgRemind.CurrentRow.Selected = true;
                    
                }
                foreach (DataGridViewRow dgvr in dgRemind.Rows)
                {
                    if (dgvr.Cells["colFavName"].Value != null)
                    {
                        dgvr.Cells["colRating"].ToolTipText = dgvr.Cells["colFavName"].Value.ToString();
                    }
                    if (dgvr.Cells["colGenreName"].Value != null)
                    {
                        dgvr.Cells["colGenre"].ToolTipText = dgvr.Cells["colGenreName"].Value.ToString();
                    }

                    if (ID > 0 && (int) dgvr.Cells["colID"].Value == ID)
                    {
                        dgRemind.CurrentRow.Selected = false;
                        dgRemind.CurrentCell = dgvr.Cells["colTitle"];
                        dgvr.Selected = true;
                        ID = 0;
                    }
                }
                if (Reminder.Default.AfterTelecast)
                {
                    if (dgRemind.CurrentRow != null)
                    {
                        timerCloser.Interval =
                            (int)(DateTime.Parse(dgRemind.CurrentRow.Cells["colTo"].Value.ToString()) - DateTime.Now).
                                TotalMilliseconds;
                        timerCloser.Enabled = true;
                    }
                }
            }
        }

        private void RemindForm_MouseMove(object sender, MouseEventArgs e)
        {
            trackBarOpacity.Visible = e.X <= 18 ? true : false;
        }

        private void dgRemind_SelectionChanged(object sender, EventArgs e)
        {
            if (dgRemind.CurrentRow != null)
            {
                pbRating.Image = (Image) dgRemind.CurrentRow.Cells["colRating"].Value;
                pbChannel.Image = (Image) dgRemind.CurrentRow.Cells["colChannelImage"].Value;
                pbGenre.Image = (Image) dgRemind.CurrentRow.Cells["colGenre"].Value;
                lblChannelName.Text = dgRemind.CurrentRow.Cells["colChannelName"].Value.ToString();
                lblChannelName.Width = 152;
                lblFromTo.Text = String.Format("{0:t} — {1:t}", dgRemind.CurrentRow.Cells["colFrom"].Value,
                                               dgRemind.CurrentRow.Cells["colTo"].Value);
                lblFromTo.Width = 150;
                rtbTitle.Text = dgRemind.CurrentRow.Cells["colTitle"].Value.ToString();
            }
        }

        private void timerCloser_Tick(object sender, EventArgs e)
        {
            timerCloser.Enabled = false;
            this.Hide();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (cbWait.Text != String.Empty)
            {
               foreach (DataRow drWaitRem in _tvProg.WaitRemind.Rows)
               {
                   if (dgRemind.CurrentRow!=null)
                   if (drWaitRem["title"].ToString() == rtbTitle.Text && 
                       (int)drWaitRem["id"] == (int)dgRemind.CurrentRow.Cells["colID"].Value)
                   {
                       drWaitRem.BeginEdit();
                       drWaitRem["waitto"] = DateTime.Now.AddMinutes(int.Parse(cbWait.Text));
                       drWaitRem.EndEdit();
                       break;
                   }
               }
            }
            this.Hide();
        }

        /// <summary>
        /// Показать канал
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShowChannel_Click(object sender, EventArgs e)
        {
            if (dgRemind.CurrentRow != null && Owner != null && Owner is MainForm)
            {
                DataTable dtChannels = (Owner as MainForm).Channels.CopyToDataTable();
                foreach (DataRow drChan in (Owner as MainForm).Channels)
                {
                    if (drChan["id"].ToString() == dgRemind.CurrentRow.Cells["colCID"].Value.ToString())
                    {
                        VideoDialog videoDialog = new VideoDialog((Owner as MainForm).PropCapture, dtChannels, int.Parse(drChan["freq"].ToString()));
                        this.Close();
                        videoDialog.ShowDialog();
                        break;
                    }
                }
                                        
            }
        }

        private void RemindForm_Load(object sender, EventArgs e)
        {
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
        }
    }
}
