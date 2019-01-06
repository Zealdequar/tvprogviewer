using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp
{
    public partial class ClassifGenreControl : UserControl
    {
        private Genres _genres;
        public ClassifGenreControl(Genres genres)
        {
            InitializeComponent();
            dgClassifGenres.Style(DataGridViewExtentions.Styles.TVGridWithEdit);
            dgClassifGenres.FitColumns(false,false, true);
            _genres = genres;
            dgClassifGenres.AutoGenerateColumns = false;
            if (!_genres.ClassifTable.Columns.Contains("prior"))
            {
                _genres.ClassifTable.Columns.Add("prior", typeof (int));
            }
            foreach (DataRow drClassifRow in _genres.ClassifTable.Rows)
            {
                drClassifRow["prior"] = _genres.ClassifTable.Rows.IndexOf(drClassifRow);
            }
            dgClassifGenres.DataSource = _genres.ClassifTable;
            dgClassifGenres.Sort(colPrior, ListSortDirection.Ascending);
        }

        private void dgClassifGenres_SelectionChanged(object sender, EventArgs e)
        {
            if (dgClassifGenres.CurrentCell != null)
            {
                if (Parent != null)
                    if (Parent.Parent != null)
                    {
                        (Parent.Parent as GenreForm).BtnEditEnabled =
                            (Parent.Parent as GenreForm).BtnDeleteEnabled = true;
                    }
                if (dgClassifGenres.CurrentCell.RowIndex == 0)
                {
                    if (Parent != null)
                        if (Parent.Parent != null)
                        {
                            (Parent.Parent as GenreForm).BtnUpEnabled = false;
                        }
                }
                else
                {
                    if (Parent != null)
                        if (Parent.Parent != null)
                        {
                            (Parent.Parent as GenreForm).BtnUpEnabled = true;
                        }
                }
                if (dgClassifGenres.CurrentCell.RowIndex == dgClassifGenres.Rows.Count - 1)
                {
                    if (Parent != null)
                        if (Parent.Parent != null)
                        {
                            (Parent.Parent as GenreForm).BtnDownEnabled = false;
                        }
                }
                else
                {
                    if (Parent != null)
                        if (Parent.Parent != null)
                        {
                            (Parent.Parent as GenreForm).BtnDownEnabled = true;
                        }
                }
            }
            else
            {
                if (Parent != null)
                    if (Parent.Parent != null)
                    {
                        (Parent.Parent as GenreForm).BtnDownEnabled =
                            (Parent.Parent as GenreForm).BtnUpEnabled =
                            (Parent.Parent as GenreForm).BtnEditEnabled =
                            (Parent.Parent as GenreForm).BtnDeleteEnabled = false;
                    }
            }
        }

        private void dgClassifGenres_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (Parent != null)
                if (Parent.Parent != null)
                {
                    (Parent.Parent as GenreForm).BtnSaveEnabled = true;
                }
        }

        private void ClassifGenreControl_Load(object sender, EventArgs e)
        {
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
        }
    }
}
