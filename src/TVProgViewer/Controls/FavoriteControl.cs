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
    public partial class FavoriteControl : UserControl
    {
        private Favorites _favorites;
        public FavoriteControl(Favorites favorites)
        {
            InitializeComponent();
            _favorites = favorites;
            dgFavorites.Style(DataGridViewExtentions.Styles.TVGridWithEdit);
            dgFavorites.FitColumns(false, false, true);
        }

        private void genreControl_Load(object sender, EventArgs e)
        {
            imgList.Images.Clear();
            imgList.Images.Add("fav", Resources.favorites25);
            dgFavorites.DataSource = _favorites.FavoritesTable;
            Preferences.SetFonts(Settings.Default.InterfaceFont, Settings.Default.TableFont);
        }

        private void dgGenres_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.Value != null)
            {
                if (e.Value.ToString() == Resources.RatingText && e.RowIndex == -1)
                {
                    e.PaintBackground(e.ClipBounds, false);

                    Point pt = e.CellBounds.Location;
                    int offset = (e.CellBounds.Width - this.imgList.Images["fav"].Size.Width)/2;
                    pt.X += offset;
                    pt.Y += (e.CellBounds.Height - this.imgList.Images["fav"].Size.Height)/2;
                    this.imgList.Draw(e.Graphics, pt, 0);
                    e.Handled = true;
                }
            }
        }

        private void dgGenres_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (Parent != null)
                if (Parent.Parent != null)
                {
                    (Parent.Parent as RatingForm).BtnSaveEnabled = true;
                }
        }

        private void dgGenres_SelectionChanged(object sender, EventArgs e)
        {
            // Установка доступности кнопок:
            if (dgFavorites.CurrentCell != null)
            {
                if (Parent != null)
                    if (Parent.Parent != null)
                    {
                        (Parent.Parent as RatingForm).BtnEditEnabled =
                            (Parent.Parent as RatingForm).BtnDeleteEnabled = true;
                    }
                if (dgFavorites.CurrentCell.RowIndex == 0)
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
                if (dgFavorites.CurrentCell.RowIndex == dgFavorites.Rows.Count - 1)
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
                    if (Parent.Parent != null)
                    {
                        (Parent.Parent as RatingForm).BtnEditEnabled =
                            (Parent.Parent as RatingForm).BtnDeleteEnabled =
                            (Parent.Parent as RatingForm).BtnDownEnabled =
                            (Parent.Parent as RatingForm).BtnUpEnabled = false;
                    }
            }
        }
    }
}
