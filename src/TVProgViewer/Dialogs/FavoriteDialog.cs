using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows.Forms;
using TVProgViewer.TVProgApp.Classes;
using TVProgViewer.TVProgApp.Dialogs;
using TVProgViewer.TVProgApp.Properties;

namespace TVProgViewer.TVProgApp
{
    public partial class FavoriteDialog : Form
    {
        public Favorite FavoriteClass { get; set; }

        public FavoriteDialog()
        {
            FavoriteClass = new Favorite();
            InitializeComponent();
        }

        public FavoriteDialog(Favorite favorite)
        {
            InitializeComponent();
            FavoriteClass = favorite;
            tbRatingName.Text = favorite.FavoriteName;
            picBox.Image = favorite.FavoriteImage;
            chkVisible.Checked = favorite.Visible;
        }

        /// <summary>
        /// Сохранение результатов
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            FavoriteClass.FavoriteName = tbRatingName.Text;
            FavoriteClass.FavoriteImage = picBox.Image;
            FavoriteClass.Visible = chkVisible.Checked;
            if (picBox.Image != null && openFileDialog1.SafeFileName != String.Empty)
            {
                string binPath = Path.Combine(Application.StartupPath, Preferences.binData);
                if (Owner.Owner != null && Owner.Owner is MainForm)
                {
                    FavoriteClass.FileName = openFileDialog1.SafeFileName;
                    (Owner.Owner as MainForm).SerialData.FavsImageList.Add(
                            new Tuple<string, Image>(openFileDialog1.SafeFileName, picBox.Image));
                    Stream dataStream = File.Create(binPath);
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(dataStream, (Owner.Owner as MainForm).SerialData);
                    dataStream.Close();
                }
            }
        }

        private void btnOpenImage_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Image.GetThumbnailImageAbort myCallback =
                    new Image.GetThumbnailImageAbort(ThumbnailCallback);
                try
                {
                    picBox.Image = Image.FromFile(openFileDialog1.FileName).GetThumbnailImage(25, 25, myCallback,
                                                                                          IntPtr.Zero);
                }
                catch (Exception ex)
                {
                    Statics.EL.LogException(ex);
                    Statics.ShowDialog(Resources.ErrorText, ex.Message, 
                        MessageDialog.MessageIcon.Alert, 
                        MessageDialog.MessageButtons.Ok);
                }
            }
        }

        private bool ThumbnailCallback()
        {
            return false;
        }
    }
}
