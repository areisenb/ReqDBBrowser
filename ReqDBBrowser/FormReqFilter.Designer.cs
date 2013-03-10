namespace ReqDBBrowser
{
    partial class FormReqFilter
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReqFilter));
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageRoot = new System.Windows.Forms.TabPage();
            this.treeViewRqRoot = new System.Windows.Forms.TreeView();
            this.tabPageTraced = new System.Windows.Forms.TabPage();
            this.treeViewRqTraced = new System.Windows.Forms.TreeView();
            this.tabPageParameter = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.numUpDwnMaxTraces = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numUpDwnMaxDownCnt = new System.Windows.Forms.NumericUpDown();
            this.numUpDwnMaxUpCnt = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.numUpDwnDownCnt = new System.Windows.Forms.NumericUpDown();
            this.numUpDwnUpCnt = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.btGo = new System.Windows.Forms.Button();
            this.btAbort = new System.Windows.Forms.Button();
            this.tabControl.SuspendLayout();
            this.tabPageRoot.SuspendLayout();
            this.tabPageTraced.SuspendLayout();
            this.tabPageParameter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDwnMaxTraces)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDwnMaxDownCnt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDwnMaxUpCnt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDwnDownCnt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDwnUpCnt)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageRoot);
            this.tabControl.Controls.Add(this.tabPageTraced);
            this.tabControl.Controls.Add(this.tabPageParameter);
            this.tabControl.Location = new System.Drawing.Point(12, 12);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(307, 289);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageRoot
            // 
            this.tabPageRoot.Controls.Add(this.treeViewRqRoot);
            this.tabPageRoot.Location = new System.Drawing.Point(4, 22);
            this.tabPageRoot.Name = "tabPageRoot";
            this.tabPageRoot.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageRoot.Size = new System.Drawing.Size(299, 263);
            this.tabPageRoot.TabIndex = 0;
            this.tabPageRoot.Text = "Root";
            this.tabPageRoot.UseVisualStyleBackColor = true;
            // 
            // treeViewRqRoot
            // 
            this.treeViewRqRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewRqRoot.Location = new System.Drawing.Point(3, 3);
            this.treeViewRqRoot.Name = "treeViewRqRoot";
            this.treeViewRqRoot.Size = new System.Drawing.Size(293, 257);
            this.treeViewRqRoot.TabIndex = 0;
            // 
            // tabPageTraced
            // 
            this.tabPageTraced.Controls.Add(this.treeViewRqTraced);
            this.tabPageTraced.Location = new System.Drawing.Point(4, 22);
            this.tabPageTraced.Name = "tabPageTraced";
            this.tabPageTraced.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageTraced.Size = new System.Drawing.Size(299, 263);
            this.tabPageTraced.TabIndex = 1;
            this.tabPageTraced.Text = "Traced";
            this.tabPageTraced.UseVisualStyleBackColor = true;
            // 
            // treeViewRqTraced
            // 
            this.treeViewRqTraced.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewRqTraced.Location = new System.Drawing.Point(3, 3);
            this.treeViewRqTraced.Name = "treeViewRqTraced";
            this.treeViewRqTraced.Size = new System.Drawing.Size(293, 257);
            this.treeViewRqTraced.TabIndex = 0;
            // 
            // tabPageParameter
            // 
            this.tabPageParameter.Controls.Add(this.label5);
            this.tabPageParameter.Controls.Add(this.numUpDwnMaxTraces);
            this.tabPageParameter.Controls.Add(this.label4);
            this.tabPageParameter.Controls.Add(this.label3);
            this.tabPageParameter.Controls.Add(this.numUpDwnMaxDownCnt);
            this.tabPageParameter.Controls.Add(this.numUpDwnMaxUpCnt);
            this.tabPageParameter.Controls.Add(this.label2);
            this.tabPageParameter.Controls.Add(this.numUpDwnDownCnt);
            this.tabPageParameter.Controls.Add(this.numUpDwnUpCnt);
            this.tabPageParameter.Controls.Add(this.label1);
            this.tabPageParameter.Location = new System.Drawing.Point(4, 22);
            this.tabPageParameter.Name = "tabPageParameter";
            this.tabPageParameter.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageParameter.Size = new System.Drawing.Size(299, 263);
            this.tabPageParameter.TabIndex = 2;
            this.tabPageParameter.Text = "Parameter";
            this.tabPageParameter.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(60, 156);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(124, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Stop on Req with traces:";
            // 
            // numUpDwnMaxTraces
            // 
            this.numUpDwnMaxTraces.Location = new System.Drawing.Point(190, 154);
            this.numUpDwnMaxTraces.Name = "numUpDwnMaxTraces";
            this.numUpDwnMaxTraces.Size = new System.Drawing.Size(67, 20);
            this.numUpDwnMaxTraces.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(76, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(108, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Max from trace steps:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(87, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(97, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Max to trace steps:";
            // 
            // numUpDwnMaxDownCnt
            // 
            this.numUpDwnMaxDownCnt.Location = new System.Drawing.Point(190, 128);
            this.numUpDwnMaxDownCnt.Name = "numUpDwnMaxDownCnt";
            this.numUpDwnMaxDownCnt.Size = new System.Drawing.Size(67, 20);
            this.numUpDwnMaxDownCnt.TabIndex = 6;
            // 
            // numUpDwnMaxUpCnt
            // 
            this.numUpDwnMaxUpCnt.Location = new System.Drawing.Point(190, 102);
            this.numUpDwnMaxUpCnt.Name = "numUpDwnMaxUpCnt";
            this.numUpDwnMaxUpCnt.Size = new System.Drawing.Size(67, 20);
            this.numUpDwnMaxUpCnt.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(68, 78);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(116, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Max levels of to traces:";
            // 
            // numUpDwnDownCnt
            // 
            this.numUpDwnDownCnt.Location = new System.Drawing.Point(190, 76);
            this.numUpDwnDownCnt.Name = "numUpDwnDownCnt";
            this.numUpDwnDownCnt.Size = new System.Drawing.Size(67, 20);
            this.numUpDwnDownCnt.TabIndex = 3;
            // 
            // numUpDwnUpCnt
            // 
            this.numUpDwnUpCnt.Location = new System.Drawing.Point(190, 50);
            this.numUpDwnUpCnt.Name = "numUpDwnUpCnt";
            this.numUpDwnUpCnt.Size = new System.Drawing.Size(67, 20);
            this.numUpDwnUpCnt.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(57, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Max levels of from traces:";
            // 
            // btGo
            // 
            this.btGo.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btGo.Location = new System.Drawing.Point(19, 307);
            this.btGo.Name = "btGo";
            this.btGo.Size = new System.Drawing.Size(293, 23);
            this.btGo.TabIndex = 1;
            this.btGo.Text = "Go!";
            this.btGo.UseVisualStyleBackColor = true;
            // 
            // btAbort
            // 
            this.btAbort.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btAbort.Location = new System.Drawing.Point(19, 336);
            this.btAbort.Name = "btAbort";
            this.btAbort.Size = new System.Drawing.Size(293, 23);
            this.btAbort.TabIndex = 2;
            this.btAbort.Text = "Abort";
            this.btAbort.UseVisualStyleBackColor = true;
            // 
            // FormReqFilter
            // 
            this.AcceptButton = this.btGo;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btAbort;
            this.ClientSize = new System.Drawing.Size(331, 394);
            this.ControlBox = false;
            this.Controls.Add(this.btAbort);
            this.Controls.Add(this.btGo);
            this.Controls.Add(this.tabControl);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormReqFilter";
            this.Text = "Filter";
            this.tabControl.ResumeLayout(false);
            this.tabPageRoot.ResumeLayout(false);
            this.tabPageTraced.ResumeLayout(false);
            this.tabPageParameter.ResumeLayout(false);
            this.tabPageParameter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDwnMaxTraces)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDwnMaxDownCnt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDwnMaxUpCnt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDwnDownCnt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numUpDwnUpCnt)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageRoot;
        private System.Windows.Forms.TabPage tabPageTraced;
        private System.Windows.Forms.TabPage tabPageParameter;
        private System.Windows.Forms.TreeView treeViewRqRoot;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btGo;
        private System.Windows.Forms.NumericUpDown numUpDwnDownCnt;
        private System.Windows.Forms.NumericUpDown numUpDwnUpCnt;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numUpDwnMaxDownCnt;
        private System.Windows.Forms.NumericUpDown numUpDwnMaxUpCnt;
        private System.Windows.Forms.NumericUpDown numUpDwnMaxTraces;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TreeView treeViewRqTraced;
        private System.Windows.Forms.Button btAbort;
    }
}