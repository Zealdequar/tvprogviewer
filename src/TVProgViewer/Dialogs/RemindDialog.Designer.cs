namespace TVProgViewer.TVProgApp.Dialogs
{
    partial class RemindDialog
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RemindDialog));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgRemain = new TVProgViewer.TVProgApp.DataGridViewExt();
            this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRemain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRec = new System.Windows.Forms.DataGridViewImageColumn();
            this.colBell = new System.Windows.Forms.DataGridViewImageColumn();
            this.colRating = new System.Windows.Forms.DataGridViewImageColumn();
            this.colGenre = new System.Windows.Forms.DataGridViewImageColumn();
            this.colAnons = new System.Windows.Forms.DataGridViewImageColumn();
            this.colIconChannel = new System.Windows.Forms.DataGridViewImageColumn();
            this.colNameChannel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDay = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFrom = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDesc = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colFavName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.imgLst = new System.Windows.Forms.ImageList(this.components);
            this.imageListBell = new System.Windows.Forms.ImageList(this.components);
            this.contextMenuTables = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.csmiSearch = new System.Windows.Forms.ToolStripMenuItem();
            this.csmiAddToFavorite = new System.Windows.Forms.ToolStripMenuItem();
            this.csmiAddToGenres = new System.Windows.Forms.ToolStripMenuItem();
            this.csmiPropChannel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.csmiCopyToBuffer = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.csmiChangeRating = new System.Windows.Forms.ToolStripMenuItem();
            this.csmiChangeType = new System.Windows.Forms.ToolStripMenuItem();
            this.pAnons = new System.Windows.Forms.Panel();
            this.rtbAnons = new System.Windows.Forms.RichTextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnSaveDesc = new System.Windows.Forms.Button();
            this.btnCancelDesc = new System.Windows.Forms.Button();
            this.btnExitDesc = new System.Windows.Forms.Button();
            this.splitter1 = new System.Windows.Forms.Splitter();
            ((System.ComponentModel.ISupportInitialize)(this.dgRemain)).BeginInit();
            this.contextMenuTables.SuspendLayout();
            this.pAnons.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgRemain
            // 
            resources.ApplyResources(this.dgRemain, "dgRemain");
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Yellow;
            this.dgRemain.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dgRemain.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.dgRemain.BackgroundColor2 = System.Drawing.Color.Empty;
            this.dgRemain.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleVertical;
            this.dgRemain.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dgRemain.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgRemain.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colId,
            this.colCID,
            this.colRemain,
            this.colRec,
            this.colBell,
            this.colRating,
            this.colGenre,
            this.colAnons,
            this.colIconChannel,
            this.colNameChannel,
            this.colDay,
            this.colFrom,
            this.colTo,
            this.colTitle,
            this.colCategory,
            this.colDesc,
            this.colFavName});
            this.dgRemain.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.dgRemain.GridColor = System.Drawing.SystemColors.ScrollBar;
            this.dgRemain.IsGradient = false;
            this.dgRemain.Name = "dgRemain";
            this.dgRemain.RowHeadersVisible = false;
            dataGridViewCellStyle9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.dgRemain.RowsDefaultCellStyle = dataGridViewCellStyle9;
            this.dgRemain.RowTemplate.Height = 25;
            this.dgRemain.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgRemain_CellContentClick);
            this.dgRemain.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.SetSatteliteOnHeaderColumn);
            this.dgRemain.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dgRemain_RowsAdded);
            this.dgRemain.SelectionChanged += new System.EventHandler(this.dgRemain_SelectionChanged);
            this.dgRemain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dgRemain_MouseUp);
            // 
            // colId
            // 
            this.colId.DataPropertyName = "id";
            resources.ApplyResources(this.colId, "colId");
            this.colId.Name = "colId";
            this.colId.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // colCID
            // 
            this.colCID.DataPropertyName = "cid";
            resources.ApplyResources(this.colCID, "colCID");
            this.colCID.Name = "colCID";
            // 
            // colRemain
            // 
            this.colRemain.DataPropertyName = "remine";
            resources.ApplyResources(this.colRemain, "colRemain");
            this.colRemain.Name = "colRemain";
            // 
            // colRec
            // 
            this.colRec.DataPropertyName = "capture";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.NullValue = "null";
            this.colRec.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.colRec, "colRec");
            this.colRec.Name = "colRec";
            // 
            // colBell
            // 
            this.colBell.DataPropertyName = "bell";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.NullValue = null;
            this.colBell.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.colBell, "colBell");
            this.colBell.Name = "colBell";
            this.colBell.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colBell.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colRating
            // 
            this.colRating.DataPropertyName = "rating";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.NullValue = null;
            this.colRating.DefaultCellStyle = dataGridViewCellStyle4;
            resources.ApplyResources(this.colRating, "colRating");
            this.colRating.Name = "colRating";
            this.colRating.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colRating.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colGenre
            // 
            this.colGenre.DataPropertyName = "genre";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.NullValue = null;
            this.colGenre.DefaultCellStyle = dataGridViewCellStyle5;
            resources.ApplyResources(this.colGenre, "colGenre");
            this.colGenre.Name = "colGenre";
            this.colGenre.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colGenre.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colAnons
            // 
            this.colAnons.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colAnons.DataPropertyName = "anons";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.NullValue = null;
            this.colAnons.DefaultCellStyle = dataGridViewCellStyle6;
            resources.ApplyResources(this.colAnons, "colAnons");
            this.colAnons.Name = "colAnons";
            this.colAnons.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colAnons.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colIconChannel
            // 
            this.colIconChannel.DataPropertyName = "image";
            resources.ApplyResources(this.colIconChannel, "colIconChannel");
            this.colIconChannel.Name = "colIconChannel";
            // 
            // colNameChannel
            // 
            this.colNameChannel.DataPropertyName = "display-name";
            resources.ApplyResources(this.colNameChannel, "colNameChannel");
            this.colNameChannel.Name = "colNameChannel";
            this.colNameChannel.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // colDay
            // 
            this.colDay.DataPropertyName = "day";
            resources.ApplyResources(this.colDay, "colDay");
            this.colDay.Name = "colDay";
            // 
            // colFrom
            // 
            this.colFrom.DataPropertyName = "start";
            dataGridViewCellStyle7.Format = "t";
            dataGridViewCellStyle7.NullValue = null;
            this.colFrom.DefaultCellStyle = dataGridViewCellStyle7;
            resources.ApplyResources(this.colFrom, "colFrom");
            this.colFrom.Name = "colFrom";
            this.colFrom.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // colTo
            // 
            this.colTo.DataPropertyName = "stop";
            dataGridViewCellStyle8.Format = "t";
            this.colTo.DefaultCellStyle = dataGridViewCellStyle8;
            resources.ApplyResources(this.colTo, "colTo");
            this.colTo.Name = "colTo";
            // 
            // colTitle
            // 
            this.colTitle.DataPropertyName = "title";
            resources.ApplyResources(this.colTitle, "colTitle");
            this.colTitle.Name = "colTitle";
            // 
            // colCategory
            // 
            this.colCategory.DataPropertyName = "category";
            resources.ApplyResources(this.colCategory, "colCategory");
            this.colCategory.Name = "colCategory";
            // 
            // colDesc
            // 
            this.colDesc.DataPropertyName = "desc";
            resources.ApplyResources(this.colDesc, "colDesc");
            this.colDesc.Name = "colDesc";
            // 
            // colFavName
            // 
            this.colFavName.DataPropertyName = "favname";
            resources.ApplyResources(this.colFavName, "colFavName");
            this.colFavName.Name = "colFavName";
            // 
            // imgLst
            // 
            this.imgLst.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            resources.ApplyResources(this.imgLst, "imgLst");
            this.imgLst.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // imageListBell
            // 
            this.imageListBell.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListBell.ImageStream")));
            this.imageListBell.TransparentColor = System.Drawing.Color.Transparent;
            this.imageListBell.Images.SetKeyName(0, "bellheader.png");
            this.imageListBell.Images.SetKeyName(1, "capture_header.png");
            // 
            // contextMenuTables
            // 
            resources.ApplyResources(this.contextMenuTables, "contextMenuTables");
            this.contextMenuTables.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.csmiSearch,
            this.csmiAddToFavorite,
            this.csmiAddToGenres,
            this.csmiPropChannel,
            this.toolStripSeparator4,
            this.csmiCopyToBuffer,
            this.toolStripSeparator5,
            this.csmiChangeRating,
            this.csmiChangeType});
            this.contextMenuTables.Name = "contextMenuTables";
            // 
            // csmiSearch
            // 
            resources.ApplyResources(this.csmiSearch, "csmiSearch");
            this.csmiSearch.Image = global::TVProgViewer.TVProgApp.Properties.Resources.color_mixer_search_256;
            this.csmiSearch.Name = "csmiSearch";
            this.csmiSearch.Click += new System.EventHandler(this.csmiSearch_Click);
            // 
            // csmiAddToFavorite
            // 
            resources.ApplyResources(this.csmiAddToFavorite, "csmiAddToFavorite");
            this.csmiAddToFavorite.Name = "csmiAddToFavorite";
            this.csmiAddToFavorite.Click += new System.EventHandler(this.csmiAddToFavorite_Click);
            // 
            // csmiAddToGenres
            // 
            resources.ApplyResources(this.csmiAddToGenres, "csmiAddToGenres");
            this.csmiAddToGenres.Name = "csmiAddToGenres";
            this.csmiAddToGenres.Click += new System.EventHandler(this.csmiAddToGenres_Click);
            // 
            // csmiPropChannel
            // 
            resources.ApplyResources(this.csmiPropChannel, "csmiPropChannel");
            this.csmiPropChannel.Name = "csmiPropChannel";
            this.csmiPropChannel.Click += new System.EventHandler(this.csmiPropChannel_Click);
            // 
            // toolStripSeparator4
            // 
            resources.ApplyResources(this.toolStripSeparator4, "toolStripSeparator4");
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            // 
            // csmiCopyToBuffer
            // 
            resources.ApplyResources(this.csmiCopyToBuffer, "csmiCopyToBuffer");
            this.csmiCopyToBuffer.Name = "csmiCopyToBuffer";
            this.csmiCopyToBuffer.Click += new System.EventHandler(this.csmiCopyToBuffer_Click);
            // 
            // toolStripSeparator5
            // 
            resources.ApplyResources(this.toolStripSeparator5, "toolStripSeparator5");
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            // 
            // csmiChangeRating
            // 
            resources.ApplyResources(this.csmiChangeRating, "csmiChangeRating");
            this.csmiChangeRating.Name = "csmiChangeRating";
            // 
            // csmiChangeType
            // 
            resources.ApplyResources(this.csmiChangeType, "csmiChangeType");
            this.csmiChangeType.Name = "csmiChangeType";
            // 
            // pAnons
            // 
            resources.ApplyResources(this.pAnons, "pAnons");
            this.pAnons.Controls.Add(this.rtbAnons);
            this.pAnons.Controls.Add(this.panel2);
            this.pAnons.Name = "pAnons";
            // 
            // rtbAnons
            // 
            resources.ApplyResources(this.rtbAnons, "rtbAnons");
            this.rtbAnons.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rtbAnons.Name = "rtbAnons";
            this.rtbAnons.ModifiedChanged += new System.EventHandler(this.rtbAnons_ModifiedChanged);
            // 
            // panel2
            // 
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Controls.Add(this.btnSaveDesc);
            this.panel2.Controls.Add(this.btnCancelDesc);
            this.panel2.Controls.Add(this.btnExitDesc);
            this.panel2.Name = "panel2";
            // 
            // btnSaveDesc
            // 
            resources.ApplyResources(this.btnSaveDesc, "btnSaveDesc");
            this.btnSaveDesc.Name = "btnSaveDesc";
            this.btnSaveDesc.UseVisualStyleBackColor = true;
            this.btnSaveDesc.Click += new System.EventHandler(this.btnSaveDesc_Click);
            // 
            // btnCancelDesc
            // 
            resources.ApplyResources(this.btnCancelDesc, "btnCancelDesc");
            this.btnCancelDesc.Name = "btnCancelDesc";
            this.btnCancelDesc.UseVisualStyleBackColor = true;
            this.btnCancelDesc.Click += new System.EventHandler(this.btnCancelDesc_Click);
            // 
            // btnExitDesc
            // 
            resources.ApplyResources(this.btnExitDesc, "btnExitDesc");
            this.btnExitDesc.Name = "btnExitDesc";
            this.btnExitDesc.UseVisualStyleBackColor = true;
            this.btnExitDesc.Click += new System.EventHandler(this.btnExitDesc_Click);
            // 
            // splitter1
            // 
            resources.ApplyResources(this.splitter1, "splitter1");
            this.splitter1.Name = "splitter1";
            this.splitter1.TabStop = false;
            // 
            // RemindDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.dgRemain);
            this.Controls.Add(this.pAnons);
            this.Name = "RemindDialog";
            this.Load += new System.EventHandler(this.RemindDialog_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgRemain)).EndInit();
            this.contextMenuTables.ResumeLayout(false);
            this.pAnons.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridViewExt dgRemain;
        private System.Windows.Forms.ImageList imgLst;
        private System.Windows.Forms.ImageList imageListBell;
        private System.Windows.Forms.ContextMenuStrip contextMenuTables;
        private System.Windows.Forms.ToolStripMenuItem csmiSearch;
        private System.Windows.Forms.ToolStripMenuItem csmiAddToFavorite;
        private System.Windows.Forms.ToolStripMenuItem csmiAddToGenres;
        private System.Windows.Forms.ToolStripMenuItem csmiPropChannel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem csmiCopyToBuffer;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem csmiChangeRating;
        private System.Windows.Forms.ToolStripMenuItem csmiChangeType;
        private System.Windows.Forms.Panel pAnons;
        private System.Windows.Forms.RichTextBox rtbAnons;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnSaveDesc;
        private System.Windows.Forms.Button btnCancelDesc;
        private System.Windows.Forms.Button btnExitDesc;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.DataGridViewTextBoxColumn colId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRemain;
        private System.Windows.Forms.DataGridViewImageColumn colRec;
        private System.Windows.Forms.DataGridViewImageColumn colBell;
        private System.Windows.Forms.DataGridViewImageColumn colRating;
        private System.Windows.Forms.DataGridViewImageColumn colGenre;
        private System.Windows.Forms.DataGridViewImageColumn colAnons;
        private System.Windows.Forms.DataGridViewImageColumn colIconChannel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNameChannel;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDay;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFrom;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDesc;
        private System.Windows.Forms.DataGridViewTextBoxColumn colFavName;
    }
}