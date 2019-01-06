using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;
using TVProgViewer.BusinessLogic.ProgObjs;
using TVProgViewer.TVProgApp.Classes;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp.Dialogs
{
    public partial class RemindDialog : Form
    {
        private TVProgClass _tvProg;
        private DataTable _remTable;
        public RemindDialog(TVProgClass tvProg, DataTable remTable, string text)
        {
            InitializeComponent();
            try
            {
                this.Text = text;
                if (Owner != null && Owner is MainForm)
                {
                    if ((Owner as MainForm).PropCapture != null)
                    {
                        colRec.Visible = true;
                    }
                    else
                    {
                        colRec.Visible = false;
                    }
                }
                dgRemain.Style(DataGridViewExtentions.Styles.TVGridView);
                dgRemain.FitColumns(false, false, true);
                dgRemain.AutoGenerateColumns = false;
                _remTable = remTable;
                _tvProg = tvProg;
                dgRemain.DataSource = _remTable;
                imgLst.Images.Clear();
                imgLst.Images.Add("sd2", Resources.satellite_dish2);
                imgLst.Images.Add("ans", Resources.GreenAnons);
                imgLst.Images.Add("gen", Resources.GenreEditor);
                imgLst.Images.Add("fav", Resources.favorites25);
                foreach (DataRow drGenre in Preferences.genres.GenresTable.Rows)
                {
                    if ((bool) drGenre["visible"])
                    {
                        ToolStripMenuItem tsmiGenre = new ToolStripMenuItem(
                            drGenre["genrename"].ToString(), (Image) drGenre["image"],
                            new EventHandler(SetGenre_Click));
                        csmiChangeType.DropDownItems.Add(tsmiGenre);
                    }
                }
                foreach (DataRow drRating in Preferences.favorites.FavoritesTable.Rows)
                {
                    if ((bool) drRating["visible"])
                    {
                        ToolStripMenuItem tsmiRating = new ToolStripMenuItem(
                            drRating["favname"].ToString(),
                            (Image) drRating["image"],
                            new EventHandler(SetRating_Click));
                        csmiChangeRating.DropDownItems.Add(tsmiRating);
                    }
                }
                foreach (DataGridViewRow dgvr in dgRemain.Rows)
                {
                    dgvr.Cells["colGenre"].ToolTipText =
                        !String.IsNullOrEmpty(dgvr.Cells["colCategory"].Value.ToString())
                            ? dgvr.Cells["colCategory"].Value.ToString()
                            : "Без типа";
                    dgvr.Cells["colRating"].ToolTipText =
                        !String.IsNullOrEmpty(dgvr.Cells["colFavName"].Value.ToString())
                            ? dgvr.Cells["colFavName"].Value.ToString()
                            : "Без рейтинга";
                }
            }
            catch{}
        }
        /// <summary>
        /// КМ: изменить жанр
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetGenre_Click(object sender, EventArgs e)
        {
            // Инициализация:
            string idChan = String.Empty;       // - код канала
            string title = String.Empty;        // - название передачи
            DateTime from = DateTime.MinValue;  // - дата начала передачи
            DateTime to = DateTime.MinValue;    // - дата завершения передачи
            DataRow foundRow = null;            // - строка передачи в DataTable
            GetTelecastInfo(ref idChan, ref title, ref from, ref to, ref foundRow);
            if (idChan != String.Empty && title != String.Empty &&
                from != DateTime.MinValue && to != DateTime.MinValue && sender is ToolStripMenuItem)
            {
                if (_tvProg.SetGenreXml(idChan, title, from, to, (sender as ToolStripMenuItem).Text))
                {
                    if (foundRow != null)
                    {
                        foundRow.BeginEdit();
                        foundRow["category"] = (sender as ToolStripMenuItem).Text;
                        foundRow.EndEdit();
                        dgRemain.CurrentRow.Cells["colGenre"].Value = (sender as ToolStripMenuItem).Image;
                        dgRemain.CurrentRow.Cells["colGenre"].ToolTipText = (sender as ToolStripMenuItem).Text;
                        dgRemain.CurrentRow.Cells["colCategory"].Value = (sender as ToolStripMenuItem).Text;
                        dgRemain.Refresh();
                    }
                }
            }
        }
        /// <summary>
        /// КМ: изменть рейтинг
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetRating_Click(object sender, EventArgs e)
        {
            // Инициализация:
            string idChan = String.Empty;       // - код канала
            string title = String.Empty;        // - название передачи
            DateTime from = DateTime.MinValue;  // - дата начала передачи
            DateTime to = DateTime.MinValue;    // - дата завершения передачи
            DataRow foundRow = null;            // - строка передачи в DataTable
            GetTelecastInfo(ref idChan, ref title, ref from, ref to, ref foundRow);
            if (idChan != String.Empty && title != String.Empty &&
                from != DateTime.MinValue && to != DateTime.MinValue && sender is ToolStripMenuItem)
            {
                if (_tvProg.SetRatingXml(idChan, title, from, to, (sender as ToolStripMenuItem).Text))
                {
                    if (foundRow != null)
                    {
                        foundRow.BeginEdit();
                        foundRow["favname"] = (sender as ToolStripMenuItem).Text;
                        foundRow.EndEdit();
                    }
                    dgRemain.CurrentRow.Cells["colRating"].Value = (sender as ToolStripMenuItem).Image;
                    dgRemain.CurrentRow.Cells["colRating"].ToolTipText = (sender as ToolStripMenuItem).Text;
                    dgRemain.CurrentRow.Cells["colFavname"].Value = (sender as ToolStripMenuItem).Text;
                    dgRemain.Refresh();
                }
            }
        }

        /// <summary>
        /// Установка значка заголовку колонки таблицы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetSatteliteOnHeaderColumn(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.Value != null)
            {
                if (e.Value.ToString() == Resources.LogoText && e.RowIndex == -1)
                {
                    e.PaintBackground(e.ClipBounds, false);

                    Point pt = e.CellBounds.Location; // where you want the bitmap in the cell

                    int offset = (e.CellBounds.Width - this.imgLst.Images["sd2"].Size.Width) / 2;
                    pt.X += offset;
                    pt.Y += (e.CellBounds.Height - this.imgLst.Images["sd2"].Size.Height) / 2;
                    this.imgLst.Draw(e.Graphics, pt, imgLst.Images.IndexOfKey("sd2"));
                    e.Handled = true;
                }
                else
                {
                    if (e.Value.ToString() == Resources.NoticeText && e.RowIndex == -1)
                    {
                        e.PaintBackground(e.ClipBounds, false);

                        Point pt = e.CellBounds.Location;
                        int offset = (e.CellBounds.Width - this.imgLst.Images["ans"].Size.Width) / 2;
                        pt.X += offset;
                        pt.Y += (e.CellBounds.Height - this.imgLst.Images["ans"].Size.Height) / 2;
                        this.imgLst.Draw(e.Graphics, pt, imgLst.Images.IndexOfKey("ans"));
                        e.Handled = true;
                    }
                    else
                    {
                        if (e.Value.ToString() == Resources.GenreText && e.RowIndex == -1)
                        {
                            e.PaintBackground(e.ClipBounds, false);

                            Point pt = e.CellBounds.Location;
                            int offset = (e.CellBounds.Width - this.imgLst.Images["gen"].Size.Width) / 2;
                            pt.X += offset;
                            pt.Y += (e.CellBounds.Height - this.imgLst.Images["gen"].Size.Height) / 2;
                            this.imgLst.Draw(e.Graphics, pt, imgLst.Images.IndexOfKey("gen"));
                            e.Handled = true;
                        }
                        else
                        {
                            if (e.Value.ToString() == Resources.RatingText && e.RowIndex == -1)
                            {
                                e.PaintBackground(e.ClipBounds, false);

                                Point pt = e.CellBounds.Location;
                                int offset = (e.CellBounds.Width - this.imgLst.Images["fav"].Size.Width) / 2;
                                pt.X += offset;
                                pt.Y += (e.CellBounds.Height - this.imgLst.Images["fav"].Size.Height) / 2;
                                this.imgLst.Draw(e.Graphics, pt, this.imgLst.Images.IndexOfKey("fav"));
                                e.Handled = true;
                            }
                            else
                            {
                                if (e.Value.ToString() == Resources.ToRemindText && e.RowIndex == -1)
                                {
                                    e.PaintBackground(e.ClipBounds, false);

                                    Point pt = e.CellBounds.Location;
                                    int offset = (e.CellBounds.Width -
                                                  this.imageListBell.Images["bellheader.png"].Size.Width) / 2;
                                    pt.X += offset;
                                    pt.Y += (e.CellBounds.Height -
                                             this.imageListBell.Images["bellheader.png"].Size.Height) / 2;
                                    this.imageListBell.Draw(e.Graphics, pt, this.imageListBell.Images.IndexOfKey("bellheader.png"));
                                    e.Handled = true;
                                }
                                else if (e.Value.ToString() == "Записать" && e.RowIndex == -1)
                                {
                                    e.PaintBackground(e.ClipBounds, false);

                                    Point pt = e.CellBounds.Location;
                                    int offset = (e.CellBounds.Width -
                                                  this.imageListBell.Images["capture_header.png"].Size.Width) / 2;
                                    pt.X += offset;
                                    pt.Y += (e.CellBounds.Height -
                                             this.imageListBell.Images["capture_header.png"].Size.Height) / 2;
                                    this.imageListBell.Draw(e.Graphics, pt, this.imageListBell.Images.IndexOfKey("capture_header.png"));
                                    e.Handled = true;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void dgRemain_SelectionChanged(object sender, EventArgs e)
        {
            if (dgRemain.CurrentRow != null)
            {
                if (!rtbAnons.Modified)
                {
                    if (dgRemain.CurrentRow.Cells["colDesc"].Value != null)
                    {
                        switch (Preferences.showAnons) // Как показывать анонсы:
                        {
                            case Preferences.ShowAnons.Ifitis:
                                if (!String.IsNullOrEmpty(dgRemain.CurrentRow.Cells["colDesc"].Value.ToString()))
                                {
                                    rtbAnons.Text = dgRemain.CurrentRow.Cells["colDesc"].Value.ToString();
                                    pAnons.Visible = true;
                                }
                                else
                                {
                                    rtbAnons.Text = String.Empty;
                                    pAnons.Visible = false;
                                }
                                break;
                            case Preferences.ShowAnons.Always:
                                rtbAnons.Text = dgRemain.CurrentRow.Cells["colDesc"].Value.ToString();
                                pAnons.Visible = true;
                                break;
                            case Preferences.ShowAnons.Never:
                                rtbAnons.Text = String.Empty;
                                pAnons.Visible = false;
                                break;
                        }
                    }
                    rtbAnons.Modified = false;
                }
            }
        }

        /// <summary>
        /// Закрыть панель
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExitDesc_Click(object sender, EventArgs e)
        {
            pAnons.Visible = false;
        }
        /// <summary>
        /// При модификации анонса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void rtbAnons_ModifiedChanged(object sender, EventArgs e)
        {
            if (rtbAnons.Modified)
            {
                btnCancelDesc.Enabled = btnSaveDesc.Enabled = true;
            }
            else
            {
                btnCancelDesc.Enabled = btnSaveDesc.Enabled = false;
            }
        }
        /// <summary>
        /// Отмена действий для анонса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancelDesc_Click(object sender, EventArgs e)
        {
            rtbAnons.Undo();
            rtbAnons.Modified = false;
        }
        /// <summary>
        /// Получение данных о передаче в текущей строке
        /// </summary>
        /// <param name="idChan">код канала</param>
        /// <param name="title">Название передачи</param>
        /// <param name="from">Дата начала передачи</param>
        /// <param name="to">Дата окончания передачи</param>
        /// <param name="foundRow">Строка в DataTable</param>
        private void GetTelecastInfo(ref string idChan, ref string title,
            ref DateTime from, ref DateTime to, ref DataRow foundRow)
        {
            if (dgRemain.CurrentRow != null)
            {
                idChan = dgRemain.CurrentRow.Cells["colCID"].Value.ToString();
                title = dgRemain.CurrentRow.Cells["colTitle"].Value.ToString();
                from = (DateTime)dgRemain.CurrentRow.Cells["colFrom"].Value;
                to = (DateTime)dgRemain.CurrentRow.Cells["colTo"].Value;
                foundRow = (Owner as MainForm).Programms.Rows.Find(
                    long.Parse(dgRemain.CurrentRow.Cells["colID"].Value.ToString()));
            }
        }

        /// <summary>
        /// Сохранение анонса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveDesc_Click(object sender, EventArgs e)
        {
            // Инициализация:
            string idChan = String.Empty;       // - код канала
            string title = String.Empty;        // - название передачи
            DateTime from = DateTime.MinValue;  // - дата начала передачи
            DateTime to = DateTime.MinValue;    // - дата завершения передачи
            DataRow foundRow = null;
            GetTelecastInfo(ref idChan, ref title, ref from, ref to, ref foundRow);
            if (idChan != String.Empty && title != String.Empty &&
                from != DateTime.MinValue && to != DateTime.MinValue)
            {
                if (_tvProg.SaveDescription(idChan, from, to, title, rtbAnons.Text))
                {
                    if (foundRow != null)
                    {
                        foundRow.BeginEdit();
                        foundRow["desc"] = rtbAnons.Text;
                        foundRow.EndEdit();
                    }
                    if (dgRemain.CurrentRow != null)
                    {
                        dgRemain.CurrentRow.Cells["colDesc"].Value = rtbAnons.Text;
                    }
                }
            }
            rtbAnons.Modified = false;
        }

        /// <summary>
        /// При щелчке на колокольчике
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgRemain_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 && (e.ColumnIndex == 3 || e.ColumnIndex == 4))
            {
                string idProg = dgRemain.Rows[e.RowIndex].Cells["colID"].Value.ToString();
                string idChan = dgRemain.Rows[e.RowIndex].Cells["colCID"].Value.ToString();
                string title = dgRemain.Rows[e.RowIndex].Cells["colTitle"].Value.ToString();
                DateTime from = (DateTime) dgRemain.Rows[e.RowIndex].Cells["colFrom"].Value;
                DateTime to = (DateTime) dgRemain.Rows[e.RowIndex].Cells["colTo"].Value;
                switch (e.ColumnIndex)
                {
                    case 3:
                        bool statusRec = false;
                        if (SetStatusRec(idProg, idChan, title, from, to, ref statusRec))
                        {
                            dgRemain.Rows[e.RowIndex].Cells["colRec"].Value = statusRec
                                                                                  ? Resources.capture
                                                                                  : Resources.capturempty;
                        }
                        break;
                    case 4:
                        bool statusBell = false;
                        if (SetStatusBell(idProg, idChan, title, from, to, ref statusBell))
                        {
                            dgRemain.Rows[e.RowIndex].Cells["colBell"].Value = statusBell
                                                                                   ? Resources.bell
                                                                                   : Resources.bellempty;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Изменение статуса напоминания
        /// </summary>
        /// <param name="idProg">Код передачи</param>
        /// <param name="idChan">Код канала</param>
        /// <param name="title">Название передачи</param>
        /// <param name="from">Дата начала передачи</param>
        /// <param name="to">Дата окончания передачи</param>
        /// <param name="statusBell">Статус колокольчика</param>
        /// <returns>Результат выполения</returns>
        private bool SetStatusBell(string idProg, string idChan, string title,
            DateTime from, DateTime to, ref bool statusBell)
        {
            statusBell = false;
            DataRow foundRow = (Owner as MainForm).Programms.Rows.Find(long.Parse(idProg));
            if (foundRow != null)
            {
                statusBell = !String.IsNullOrEmpty(foundRow["remind"].ToString())
                                      ? !(bool)foundRow["remind"]
                                      : true;
                if (_tvProg.SetRemind(idChan, title, from, to, statusBell))
                {
                    foundRow.BeginEdit();
                    foundRow["remind"] = statusBell;
                    foundRow.EndEdit();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Изменение статуса видеозахвата
        /// </summary>
        /// <param name="idProg">Код передачи</param>
        /// <param name="idChan">Код канала</param>
        /// <param name="title">Название передачи</param>
        /// <param name="from">Дата начала передачи</param>
        /// <param name="to">Дата окончания прередачи</param>
        /// <param name="statusRec">Статус видеозахвата</param>
        /// <returns></returns>
        private bool SetStatusRec(string idProg, string idChan, string title, DateTime from, DateTime to, ref bool statusRec)
        {
            statusRec = false;
            DataRow foundRow = (Owner as MainForm).Programms.Rows.Find(long.Parse(idProg));
            if (foundRow != null)
            {
                statusRec = !String.IsNullOrEmpty(foundRow["record"].ToString())
                                ? !(bool)foundRow["record"]
                                : true;
                if (statusRec)
                {
                    DataRow[] drsCasts =
                    (Owner as MainForm).Programms.Select(
                        String.Format("((start >= '{0}' and (start < '{1}' and stop >= '{1}')) or " +
                        "(start <= '{0}' and (stop > '{0}' and stop <= '{1}')) or " +
                        "(start >= '{0}' and stop <= '{1}') or (start <='{0}' and stop >= '{1}')) and record = True", from, to));
                    if (drsCasts.Length > 0)
                    {
                        if (drsCasts.Length == 1)
                        {
                            if (drsCasts[0]["title"].ToString() != title || drsCasts[0]["cid"].ToString() != idChan)
                            {
                                Statics.ShowDialog(Resources.WarningText, Resources.ImpossibleRecText,
                                                   MessageDialog.MessageIcon.Alert, MessageDialog.MessageButtons.Ok);
                                return false;
                            }
                        }
                        else
                        {
                            Statics.ShowDialog(Resources.WarningText, Resources.ImpossibleRecText,
                                               MessageDialog.MessageIcon.Alert, MessageDialog.MessageButtons.Ok);
                            return false;
                        }
                    }
                }
                if (_tvProg.SetRecord(idChan, title, from, to, statusRec))
                {
                    foundRow.BeginEdit();
                    foundRow["record"] = statusRec;
                    foundRow.EndEdit();
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Когда все строки добавлены в датагрид
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgRemain_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int index = e.RowIndex; index < e.RowIndex + e.RowCount; index++)
            {
                (sender as DataGridViewExt).Rows[index].ContextMenuStrip = contextMenuTables;
            }
        }

        /// <summary>
        /// Отмечать строку и при щелчке на правую кнопку мыши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgRemain_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (sender is DataGridViewExt)
                {
                    DataGridViewExt.HitTestInfo hti;
                    hti = (sender as DataGridViewExt).HitTest(e.X, e.Y);
                    if (hti.Type == DataGridViewHitTestType.Cell)
                    {
                        foreach (DataGridViewRow dgvr in (sender as DataGridViewExt).SelectedRows)
                        {
                            dgvr.Selected = false;
                        }
                        (sender as DataGridViewExt).Rows[hti.RowIndex].Selected = true;
                        foreach (DataGridViewColumn dgvc in (sender as DataGridViewExt).Columns)
                        {
                            if (dgvc.Visible)
                            {
                                (sender as DataGridViewExt).CurrentCell =
                                    (sender as DataGridViewExt).Rows[hti.RowIndex].Cells[dgvc.Index];
                                break;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Копировать в буфер обмена
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void csmiCopyToBuffer_Click(object sender, EventArgs e)
        {
            if (dgRemain.CurrentRow != null)
            {
                Clipboard.SetText(dgRemain.CurrentRow.Cells["colNameChannel"].Value +
                                  String.Format(" {0:t} — {1:t} ",
                                                (DateTime)dgRemain.CurrentRow.Cells["colFrom"].Value,
                                                (DateTime)dgRemain.CurrentRow.Cells["colTo"].Value) +
                                  dgRemain.CurrentRow.Cells["colTitle"].Value);
            }
        }

        /// <summary>
        /// Открыть диалог поиска по названию передачи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void csmiSearch_Click(object sender, EventArgs e)
        {
            if (dgRemain.CurrentRow != null)
            {
                SearchDialog _schForm = new SearchDialog(_tvProg,
                                                         dgRemain.CurrentRow.Cells["colTitle"].Value.ToString());
                _schForm.ShowDialog(this);
            }
        }

        /// <summary>
        /// Добавить к любимым
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void csmiAddToFavorite_Click(object sender, EventArgs e)
        {
            string txtFavorite = String.Empty;
            if (dgRemain.CurrentRow != null)
            {
                txtFavorite = dgRemain.CurrentRow.Cells["colTitle"].Value.ToString();
            }
            ClassifFavoriteDialog clFavoriteDialog = new ClassifFavoriteDialog(Preferences.favorites, txtFavorite);
            if (clFavoriteDialog.ShowDialog(this) == DialogResult.OK)
            {
                if (!Preferences.favorites.ClassifTable.Columns.Contains("deleteafter"))
                {
                    Preferences.favorites.ClassifTable.Columns.Add("deleteafter", typeof(DateTime));
                }
                if (clFavoriteDialog.Edit)
                {
                    // Изменение любимости передачи
                    DataRow[] drsFavEdit = Preferences.favorites.ClassifTable.Select(
                        String.Format("Contain LIKE '*{0}*'", clFavoriteDialog.ClassFavorite.Contain));
                    if (drsFavEdit.Length > 0)
                    {
                        drsFavEdit[0].BeginEdit();
                        drsFavEdit[0]["Contain"] = clFavoriteDialog.ClassFavorite.Contain;
                        drsFavEdit[0]["NonContain"] = clFavoriteDialog.ClassFavorite.NonContain;
                        drsFavEdit[0]["fid"] = clFavoriteDialog.ClassFavorite.IdFav;
                        DataRow[] drsFavorite =
                            Preferences.favorites.FavoritesTable.Select("id = " +
                                                                        clFavoriteDialog.ClassFavorite.IdFav);
                        if (drsFavorite.Length > 0)
                        {
                            drsFavEdit[0]["image"] = drsFavorite[0]["image"];
                        }
                        if (clFavoriteDialog.ClassFavorite.TsDeleteAfter.Year >= 2011)
                        {
                            drsFavEdit[0]["deleteafter"] = clFavoriteDialog.ClassFavorite.TsDeleteAfter;
                        }
                        drsFavEdit[0]["remind"] = clFavoriteDialog.ClassFavorite.Remind;
                        drsFavEdit[0].EndEdit();
                    }
                }
                else
                {
                    // Добавление к любимым
                    DataRow drClassFavorite = Preferences.favorites.ClassifTable.NewRow();
                    drClassFavorite["Contain"] = clFavoriteDialog.ClassFavorite.Contain;
                    drClassFavorite["NonContain"] = clFavoriteDialog.ClassFavorite.NonContain;
                    drClassFavorite["fid"] = clFavoriteDialog.ClassFavorite.IdFav;
                    DataRow[] drsFavorite =
                        Preferences.favorites.FavoritesTable.Select("id = " + clFavoriteDialog.ClassFavorite.IdFav);
                    foreach (DataRow dataRow in drsFavorite)
                    {
                        drClassFavorite["image"] = dataRow["image"];
                    }
                    if (clFavoriteDialog.ClassFavorite.TsDeleteAfter.Year >= 2011)
                    {
                        drClassFavorite["deleteafter"] = clFavoriteDialog.ClassFavorite.TsDeleteAfter;
                    }
                    drClassFavorite["remind"] = clFavoriteDialog.ClassFavorite.Remind;
                    Preferences.favorites.ClassifTable.Rows.InsertAt(drClassFavorite, 0);
                }

                foreach (DataRow drClassifRow in Preferences.favorites.ClassifTable.Rows)
                {
                    drClassifRow["prior"] = Preferences.favorites.ClassifTable.Rows.IndexOf(drClassifRow);
                }
                ;
                string xmlPath = Path.Combine(Application.StartupPath, Preferences.xmlClassifFavorites);
                DataTable dtSerial = Preferences.favorites.ClassifTable.Copy();
                dtSerial.Columns.Remove("id");
                dtSerial.Columns.Remove("image");
                dtSerial.WriteXml(xmlPath);
            }
        }

        /// <summary>
        /// Добавить в классификатор жанров передач
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void csmiAddToGenres_Click(object sender, EventArgs e)
        {
            string txtGenre = String.Empty;
            if (dgRemain.CurrentRow != null)
            {
                txtGenre = dgRemain.CurrentRow.Cells["colTitle"].Value.ToString();
            }
            ClassifGenreDialog clGenreDialog = new ClassifGenreDialog(Preferences.genres, txtGenre);
            if (clGenreDialog.ShowDialog(this) == DialogResult.OK)
            {
                if (clGenreDialog.Edit)
                {
                    // Редактирование имеющегося класса:
                    DataRow[] drsGenreEdit = Preferences.genres.ClassifTable.Select(
                        String.Format("Contain LIKE '*{0}*'", clGenreDialog.ClassGenre.Contain));
                    if (drsGenreEdit.Length > 0)
                    {
                        drsGenreEdit[0].BeginEdit();
                        drsGenreEdit[0]["Contain"] = clGenreDialog.ClassGenre.Contain;
                        drsGenreEdit[0]["NonContain"] = clGenreDialog.ClassGenre.NonContain;
                        drsGenreEdit[0]["gid"] = clGenreDialog.ClassGenre.IdGenre;
                        DataRow[] drsGenre =
                            Preferences.genres.GenresTable.Select("id = " +
                                                                  clGenreDialog.ClassGenre.IdGenre);
                        if (drsGenre.Length > 0)
                        {
                            drsGenreEdit[0]["image"] = drsGenre[0]["image"];
                        }
                        if (clGenreDialog.ClassGenre.TsDeleteAfter.Year >= 2011)
                        {
                            drsGenreEdit[0]["deleteafter"] = clGenreDialog.ClassGenre.TsDeleteAfter;
                        }
                        drsGenreEdit[0].EndEdit();
                    }
                }
                else
                {
                    // Добавление в классификатор:
                    if (!Preferences.genres.ClassifTable.Columns.Contains("deleteafter"))
                    {
                        Preferences.genres.ClassifTable.Columns.Add("deleteafter", typeof(DateTime));
                    }
                    DataRow drClassGenre = Preferences.genres.ClassifTable.NewRow();
                    drClassGenre["Contain"] = clGenreDialog.ClassGenre.Contain;
                    drClassGenre["NonContain"] = clGenreDialog.ClassGenre.NonContain;
                    drClassGenre["gid"] = clGenreDialog.ClassGenre.IdGenre;
                    DataRow[] drsGenre =
                        Preferences.genres.GenresTable.Select("id = " + clGenreDialog.ClassGenre.IdGenre);
                    foreach (DataRow dataRow in drsGenre)
                    {
                        drClassGenre["image"] = dataRow["image"];
                    }
                    if (clGenreDialog.ClassGenre.TsDeleteAfter.Year >= 2011)
                    {
                        drClassGenre["deleteafter"] = clGenreDialog.ClassGenre.TsDeleteAfter;
                    }
                    Preferences.genres.ClassifTable.Rows.InsertAt(drClassGenre, 0);
                }
                foreach (DataRow drClassifRow in Preferences.genres.ClassifTable.Rows)
                {
                    drClassifRow["prior"] = Preferences.genres.ClassifTable.Rows.IndexOf(drClassifRow);
                }
                ;
                string xmlPath = Path.Combine(Application.StartupPath, Preferences.xmlClassifGenres);
                DataTable dtSerial = Preferences.genres.ClassifTable.Copy();
                dtSerial.Columns.Remove("id");
                dtSerial.Columns.Remove("image");
                dtSerial.WriteXml(xmlPath);
            }
        }

        /// <summary>
        /// Просмотреть свойства канала
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void csmiPropChannel_Click(object sender, EventArgs e)
        {
            string strCID = String.Empty;
            if (dgRemain.CurrentRow != null)
            {
                strCID = dgRemain.CurrentRow.Cells["colCID"].Value.ToString();
            }
            DataRow[] drsChan = (Owner as MainForm).Channels.CopyToDataTable().Select("id = " + strCID);
            if (drsChan.Length > 0)
            {
                Channel chan = new Channel((bool)drsChan[0]["visible"],
                                           drsChan[0]["display-name"].ToString(),
                                           uint.Parse(drsChan[0]["number"].ToString()),
                                           drsChan[0]["user-name"].ToString(),
                                           (Image)drsChan[0]["icon"],
                                           drsChan[0]["diff"].ToString());
                EditChannelDialog editChanDlg = new EditChannelDialog(chan);
                if (editChanDlg.ShowDialog(this) == DialogResult.OK)
                {
                    string xmlOptChannelFile = Path.Combine(Application.StartupPath, Preferences.xmlOptChannelFile);
                    if (!File.Exists(xmlOptChannelFile))
                    {
                        XmlTextWriter tr = new XmlTextWriter(xmlOptChannelFile, null);
                        tr.WriteStartDocument();
                        tr.WriteStartElement("Root");
                        tr.WriteStartElement("channel");
                        tr.WriteAttributeString("id", strCID);
                        tr.WriteAttributeString("visible", chan.Visible.ToString());
                        tr.WriteAttributeString("user-name", chan.UserSyn);
                        tr.WriteAttributeString("number", chan.Number.ToString());
                        tr.WriteAttributeString("diff", chan.Diff);
                        tr.WriteEndElement();
                        tr.WriteEndElement();
                        tr.WriteEndDocument();
                        tr.Flush();
                        tr.Close();
                    }
                    else
                    {
                        XmlDocument xdoc = new XmlDocument();
                        xdoc.Load(xmlOptChannelFile);
                        XPathNavigator xNav = ((IXPathNavigable)xdoc).CreateNavigator();
                        XPathNodeIterator itr = xNav.Select("/Root/channel[@id='" + strCID + "']");
                        while (itr.MoveNext())
                        {
                            itr.Current.MoveToAttribute("visible", String.Empty);
                            itr.Current.SetValue(chan.Visible.ToString());
                            itr.Current.MoveToParent();
                            itr.Current.MoveToAttribute("user-name", String.Empty);
                            itr.Current.SetValue(chan.UserSyn);
                            itr.Current.MoveToParent();
                            itr.Current.MoveToAttribute("number", String.Empty);
                            itr.Current.SetValue(chan.Number.ToString());
                            itr.Current.MoveToParent();
                            itr.Current.MoveToAttribute("diff", String.Empty);
                            itr.Current.SetValue(chan.Diff);
                        }
                        xdoc.Save(xmlOptChannelFile);
                    }
                }
                (Owner as MainForm).ShowData();
            }
        }

        private void RemindDialog_Load(object sender, EventArgs e)
        {
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
        }
    }
}
