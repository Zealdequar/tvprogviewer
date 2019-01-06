using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using TVProgViewer.BusinessLogic.ProgObjs;
using TVProgViewer.TVProgApp.Classes;
using TVProgViewer.TVProgApp.Dialogs;
using TVProgViewer.TVProgApp.Properties;
using TVProgViewer.BusinessLogic.ProgObjs;

namespace TVProgViewer.TVProgApp
{
    public partial class ChannelsForm : Form
    {
        private OptChannelsDialog optChanForm = new OptChannelsDialog();
        private TVProgClass _tvProgClass;
        private ImageList _imgLst = new ImageList();
        private int _curInd = 0;
        private int _qtyChecked;

        public ChannelsForm()
        {
            InitializeComponent();
            dgOptChannels.AutoGenerateColumns = false;
            if (TVEnvironment.currentUser == null)
            {
                colDisplayName.Visible = false;
                colFreq.Visible = false;
                colOnOff.Visible = false;
                colNumber.Visible = false;
                if (TVEnvironment.systemChannelList != null)
                    dgOptChannels.DataSource = TVEnvironment.systemChannelList;
            }
            else
            {
                
                List<UserChannel> userChannels = Controllers.TvProgController
                    .GetUserChannelList(TVEnvironment.currentUser.UserID, 
                    TVEnvironment.systemChannelList.First().TVProgViewerID).ToList();
                if (TVEnvironment.systemChannelList == null || TVEnvironment.systemChannelList.Count() == 0) return;
                List<SystemChannel> chList = TVEnvironment.systemChannelList.ToList<SystemChannel>();
                chList.ForEach(sch => 
                { 
                    sch.Visible = userChannels.Find(uch => uch.ChannelID == sch.ChannelID) != null;
                    if (sch.Visible)
                    {
                        sch.UserChannelID = userChannels.Where(uch => uch.ChannelID == sch.ChannelID).First().UserChannelID;
                        sch.OrderCol = userChannels.Where(uch => uch.ChannelID == sch.ChannelID).First().OrderCol; 
                    }
                });
                if (chList != null)
                    dgOptChannels.DataSource = chList;
                colDisplayName.Visible = true;
                colFreq.Visible = true;
                colOnOff.Visible = true;
                colNumber.Visible = true;
            }
            
            dgOptChannels.Style(DataGridViewExtentions.Styles.TVGridWithEdit);
            dgOptChannels.FitColumns(false, false, true);

            
            
        }

        private void ChannelsForm_Load(object sender, EventArgs e)
        {
            ShowStat();
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
        }

        /// <summary>
        /// Установка значка заголовку колонки таблицы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetSatteliteOnHeaderColumn(object sender, DataGridViewCellPaintingEventArgs e)
        {
            /*if (e.Value != null)
            {
                if (e.Value.ToString() == Resources.LogoText && e.RowIndex == -1)
                {
                    e.PaintBackground(e.ClipBounds, false);

                    Point pt = e.CellBounds.Location; // where you want the bitmap in the cell

                    int offset = (e.CellBounds.Width - this._imgLst.Images[0].Size.Width)/2;
                    pt.X += offset;
                    pt.Y += (e.CellBounds.Height - this._imgLst.Images[0].Size.Height)/2;
                    this._imgLst.Draw(e.Graphics, pt, 0);
                    e.Handled = true;
                }
            } */
        }

        private void dgChannels_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            SetSatteliteOnHeaderColumn(sender, e);
        }

        /// <summary>
        /// Сохранение настроек
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            dgOptChannels.EndEdit();
            //string xmlOptChannelFile = Path.Combine(Application.StartupPath, Preferences.xmlOptChannelFile);

            foreach (DataGridViewRow dgrvr in dgOptChannels.Rows)
            {
                if (dgrvr.Cells["colId"].Value == null || dgrvr.Cells["colOnOff"].Value == null) continue;

                if ((Boolean)dgrvr.Cells["colOnOff"].Value)
                {
                    Controllers.TvProgController.InsertUserChannel(
                        (int)dgrvr.Cells["colUserChannelID"].Value,
                        TVEnvironment.currentUser.UserID,
                        TVEnvironment.systemChannelList.First().TVProgViewerID,
                        (int)dgrvr.Cells["colId"].Value,
                        dgrvr.Cells["colDisplayName"].Value.ToString(),
                        (int)dgrvr.Cells["colNumber"].Value);
                    dgrvr.Cells["colOnOff"].Value.ToString();
                }
                else
                {
                    Controllers.TvProgController.DeleteUserChannel(
                TVEnvironment.currentUser.UserID,
                (int)dgrvr.Cells["colID"].Value);
                }
            }
                
