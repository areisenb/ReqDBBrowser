using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace ReqDBBrowser
{
    public partial class FormMain : Form
    {
        TreePanel treePanel;
        TreeViewReq treeView;
        ReqProProject reqDBBrowser;
        ReqUIBox reqCurrentUIBox;

        TextBox textBReq;
        DataGridView dataGridReq;

        ArrayList arrListReqKey;


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
            System.Security.Principal.WindowsIdentity wi = System.Security.Principal.WindowsIdentity.GetCurrent();
            string [] astrName;
            string strName="";

            astrName = System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split ('\\');
            if (astrName.GetUpperBound (0) >= 1)
                strName = astrName [1];
            fmOpenProject = new FormOpenProject(
                reqDBBrowser.GetProjectCatalog (), "", 
                reqDBBrowser.ProjectFileFromProject, 
                strName);
            if (fmOpenProject.ShowDialog() == DialogResult.OK)
            {
                int nPackageCount;
                reqDBBrowser.OpenProject(fmOpenProject.strProjectFile,
                    fmOpenProject.strUser,
                    fmOpenProject.strPassword,
                    new ReqProProject.RequestCredentialsCallback (this.RequestCredentialsCallback),
                    new ReqProProject.ShowOpenReleatedProjectErrorCallback (this.ShowOpenReleatedProjectErrorCallback));
                treePanel.CreateTree(reqDBBrowser.ReadReqTree(out nPackageCount));
            }
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            TreeViewReq.NodeSelectedCallback nodeSelectedCallback = new TreeViewReq.NodeSelectedCallback(this.NodeSelectedCallback);

            treeView = new TreeViewReq(nodeSelectedCallback);
            treeView.Dock = DockStyle.Fill;
            treePanel = new TreePanel(ref treeView);
            splitContainerMain.Panel1.Controls.Add(treeView);
        }

        public bool RequestCredentialsCallback(string strProjectDesc, ref string strUser, out string strPassword)
        {
            strPassword = "";
            return false;
        }

        public void ShowOpenReleatedProjectErrorCallback(string strProjectDesc)
        {
        }

        public void NodeSelectedCallback(int nKey, bool doubleClick, MouseButtons mButton)
        {
            string strText;
            ReqProRequirementPrx reqReqPrx;

            reqReqPrx = reqDBBrowser.GetRequirementPrx(nKey);

            this.reqCurrentUIBox = new ReqUIBox
                (nKey, reqReqPrx.Name, reqReqPrx.Text, new Point(0, 0), DefaultFont);

            if (textBReq != null)
                tabPageTree.Controls.Remove(textBReq);

            textBReq = new TextBox();
            textBReq.Location = new Point(0, 150);
            textBReq.Size = new Size(200, 100);
            textBReq.Multiline = true;
            textBReq.Text = reqReqPrx.Name + "\n" + reqReqPrx.Text;
            textBReq.ReadOnly = true;
            tabPageTree.Controls.Add (textBReq);

            PrepareDataGrid();
            PopulateDataGrid(reqReqPrx, 2, 2, 15);


        }

        void PrepareDataGrid()
        {
            DataGridView dataGridOld = null;

            if (dataGridReq != null)
            {
                dataGridOld = dataGridReq;
                tabPageTable.Controls.Remove(dataGridReq);
            }

            dataGridReq = new DataGridView();
            tabPageTable.Controls.Add(dataGridReq);

            dataGridReq.ColumnCount = 4;
            dataGridReq.Location = new Point(0, 0);
            dataGridReq.Dock = DockStyle.Fill;
            dataGridReq.AllowUserToAddRows = false;
            dataGridReq.AllowUserToDeleteRows = false;

            dataGridReq.Columns[0].Name = "Name";
            dataGridReq.Columns[0].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridReq.Columns[1].Name = "Text";
            dataGridReq.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridReq.Columns[2].Name = "Trace to";
            dataGridReq.Columns[2].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridReq.Columns[3].Name = "Trace from";
            dataGridReq.Columns[3].DefaultCellStyle.WrapMode = DataGridViewTriState.True;

            if (dataGridOld != null)
            {
                for (int i = 0; i < 4; i++)
                    dataGridReq.Columns[i].Width = dataGridOld.Columns[i].Width;
            }

            DataGridViewRow row = dataGridReq.RowTemplate;
            row.Height = dataGridReq.Font.Height * 11/2;

            arrListReqKey = new ArrayList();
        }

        void PopulateDataGrid(ReqProRequirementPrx reqReqPrx, int nUpCount, int nDownCount, int nMaxTraces)
        {
            ReqProRequirementPrx[] aTracesTo;
            ReqProRequirementPrx[] aTracesFrom;
            string[] astrReq;

            aTracesTo = reqReqPrx.GetRequirementTracesTo(nMaxTraces);
            aTracesFrom = reqReqPrx.GetRequirementTracesFrom(nMaxTraces);

            astrReq = new string[] 
                {
                    reqReqPrx.TagName,
                    reqReqPrx.Text,
                    FormatTrace(aTracesTo), 
                    FormatTrace (aTracesFrom)
                };

            arrListReqKey.Add(reqReqPrx.Key);

            dataGridReq.Rows.Add(astrReq);

            if ((aTracesFrom.GetLength(0) > 0) && (nUpCount >= 0))
            {
                --nUpCount;
                foreach (ReqProRequirementPrx reqReqPrxFrom in aTracesFrom)
                {
                    if (!arrListReqKey.Contains(reqReqPrxFrom.Key))
                        PopulateDataGrid(reqReqPrxFrom, nUpCount, nDownCount, nMaxTraces);
                    /*
                    astrReq = new string[] 
                        {
                            reqReqPrxFrom.TagName, 
                            reqReqPrxFrom.Text,
                            "", ""
                        };
                    dataGridReq.Rows.Add(astrReq);*/
                }
            }

            if ((aTracesTo.GetLength(0) > 0) && (nDownCount >= 0))
            {
                --nDownCount;
                foreach (ReqProRequirementPrx reqReqPrxTo in aTracesTo)
                {
                    if (!arrListReqKey.Contains(reqReqPrxTo.Key))
                        PopulateDataGrid(reqReqPrxTo, nUpCount, nDownCount, nMaxTraces);
                    /*
                    astrReq = new string[] 
                        {
                            reqReqPrxTo.TagName, 
                            reqReqPrxTo.Text,
                            "", ""
                        };
                    dataGridReq.Rows.Add(astrReq);*/
                }
            }

        }

        string FormatTrace(ReqProRequirementPrx[] anKeys)
        {
            string strRet = "";
            foreach (ReqProRequirementPrx reqReqPrx in anKeys)
            {
                strRet += reqReqPrx.Tag+"\n";
            }
            return strRet;
        }


        /* private void tabPageTree_Paint(object sender, PaintEventArgs e)
        {
            Graphics graphics = e.Graphics;
            reqCurrentUIBox.Paint(e.Graphics);
            graphics.Dispose();
        }*/
    }
}