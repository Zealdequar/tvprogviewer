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
    public partial class ClassifFavoriteControl : UserControl
    {
        private Favorites _favorites;
        public ClassifFavoriteControl(Favorites favorites)
        {
            InitializeComponent();
            dgClassifFavorites.Style(DataGridViewExtentions.Styles.TVGridWithEdit);
            dgClassifFavorites.FitColumns(false, false, true);
            _favorites = favorites;
            dgClassifFavorites.AutoGenerateColumns = false;
            if (!_favorites.ClassifTable.Columns.Contains("prior"))
            {
                _favorites.ClassifTable.Columns.Add("prior", typeof (int));
            }
            foreach (DataRow drClassifRow in _favorites.ClassifTable.Rows)
            {
                drClassifRow["prior"] = _favorites.ClassifTable.Rows.IndexOf(drClassifRow);
            }
            dgClassifFavorites.DataSource = _favorites.ClassifTable;
            dgClassifFavorites.Sort(colPrior, ListSortDirection.Ascending);
        }

        private void dgClassifGenres_SelectionChanged(object sender, EventArgs e)
        {
            if (dgClassifFavorites.CurrentCell != null)
            {
                if (Parent != null)
                    if (Parent.Parent != null)
                    {
                        (Parent.Parent as RatingForm).BtnEditEnabled =
                            (Parent.Parent as RatingForm).BtnDeleteEnabled = true;
                    }
                if (dgClassifFavorites.CurrentCell.RowIndex == 0)
                {
                    if (Parent != null)
                        if (Parent.Parent != null)
                        {
                            (Parent.Parent as RatingForm).BtnUpEnabled = false;
                        }
                }
                else
                {
                    if (Parent != null)
                        if (Parent.Parent != null)
                        {
                            (Parent.Parent as RatingForm).BtnUpEnabled = true;
                        }
                }
                if (dgClassifFavorites.CurrentCell.RowIndex == dgClassifFavorites.Rows.Count - 1)
                {
                    if (Parent != null)
                        if (Parent.Parent != null)
                        {
                            (Parent.Parent as RatingForm).BtnDownEnabled = false;
                        }
                }
                else
                {
                    if (Parent != null)
                        if (Parent.Parent != null)
                        {
                            (Parent.Parent as RatingForm).BtnDownEnabled = true;
                        }
                }
            }
            else
            {
                if (Parent != null)
                {
                    if (Parent.Parent != null)
                    {
                        (Parent.Parent as RatingForm).BtnDownEnabled =
                            (Parent.Parent as RatingForm).BtnUpEnabled =
                            (Parent.Parent as RatingForm).BtnEditEnabled =
                            (Parent.Parent as RatingForm).BtnDeleteEnabled = false;
                    }
                }
            }
        }

        private void dgClassifGenres_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (Parent != null)
                if (Parent.Parent != null)
                {
                    (Parent.Parent as RatingForm).BtnSaveEnabled = true;
                }
        }

        private void ClassifFavoriteControl_Load(object sender, EventArgs e)
        {
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
        }
    }
}