            if (Owner != null) (Owner as MainForm).ShowData();
            this.Close();
        }

        private void btnExpOpt_Click(object sender, EventArgs e)
        {
            if (optChanForm.ShowDialog() == DialogResult.OK)
            {
                if (optChanForm.AcceptChangesImmidiatly)
                {
                    if (Statics.ShowDialog(Resources.ConfirmChoiceText,
                                           Resources.AcceptTimeDiffText,
                                           MessageDialog.MessageIcon.Question, MessageDialog.MessageButtons.YesNo) ==
                        DialogResult.Yes)
                    {
                        foreach (DataGridViewRow dgrv in dgOptChannels.Rows)
                        {
                            dgrv.Cells["colDiff"].Value = optChanForm.Diff;
                        }
                    }
                }
                else
                {
                    string xmlOptChannelFile = Path.Combine(Application.StartupPath, Preferences.xmlOptChannelFile);
                    XmlDocument xdoc = new XmlDocument();
                    xdoc.Load(xmlOptChannelFile);
                    XPathNavigator xNav = ((IXPathNavigable) xdoc).CreateNavigator();
                    foreach (DataGridViewRow dgrvr in dgOptChannels.Rows)
                    {
                        XPathNodeIterator itr = xNav.Select("/Root/channel[@id='" + dgrvr.Cells["colId"].Value + "']");
                        while (itr.MoveNext())
                        {
                            itr.Current.MoveToAttribute("diff", String.Empty);
                            itr.Current.SetValue(optChanForm.Diff);
                        }
                    }
                    xdoc.Save(xmlOptChannelFile);
                }
                if (optChanForm.UnshowUnchekedChannels)
                {

                }
                else
                {
                    if (Statics.ShowDialog(Resources.ConfirmChoiceText,
                                           Resources.ConfirmDeleteChannelsText,
                                           MessageDialog.MessageIcon.Question, MessageDialog.MessageButtons.YesNo) ==
                        DialogResult.Yes)
                    {
                        List<string> lst = new List<string>();
                        this.Cursor = Cursors.WaitCursor;
                        foreach (DataGridViewRow dgrv in dgOptChannels.Rows)
                        {
                            if (bool.Parse(dgrv.Cells["colOnOff"].Value.ToString()) == false)
                            {
                                lst.Add(dgrv.Cells["colId"].Value.ToString());
                            }
                        }
                        _tvProgClass.DeleteChannels(lst);
                        foreach (string strChan in lst)
                        {
                        }

                        ShowStat();
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            List<string> lst = new List<string>();
            this.Cursor = Cursors.WaitCursor;
            bool pr = false;
            if (dgOptChannels.SelectedRows.Count > 0) // Есть выделенные строки:
            {
                if (Statics.ShowDialog(Resources.ConfirmChoiceText,
                                       Resources.ConfirmDeleteChannelsText,
                                       MessageDialog.MessageIcon.Question,
                                       MessageDialog.MessageButtons.YesNo) == DialogResult.Yes)
                {
                    foreach (DataGridViewRow dgrv in dgOptChannels.SelectedRows)
                    {
                        lst.Add(dgrv.Cells["colId"].Value.ToString());
                        pr = true;
                    }
                }
            }
            else if (dgOptChannels.SelectedCells.Count > 0) // Есть выделенные ячейки:
            {
                if (Statics.ShowDialog(Resources.ConfirmChoiceText,
                                       Resources.ConfirmDeleteChannelsText,
                                       MessageDialog.MessageIcon.Question,
                                       MessageDialog.MessageButtons.YesNo) == DialogResult.Yes)
                {
                    foreach (DataGridViewCell dgvc in dgOptChannels.SelectedCells)
                    {

                        lst.Add(dgOptChannels.Rows[dgvc.RowIndex].Cells["colId"].Value.ToString());
                        pr = true;
                    }
                }
            }
            if (pr) _tvProgClass.DeleteChannels(lst);
            ShowStat();
            this.Cursor = Cursors.Default;
        }

        private void ShowStat()
        {
            lblNumberChannel.Text = Resources.TotalChannelsText + dgOptChannels.Rows.Count + ",";
            int qtyChecked = 0;
            foreach (DataGridViewRow dataGridViewRow in dgOptChannels.Rows)
            {
                if (dataGridViewRow.Cells["colOnOff"].Value == null) continue;
                if ((bool) dataGridViewRow.Cells["colOnOff"].Value) qtyChecked++;
            }
            lblCheckedChannel.Text = Resources.CheckedText + qtyChecked;
        }


        private void btnEdit_Click(object sender, EventArgs e)
        {
            Channel chan = new Channel((bool) dgOptChannels.Rows[_curInd].Cells["colOnOff"].Value,
                                       dgOptChannels.Rows[_curInd].Cells["colChannelName"].Value.ToString(),
                                       (uint) dgOptChannels.Rows[_curInd].Cells["colNumber"].Value,
                                       dgOptChannels.Rows[_curInd].Cells["colDisplayName"].Value.ToString(),
                                       (Image) dgOptChannels.Rows[_curInd].Cells["colImage"].Value,
                                       dgOptChannels.Rows[_curInd].Cells["colDiff"].Value.ToString());
            EditChannelDialog editChannelForm = new EditChannelDialog(chan);
            if (editChannelForm.ShowDialog() == DialogResult.OK)
            {
                dgOptChannels.Rows[_curInd].Cells["colOnOff"].Value = chan.Visible;
                dgOptChannels.Rows[_curInd].Cells["colChannelName"].Value = chan.Name;
                dgOptChannels.Rows[_curInd].Cells["colNumber"].Value = chan.Number;
                dgOptChannels.Rows[_curInd].Cells["colDisplayName"].Value = chan.UserSyn;
                dgOptChannels.Rows[_curInd].Cells["colImage"].Value = chan.Emblem;
                dgOptChannels.Rows[_curInd].Cells["colDiff"].Value = chan.Diff;
            }
        }

        private void dgChannels_SelectionChanged(object sender, EventArgs e)
        {
            if (dgOptChannels.CurrentRow != null)
            {
                _curInd = dgOptChannels.CurrentRow.Index;
                btnEdit.Enabled = btnRemove.Enabled = true;
            }
            else
            {
                btnEdit.Enabled = btnRemove.Enabled = false;
            }

        }

        private void dgChannels_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dgOptChannels.Columns[e.ColumnIndex].Name == "colOnOff")
            {
                DataGridViewCheckBoxCell checkCell =
                    (DataGridViewCheckBoxCell) dgOptChannels.Rows[e.RowIndex].Cells["colOnOff"];
                if ((bool) checkCell.Value) lblCheckedChannel.Text = Resources.CheckedText + ++_qtyChecked;
                else lblCheckedChannel.Text = Resources.CheckedText + --_qtyChecked;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Для обратного вызова
        /// </summary>
        /// <returns></returns>
        private static bool ThumbnailCallback()
        {
            return false;
        }

        /// <summary>
        /// Обновление картинок
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSrcRefresh_Click(object sender, EventArgs e)
        {
            string gifDir = Application.StartupPath + @"\Gifs";
            if (!Directory.Exists(gifDir))
                Directory.CreateDirectory(gifDir);
/*            foreach (DataRow drChannel in _dtChannels.Rows)
            {
                string gifFileName = gifDir + "\\" + drChannel["id"] + ".gif";
                string src = drChannel["icon-src"].ToString();
                // Загрузка и сохранение значка канала:
                byte[] buffer = new byte[4096]; // - буффер чтения/записи
                int read = 0; // - для определения считывания данных
                Stream stream = null; // - поток для распаковки
                HttpWebResponse hwResp; // - для ответа сервера

                if (!String.IsNullOrEmpty(src))
                {
                    try
                    {
                        HttpWebRequest hwReq = (HttpWebRequest) HttpWebRequest.Create(src);
                        hwResp = (HttpWebResponse) hwReq.GetResponse();
                        stream = hwResp.GetResponseStream();
                        using (FileStream fs = new FileStream(gifFileName, FileMode.Create))
                        {
                            while ((read = stream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                fs.Write(buffer, 0, buffer.Length);
                                //byte[] gif = wc.DownloadData(src);
                                //File.WriteAllBytes(gifFileName, gif);
                            }
                            fs.Flush();
                        }
                    }
                    catch (Exception ex)
                    {
                        Statics.EL.LogException(ex);
                    }
                }
                Image.GetThumbnailImageAbort myCallback =
                    new Image.GetThumbnailImageAbort(ThumbnailCallback);
                // Сжатие значка до заданных размеров:
                if (File.Exists(gifFileName))
                {
                    drChannel["icon"] = Image.FromFile(gifFileName).GetThumbnailImage(25, 25, myCallback,
                                                                                      IntPtr.Zero);
                }
                else
                {
                    if (Resources.ResourceManager.GetObject("satellite_dish") != null)
                    {
                        drChannel["icon"] = ((Image) Resources.ResourceManager.GetObject("satellite_dish")).
                            GetThumbnailImage(25, 25, myCallback, IntPtr.Zero);
                    }
                }
            }*/
            
        }
    }
}
