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
        ProjectFileFromProject projectFileFromProject;
        bool bUserSets;

        public FormOpenProject(string[] strProjects, string strProjectFile, ProjectFileFromProject projectFileFromProject, string strUser)
        {
            InitializeComponent();
            bUserSets = false;
            bool bProjectsGot = false;
            foreach (string strProject in strProjects)
            {
                listBoxProject.Items.Add(strProject);
                bProjectsGot = true;
            }
            this.projectFileFromProject = projectFileFromProject;
            bUserSets = true;
            if (bProjectsGot)
                listBoxProject.SelectedIndex = 1;
            textBoxUser.Text = strUser;
        }

        public delegate string ProjectFileFromProject(string strProject);

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

        private void listBoxProject_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bUserSets)
            {
                bUserSets = false;
                string strProject = projectFileFromProject((string)listBoxProject.SelectedItem);
                textBoxProject.Text = strProject;
                bUserSets = true;
            }
        }

        private void textBoxProject_TextChanged(object sender, EventArgs e)
        {
            if (bUserSets)
            {
                bUserSets = false;
                listBoxProject.SelectedIndex = 0;
                bUserSets = true;
            }
        }
    }
}