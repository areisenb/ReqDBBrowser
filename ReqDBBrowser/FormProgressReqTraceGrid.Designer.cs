namespace ReqDBBrowser
{
    partial class FormProgressReqTraceGrid
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
            this.progressBarReqCnt = new System.Windows.Forms.ProgressBar();
            this.textBoxLog = new System.Windows.Forms.TextBox();
            this.textBoxReqCnt = new System.Windows.Forms.TextBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.groupBoxReqCnt = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // progressBarReqCnt
            // 
            this.progressBarReqCnt.Location = new System.Drawing.Point(93, 103);
            this.progressBarReqCnt.Name = "progressBarReqCnt";
            this.progressBarReqCnt.Size = new System.Drawing.Size(190, 23);
            this.progressBarReqCnt.TabIndex = 0;
            // 
            // textBoxLog
            // 
            this.textBoxLog.HideSelection = false;
            this.textBoxLog.Location = new System.Drawing.Point(25, 158);
            this.textBoxLog.Multiline = true;
            this.textBoxLog.Name = "textBoxLog";
            this.textBoxLog.ReadOnly = true;
            this.textBoxLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxLog.Size = new System.Drawing.Size(326, 169);
            this.textBoxLog.TabIndex = 1;
            this.textBoxLog.WordWrap = false;
            // 
            // textBoxReqCnt
            // 
            this.textBoxReqCnt.Location = new System.Drawing.Point(93, 56);
            this.textBoxReqCnt.Name = "textBoxReqCnt";
            this.textBoxReqCnt.ReadOnly = true;
            this.textBoxReqCnt.Size = new System.Drawing.Size(190, 20);
            this.textBoxReqCnt.TabIndex = 2;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Enabled = false;
            this.buttonCancel.Location = new System.Drawing.Point(151, 361);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 3;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBoxReqCnt
            // 
            this.groupBoxReqCnt.Location = new System.Drawing.Point(25, 35);
            this.groupBoxReqCnt.Name = "groupBoxReqCnt";
            this.groupBoxReqCnt.Size = new System.Drawing.Size(326, 106);
            this.groupBoxReqCnt.TabIndex = 4;
            this.groupBoxReqCnt.TabStop = false;
            this.groupBoxReqCnt.Text = "Requirements";
            // 
            // FormProgressReqTraceGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(376, 413);
            this.ControlBox = false;
            this.Controls.Add(this.progressBarReqCnt);
            this.Controls.Add(this.textBoxReqCnt);
            this.Controls.Add(this.groupBoxReqCnt);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.textBoxLog);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormProgressReqTraceGrid";
            this.Text = "Read Requirement Traces";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBarReqCnt;
        private System.Windows.Forms.TextBox textBoxLog;
        private System.Windows.Forms.TextBox textBoxReqCnt;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.GroupBox groupBoxReqCnt;
    }
}