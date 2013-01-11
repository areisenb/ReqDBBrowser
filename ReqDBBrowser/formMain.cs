using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ReqDBBrowser
{
    public partial class formMain : Form
    {
        public formMain()
        {
            InitializeComponent();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            formAbout fmAbout;
            fmAbout = new formAbout();
            fmAbout.ShowDialog(this);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormOpenProject fmOpenProject;
            fmOpenProject = new FormOpenProject("", "", "");
            if (fmOpenProject.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(fmOpenProject.strProjectFile);
            }
        }
    }
}