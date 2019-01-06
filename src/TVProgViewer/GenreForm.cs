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
    public partial class GenreForm : Form
    {
        private Genres _genres;
        private GenreControl _genControl;
        private ClassifGenreControl _classifGenreControl;
        

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

        public GenreForm(Genres genres)
        {
            InitializeComponent();
           
            _genres = genres;
            _genControl = new GenreControl(_genres);
            tbTitle.Text = Resources.TypesOfTelecastsText;
            _classifGenreControl = new ClassifGenreControl(_genres);
            pGenre.Controls.Add(_genControl);
            _genControl.Dock = DockStyle.Fill;
        }

        private void GenreForm_Load(object sender, EventArgs e)
        {
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClassifGenres_Click(object sender, EventArgs e)
        {
            pGenre.Controls.Clear();
            tbTitle.Text = Resources.KeywordsText;
            pGenre.Controls.Add(_classifGenreControl);
            _classifGenreControl.Dock = DockStyle.Fill;
        }

        private void btnGenres_Click(object sender, EventArgs e)
        {
            _genControl = new GenreControl(_genres);
            pGenre.Controls.Clear();
            tbTitle.Text = Resources.TypesOfTelecastsText;
            pGenre.Controls.Add(_genControl);
            _genControl.Dock = DockStyle.Fill;
        }
        
        /// <summary>
        /// Добавление класса жанра
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (tbTitle.Text == Resources.KeywordsText)
            {
                ClassifGenreDialog classifGenreDialog = new ClassifGenreDialog(_genres);
                if (classifGenreDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (!_genres.ClassifTable.Columns.Contains("deleteafter"))
                    {
                        _genres.ClassifTable.Columns.Add("deleteafter", typeof (DateTime));
                    }
                    DataRow drClassGenre = _genres.ClassifTable.NewRow();
                    drClassGenre["Contain"] = classifGenreDialog.ClassGenre.Contain;
                    drClassGenre["NonContain"] = classifGenreDialog.ClassGenre.NonContain;
                    drClassGenre["gid"] = classifGenreDialog.ClassGenre.IdGenre;
                    DataRow[] drsGenre =
                        _genres.GenresTable.Select("id = " + classifGenreDialog.ClassGenre.IdGenre);
                    foreach (DataRow dataRow in drsGenre)
                    {
                        drClassGenre["image"] = dataRow["image"];
                    }
                    if (classifGenreDialog.ClassGenre.TsDeleteAfter.Year >= 2011)
                    {
                        drClassGenre["deleteafter"] = classifGenreDialog.ClassGenre.TsDeleteAfter;
                    }
                    _genres.ClassifTable.Rows.InsertAt(drClassGenre, 0);
                    foreach (DataRow drClassifRow in _genres.ClassifTable.Rows)
                    {
                        drClassifRow["prior"] = _genres.ClassifTable.Rows.IndexOf(drClassifRow);
                    }
                    btnSave.Enabled = true;
                }
            }
            else if (tbTitle.Text == Resources.TypesOfTelecastsText)
            {
                GenreDialog genreDialog = new GenreDialog();
                if (genreDialog.ShowDialog(this) == DialogResult.OK)
                {
                    _genres.GenresTable.Rows.Add(null, genreDialog.GenreClass.Visible,
                                                 genreDialog.GenreClass.GenreImage, genreDialog.GenreClass.FileName,
                                                 genreDialog.GenreClass.GenreName);
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
             foreach (Control userctrl in pGenre.Controls)
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
                                    ClassifGenre classifGenre = new ClassifGenre(
                                        (ctrl as DataGridViewExt).CurrentRow.Cells["colContain"].Value.ToString(),
                                        (ctrl as DataGridViewExt).CurrentRow.Cells["colDontContain"].Value.ToString(),
                                        (int) (ctrl as DataGridViewExt).CurrentRow.Cells["colGid"].Value,
                                        tsDeleteAfter);
                                    ClassifGenreDialog classifGenreDialog = new ClassifGenreDialog(_genres, classifGenre);
                                    if (classifGenreDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                                    {
                                        if (!_genres.ClassifTable.Columns.Contains("deleteafter"))
                                        {
                                            _genres.ClassifTable.Columns.Add("deleteafter", typeof (DateTime));
                                        }
                                        foreach (DataRow drClassGenre in _genres.ClassifTable.Rows)
                                        {
                                            if ((int) drClassGenre["id"] ==
                                                (int) (ctrl as DataGridViewExt).CurrentRow.Cells["colGenreID"].Value)
                                            {
                                                drClassGenre["Contain"] = classifGenreDialog.ClassGenre.Contain;
                                                drClassGenre["NonContain"] = classifGenreDialog.ClassGenre.NonContain;
                                                drClassGenre["gid"] = classifGenreDialog.ClassGenre.IdGenre;
                                                DataRow[] drsGenre =
                                                    _genres.GenresTable.Select("id = " +
                                                                               classifGenreDialog.ClassGenre.IdGenre);
                                                foreach (DataRow dataRow in drsGenre)
                                                {
                                                    drClassGenre["image"] = dataRow["image"];
                                                }
                                                if (classifGenreDialog.ClassGenre.TsDeleteAfter.Year >= 2011)
                                                {
                                                    drClassGenre["deleteafter"] =
                                                        classifGenreDialog.ClassGenre.TsDeleteAfter;
                                                }
                                                (ctrl as DataGridViewExt).DataSource = _genres.ClassifTable;
                                                btnSave.Enabled = true;
                                                break;
                                            }
                                        }
                                        
                                    }
                                }
                                else if (tbTitle.Text == Resources.TypesOfTelecastsText)
                                {
                                    Genre genre = new Genre(
                                        (bool) (ctrl as DataGridViewExt).CurrentRow.Cells["colVisible"].Value,
                                        (ctrl as DataGridViewExt).CurrentRow.Cells["colGenreImage"].Value != DBNull.Value
                                        ? (Image)(ctrl as DataGridViewExt).CurrentRow.Cells["colGenreImage"].Value : null,
                                        (ctrl as DataGridViewExt).CurrentRow.Cells["colImageName"].Value.ToString(),
                                        (ctrl as DataGridViewExt).CurrentRow.Cells["colGenreName"].Value.ToString());
                                    GenreDialog genreDialog = new GenreDialog(genre);
                                    if (genreDialog.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                                    {
                                        foreach (DataRow drGenre in _genres.GenresTable.Rows)
                                        {
                                            if ((int) drGenre["id"] ==
                                                (int) (ctrl as DataGridViewExt).CurrentRow.Cells["colGenreID"].Value)
                                            {
                                                drGenre["visible"] = genreDialog.GenreClass.Visible;
                                                drGenre["image"] = genreDialog.GenreClass.GenreImage;
                                                drGenre["imagename"] = genreDialog.GenreClass.FileName;
                                                drGenre["genrename"] = genreDialog.GenreClass.GenreName;
                                                (ctrl as DataGridViewExt).DataSource = _genres.GenresTable;
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
                foreach (Control userctrl in pGenre.Controls)
                {
                    foreach (Control ctrl in userctrl.Controls)
                    {
                        if (ctrl is DataGridViewExt)
                        {
                            (ctrl as DataGridViewExt).EndEdit();
                            foreach (DataGridViewRow dgvr in (ctrl as DataGridViewExt).Rows)
                            {
                                foreach (DataRow drClassifGenre in _genres.ClassifTable.Rows)
                                {
                                    if ((int) dgvr.Cells["colGenreId"].Value == (int) drClassifGenre["id"])
                                    {
                                        drClassifGenre.BeginEdit();
                                        drClassifGenre["contain"] =  dgvr.Cells["colContain"].Value.ToString();
                                        drClassifGenre["noncontain"] = dgvr.Cells["colDontContain"].Value.ToString();
                                        DateTime tsDeleteAfter;
                                        DateTime.TryParse(dgvr.Cells["colDeleteAfter"].Value.ToString(), out tsDeleteAfter);
                                        drClassifGenre["deleteafter"] = tsDeleteAfter;
                                        drClassifGenre.EndEdit();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                string xmlPath = Path.Combine(Application.StartupPath, Preferences.xmlClassifGenres);
                DataTable dtSerial = _genres.ClassifTable.Copy();
                dtSerial.Columns.Remove("id");
                dtSerial.Columns.Remove("image");
                dtSerial.WriteXml(xmlPath);
            }
            else if (tbTitle.Text == Resources.TypesOfTelecastsText)
            {
                foreach (Control userctrl in pGenre.Controls)
                {
                    foreach (Control ctrl in userctrl.Controls)
                    {
                        if (ctrl is DataGridViewExt)
                        {
                            (ctrl as DataGridViewExt).EndEdit();
                            foreach (DataGridViewRow dgvr in (ctrl as DataGridViewExt).Rows)
                            {
                                foreach (DataRow drGenre in _genres.GenresTable.Rows)
                                {
                                   if ((int)dgvr.Cells["colGenreId"].Value == (int) drGenre["id"])
                                   {
                                       drGenre.BeginEdit();
                                       drGenre["visible"] = (bool)dgvr.Cells["colVisible"].Value;
                                       drGenre["genrename"] = dgvr.Cells["colGenreName"].Value.ToString();
                                       drGenre.EndEdit();
                                       break;
                                   }
                                }
                            }
                        }
                    }
                }
                string xmlPath = Path.Combine(Application.StartupPath, Preferences.xmlGenres);
                DataTable dtSerial = _genres.GenresTable.Copy();
                dtSerial.Columns.Remove("id");
                dtSerial.Columns.Remove("image");
                dtSerial.WriteXml(xmlPath);
            }
            btnSave.Enabled = false;
        }
       
        /// <summary>
        /// Перемещение класса вверх
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUp_Click(object sender, EventArgs e)
        {
            if (tbTitle.Text == Resources.KeywordsText)
            {
                foreach (Control userctrl in pGenre.Controls)
                {
                    foreach (Control ctrl in userctrl.Controls)
                    {
                        if (ctrl is DataGridViewExt)
                        {
                            DataRow[] drsMoveUp = _genres.ClassifTable.Select("id = " +
                                                        (ctrl as DataGridViewExt).CurrentRow.Cells["colGenreId"].Value);
                            int index = 0;
                            DataRow drTemp = _genres.ClassifTable.NewRow();
                            foreach (DataRow drMoveUp in drsMoveUp)
                            {
                                index = _genres.ClassifTable.Rows.IndexOf(drMoveUp);
                                drTemp["image"] = drMoveUp["image"];
                                drTemp["gid"] = drMoveUp["gid"];
                                drTemp["contain"] = drMoveUp["contain"];
                                drTemp["noncontain"] = drMoveUp["noncontain"];
                                drTemp["deleteafter"] = drMoveUp["deleteafter"];
                            }
                            _genres.ClassifTable.Rows.RemoveAt(index);
                            _genres.ClassifTable.Rows.InsertAt(drTemp, --index);

                            foreach (DataGridViewRow dgrv in (ctrl as DataGridViewExt).Rows)
                            {
                                if (dgrv.Cells["colGenreID"].Value.ToString() == drTemp["id"].ToString())
                                {
                                    index = dgrv.Index;
                                    break;
                                }
                            }
                                
                            (ctrl as DataGridViewExt).FirstDisplayedScrollingRowIndex = index;
                            (ctrl as DataGridViewExt).CurrentCell = (ctrl as DataGridViewExt).Rows[index].Cells["colContain"];
                            (ctrl as DataGridViewExt).Rows[index].Selected = true;
                            foreach (DataRow drClassifRow in _genres.ClassifTable.Rows)
                            {
                                drClassifRow["prior"] = _genres.ClassifTable.Rows.IndexOf(drClassifRow);
                            }
                            btnSave.Enabled = true;
                        }
                    }
                }
            }
            else if (tbTitle.Text == Resources.TypesOfTelecastsText)
            {
                foreach (Control userctrl in pGenre.Controls)
                {
                    foreach (Control ctrl in userctrl.Controls)
                    {
                        if (ctrl is DataGridViewExt)
                        {
                            DataRow[] drsMoveUp = _genres.GenresTable.Select("id = " +
                                                        (ctrl as DataGridViewExt).CurrentRow.Cells["colGenreId"].Value);
                            int index = 0;
                            DataRow drTemp = _genres.GenresTable.NewRow();
                            foreach (DataRow drMoveUp in drsMoveUp)
                            {
                                index = _genres.GenresTable.Rows.IndexOf(drMoveUp);
                                drTemp["image"] = drMoveUp["image"];
                                drTemp["genrename"] = drMoveUp["genrename"];
                                drTemp["imagename"] = drMoveUp["imagename"];
                                drTemp["visible"] = drMoveUp["visible"];
                            }
                            _genres.GenresTable.Rows.RemoveAt(index);
                            _genres.GenresTable.Rows.InsertAt(drTemp, --index);

                            foreach (DataGridViewRow dgrv in (ctrl as DataGridViewExt).Rows)
                            {
                                if (dgrv.Cells["colGenreID"].Value.ToString() == drTemp["id"].ToString())
                                {
                                    index = dgrv.Index;
                                    break;
                                }
                            }

                            (ctrl as DataGridViewExt).FirstDisplayedScrollingRowIndex = index;
                            (ctrl as DataGridViewExt).CurrentCell = (ctrl as DataGridViewExt).Rows[index].Cells["colGenreImage"];
                            (ctrl as DataGridViewExt).Rows[index].Selected = true;
                            
                            btnSave.Enabled = true;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Перемещение класса вниз
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDown_Click(object sender, EventArgs e)
        {
            if (tbTitle.Text == Resources.KeywordsText)
            {
                foreach (Control userctrl in pGenre.Controls)
                {
                    foreach (Control ctrl in userctrl.Controls)
                    {
                        if (ctrl is DataGridViewExt)
                        {
                            DataRow[] drsMoveUp = _genres.ClassifTable.Select("id = " +
                                                        (ctrl as DataGridViewExt).CurrentRow.Cells["colGenreId"].Value);
                            int index = 0;
                            DataRow drTemp = _genres.ClassifTable.NewRow();
                            foreach (DataRow drMoveDown in drsMoveUp)
                            {
                                index = _genres.ClassifTable.Rows.IndexOf(drMoveDown);
                                drTemp["image"] = drMoveDown["image"];
                                drTemp["gid"] = drMoveDown["gid"];
                                drTemp["contain"] = drMoveDown["contain"];
                                drTemp["noncontain"] = drMoveDown["noncontain"];
                                drTemp["deleteafter"] = drMoveDown["deleteafter"];
                            }
                            _genres.ClassifTable.Rows.RemoveAt(index);
                            _genres.ClassifTable.Rows.InsertAt(drTemp, ++index);

                            foreach (DataGridViewRow dgrv in (ctrl as DataGridViewExt).Rows)
                            {
                                if (dgrv.Cells["colGenreID"].Value.ToString() == drTemp["id"].ToString())
                                {
                                    index = dgrv.Index;
                                    break;
                                }
                            }

                            (ctrl as DataGridViewExt).FirstDisplayedScrollingRowIndex = index;
                            (ctrl as DataGridViewExt).CurrentCell = (ctrl as DataGridViewExt).Rows[index].Cells["colContain"];
                            (ctrl as DataGridViewExt).Rows[index].Selected = true;
                            foreach (DataRow drClassifRow in _genres.ClassifTable.Rows)
                            {
                                drClassifRow["prior"] = _genres.ClassifTable.Rows.IndexOf(drClassifRow);
                            }
                            btnSave.Enabled = true;
                        }
                    }
                }
            }else if (tbTitle.Text == Resources.TypesOfTelecastsText)
            {
                foreach (Control userctrl in pGenre.Controls)
                {
                    foreach (Control ctrl in userctrl.Controls)
                    {
                        if (ctrl is DataGridViewExt)
                        {
                            DataRow[] drsMoveDown = _genres.GenresTable.Select("id = " +
                                                        (ctrl as DataGridViewExt).CurrentRow.Cells["colGenreId"].Value);
                            int index = 0;
                            DataRow drTemp = _genres.GenresTable.NewRow();
                            foreach (DataRow drMoveDown in drsMoveDown)
                            {
                                index = _genres.GenresTable.Rows.IndexOf(drMoveDown);
                                drTemp["image"] = drMoveDown["image"];
                                drTemp["genrename"] = drMoveDown["genrename"];
                                drTemp["imagename"] = drMoveDown["imagename"];
                                drTemp["visible"] = drMoveDown["visible"];
                            }
                            _genres.GenresTable.Rows.RemoveAt(index);
                            _genres.GenresTable.Rows.InsertAt(drTemp, ++index);

                            foreach (DataGridViewRow dgrv in (ctrl as DataGridViewExt).Rows)
                            {
                                if (dgrv.Cells["colGenreID"].Value.ToString() == drTemp["id"].ToString())
                                {
                                    index = dgrv.Index;
                                    break;
                                }
                            }

                            (ctrl as DataGridViewExt).FirstDisplayedScrollingRowIndex = index;
                            (ctrl as DataGridViewExt).CurrentCell = (ctrl as DataGridViewExt).Rows[index].Cells["colGenreImage"];
                            (ctrl as DataGridViewExt).Rows[index].Selected = true;
                            
                            btnSave.Enabled = true;
                        }
                    }
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            foreach (Control userctrl in pGenre.Controls)
            {
                foreach (Control ctrl in userctrl.Controls)
                {
                    if (ctrl is DataGridViewExt)
                    {
                        if ((ctrl as DataGridViewExt).SelectedRows.Count > 0) // Есть выделенные строки:
                        {
                            if (Statics.ShowDialog(Resources.ConfirmChoiceText, Resources.DeleteAllRowsText, 
                                MessageDialog.MessageIcon.Question, 
                                MessageDialog.MessageButtons.YesNo) == DialogResult.Yes)
                            {
                                foreach (DataGridViewRow dgrv in (ctrl as DataGridViewExt).SelectedRows)
                                {
                                    if (tbTitle.Text == Resources.KeywordsText)
                                    {
                                        DataRow[] drsRemove = _genres.ClassifTable.Select("id = " +
                                                                                          dgrv.Cells["colGenreId"].
                                                                                              Value);
                                        int index = 0;
                                        DataRow drTemp = _genres.ClassifTable.NewRow();
                                        foreach (DataRow drRemove in drsRemove)
                                        {
                                            index = _genres.ClassifTable.Rows.IndexOf(drRemove);
                                        }
                                        _genres.ClassifTable.Rows.RemoveAt(index);
                                    }
                                    else if (tbTitle.Text == Resources.TypesOfTelecastsText)
                                    {
                                        DataRow[] drsRemove = _genres.GenresTable.Select("id = " +
                                                                                         dgrv.Cells["colGenreId"].
                                                                                             Value);
                                        int index = 0;
                                        DataRow drTemp = _genres.ClassifTable.NewRow();
                                        foreach (DataRow drRemove in drsRemove)
                                        {
                                            index = _genres.ClassifTable.Rows.IndexOf(drRemove);
                                        }
                                        _genres.GenresTable.Rows.RemoveAt(index);
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
                                        DataRow[] drsRemove = _genres.ClassifTable.Select("id = " +
                                                                                          (ctrl as DataGridViewExt).
                                                                                              Rows[dgvc.RowIndex].
                                                                                              Cells["colGenreId"].
                                                                                              Value);
                                        int index = 0;
                                        foreach (DataRow drRemove in drsRemove)
                                        {
                                            index = _genres.ClassifTable.Rows.IndexOf(drRemove);
                                        }
                                        _genres.ClassifTable.Rows.RemoveAt(index);
                                    }
                                    else if (tbTitle.Text == Resources.TypesOfTelecastsText)
                                    {
                                        DataRow[] drsRemove = _genres.GenresTable.Select("id = " +
                                                                                         (ctrl as DataGridViewExt).Rows[
                                                                                             dgvc.RowIndex].Cells[
                                                                                                 "colGenreId"].Value);
                                        int index = 0;
                                        foreach (DataRow drRemove in drsRemove)
                                        {
                                            index = _genres.GenresTable.Rows.IndexOf(drRemove);
                                        }
                                        _genres.GenresTable.Rows.RemoveAt(index);
                                    }
                                }
                                btnSave.Enabled = true;
                            }
                        }

                        foreach (DataRow drClassifRow in _genres.ClassifTable.Rows)
                        {
                            drClassifRow["prior"] = _genres.ClassifTable.Rows.IndexOf(drClassifRow);
                        }
                        btnSave.Enabled = true;
                    }
                }
            }
        }


        private void GenreForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (btnSave.Enabled)
            {
                if (Statics.ShowDialog(Resources.ConfirmChoiceText, Resources.SaveAllChangesText, 
                    MessageDialog.MessageIcon.Question, MessageDialog.MessageButtons.YesNo) == DialogResult.Yes)
                {
                    btnSave_Click(sender, e);
                }
            }
        }
    }
}
