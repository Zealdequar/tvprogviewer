namespace TVProgViewer.TVProgApp
{
    partial class GenreControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GenreControl));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgGenres = new TVProgViewer.TVProgApp.DataGridViewExt();
            this.colGenreId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVisible = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colGenreImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.colGenreName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colImageName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgGenres)).BeginInit();
            this.SuspendLayout();
            // 
            // dgGenres
            // 
            resources.ApplyResources(this.dgGenres, "dgGenres");
            this.dgGenres.BackgroundColor2 = System.Drawing.Color.Empty;
            this.dgGenres.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgGenres.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colGenreId,
            this.colVisible,
            this.colGenreImage,
            this.colGenreName,
            this.colImageName});
            this.dgGenres.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.dgGenres.IsGradient = false;
            this.dgGenres.Name = "dgGenres";
            this.dgGenres.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgGenres_CellEndEdit);
            this.dgGenres.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgGenres_CellPainting);
            this.dgGenres.SelectionChanged += new System.EventHandler(this.dgGenres_SelectionChanged);
            // 
            // colGenreId
            // 
            this.colGenreId.DataPropertyName = "id";
            resources.ApplyResources(this.colGenreId, "colGenreId");
            this.colGenreId.Name = "colGenreId";
            // 
            // colVisible
            // 
            this.colVisible.DataPropertyName = "visible";
            resources.ApplyResources(this.colVisible, "colVisible");
            this.colVisible.Name = "colVisible";
            this.colVisible.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colVisible.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colGenreImage
            // 
            this.colGenreImage.DataPropertyName = "image";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.NullValue = null;
            this.colGenreImage.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.colGenreImage, "colGenreImage");
            this.colGenreImage.Name = "colGenreImage";
            // 
            // colGenreName
            // 
            this.colGenreName.DataPropertyName = "genrename";
            resources.ApplyResources(this.colGenreName, "colGenreName");
            this.colGenreName.Name = "colGenreName";
            // 
            // colImageName
            // 
            this.colImageName.DataPropertyName = "imagename";
            resources.ApplyResources(this.colImageName, "colImageName");
            this.colImageName.Name = "colImageName";
            // 
            // imgList
            // 
            this.imgList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            resources.ApplyResources(this.imgList, "imgList");
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // GenreControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgGenres);
            this.Name = "GenreControl";
            this.Load += new System.EventHandler(this.genreControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgGenres)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridViewExt dgGenres;
        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGenreId;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colVisible;
        private System.Windows.Forms.DataGridViewImageColumn colGenreImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGenreName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colImageName;
    }
}
