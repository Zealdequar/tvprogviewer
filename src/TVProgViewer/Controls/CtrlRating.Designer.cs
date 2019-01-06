namespace TVProgViewer.TVProgApp
{
    partial class CtrlRating
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtrlRating));
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.listViewRating = new System.Windows.Forms.ListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.pRemind = new System.Windows.Forms.Panel();
            this.chkWithoutRecord = new System.Windows.Forms.CheckBox();
            this.chkWithRecord = new System.Windows.Forms.CheckBox();
            this.chkWithoutRemind = new System.Windows.Forms.CheckBox();
            this.chkWithRemind = new System.Windows.Forms.CheckBox();
            this.pRemind.SuspendLayout();
            this.SuspendLayout();
            // 
            // imgList
            // 
            this.imgList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            resources.ApplyResources(this.imgList, "imgList");
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // listViewRating
            // 
            resources.ApplyResources(this.listViewRating, "listViewRating");
            this.listViewRating.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewRating.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewRating.CheckBoxes = true;
            this.listViewRating.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4});
            this.listViewRating.HotTracking = true;
            this.listViewRating.HoverSelection = true;
            this.listViewRating.Name = "listViewRating";
            this.listViewRating.SmallImageList = this.imgList;
            this.listViewRating.UseCompatibleStateImageBehavior = false;
            this.listViewRating.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader4
            // 
            resources.ApplyResources(this.columnHeader4, "columnHeader4");
            // 
            // pRemind
            // 
            resources.ApplyResources(this.pRemind, "pRemind");
            this.pRemind.Controls.Add(this.chkWithoutRecord);
            this.pRemind.Controls.Add(this.chkWithRecord);
            this.pRemind.Controls.Add(this.chkWithoutRemind);
            this.pRemind.Controls.Add(this.chkWithRemind);
            this.pRemind.Name = "pRemind";
            // 
            // chkWithoutRecord
            // 
            resources.ApplyResources(this.chkWithoutRecord, "chkWithoutRecord");
            this.chkWithoutRecord.Checked = true;
            this.chkWithoutRecord.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWithoutRecord.Name = "chkWithoutRecord";
            this.chkWithoutRecord.UseVisualStyleBackColor = true;
            // 
            // chkWithRecord
            // 
            resources.ApplyResources(this.chkWithRecord, "chkWithRecord");
            this.chkWithRecord.Checked = true;
            this.chkWithRecord.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWithRecord.Name = "chkWithRecord";
            this.chkWithRecord.UseVisualStyleBackColor = true;
            // 
            // chkWithoutRemind
            // 
            resources.ApplyResources(this.chkWithoutRemind, "chkWithoutRemind");
            this.chkWithoutRemind.Checked = true;
            this.chkWithoutRemind.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWithoutRemind.Name = "chkWithoutRemind";
            this.chkWithoutRemind.UseVisualStyleBackColor = true;
            // 
            // chkWithRemind
            // 
            resources.ApplyResources(this.chkWithRemind, "chkWithRemind");
            this.chkWithRemind.Checked = true;
            this.chkWithRemind.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWithRemind.Name = "chkWithRemind";
            this.chkWithRemind.UseVisualStyleBackColor = true;
            // 
            // CtrlRating
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listViewRating);
            this.Controls.Add(this.pRemind);
            this.Name = "CtrlRating";
            this.Load += new System.EventHandler(this.CtrlRating_Load);
            this.pRemind.ResumeLayout(false);
            this.pRemind.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ImageList imgList;
        private System.Windows.Forms.ListView listViewRating;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.Panel pRemind;
        private System.Windows.Forms.CheckBox chkWithoutRemind;
        private System.Windows.Forms.CheckBox chkWithRemind;
        private System.Windows.Forms.CheckBox chkWithoutRecord;
        private System.Windows.Forms.CheckBox chkWithRecord;
    }
}
