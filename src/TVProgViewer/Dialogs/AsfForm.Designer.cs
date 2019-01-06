namespace TVProgViewer.TVProgApp.Dialogs
{
    partial class AsfForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AsfForm));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.cbAsf = new System.Windows.Forms.ComboBox();
            this.chbIndex = new System.Windows.Forms.CheckBox();
            this.lblVideoInfo = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblAudioInfo = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // cbAsf
            // 
            resources.ApplyResources(this.cbAsf, "cbAsf");
            this.cbAsf.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbAsf.FormattingEnabled = true;
            this.cbAsf.Name = "cbAsf";
            this.cbAsf.SelectedIndexChanged += new System.EventHandler(this.cbAsf_SelectedIndexChanged);
            // 
            // chbIndex
            // 
            resources.ApplyResources(this.chbIndex, "chbIndex");
            this.chbIndex.Checked = true;
            this.chbIndex.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chbIndex.Name = "chbIndex";
            this.chbIndex.UseVisualStyleBackColor = true;
            this.chbIndex.CheckedChanged += new System.EventHandler(this.chbIndex_CheckedChanged);
            // 
            // lblVideoInfo
            // 
            resources.ApplyResources(this.lblVideoInfo, "lblVideoInfo");
            this.lblVideoInfo.Name = "lblVideoInfo";
            // 
            // lblDescription
            // 
            resources.ApplyResources(this.lblDescription, "lblDescription");
            this.lblDescription.Name = "lblDescription";
            // 
            // lblAudioInfo
            // 
            resources.ApplyResources(this.lblAudioInfo, "lblAudioInfo");
            this.lblAudioInfo.Name = "lblAudioInfo";
            // 
            // AsfForm
            // 
            this.AcceptButton = this.btnOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblAudioInfo);
            this.Controls.Add(this.lblVideoInfo);
            this.Controls.Add(this.chbIndex);
            this.Controls.Add(this.cbAsf);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AsfForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AsfForm_FormClosing);
            this.Load += new System.EventHandler(this.AsfForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ComboBox cbAsf;
        private System.Windows.Forms.CheckBox chbIndex;
        private System.Windows.Forms.Label lblVideoInfo;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblAudioInfo;
    }
}