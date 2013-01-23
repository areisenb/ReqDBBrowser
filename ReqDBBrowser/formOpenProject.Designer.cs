namespace ReqDBBrowser
{
    partial class FormOpenProject
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOpenProject));
            this.labelProjectFile = new System.Windows.Forms.Label();
            this.labelUser = new System.Windows.Forms.Label();
            this.labelPassword = new System.Windows.Forms.Label();
            this.textBoxProject = new System.Windows.Forms.TextBox();
            this.textBoxUser = new System.Windows.Forms.TextBox();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.buttonOpen = new System.Windows.Forms.Button();
            this.buttonFOpen = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.listBoxProject = new System.Windows.Forms.ListBox();
            this.labelProjects = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // labelProjectFile
            // 
            this.labelProjectFile.AutoSize = true;
            this.labelProjectFile.Location = new System.Drawing.Point(49, 69);
            this.labelProjectFile.Name = "labelProjectFile";
            this.labelProjectFile.Size = new System.Drawing.Size(40, 13);
            this.labelProjectFile.TabIndex = 0;
            this.labelProjectFile.Text = "Project";
            // 
            // labelUser
            // 
            this.labelUser.AutoSize = true;
            this.labelUser.Location = new System.Drawing.Point(60, 100);
            this.labelUser.Name = "labelUser";
            this.labelUser.Size = new System.Drawing.Size(29, 13);
            this.labelUser.TabIndex = 1;
            this.labelUser.Text = "User";
            // 
            // labelPassword
            // 
            this.labelPassword.AutoSize = true;
            this.labelPassword.Location = new System.Drawing.Point(36, 131);
            this.labelPassword.Name = "labelPassword";
            this.labelPassword.Size = new System.Drawing.Size(53, 13);
            this.labelPassword.TabIndex = 2;
            this.labelPassword.Text = "Password";
            // 
            // textBoxProject
            // 
            this.textBoxProject.Location = new System.Drawing.Point(95, 69);
            this.textBoxProject.Name = "textBoxProject";
            this.textBoxProject.Size = new System.Drawing.Size(406, 20);
            this.textBoxProject.TabIndex = 3;
            this.textBoxProject.TextChanged += new System.EventHandler(this.textBoxProject_TextChanged);
            // 
            // textBoxUser
            // 
            this.textBoxUser.Location = new System.Drawing.Point(96, 100);
            this.textBoxUser.Name = "textBoxUser";
            this.textBoxUser.Size = new System.Drawing.Size(405, 20);
            this.textBoxUser.TabIndex = 4;
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(95, 131);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(406, 20);
            this.textBoxPassword.TabIndex = 5;
            // 
            // buttonOpen
            // 
            this.buttonOpen.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOpen.Location = new System.Drawing.Point(369, 173);
            this.buttonOpen.Name = "buttonOpen";
            this.buttonOpen.Size = new System.Drawing.Size(132, 23);
            this.buttonOpen.TabIndex = 6;
            this.buttonOpen.Text = "Open";
            this.buttonOpen.UseVisualStyleBackColor = true;
            // 
            // buttonFOpen
            // 
            this.buttonFOpen.Location = new System.Drawing.Point(507, 67);
            this.buttonFOpen.Name = "buttonFOpen";
            this.buttonFOpen.Size = new System.Drawing.Size(44, 23);
            this.buttonFOpen.TabIndex = 7;
            this.buttonFOpen.Text = "...";
            this.buttonFOpen.UseVisualStyleBackColor = true;
            this.buttonFOpen.Click += new System.EventHandler(this.buttonFOpen_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(96, 173);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(132, 23);
            this.buttonCancel.TabIndex = 8;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // listBoxProject
            // 
            this.listBoxProject.FormattingEnabled = true;
            this.listBoxProject.HorizontalScrollbar = true;
            this.listBoxProject.Items.AddRange(new object[] {
            "<User Specific>"});
            this.listBoxProject.Location = new System.Drawing.Point(95, 24);
            this.listBoxProject.Name = "listBoxProject";
            this.listBoxProject.Size = new System.Drawing.Size(405, 30);
            this.listBoxProject.TabIndex = 9;
            this.listBoxProject.SelectedIndexChanged += new System.EventHandler(this.listBoxProject_SelectedIndexChanged);
            // 
            // labelProjects
            // 
            this.labelProjects.AutoSize = true;
            this.labelProjects.Location = new System.Drawing.Point(44, 24);
            this.labelProjects.Name = "labelProjects";
            this.labelProjects.Size = new System.Drawing.Size(45, 13);
            this.labelProjects.TabIndex = 10;
            this.labelProjects.Text = "Projects";
            // 
            // FormOpenProject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 224);
            this.Controls.Add(this.labelProjects);
            this.Controls.Add(this.listBoxProject);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonFOpen);
            this.Controls.Add(this.buttonOpen);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.textBoxUser);
            this.Controls.Add(this.textBoxProject);
            this.Controls.Add(this.labelPassword);
            this.Controls.Add(this.labelUser);
            this.Controls.Add(this.labelProjectFile);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FormOpenProject";
            this.Text = "Open Project";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelProjectFile;
        private System.Windows.Forms.Label labelUser;
        private System.Windows.Forms.Label labelPassword;
        private System.Windows.Forms.TextBox textBoxProject;
        private System.Windows.Forms.TextBox textBoxUser;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Button buttonOpen;
        private System.Windows.Forms.Button buttonFOpen;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.ListBox listBoxProject;
        private System.Windows.Forms.Label labelProjects;
    }
}