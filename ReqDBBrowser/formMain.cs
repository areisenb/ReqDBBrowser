using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ReqDBBrowser
{
    public partial class FormMain : Form
    {
        TreePanel treePanel;
        TreeView treeView;
        ReqProProject reqDBBrowser;

        public FormMain()
        {
            InitializeComponent();
            reqDBBrowser = new ReqProProject();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormAbout fmAbout;
            fmAbout = new FormAbout();
            fmAbout.ShowDialog(this);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormOpenProject fmOpenProject;
            fmOpenProject = new FormOpenProject("", "");
            if (fmOpenProject.ShowDialog() == DialogResult.OK)
            {
                int nPackageCount;
                reqDBBrowser.OpenProject(fmOpenProject.strProjectFile,
                    fmOpenProject.strUser,
                    fmOpenProject.strPassword);
                treePanel.CreateTree(reqDBBrowser.ReadReqTree(out nPackageCount));
            }
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            treeView = new TreeView();
            treeView.Dock = DockStyle.Fill;
            treePanel = new TreePanel(ref treeView);
            splitContainerMain.Panel1.Controls.Add(treeView);
        }
    }
}