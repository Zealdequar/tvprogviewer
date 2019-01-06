namespace TVProgViewer.TVProgApp.Controls
{
    partial class OnRunningSettings
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OnRunningSettings));
            this.gbStartPage = new System.Windows.Forms.GroupBox();
            this.rbChannels = new System.Windows.Forms.RadioButton();
            this.rbDays = new System.Windows.Forms.RadioButton();
            this.rbNext = new System.Windows.Forms.RadioButton();
            this.rbNow = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chbDontShowLogo = new System.Windows.Forms.CheckBox();
            this.gbWindow = new System.Windows.Forms.GroupBox();
            this.rbMaximize = new System.Windows.Forms.RadioButton();
            this.rbRestore = new System.Windows.Forms.RadioButton();
            this.rbMinimize = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.chbAutorun = new System.Windows.Forms.CheckBox();
            this.gbStartPage.SuspendLayout();
            this.panel1.SuspendLayout();
            this.gbWindow.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbStartPage
            // 
            this.gbStartPage.Controls.Add(this.rbChannels);
            this.gbStartPage.Controls.Add(this.rbDays);
            this.gbStartPage.Controls.Add(this.rbNext);
            this.gbStartPage.Controls.Add(this.rbNow);
            resources.ApplyResources(this.gbStartPage, "gbStartPage");
            this.gbStartPage.Name = "gbStartPage";
            this.gbStartPage.TabStop = false;
            // 
            // rbChannels
            // 
            resources.ApplyResources(this.rbChannels, "rbChannels");
            this.rbChannels.Name = "rbChannels";
            this.rbChannels.Tag = "3";
            this.rbChannels.UseVisualStyleBackColor = true;
            this.rbChannels.CheckedChanged += new System.EventHandler(this.OnCheckedMainTabMode);
            // 
            // rbDays
            // 
            resources.ApplyResources(this.rbDays, "rbDays");
            this.rbDays.Name = "rbDays";
            this.rbDays.Tag = "2";
            this.rbDays.UseVisualStyleBackColor = true;
            this.rbDays.CheckedChanged += new System.EventHandler(this.OnCheckedMainTabMode);
            // 
            // rbNext
            // 
            resources.ApplyResources(this.rbNext, "rbNext");
            this.rbNext.Name = "rbNext";
            this.rbNext.Tag = "1";
            this.rbNext.UseVisualStyleBackColor = true;
            this.rbNext.CheckedChanged += new System.EventHandler(this.OnCheckedMainTabMode);
            // 
            // rbNow
            // 
            resources.ApplyResources(this.rbNow, "rbNow");
            this.rbNow.Checked = true;
            this.rbNow.Name = "rbNow";
            this.rbNow.TabStop = true;
            this.rbNow.Tag = "0";
            this.rbNow.UseVisualStyleBackColor = true;
            this.rbNow.CheckedChanged += new System.EventHandler(this.OnCheckedMainTabMode);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chbDontShowLogo);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // chbDontShowLogo
            // 
            resources.ApplyResources(this.chbDontShowLogo, "chbDontShowLogo");
            this.chbDontShowLogo.Name = "chbDontShowLogo";
            this.chbDontShowLogo.UseVisualStyleBackColor = true;
            this.chbDontShowLogo.CheckedChanged += new System.EventHandler(this.chbDontShowLogo_CheckedChanged);
            // 
            // gbWindow
            // 
            this.gbWindow.Controls.Add(this.rbMaximize);
            this.gbWindow.Controls.Add(this.rbRestore);
            this.gbWindow.Controls.Add(this.rbMinimize);
            resources.ApplyResources(this.gbWindow, "gbWindow");
            this.gbWindow.Name = "gbWindow";
            this.gbWindow.TabStop = false;
            // 
            // rbMaximize
            // 
            resources.ApplyResources(this.rbMaximize, "rbMaximize");
            this.rbMaximize.Checked = true;
            this.rbMaximize.Name = "rbMaximize";
            this.rbMaximize.TabStop = true;
            this.rbMaximize.Tag = "2";
            this.rbMaximize.UseVisualStyleBackColor = true;
            this.rbMaximize.CheckedChanged += new System.EventHandler(this.OnCheckedWindowMode);
            // 
            // rbRestore
            // 
            resources.ApplyResources(this.rbRestore, "rbRestore");
            this.rbRestore.Name = "rbRestore";
            this.rbRestore.Tag = "1";
            this.rbRestore.UseVisualStyleBackColor = true;
            this.rbRestore.CheckedChanged += new System.EventHandler(this.OnCheckedWindowMode);
            // 
            // rbMinimize
            // 
            resources.ApplyResources(this.rbMinimize, "rbMinimize");
            this.rbMinimize.Name = "rbMinimize";
            this.rbMinimize.Tag = "0";
            this.rbMinimize.UseVisualStyleBackColor = true;
            this.rbMinimize.CheckedChanged += new System.EventHandler(this.OnCheckedWindowMode);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.chbAutorun);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // chbAutorun
            // 
            resources.ApplyResources(this.chbAutorun, "chbAutorun");
            this.chbAutorun.Name = "chbAutorun";
            this.chbAutorun.UseVisualStyleBackColor = true;
            this.chbAutorun.CheckedChanged += new System.EventHandler(this.chbAutorun_CheckedChanged);
            // 
            // OnRunningSettings
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.gbWindow);
            this.Controls.Add(this.gbStartPage);
            this.Controls.Add(this.panel1);
            this.Name = "OnRunningSettings";
            this.Load += new System.EventHandler(this.OnRunningSettings_Load);
            this.gbStartPage.ResumeLayout(false);
            this.gbStartPage.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.gbWindow.ResumeLayout(false);
            this.gbWindow.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbStartPage;
        private System.Windows.Forms.RadioButton rbChannels;
        private System.Windows.Forms.RadioButton rbDays;
        private System.Windows.Forms.RadioButton rbNext;
        private System.Windows.Forms.RadioButton rbNow;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.CheckBox chbDontShowLogo;
        private System.Windows.Forms.GroupBox gbWindow;
        private System.Windows.Forms.RadioButton rbMaximize;
        private System.Windows.Forms.RadioButton rbRestore;
        private System.Windows.Forms.RadioButton rbMinimize;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox chbAutorun;

    }
}
