namespace ReqDBBrowser
{
    partial class FormProgressReqTree
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormProgressReqTree));
            this.groupBoxReqPkgTree = new System.Windows.Forms.GroupBox();
            this.textBoxReqPkgTree = new System.Windows.Forms.TextBox();
            this.progressBarReqPkgTree = new System.Windows.Forms.ProgressBar();
            this.groupBoxReqTree = new System.Windows.Forms.GroupBox();
            this.textBoxReqTree = new System.Windows.Forms.TextBox();
            this.progressBarReqTree = new System.Windows.Forms.ProgressBar();
            this.textBoxReqTreeLog = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxReqPkgTree.SuspendLayout();
            this.groupBoxReqTree.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxReqPkgTree
            // 
            this.groupBoxReqPkgTree.Controls.Add(this.textBoxReqPkgTree);
            this.groupBoxReqPkgTree.Controls.Add(this.progressBarReqPkgTree);
            this.groupBoxReqPkgTree.Location = new System.Drawing.Point(58, 30);
            this.groupBoxReqPkgTree.Name = "groupBoxReqPkgTree";
            this.groupBoxReqPkgTree.Size = new System.Drawing.Size(261, 106);
            this.groupBoxReqPkgTree.TabIndex = 0;
            this.groupBoxReqPkgTree.TabStop = false;
            this.groupBoxReqPkgTree.Text = "Packages";
            // 
            // textBoxReqPkgTree
            // 
            this.textBoxReqPkgTree.Location = new System.Drawing.Point(37, 33);
            this.textBoxReqPkgTree.Name = "textBoxReqPkgTree";
            this.textBoxReqPkgTree.ReadOnly = true;
            this.textBoxReqPkgTree.Size = new System.Drawing.Size(190, 20);
            this.textBoxReqPkgTree.TabIndex = 1;
            // 
            // progressBarReqPkgTree
            // 
            this.progressBarReqPkgTree.Location = new System.Drawing.Point(37, 67);
            this.progressBarReqPkgTree.Name = "progressBarReqPkgTree";
            this.progressBarReqPkgTree.Size = new System.Drawing.Size(190, 23);
            this.progressBarReqPkgTree.TabIndex = 0;
            // 
            // groupBoxReqTree
            // 
            this.groupBoxReqTree.Controls.Add(this.textBoxReqTree);
            this.groupBoxReqTree.Controls.Add(this.progressBarReqTree);
            this.groupBoxReqTree.Location = new System.Drawing.Point(58, 164);
            this.groupBoxReqTree.Name = "groupBoxReqTree";
            this.groupBoxReqTree.Size = new System.Drawing.Size(261, 106);
            this.groupBoxReqTree.TabIndex = 1;
            this.groupBoxReqTree.TabStop = false;
            this.groupBoxReqTree.Text = "Requirements";
            // 
            // textBoxReqTree
            // 
            this.textBoxReqTree.Location = new System.Drawing.Point(37, 33);
            this.textBoxReqTree.Name = "textBoxReqTree";
            this.textBoxReqTree.ReadOnly = true;
            this.textBoxReqTree.Size = new System.Drawing.Size(190, 20);
            this.textBoxReqTree.TabIndex = 1;
            // 
            // progressBarReqTree
            // 
            this.progressBarReqTree.Location = new System.Drawing.Point(37, 67);
            this.progressBarReqTree.Name = "progressBarReqTree";
            this.progressBarReqTree.Size = new System.Drawing.Size(190, 23);
            this.progressBarReqTree.TabIndex = 0;
            // 
            // textBoxReqTreeLog
            // 
            this.textBoxReqTreeLog.Location = new System.Drawing.Point(58, 293);
            this.textBoxReqTreeLog.Multiline = true;
            this.textBoxReqTreeLog.Name = "textBoxReqTreeLog";
            this.textBoxReqTreeLog.ReadOnly = true;
            this.textBoxReqTreeLog.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.textBoxReqTreeLog.Size = new System.Drawing.Size(261, 169);
            this.textBoxReqTreeLog.TabIndex = 2;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Enabled = false;
            this.buttonCancel.Location = new System.Drawing.Point(150, 498);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // FormProgressReqTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 565);
            this.ControlBox = false;
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.textBoxReqTreeLog);
            this.Controls.Add(this.groupBoxReqTree);
            this.Controls.Add(this.groupBoxReqPkgTree);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormProgressReqTree";
            this.Text = "Read Requirement Tree";
            this.Load += new System.EventHandler(this.FormProgressReqTree_Load);
            this.groupBoxReqPkgTree.ResumeLayout(false);
            this.groupBoxReqPkgTree.PerformLayout();
            this.groupBoxReqTree.ResumeLayout(false);
            this.groupBoxReqTree.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxReqPkgTree;
        private System.Windows.Forms.ProgressBar progressBarReqPkgTree;
        private System.Windows.Forms.GroupBox groupBoxReqTree;
        private System.Windows.Forms.ProgressBar progressBarReqTree;
        private System.Windows.Forms.TextBox textBoxReqPkgTree;
        private System.Windows.Forms.TextBox textBoxReqTree;
        private System.Windows.Forms.TextBox textBoxReqTreeLog;
        private System.Windows.Forms.Button buttonCancel;
    }
}