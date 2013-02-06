using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ReqDBBrowser
{
    public partial class FormAbout : Form
    {
        public FormAbout()
        {
            InitializeComponent();
        }

        private void FormAbout_Load(object sender, EventArgs e)
        {
            string strVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            labelVersion.Text = "Version: " + strVersion;
        }
    }
}