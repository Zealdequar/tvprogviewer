namespace TVProgViewer.TVProgApp
{
    partial class ClassifGenreControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ClassifGenreControl));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgClassifGenres = new TVProgViewer.TVProgApp.DataGridViewExt();
            this.colGenreID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colGid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colType = new System.Windows.Forms.DataGridViewImageColumn();
            this.colContain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDontContain = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDeleteAfter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrior = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgClassifGenres)).BeginInit();
            this.SuspendLayout();
            // 
            // dgClassifGenres
            // 
            resources.ApplyResources(this.dgClassifGenres, "dgClassifGenres");
            this.dgClassifGenres.BackgroundColor2 = System.Drawing.Color.Empty;
            this.dgClassifGenres.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgClassifGenres.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colGenreID,
            this.colGid,
            this.colType,
            this.colContain,
            this.colDontContain,
            this.colDeleteAfter,
            this.colPrior});
            this.dgClassifGenres.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Horizontal;
            this.dgClassifGenres.IsGradient = false;
            this.dgClassifGenres.Name = "dgClassifGenres";
            this.dgClassifGenres.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgClassifGenres_CellEndEdit);
            this.dgClassifGenres.SelectionChanged += new System.EventHandler(this.dgClassifGenres_SelectionChanged);
            // 
            // colGenreID
            // 
            this.colGenreID.DataPropertyName = "id";
            resources.ApplyResources(this.colGenreID, "colGenreID");
            this.colGenreID.Name = "colGenreID";
            // 
            // colGid
            // 
            this.colGid.DataPropertyName = "gid";
            resources.ApplyResources(this.colGid, "colGid");
            this.colGid.Name = "colGid";
            // 
            // colType
            // 
            this.colType.DataPropertyName = "image";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.NullValue = null;
            this.colType.DefaultCellStyle = dataGridViewCellStyle1;
            resources.ApplyResources(this.colType, "colType");
            this.colType.Name = "colType";
            this.colType.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colType.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // colContain
            // 
            this.colContain.DataPropertyName = "contain";
            resources.ApplyResources(this.colContain, "colContain");
            this.colContain.Name = "colContain";
            // 
            // colDontContain
            // 
            this.colDontContain.DataPropertyName = "noncontain";
            resources.ApplyResources(this.colDontContain, "colDontContain");
            this.colDontContain.Name = "colDontContain";
            // 
            // colDeleteAfter
            // 
            this.colDeleteAfter.DataPropertyName = "deleteafter";
            resources.ApplyResources(this.colDeleteAfter, "colDeleteAfter");
            this.colDeleteAfter.Name = "colDeleteAfter";
            // 
            // colPrior
            // 
            this.colPrior.DataPropertyName = "prior";
            resources.ApplyResources(this.colPrior, "colPrior");
            this.colPrior.Name = "colPrior";
            // 
            // ClassifGenreControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dgClassifGenres);
            this.Name = "ClassifGenreControl";
            this.Load += new System.EventHandler(this.ClassifGenreControl_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgClassifGenres)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridViewExt dgClassifGenres;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGenreID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGid;
        private System.Windows.Forms.DataGridViewImageColumn colType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colContain;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDontContain;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDeleteAfter;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrior;
    }
}
