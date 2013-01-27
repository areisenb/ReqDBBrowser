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

        DataGridView dataGridReq;
        ReqTraceGrid reqTraceGrid;

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
            ReqProRequirementPrx reqReqPrx;

            reqReqPrx = reqDBBrowser.GetRequirementPrx(nKey);

            FillReqTraceGrid(reqReqPrx, 2, 2, 15, 3, 3);
            PrepareDataGrid();
            PopulateDataGrid(2, 2);
            PopulateGraph(2, 2);

        }

        void FillReqTraceGrid(ReqProRequirementPrx reqReqPrx,
            int nUpCount, int nDownCount, int nMaxTraces, int nMaxUpCount, int nMaxDownCount)
        {
            reqTraceGrid = new ReqTraceGrid(nUpCount, nDownCount, nMaxTraces, nMaxUpCount, nMaxDownCount);
            reqTraceGrid.AddReq(reqReqPrx);
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

        }

        void PopulateDataGrid(int nUpCount, int nDownCount)
        {
            string[] astrReq;
            ReqTraceGrid.ReqTraceNode reqTraceNode;

            for (int i = nUpCount; i >= -nDownCount; i--)
                for (int j = 0; j < reqTraceGrid.GetElementCount (i); j++)
                {
                    reqTraceNode = reqTraceGrid[i, j];
                    if (reqTraceNode != null)
                    {
                        astrReq = new string[] 
                            {
                                reqTraceNode.TagName,
                                reqTraceNode.Text,
                                reqTraceNode.GetTraceToString (),
                                reqTraceNode.GetTraceFromString()
                            };
                        dataGridReq.Rows.Add(astrReq);
                    }
                }
        }

        void PopulateGraph(int nUpCount, int nDownCount)
        {
            ReqTraceGrid.ReqTraceNode reqTraceNode;
            TextBox textBReq;
            int j = 0;
            Size sizeText;
            Size sizeTagName;
            int [] nTraceToX;
            int [] nTraceToY;

            sizeText = new Size(200, 100);
            sizeTagName = new Size(200, 20);

            tabPageTree.Controls.Clear();

            for (int i = nUpCount; i >= -nDownCount; i--, j++)
                for (int k = 0; k < reqTraceGrid.GetElementCount(i); k++)
                {
                    reqTraceNode = reqTraceGrid[i, k];
                    if (reqTraceNode != null)
                    {
                        textBReq = new TextBox();
                        textBReq.Location = new Point(k * 250, j * 150);
                        textBReq.Size = sizeTagName;
                        textBReq.Multiline = true;
                        textBReq.ReadOnly = true;
                        textBReq.Text = reqTraceNode.TagName;
                        tabPageTree.Controls.Add(textBReq);

                        textBReq = new TextBox();
                        textBReq.Location = new Point(k * 250, j * 150 + sizeTagName.Height);
                        textBReq.Size = sizeText;
                        textBReq.Multiline = true;
                        textBReq.ReadOnly = true;
                        textBReq.Text = reqTraceNode.Text;
                        tabPageTree.Controls.Add(textBReq);
                        reqTraceNode.GetTraceToCoord (out nTraceToX, out nTraceToY);
                    }
                }
        }
    }
}