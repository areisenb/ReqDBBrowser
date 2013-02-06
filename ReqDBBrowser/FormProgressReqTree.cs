using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ReqDBBrowser
{
    public partial class FormProgressReqTree : Form
    {
        int nSumTreeElements;
        int nSumRequirements;
        bool bCancel;

        public FormProgressReqTree()
        {
            bCancel = false;
            InitializeComponent();
        }

        public bool ShowProgressReqTree(int nSumTreeElements, int nReadTreeElements, int nSumRequirements, int nReadRequirements, string strLogg)
        {
            if (nSumTreeElements > 0)
            {
                this.nSumTreeElements = nSumTreeElements;
                progressBarReqPkgTree.Minimum = 0;
                progressBarReqPkgTree.Maximum = nSumTreeElements;
            }
            if (nReadTreeElements > 0)
            {
                progressBarReqPkgTree.Value = nReadTreeElements;
                textBoxReqPkgTree.Text = nReadTreeElements + " of " + this.nSumTreeElements;
            }
            if (nSumRequirements > 0)
            {
                this.nSumRequirements = nSumRequirements;
                progressBarReqTree.Minimum = 0;
                progressBarReqTree.Maximum = nSumRequirements;
            }
            if (nReadRequirements > 0)
            {
                progressBarReqTree.Value = nReadRequirements;
                textBoxReqTree.Text = nReadRequirements + " of " + this.nSumRequirements;
            }

            if (strLogg != null)
                textBoxReqTreeLog.Text += (strLogg + "\r\n");
            if ((nSumRequirements == 0) && (nReadRequirements == 0) && (nSumTreeElements == 0) && (nReadTreeElements == 0) && (strLogg == null))
                // if nobody wants to signal nothing - I am obsolete
                Close();
            Application.DoEvents();
            return bCancel;
        }

        private void FormProgressReqTree_Load(object sender, EventArgs e)
        {

        }
    }
}