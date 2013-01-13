using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ReqDBBrowser
{
    public partial class FormOpenProject : Form
    {
        public FormOpenProject(string strProjectFile, string strUser)
        {
            InitializeComponent();
        }

        private void buttonFOpen_Click(object sender, EventArgs e)
        {
            string Pfad = string.Empty;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ReqPro Project Files (*.rqs)|*.rqs";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
                textBoxProject.Text = openFileDialog.FileName;
        }

        public string strProjectFile
        {
            get { return textBoxProject.Text; }
        }
        public string strUser
        {
            get { return textBoxUser.Text; }
        }
        public string strPassword
        {
            get { return textBoxPassword.Text; }
        }
    }
}