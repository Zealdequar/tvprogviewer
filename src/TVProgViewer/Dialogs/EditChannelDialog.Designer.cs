namespace TVProgViewer.TVProgApp
{
    partial class EditChannelDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditChannelDialog));
            this.chbShowChannel = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tlpChannel = new System.Windows.Forms.TableLayoutPanel();
            this.numNumber = new System.Windows.Forms.NumericUpDown();
            this.lblEmblema = new System.Windows.Forms.Label();
            this.pbImage = new System.Windows.Forms.PictureBox();
            this.lblDiff = new System.Windows.Forms.Label();
            this.tbmDiff = new System.Windows.Forms.MaskedTextBox();
            this.lblSyn = new System.Windows.Forms.Label();
            this.tbSyn = new System.Windows.Forms.TextBox();
            this.lblChannel = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.tlpChannel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).BeginInit();
            this.SuspendLayout();
            // 
            // chbShowChannel
            // 
            resources.ApplyResources(this.chbShowChannel, "chbShowChannel");
            this.chbShowChannel.Name = "chbShowChannel";
            this.chbShowChannel.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // tlpChannel
            // 
            resources.ApplyResources(this.tlpChannel, "tlpChannel");
            this.tlpChannel.Controls.Add(this.numNumber, 1, 0);
            this.tlpChannel.Controls.Add(this.label1, 0, 0);
            this.tlpChannel.Controls.Add(this.lblEmblema, 0, 1);
            this.tlpChannel.Controls.Add(this.pbImage, 1, 1);
            this.tlpChannel.Controls.Add(this.lblDiff, 0, 2);
            this.tlpChannel.Controls.Add(this.tbmDiff, 1, 2);
            this.tlpChannel.Controls.Add(this.lblSyn, 2, 0);
            this.tlpChannel.Controls.Add(this.tbSyn, 3, 0);
            this.tlpChannel.Name = "tlpChannel";
            // 
            // numNumber
            // 
            resources.ApplyResources(this.numNumber, "numNumber");
            this.numNumber.Name = "numNumber";
            // 
            // lblEmblema
            // 
            resources.ApplyResources(this.lblEmblema, "lblEmblema");
            this.lblEmblema.Name = "lblEmblema";
            // 
            // pbImage
            // 
            resources.ApplyResources(this.pbImage, "pbImage");
            this.pbImage.Name = "pbImage";
            this.pbImage.TabStop = false;
            // 
            // lblDiff
            // 
            resources.ApplyResources(this.lblDiff, "lblDiff");
            this.lblDiff.Name = "lblDiff";
            // 
            // tbmDiff
            // 
            resources.ApplyResources(this.tbmDiff, "tbmDiff");
            this.tbmDiff.Name = "tbmDiff";
            // 
            // lblSyn
            // 
            resources.ApplyResources(this.lblSyn, "lblSyn");
            this.lblSyn.Name = "lblSyn";
            // 
            // tbSyn
            // 
            resources.ApplyResources(this.tbSyn, "tbSyn");
            this.tbSyn.Name = "tbSyn";
            // 
            // lblChannel
            // 
            resources.ApplyResources(this.lblChannel, "lblChannel");
            this.lblChannel.Name = "lblChannel";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.btnOK, "btnOK");
            this.btnOK.Name = "btnOK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // EditChannelDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lblChannel);
            this.Controls.Add(this.tlpChannel);
            this.Controls.Add(this.chbShowChannel);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "EditChannelDialog";
            this.Load += new System.EventHandler(this.EditChannelDialog_Load);
            this.tlpChannel.ResumeLayout(false);
            this.tlpChannel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbImage)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox chbShowChannel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TableLayoutPanel tlpChannel;
        private System.Windows.Forms.NumericUpDown numNumber;
        private System.Windows.Forms.Label lblEmblema;
        private System.Windows.Forms.PictureBox pbImage;
        private System.Windows.Forms.Label lblDiff;
        private System.Windows.Forms.MaskedTextBox tbmDiff;
        private System.Windows.Forms.Label lblSyn;
        private System.Windows.Forms.TextBox tbSyn;
        private System.Windows.Forms.Label lblChannel;
    }
}