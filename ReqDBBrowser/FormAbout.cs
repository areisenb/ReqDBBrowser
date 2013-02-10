using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Deployment.Application;

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
            Version vClickOnce;
            String strVersion;

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                vClickOnce = ApplicationDeployment.CurrentDeployment.CurrentVersion;
                labelInstallVar.Text = string.Format("ClickOnce published Version: v{0}.{1}.{2}.{3}",
                    vClickOnce.Major, vClickOnce.Minor, vClickOnce.Build, vClickOnce.Revision);
            }
            else
                labelInstallVar.Text = "Local Installation";

            strVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            labelVersion.Text = "Version: " + strVersion;


        }
    }
}