using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TVProgViewer.TVProgApp.Classes;
using TVProgViewer.TVProgApp.Dialogs;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp
{
    public partial class RatingForm : Form
    {
        private Favorites _favorites;
        private FavoriteControl _favControl;
        private ClassifFavoriteControl _classifFavoriteControl;
        

        public bool BtnEditEnabled
        {
            get { return btnEdit.Enabled; }
            set { btnEdit.Enabled = value; }
        }

        public bool BtnDeleteEnabled
        {
            get { return btnRemove.Enabled; }
            set { btnRemove.Enabled = value; }
        }

        public bool BtnUpEnabled
        {
            get { return btnUp.Enabled; }
            set { btnUp.Enabled = value; }
        }

        public bool BtnDownEnabled
        {
            get { return btnDown.Enabled; }
            set { btnDown.Enabled = value; }
        }

        public bool BtnSaveEnabled
        {
            get { return btnSave.Enabled; }
            set { btnSave.Enabled = value; }
        }

        public RatingForm(Favorites favorites)
        {
            InitializeComponent();
           
            _favorites = favorites;
            _favControl = new FavoriteControl(_favorites);
            tbTitle.Text = Resources.RatingsText;
            _classifFavoriteControl = new ClassifFavoriteControl(_favorites);
            pRating.Controls.Add(_favControl);
            _favControl.Dock = DockStyle.Fill;
        }

        
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClassifFavorites_Click(object sender, EventArgs e)
        {
            pRating.Controls.Clear();
            tbTitle.Text = Resources.KeywordsText;
            pRating.Controls.Add(_classifFavoriteControl);
            _classifFavoriteControl.Dock = DockStyle.Fill;
        }

        private void btnFavorites_Click(object sender, EventArgs e)
        {
            _favControl = new FavoriteControl(_favorites);
            pRating.Controls.Clear();
            tbTitle.Text = Resources.RatingsText;
            pRating.Controls.Add(_favControl);
            _favControl.Dock = DockStyle.Fill;
        }
        
        /// <summary>
        /// Добавление класса рейтинга
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (tbTitle.Text == Resources.KeywordsText)
            {
                ClassifFavoriteDialog classifFavoriteDialog = new ClassifFavoriteDialog(_favorites);
                if (classifFavoriteDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (!_favorites.ClassifTable.Columns.Contains("deleteafter"))
                    {
                        _favorites.ClassifTable.Columns.Add("deleteafter", typeof (DateTime));
                    }
                    DataRow drClassFavorite = _favorites.ClassifTable.NewRow();
                    drClassFavorite["Contain"] = classifFavoriteDialog.ClassFavorite.Contain;
                    drClassFavorite["NonContain"] = classifFavoriteDialog.ClassFavorite.NonContain;
                    drClassFavorite["fid"] = classifFavoriteDialog.ClassFavorite.IdFav;
                    DataRow[] drsFavorite =
                        _favorites.FavoritesTable.Select("id = " + classifFavoriteDialog.ClassFavorite.IdFav);
                    foreach (DataRow dataRow in drsFavorite)
                    {
                        drClassFavorite["image"] = dataRow["image"];
                    }
                    if (classifFavoriteDialog.ClassFavorite.TsDeleteAfter.Year >= 2011)
                    {
                        drClassFavorite["deleteafter"] = classifFavoriteDialog.ClassFavorite.TsDeleteAfter;
                    }
                    drClassFavorite["remind"] = classifFavoriteDialog.ClassFavorite.Remind;
                    _favorites.ClassifTable.Rows.InsertAt(drClassFavorite, 0);
                    foreach (DataRow drClassifRow in _favorites.ClassifTable.Rows)
                    {
                        drClassifRow["prior"] = _favorites.ClassifTable.Rows.IndexOf(drClassifRow);
                    }
                    btnSave.Enabled = true;
                }
            }
            else if (tbTitle.Text == Resources.RatingsText)
            {
                FavoriteDialog favoriteDialog = new FavoriteDialog();
                if (favoriteDialog.ShowDialog(this) == DialogResult.OK)
                {
                    _favorites.FavoritesTable.Rows.Add(null, favoriteDialog.FavoriteClass.Visible,
                                                 favoriteDialog.FavoriteClass.FavoriteImage, favoriteDialog.FavoriteClass.FileName,
                                                 favoriteDialog.FavoriteClass.FavoriteName);
                    btnSave.Enabled = true;
                }
            }
        }

       


        /// <summary>
        /// Редактирование класса жанра
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEdit_Click(object sender, EventArgs e)
        {
            foreach (Control userctrl in pRating.Controls)
            {
                foreach (Control ctrl in userctrl.Controls)
                {
                    if (ctrl is DataGridViewExt)
                    {
                        if ((ctrl as DataGridViewExt).CurrentRow != null)
                        {
                            if (tbTitle.Text == Resources.KeywordsText)
                            {
                                DateTime tsDeleteAfter;
                                DateTime.TryParse(
                                    (ctrl as DataGridViewExt).CurrentRow.Cells["colDeleteAfter"].Value.ToString(),
                                    out tsDeleteAfter);
                                ClassifFavorite classifFavorite = new ClassifFavorite(
                                    (ctrl as DataGridViewExt).CurrentRow.Cells["colContain"].Value.ToString(),
                                    (ctrl as DataGridViewExt).CurrentRow.Cells["colDontContain"].Value.ToString(),
                                    (int) (ctrl as DataGridViewExt).CurrentRow.Cells["colFid"].Value,
                                    tsDeleteAfter,
                                    (bool) (ctrl as DataGridViewExt).CurrentRow.Cells["colRemind"].Value);
                                ClassifFavoriteDialog classifFavoriteDialog =
                                    new ClassifFavoriteDialog(_favorites, classifFavorite);
                                if (classifFavoriteDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                {
                                    if (!_favorites.ClassifTable.Columns.Contains("deleteafter"))
                                    {
                                        _favorites.ClassifTable.Columns.Add("deleteafter", typeof (DateTime));
                                    }
                                    foreach (DataRow drClassFavorite in _favorites.ClassifTable.Rows)
                                    {
                                        if ((int) drClassFavorite["id"] ==
                                            (int) (ctrl as DataGridViewExt).CurrentRow.Cells["colRatingClassID"].Value)
                                        {
                                            drClassFavorite["Contain"] = classifFavoriteDialog.ClassFavorite.Contain;
                                            drClassFavorite["NonContain"] =
                                                classifFavoriteDialog.ClassFavorite.NonContain;
                                            drClassFavorite["fid"] = classifFavoriteDialog.ClassFavorite.IdFav;
                                            DataRow[] drsFavorite =
                                                _favorites.FavoritesTable.Select("id = " +
                                                                                 classifFavoriteDialog.ClassFavorite.
                                                                                     IdFav);
                                            foreach (DataRow dataRow in drsFavorite)
                                            {
                                                drClassFavorite["image"] = dataRow["image"];
                                            }
                                            if (classifFavoriteDialog.ClassFavorite.TsDeleteAfter.Year >= 2011)
                                            {
                                                drClassFavorite["deleteafter"] =
                                                    classifFavoriteDialog.ClassFavorite.TsDeleteAfter;
                                            }
                                            drClassFavorite["remind"] = classifFavoriteDialog.ClassFavorite.Remind;
                                            (ctrl as DataGridViewExt).DataSource = _favorites.ClassifTable;
                                            btnSave.Enabled = true;
                                            break;
                                        }
                                    }

                                }
                            }
                            else if (tbTitle.Text == Resources.RatingsText)
                            {
                                Favorite favorite = new Favorite(
                                    (bool) (ctrl as DataGridViewExt).CurrentRow.Cells["colVisible"].Value,
                                    (ctrl as DataGridViewExt).CurrentRow.Cells["colFavImage"].Value != DBNull.Value
                                        ? (Image) (ctrl as DataGridViewExt).CurrentRow.Cells["colFavImage"].Value
                                        : null,
                                    (ctrl as DataGridViewExt).CurrentRow.Cells["colImageName"].Value.ToString(),
                                    (ctrl as DataGridViewExt).CurrentRow.Cells["colFavName"].Value.ToString());
                                FavoriteDialog favoriteDialog = new FavoriteDialog(favorite);
                                if (favoriteDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                                {
                                    foreach (DataRow drFavorite in _favorites.FavoritesTable.Rows)
                                    {
                                        if ((int) drFavorite["id"] ==
                                            (int) (ctrl as DataGridViewExt).CurrentRow.Cells["colFavID"].Value)
                                        {
                                            drFavorite["visible"] = favoriteDialog.FavoriteClass.Visible;
                                            drFavorite["image"] = favoriteDialog.FavoriteClass.FavoriteImage;
                                            drFavorite["imagename"] = favoriteDialog.FavoriteClass.FileName;
                                            drFavorite["favname"] = favoriteDialog.FavoriteClass.FavoriteName;
                                            (ctrl as DataGridViewExt).DataSource = _favorites.FavoritesTable;
                                            btnSave.Enabled = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Сохранение таблицы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (tbTitle.Text == Resources.KeywordsText)
            {
                foreach (Control userctrl in pRating.Controls)
                {
                    foreach (Control ctrl in userctrl.Controls)
                    {
                        if (ctrl is DataGridViewExt)
                        {
                            (ctrl as DataGridViewExt).EndEdit();
                            foreach (DataGridViewRow dgvr in (ctrl as DataGridViewExt).Rows)
                            {
                                foreach (DataRow drClassifFavorite in _favorites.ClassifTable.Rows)
                                {
                                    if ((int) dgvr.Cells["colRatingClassID"].Value == (int) drClassifFavorite["id"])
                                    {
                                        drClassifFavorite.BeginEdit();
                                        drClassifFavorite["contain"] =  dgvr.Cells["colContain"].Value.ToString();
                                        drClassifFavorite["noncontain"] = dgvr.Cells["colDontContain"].Value.ToString();
                                        DateTime tsDeleteAfter;
                                        DateTime.TryParse(dgvr.Cells["colDeleteAfter"].Value.ToString(), out tsDeleteAfter);
                                        drClassifFavorite["deleteafter"] = tsDeleteAfter;
                                        drClassifFavorite["remind"] = (bool) dgvr.Cells["colRemind"].Value;
                                        drClassifFavorite.EndEdit();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                string xmlPath = Path.Combine(Application.StartupPath, Preferences.xmlClassifFavorites);
                DataTable dtSerial = _favorites.ClassifTable.Copy();
                dtSerial.Columns.Remove("id");
                dtSerial.Columns.Remove("image");
                dtSerial.WriteXml(xmlPath);
            }
            else if (tbTitle.Text == Resources.RatingsText)
            {
                foreach (Control userctrl in pRating.Controls)
                {
                    foreach (Control ctrl in userctrl.Controls)
                    {
                        if (ctrl is DataGridViewExt)
                        {
                            (ctrl as DataGridViewExt).EndEdit();
                            foreach (DataGridViewRow dgvr in (ctrl as DataGridViewExt).Rows)
                            {
                                foreach (DataRow drFavorite in _favorites.FavoritesTable.Rows)
                                {
                                   if ((int)dgvr.Cells["colFavId"].Value == (int) drFavorite["id"])
                                   {
                                       drFavorite.BeginEdit();
                                       drFavorite["visible"] = (bool)dgvr.Cells["colVisible"].Value;
                                       drFavorite["favname"] = dgvr.Cells["colFavName"].Value.ToString();
                                       drFavorite.EndEdit();
                                       break;
                                   }
                                }
                            }
                        }
                    }
                }
                string xmlPath = Path.Combine(Application.StartupPath, Preferences.xmlFavorites);
                DataTable dtSerial = _favorites.FavoritesTable.Copy();
                dtSerial.Columns.Remove("id");
                dtSerial.Columns.Remove("image");
                dtSerial.WriteXml(xmlPath);
            }
            btnSave.Enabled = false;
        }
       
        /// <summary>
        /// Перемещение вверх
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUp_Click(object sender, EventArgs e)
        {
            if (tbTitle.Text == Resources.KeywordsText)
            {
                foreach (Control userctrl in pRating.Controls)
                {
                    foreach (Control ctrl in userctrl.Controls)
                    {
                        if (ctrl is DataGridViewExt)
                        {
                            DataRow[] drsMoveUp = _favorites.ClassifTable.Select("id = " +
                                             (ctrl as DataGridViewExt).CurrentRow.Cells["colRatingClassID"].Value);
                            int index = 0;
                            DataRow drTemp = _favorites.ClassifTable.NewRow();
                            foreach (DataRow drMoveUp in drsMoveUp)
                            {
                                index = _favorites.ClassifTable.Rows.IndexOf(drMoveUp);
                                drTemp["image"] = drMoveUp["image"];
                                drTemp["fid"] = drMoveUp["fid"];
                                drTemp["contain"] = drMoveUp["contain"];
                                drTemp["noncontain"] = drMoveUp["noncontain"];
                                drTemp["deleteafter"] = drMoveUp["deleteafter"];
                            }
                            _favorites.ClassifTable.Rows.RemoveAt(index);
                            _favorites.ClassifTable.Rows.InsertAt(drTemp, --index);

                            foreach (DataGridViewRow dgrv in (ctrl as DataGridViewExt).Rows)
                            {
                                if (dgrv.Cells["colRatingClassID"].Value.ToString() == drTemp["id"].ToString())
                                {
                                    index = dgrv.Index;
                                    break;
                                }
                            }
                                
                            (ctrl as DataGridViewExt).FirstDisplayedScrollingRowIndex = index;
                            (ctrl as DataGridViewExt).CurrentCell = (ctrl as DataGridViewExt).Rows[index].Cells["colContain"];
                            (ctrl as DataGridViewExt).Rows[index].Selected = true;
                            foreach (DataRow drClassifRow in _favorites.ClassifTable.Rows)
                            {
                                drClassifRow["prior"] = _favorites.ClassifTable.Rows.IndexOf(drClassifRow);
                            }
                            btnSave.Enabled = true;
                        }
                    }
                }
            }
            else if (tbTitle.Text == Resources.RatingsText)
            {
                foreach (Control userctrl in pRating.Controls)
                {
                    foreach (Control ctrl in userctrl.Controls)
                    {
                        if (ctrl is DataGridViewExt)
                        {
                            DataRow[] drsMoveUp = _favorites.FavoritesTable.Select("id = " +
                                                        (ctrl as DataGridViewExt).CurrentRow.Cells["colFavId"].Value);
                            int index = 0;
                            DataRow drTemp = _favorites.FavoritesTable.NewRow();
                            foreach (DataRow drMoveUp in drsMoveUp)
                            {
                                index = _favorites.FavoritesTable.Rows.IndexOf(drMoveUp);
                                drTemp["image"] = drMoveUp["image"];
                                drTemp["favname"] = drMoveUp["favname"];
                                drTemp["imagename"] = drMoveUp["imagename"];
                                drTemp["visible"] = drMoveUp["visible"];
                            }
                            _favorites.FavoritesTable.Rows.RemoveAt(index);
                            _favorites.FavoritesTable.Rows.InsertAt(drTemp, --index);

                            foreach (DataGridViewRow dgrv in (ctrl as DataGridViewExt).Rows)
                            {
                                if (dgrv.Cells["colFavID"].Value.ToString() == drTemp["id"].ToString())
                                {
                                    index = dgrv.Index;
                                    break;
                                }
                            }

                            (ctrl as DataGridViewExt).FirstDisplayedScrollingRowIndex = index;
                            (ctrl as DataGridViewExt).CurrentCell = (ctrl as DataGridViewExt).Rows[index].Cells["colFavImage"];
                            (ctrl as DataGridViewExt).Rows[index].Selected = true;
                            
                            btnSave.Enabled = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Перемещение вниз
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDown_Click(object sender, EventArgs e)
        {
            if (tbTitle.Text == Resources.KeywordsText)
            {
                foreach (Control userctrl in pRating.Controls)
                {
                    foreach (Control ctrl in userctrl.Controls)
                    {
                        if (ctrl is DataGridViewExt)
                        {
                            DataRow[] drsMoveUp = _favorites.ClassifTable.Select("id = " +
                                        (ctrl as DataGridViewExt).CurrentRow.Cells["colRatingClassID"].Value);
                            int index = 0;
                            DataRow drTemp = _favorites.ClassifTable.NewRow();
                            foreach (DataRow drMoveDown in drsMoveUp)
                            {
                                index = _favorites.ClassifTable.Rows.IndexOf(drMoveDown);
                                drTemp["image"] = drMoveDown["image"];
                                drTemp["fid"] = drMoveDown["fid"];
                                drTemp["contain"] = drMoveDown["contain"];
                                drTemp["noncontain"] = drMoveDown["noncontain"];
                                drTemp["deleteafter"] = drMoveDown["deleteafter"];
                            }
                            _favorites.ClassifTable.Rows.RemoveAt(index);
                            _favorites.ClassifTable.Rows.InsertAt(drTemp, ++index);

                            foreach (DataGridViewRow dgrv in (ctrl as DataGridViewExt).Rows)
                            {
                                if (dgrv.Cells["colRatingClassID"].Value.ToString() == drTemp["id"].ToString())
                                {
                                    index = dgrv.Index;
                                    break;
                                }
                            }

                            (ctrl as DataGridViewExt).FirstDisplayedScrollingRowIndex = index;
                            (ctrl as DataGridViewExt).CurrentCell = (ctrl as DataGridViewExt).Rows[index].Cells["colContain"];
                            (ctrl as DataGridViewExt).Rows[index].Selected = true;
                            foreach (DataRow drClassifRow in _favorites.ClassifTable.Rows)
                            {
                                drClassifRow["prior"] = _favorites.ClassifTable.Rows.IndexOf(drClassifRow);
                            }
                            btnSave.Enabled = true;
                        }
                    }
                }
            }else if (tbTitle.Text == Resources.RatingsText)
            {
                foreach (Control userctrl in pRating.Controls)
                {
                    foreach (Control ctrl in userctrl.Controls)
                    {
                        if (ctrl is DataGridViewExt)
                        {
                            DataRow[] drsMoveDown = _favorites.FavoritesTable.Select("id = " +
                                                        (ctrl as DataGridViewExt).CurrentRow.Cells["colFavId"].Value);
                            int index = 0;
                            DataRow drTemp = _favorites.FavoritesTable.NewRow();
                            foreach (DataRow drMoveDown in drsMoveDown)
                            {
                                index = _favorites.FavoritesTable.Rows.IndexOf(drMoveDown);
                                drTemp["image"] = drMoveDown["image"];
                                drTemp["favname"] = drMoveDown["favname"];
                                drTemp["imagename"] = drMoveDown["imagename"];
                                drTemp["visible"] = drMoveDown["visible"];
                            }
                            _favorites.FavoritesTable.Rows.RemoveAt(index);
                            _favorites.FavoritesTable.Rows.InsertAt(drTemp, ++index);

                            foreach (DataGridViewRow dgrv in (ctrl as DataGridViewExt).Rows)
                            {
                                if (dgrv.Cells["colFavId"].Value.ToString() == drTemp["id"].ToString())
                                {
                                    index = dgrv.Index;
                                    break;
                                }
                            }

                            (ctrl as DataGridViewExt).FirstDisplayedScrollingRowIndex = index;
                            (ctrl as DataGridViewExt).CurrentCell = (ctrl as DataGridViewExt).Rows[index].Cells["colFavImage"];
                            (ctrl as DataGridViewExt).Rows[index].Selected = true;
                            
                            btnSave.Enabled = true;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Удаление
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRemove_Click(object sender, EventArgs e)
        {

            foreach (Control userctrl in pRating.Controls)
            {
                foreach (Control ctrl in userctrl.Controls)
                {
                    if (ctrl is DataGridViewExt)
                    {
                        if ((ctrl as DataGridViewExt).SelectedRows.Count > 0) // Есть выделенные строки:
                        {
                            if (Statics.ShowDialog( Resources.ConfirmChoiceText, Resources.DeleteAllRowsText,
                                                MessageDialog.MessageIcon.Question, 
                                                MessageDialog.MessageButtons.YesNo) == DialogResult.Yes)
                            {
                                foreach (DataGridViewRow dgrv in (ctrl as DataGridViewExt).SelectedRows)
                                {
                                    if (tbTitle.Text == Resources.KeywordsText)
                                    {
                                        DataRow[] drsRemove = _favorites.ClassifTable.Select("id = " +
                                                                                          dgrv.Cells["colRatingClassID"].
                                                                                              Value);
                                        int index = 0;
                                        DataRow drTemp = _favorites.ClassifTable.NewRow();
                                        foreach (DataRow drRemove in drsRemove)
                                        {
                                            index = _favorites.ClassifTable.Rows.IndexOf(drRemove);
                                        }
                                        _favorites.ClassifTable.Rows.RemoveAt(index);
                                    }
                                    else if (tbTitle.Text == Resources.RatingsText)
                                    {
                                        DataRow[] drsRemove = _favorites.FavoritesTable.Select("id = " +
                                                                                         dgrv.Cells["colFavId"].
                                                                                             Value);
                                        int index = 0;
                                        DataRow drTemp = _favorites.ClassifTable.NewRow();
                                        foreach (DataRow drRemove in drsRemove)
                                        {
                                            index = _favorites.ClassifTable.Rows.IndexOf(drRemove);
                                        }
                                        _favorites.FavoritesTable.Rows.RemoveAt(index);
                                    }
                                    btnSave.Enabled = true;
                                }
                            }
                        }
                        else if ((ctrl as DataGridViewExt).SelectedCells.Count > 0) // Есть выделенные ячейки:
                        {
                            if (Statics.ShowDialog(Resources.ConfirmChoiceText, Resources.DeleteAllRowsText,
                                                MessageDialog.MessageIcon.Question,
                                                MessageDialog.MessageButtons.YesNo) == DialogResult.Yes)
                            {
                                foreach (DataGridViewCell dgvc in (ctrl as DataGridViewExt).SelectedCells)
                                {
                                    if (tbTitle.Text == Resources.KeywordsText)
                                    {
                                        DataRow[] drsRemove = _favorites.ClassifTable.Select("id = " +
                                                                                          (ctrl as DataGridViewExt).
                                                                                              Rows[dgvc.RowIndex].
                                                                                              Cells["colRatingClassID"].
                                                                                              Value);
                                        int index = 0;
                                        foreach (DataRow drRemove in drsRemove)
                                        {
                                            index = _favorites.ClassifTable.Rows.IndexOf(drRemove);
                                        }
                                        _favorites.ClassifTable.Rows.RemoveAt(index);
                                    }
                                    else if (tbTitle.Text == Resources.RatingsText)
                                    {
                                        DataRow[] drsRemove = _favorites.FavoritesTable.Select("id = " +
                                                                                         (ctrl as DataGridViewExt).Rows[
                                                                                             dgvc.RowIndex].Cells[
                                                                                                 "colFavId"].Value);
                                        int index = 0;
                                        foreach (DataRow drRemove in drsRemove)
                                        {
                                            index = _favorites.FavoritesTable.Rows.IndexOf(drRemove);
                                        }
                                        _favorites.FavoritesTable.Rows.RemoveAt(index);
                                    }
                                }
                                btnSave.Enabled = true;
                            }
                        }

                        foreach (DataRow drClassifRow in _favorites.ClassifTable.Rows)
                        {
                            drClassifRow["prior"] = _favorites.ClassifTable.Rows.IndexOf(drClassifRow);
                        }
                        btnSave.Enabled = true;
                    }
                }
            }
        }
    /// <summary>
    /// По закрытии формы
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
        private void GenreForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnSave.Enabled)
            {
                if (Statics.ShowDialog(Resources.ConfirmChoiceText, Resources.SaveAllChangesText,
                    MessageDialog.MessageIcon.Question, 
                    MessageDialog.MessageButtons.YesNo) == DialogResult.Yes)
                {
                    btnSave_Click(sender, e);
                }
            }
        }

        private void RatingForm_Load(object sender, EventArgs e)
        {
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
        }

       
    }
}
