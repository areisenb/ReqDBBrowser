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
    public partial class FormMain : Form, TreeViewReq.ITreeViewReqCb
    {
        TreeViewReq treeViewRq;
        ReqProProject reqDBBrowser;

        DataGridView dataGridReq;
        ReqTraceGrid reqTraceGrid;
        ArrayList arrTraceDwg;
        int oldTabPageTableWidth;

        public FormMain()
        {
            InitializeComponent();
            reqDBBrowser = new ReqProProject();
            arrTraceDwg = new ArrayList();
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
                string strErrDiag;
                Cursor = Cursors.WaitCursor;
                if (reqDBBrowser.OpenProject(fmOpenProject.strProjectFile,
                    fmOpenProject.strUser,
                    fmOpenProject.strPassword,
                    new ReqProProject.RequestCredentialsCallback(this.RequestCredentialsCallback),
                    new ReqProProject.ShowOpenReleatedProjectErrorCallback(this.ShowOpenReleatedProjectErrorCallback),
                    out strErrDiag))
                    CreateReqTree ();
                else
                    MessageBox.Show(strErrDiag, "Error Open Project", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cursor = Cursors.Default;
            }
        }

        private void CreateReqTree()
        {
            int nPackageCount;
            FormProgressReqTree formProgressReqTree = new FormProgressReqTree();

            formProgressReqTree.Show();
            treeViewRq.CreateTree(reqDBBrowser.ReadReqTree(out nPackageCount, formProgressReqTree.ShowProgressReqTree));
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            reqDBBrowser.CloseProject();
            CleanupViews();
            Cursor = Cursors.Default;
        }

        public void CleanupViews()
        {
            if (treeViewRq != null)
                splitContainerMain.Panel1.Controls.Remove(treeViewRq);
            treeViewRq = new TreeViewReq(this as TreeViewReq.ITreeViewReqCb, imageListReqTree);
            treeViewRq.Dock = DockStyle.Fill;
            splitContainerMain.Panel1.Controls.Add(treeViewRq);
            //this.ResizeRedraw = true; 
        }

        private void formMain_Load(object sender, EventArgs e)
        {
            CleanupViews();
        }

        public bool RequestCredentialsCallback(string strProjectDesc, ref string strUser, out string strPassword)
        {
            strPassword = "";
            return false;
        }

        public void ShowOpenReleatedProjectErrorCallback(string strProjectDesc)
        {
        }

        public void DoRequirementTraces(ArrayList arrKeys)
        {
            ReqProRequirementPrx reqReqPrx;
            ArrayList arrReqPrx = new ArrayList();

            Cursor = Cursors.WaitCursor;
            Invalidate();
            this.tabPageTree.Invalidate();
            foreach (int nKey in arrKeys)
            {
                reqReqPrx = reqDBBrowser.GetRequirementPrx(nKey);
                arrReqPrx.Add (reqReqPrx);
            }

            Cursor = Cursors.Default;
            FillReqTraceGrid(arrReqPrx, 2, 2, 15, 3, 3);
            Cursor = Cursors.WaitCursor;
            PrepareDataGrid();
            PopulateDataGrid(2, 2);
            PopulateGraph(2, 2);
            Cursor = Cursors.Default;
        }

        void FillReqTraceGrid(ArrayList arrReqPrx,
            int nUpCount, int nDownCount, int nMaxTraces, int nMaxUpCount, int nMaxDownCount)
        {
            FormProgressReqTraceGrid formProgressTraceGrid;
            formProgressTraceGrid = new FormProgressReqTraceGrid();
            formProgressTraceGrid.Show();

            reqTraceGrid = new ReqTraceGrid(nUpCount, nDownCount, nMaxTraces, 
                nMaxUpCount, nMaxDownCount, formProgressTraceGrid.ShowProgressReqTraceGrid);
            reqTraceGrid.AddReq(arrReqPrx);
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

            int nWidth = tabPageTable.Size.Width - dataGridReq.RowHeadersWidth;

            dataGridReq.Columns[0].Name = "Name";
            dataGridReq.Columns[0].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridReq.Columns[0].Width = nWidth / 10;
            dataGridReq.Columns[1].Name = "Text";
            dataGridReq.Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridReq.Columns[1].Width = nWidth / 2;
            dataGridReq.Columns[2].Name = "Trace to";
            dataGridReq.Columns[2].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridReq.Columns[2].Width = nWidth / 5;
            dataGridReq.Columns[3].Name = "Trace from";
            dataGridReq.Columns[3].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            dataGridReq.Columns[3].Width = nWidth / 5;

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
            const int nXSpacing = 250;
            const int nYSpacing = 200;

            tabPageTree.Controls.Clear();
            arrTraceDwg.Clear();


            for (int i = nUpCount; i >= -nDownCount; i--, j++)
                for (int k = 0; k < reqTraceGrid.GetElementCount(i); k++)
                {
                    reqTraceNode = reqTraceGrid[i, k];
                    if (reqTraceNode != null)
                    {
                        textBReq = new TextBox();
                        textBReq.Location = new Point(k * nXSpacing, j * nYSpacing);
                        textBReq.Size = sizeTagName;
                        textBReq.Multiline = true;
                        textBReq.ReadOnly = true;
                        textBReq.Text = reqTraceNode.TagName;
                        tabPageTree.Controls.Add(textBReq);

                        textBReq = new TextBox();
                        textBReq.Location = new Point(k * nXSpacing, j * nYSpacing + sizeTagName.Height);
                        textBReq.Size = sizeText;
                        textBReq.Multiline = true;
                        textBReq.ReadOnly = true;
                        textBReq.Text = reqTraceNode.Text;
                        tabPageTree.Controls.Add(textBReq);
                        reqTraceNode.GetTraceToCoord (out nTraceToX, out nTraceToY);
                        for (int l = nTraceToX.GetLength(0)-1; l >= 0; l--)
                            if ((nTraceToX[l] != int.MinValue) && (reqTraceNode.X != int.MinValue))
                                arrTraceDwg.Add (new ReqTraceUI (
                                    reqTraceNode.X * nXSpacing + sizeText.Width / 2,
                                    (nUpCount - reqTraceNode.Y) * nYSpacing + sizeTagName.Height + sizeText.Height,
                                    nTraceToX[l] * nXSpacing + sizeText.Width / 2,
                                    (nUpCount - nTraceToY[l]) * nYSpacing));
                    }
                }
        }

        private void tabPageTree_Paint(object sender, PaintEventArgs e)
        {
            if (arrTraceDwg != null)
                foreach (ReqTraceUI reqTrace in arrTraceDwg)
                {
                    reqTrace.Draw(e.Graphics, tabPageTree.AutoScrollPosition);
                }
        }

        private void tabPageTree_ClientSizeChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        private void tabPageTree_Scroll(object sender, ScrollEventArgs e)
        {
            this.Invalidate();
            this.tabPageTree.Invalidate();
        }

        private void toolStripButtonRefresh_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            CleanupViews();
            Cursor = Cursors.Default;
            CreateReqTree();
        }

        private void tabDetails_SizeChanged(object sender, EventArgs e)
        {
            if (dataGridReq != null)
            {
                int nInc = tabPageTable.Size.Width - dataGridReq.RowHeadersWidth - oldTabPageTableWidth;
                if (nInc > 0)
                {
                    int nOldWidth = 0;
                    for (int i=0; i<dataGridReq.Columns.Count; i++)
                        nOldWidth += dataGridReq.Columns[i].Width;
                    if (nOldWidth < oldTabPageTableWidth)
                    {
                        dataGridReq.Columns[1].Width += nInc / 2;
                        dataGridReq.Columns[0].Width += nInc / 10;
                        dataGridReq.Columns[2].Width += nInc / 5;
                        dataGridReq.Columns[3].Width += nInc / 5;
                    }
                }
                oldTabPageTableWidth = tabPageTable.Size.Width - dataGridReq.RowHeadersWidth;
            }
        }

        /* from TreeViewReq.ITreeViewReqCb */
        public void GetReqCtxMenu(out string[] astrMnuEntry)
        {
            astrMnuEntry = new string[] {
                "Requirement Traces",
                "View Log",
                "View Details"
            };
        }

        public void GetPkgCtxMenu(out string[] astrMnuEntry)
        {
            astrMnuEntry = new string[] {
                "Requirement Traces",
                "View Log",
                "View Requirements"
            };
        }

        public void ReqMenuAction(int nKey, int nMenuItem, string strMenuText)
        {
            switch (nMenuItem)
            {
                case 0:
                    {
                        ArrayList arrReq = new ArrayList();
                        arrReq.Add(nKey);
                        DoRequirementTraces(arrReq);
                    }
                    break;
                case 1:
                    {
                        int nCount;
                        ReqPro40.Requirement rpxReq = reqDBBrowser.GetRequirement(nKey);
                        ReqPro40.Revision rpxRev;

                        nCount = rpxReq.Revisions.Count;
                        for (int i=0; i<nCount; i++) 
                        {
                            rpxRev = rpxReq.Revisions[i+1, ReqPro40.enumRevisionLookups.eRevLookup_Index];
                            System.Diagnostics.Trace.WriteLine(rpxReq.get_Tag(ReqPro40.enumTagFormat.eTagFormat_Tag) + 
                                " Rev.: " + rpxRev.VersionNumber +
                                " Date: " + rpxRev.VersionDateTime +
                                " User: " + rpxRev.VersionUser.FullName +
                                " Change: " + rpxRev.VersionReason);
                        }
                    }
                    break;
                default:
                    MessageBox.Show("Requirement " + strMenuText + " not yet implemented");
                    break;
            }
        }

        public void PkgMenuAction(ArrayList arrReqKeys, ArrayList arrOtherKeys,
            int nMenuItem, string strMenuText)
        {
            switch (nMenuItem)
            {
                case 0:
                    DoRequirementTraces(arrReqKeys);
                    break;
                default:
                    MessageBox.Show("Package " + strMenuText + " not yet implemented");
                    break;
            }
        }
    }
}