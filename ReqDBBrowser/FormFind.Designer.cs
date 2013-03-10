namespace ReqDBBrowser
{
    partial class FormFind
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
            this.cbTag = new System.Windows.Forms.CheckBox();
            this.cbName = new System.Windows.Forms.CheckBox();
            this.cbText = new System.Windows.Forms.CheckBox();
            this.tbFindString = new System.Windows.Forms.TextBox();
            this.btFind = new System.Windows.Forms.Button();
            this.btCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbSimple = new System.Windows.Forms.RadioButton();
            this.rbRegex = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cbTag
            // 
            this.cbTag.AutoSize = true;
            this.cbTag.Checked = true;
            this.cbTag.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbTag.Location = new System.Drawing.Point(12, 12);
            this.cbTag.Name = "cbTag";
            this.cbTag.Size = new System.Drawing.Size(68, 17);
            this.cbTag.TabIndex = 0;
            this.cbTag.Text = "Req Tag";
            this.cbTag.UseVisualStyleBackColor = true;
            // 
            // cbName
            // 
            this.cbName.AutoSize = true;
            this.cbName.Checked = true;
            this.cbName.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbName.Location = new System.Drawing.Point(12, 35);
            this.cbName.Name = "cbName";
            this.cbName.Size = new System.Drawing.Size(77, 17);
            this.cbName.TabIndex = 1;
            this.cbName.Text = "Req Name";
            this.cbName.UseVisualStyleBackColor = true;
            // 
            // cbText
            // 
            this.cbText.AutoSize = true;
            this.cbText.Checked = true;
            this.cbText.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbText.Location = new System.Drawing.Point(12, 58);
            this.cbText.Name = "cbText";
            this.cbText.Size = new System.Drawing.Size(70, 17);
            this.cbText.TabIndex = 2;
            this.cbText.Text = "Req Text";
            this.cbText.UseVisualStyleBackColor = true;
            // 
            // tbFindString
            // 
            this.tbFindString.Location = new System.Drawing.Point(12, 92);
            this.tbFindString.Name = "tbFindString";
            this.tbFindString.Size = new System.Drawing.Size(260, 20);
            this.tbFindString.TabIndex = 3;
            // 
            // btFind
            // 
            this.btFind.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btFind.Location = new System.Drawing.Point(12, 118);
            this.btFind.Name = "btFind";
            this.btFind.Size = new System.Drawing.Size(75, 23);
            this.btFind.TabIndex = 4;
            this.btFind.Text = "Find";
            this.btFind.UseVisualStyleBackColor = true;
            // 
            // btCancel
            // 
            this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btCancel.Location = new System.Drawing.Point(197, 118);
            this.btCancel.Name = "btCancel";
            this.btCancel.Size = new System.Drawing.Size(75, 23);
            this.btCancel.TabIndex = 5;
            this.btCancel.Text = "Cancel";
            this.btCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbRegex);
            this.groupBox1.Controls.Add(this.rbSimple);
            this.groupBox1.Location = new System.Drawing.Point(95, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(177, 74);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search Expression";
            // 
            // rbSimple
            // 
            this.rbSimple.AutoSize = true;
            this.rbSimple.Checked = true;
            this.rbSimple.Location = new System.Drawing.Point(55, 23);
            this.rbSimple.Name = "rbSimple";
            this.rbSimple.Size = new System.Drawing.Size(56, 17);
            this.rbSimple.TabIndex = 0;
            this.rbSimple.TabStop = true;
            this.rbSimple.Text = "Simple";
            this.rbSimple.UseVisualStyleBackColor = true;
            // 
            // rbRegex
            // 
            this.rbRegex.AutoSize = true;
            this.rbRegex.Location = new System.Drawing.Point(55, 45);
            this.rbRegex.Name = "rbRegex";
            this.rbRegex.Size = new System.Drawing.Size(116, 17);
            this.rbRegex.TabIndex = 1;
            this.rbRegex.TabStop = true;
            this.rbRegex.Text = "Regular Expression";
            this.rbRegex.UseVisualStyleBackColor = true;
            // 
            // FormFind
            // 
            this.AcceptButton = this.btFind;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btCancel;
            this.ClientSize = new System.Drawing.Size(284, 153);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btCancel);
            this.Controls.Add(this.btFind);
            this.Controls.Add(this.tbFindString);
            this.Controls.Add(this.cbText);
            this.Controls.Add(this.cbName);
            this.Controls.Add(this.cbTag);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormFind";
            this.Text = "Find";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox cbTag;
        private System.Windows.Forms.CheckBox cbName;
        private System.Windows.Forms.CheckBox cbText;
        private System.Windows.Forms.TextBox tbFindString;
        private System.Windows.Forms.Button btFind;
        private System.Windows.Forms.Button btCancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbSimple;
        private System.Windows.Forms.RadioButton rbRegex;
    }
}