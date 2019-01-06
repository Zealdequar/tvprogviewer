namespace TVProgViewer.TVProgApp
{
    partial class CtrlDays
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CtrlDays));
            this.listViewDays = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imgList = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // listViewDays
            // 
            resources.ApplyResources(this.listViewDays, "listViewDays");
            this.listViewDays.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewDays.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listViewDays.CheckBoxes = true;
            this.listViewDays.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader2});
            this.listViewDays.HotTracking = true;
            this.listViewDays.HoverSelection = true;
            this.listViewDays.Name = "listViewDays";
            this.listViewDays.SmallImageList = this.imgList;
            this.listViewDays.UseCompatibleStateImageBehavior = false;
            this.listViewDays.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            resources.ApplyResources(this.columnHeader3, "columnHeader3");
            // 
            // columnHeader2
            // 
            resources.ApplyResources(this.columnHeader2, "columnHeader2");
            // 
            // imgList
            // 
            this.imgList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imgList.ImageStream")));
            this.imgList.TransparentColor = System.Drawing.Color.Transparent;
            this.imgList.Images.SetKeyName(0, "Mon.png");
            this.imgList.Images.SetKeyName(1, "Tue.png");
            this.imgList.Images.SetKeyName(2, "Wen.png");
            this.imgList.Images.SetKeyName(3, "Ths.png");
            this.imgList.Images.SetKeyName(4, "Fri.png");
            this.imgList.Images.SetKeyName(5, "Sat.png");
            this.imgList.Images.SetKeyName(6, "Sun.png");
            // 
            // CtrlDays
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.listViewDays);
            this.Name = "CtrlDays";
            this.Load += new System.EventHandler(this.CtrlDays_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listViewDays;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ImageList imgList;
    }
}
