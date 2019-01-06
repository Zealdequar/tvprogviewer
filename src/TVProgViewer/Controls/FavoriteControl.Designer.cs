namespace TVProgViewer.TVProgApp
{
    partial class FavoriteControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FavoriteControl));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgFavorites = new TVProgViewer.TVProgApp.DataGridViewExt();
            this.colFavId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colVisible = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colFavImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.colFavName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colImageName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgFavorites)).BeginInit();
            this.SuspendLayout();
            // 
            // dgFavorites
            // 
            resources.ApplyResources(this.dgFavorites, "dgFavorites");
            this.dgFavorites.BackgroundColor2 = System.Drawing.Color.Empty;
            this.dgFavorites.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgFavorites.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFavId,
            this.colVisible,
            this.colFavImage,
            this.colFavName,
            this.colImageName});
            this.dgFavorites.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.dgFavorites.IsGradient = false;
            this.dgFavorites.Name = "dgFavorites";
            this.dgFavorites.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgGenres_CellEndEdit);
            this.dgFavorites.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dgGenres_CellPainting);
            this.dgFavorites.SelectionChanged += new System.EventHandler(this.dgGenres_SelectionChanged);
            // 
            // colFavId
            // 
            this.colFavId.DataPropertyName = "id";
            resources.ApplyResources(this.colFavId, "colFavId");
            this.colFavId.Name = "colFavId";
            // 
            // colVisible
            // 
            this.colVisible.DataPropertyName = "visible";
            resources.ApplyResources(this.colVisible, "colVisible");
            this.colVisible.Name = "colVisible";
            this.colVisible.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colVisible.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colFavImage
            // 
            this.colFavImage.DataPropertyName = "image";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.NullValue = null;
            this.colFavImage.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.colFavImage, "colFavImage");
            this.colFavImage.Name = "colFavImage";
            // 
            // colFavName
            // 
            this.colFavName.DataPropertyName = "favname";
            resources.ApplyResources(this.colFavName, "colFavName");
            this.colFavName.Name = "colFavName";
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
            // FavoriteControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgFavorites);
            this.Name = "FavoriteControl";
            this.Load += new System.EventHandler(this.genreControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgFavorites)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridViewExt dgFavorites;
        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFavId;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colVisible;
        private System.Windows.Forms.DataGridViewImageColumn colFavImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFavName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colImageName;
    }
}
