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
    public partial class FormMain : Form, ReqProProject.IReqProProjectCb, TreeViewReq.ITreeViewReqCb
    {
        TreeViewReq treeViewRq;
        ReqProProject reqDBBrowser;

        DataGridView dataGridReq;
        ReqTraceGrid reqTraceGrid;
        ArrayList arrTraceDwg;
        int oldTabPageTableWidth;
        FormProgressReqTree formProgressReqTree;

        public FormMain()
        {
            InitializeComponent();
            reqDBBrowser = new ReqProProject(this as ReqProProject.IReqProProjectCb);
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
                formProgressReqTree = new FormProgressReqTree();
                formProgressReqTree.Show();
                if (reqDBBrowser.OpenProject(fmOpenProject.strProjectFile,
                    fmOpenProject.strUser,
                    fmOpenProject.strPassword,
                    out strErrDiag))
                    CreateReqTree ();
                else
                    MessageBox.Show(strErrDiag, "Error Open Project", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateReqTree()
        {
            int nPackageCount;

            treeViewRq.CreateTree(reqDBBrowser.ReadReqTree(out nPackageCount));
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

        /* from IReqProProjectCb */
        public bool RequestCredentials(string strProjectDesc, ref string strUser, out string strPassword)
        {
            FormRequestCredentials formRequestCredentials;

            formRequestCredentials = new FormRequestCredentials(strProjectDesc, strUser);
            if (formRequestCredentials.ShowDialog() == DialogResult.OK)
            {
                strUser = formRequestCredentials.User;
                strPassword = formRequestCredentials.Password;
                return true;
            }
            strPassword = "";
            return false;
        }

        public void ShowOpenRelatedProjectError(string strProjectDesc)
        {
        }

        public bool ShowProgressReqPkgTree(int nSumTreeElements, int nReadTreeElements,
            int nSumRequirements, int nReadRequirements, string strLogg)
        {
            return formProgressReqTree.ShowProgressReqTree
                (nSumTreeElements, nReadTreeElements, nSumRequirements, nReadRequirements, strLogg);
        }

        public bool ShowProgressOpenProject(string strLogg)
        {
            return formProgressReqTree.ShowProgressReqTree(0, 0, 0, 0, strLogg);
        }

        public void DoRequirementTraces(ArrayList arrKeys, string strTreePathName)
        {
            ReqProRequirementPrx reqReqPrx;
            ArrayList arrReqPrx = new ArrayList();

            Cursor = Cursors.WaitCursor;
            if (strTreePathName.Length == 0)
            {
                reqReqPrx = reqDBBrowser.GetRequirementPrx((int)arrKeys[0]);
                strTreePathName = reqReqPrx.TagName;
            }
            toolStripLabelTraceRoot.Text = strTreePathName;

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
            Invalidate();
            this.tabPageTree.Invalidate();
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
            ToolTip toolTipHdr;

            int j = 0;
            Size sizeText;
            Size sizeTagName;
            int [] nTraceToX;
            int [] nTraceToY;
            int nTraces;

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
                        toolTipHdr = new ToolTip();
                        
                        textBReq = new TextBox();
                        textBReq.Location = new Point(k * nXSpacing + nXSpacing/2, j * nYSpacing + nYSpacing/2);
                        textBReq.Size = sizeTagName;
                        textBReq.Multiline = true;
                        textBReq.ReadOnly = true;
                        textBReq.Text = reqTraceNode.TagName;
                        toolTipHdr.BackColor = Color.Yellow;
                        toolTipHdr.SetToolTip(textBReq, reqTraceNode.TagName);
                        tabPageTree.Controls.Add(textBReq);

                        textBReq = new TextBox();
                        textBReq.Location = new Point(k * nXSpacing + nXSpacing/2, j * nYSpacing + sizeTagName.Height + nYSpacing/2);
                        textBReq.Size = sizeText;
                        textBReq.Multiline = true;
                        textBReq.ReadOnly = true;
                        textBReq.ScrollBars = ScrollBars.Vertical;
                        textBReq.Text = reqTraceNode.Text;
                        tabPageTree.Controls.Add(textBReq);
                        reqTraceNode.GetTraceToCoord (out nTraceToX, out nTraceToY);
                        
                        nTraces = reqTraceNode.AreTooManyTracesFrom();
                        if (nTraces > 0)
                            arrTraceDwg.Insert (0, new ReqTraceUIArrowDwn (nTraces, 
                                k * nXSpacing + nXSpacing/2 + sizeTagName.Width/2, 
                                j * nYSpacing + nYSpacing/2));

                        nTraces = reqTraceNode.AreTooManyTracesTo();
                        if (nTraces > 0)
                            arrTraceDwg.Insert(0, new ReqTraceUIArrowUp (nTraces,
                                k * nXSpacing + nXSpacing / 2 + sizeTagName.Width / 2,
                                j * nYSpacing + sizeTagName.Height + sizeText.Height + nYSpacing / 2));

                        for (int l = nTraceToX.GetLength(0)-1; l >= 0; l--)
                            if ((nTraceToX[l] != int.MinValue) && (reqTraceNode.X != int.MinValue))
                                arrTraceDwg.Add (new ReqTraceUITraceArrow (
                                    reqTraceNode.X * nXSpacing + sizeText.Width / 2 + nXSpacing/2,
                                    (nUpCount - reqTraceNode.Y) * nYSpacing + sizeTagName.Height + sizeText.Height + nYSpacing/2,
                                    nTraceToX[l] * nXSpacing + sizeText.Width / 2 + nXSpacing/2,
                                    (nUpCount - nTraceToY[l]) * nYSpacing + nYSpacing/2));
                    }
                }
        }

        private void ShowRequirementLog(int nKey)
        {
            FormGenericTable genTable;
            ReqProRequirementPrx.sHistEntry[] asHistory;
            ReqProRequirementPrx rpxReq;
            int nCount;
            string[,] astrGrid;
            string[] astrHead;
            float[] afHeadSize;
            
            rpxReq = reqDBBrowser.GetRequirementPrx (nKey);
            astrHead = new string[2];
            astrHead[0] = rpxReq.Name;
            astrHead[1] = rpxReq.Text;
            afHeadSize = new float[2];
            afHeadSize[0] = 3F / 2;
            afHeadSize[1] = 11F / 2;

            genTable = new FormGenericTable("Log - Requirement: " + rpxReq.Tag, astrHead, afHeadSize);
            rpxReq.GetRequirementLog(out asHistory);
            nCount = asHistory.GetLength(0);

            astrGrid = new string [nCount+1,4];
            astrGrid[0, 0] = "Rev";
            astrGrid[0, 1] = "Date";
            astrGrid[0, 2] = "User";
            astrGrid[0, 3] = "Description";

            for (int i = 1; i <= nCount; i++)
            {
                astrGrid[i,0] = asHistory[i-1].strRevision;
                astrGrid[i,1] = asHistory[i-1].date.ToString("u");
                astrGrid[i,2] = asHistory[i-1].strUser;
                astrGrid[i,3] = asHistory[i-1].strDesc;
            }

            genTable.SetGridContent(astrGrid);
            genTable.Show();
        }

        private void ShowRequirementsLog(ArrayList arrKeys, string strTreePathName)
        {
            ReqProRequirementPrx rpxReq;
            FormGenericTable genTable;
            ReqProRequirementPrx.sHistEntry[] asPerReqHistory;
            int nCount;
            string[,] astrGrid;
            List<ReqProRequirementPrx.sHistEntry> listHistory;
            List<string> listTag;
            List<string> listName;

            listHistory = new List<ReqProRequirementPrx.sHistEntry>();
            listTag = new List<string>();
            listName = new List<string>();

            genTable = new FormGenericTable("Log - Package: " + strTreePathName, null, null);

            foreach (int nKey in arrKeys)
            {
                rpxReq = reqDBBrowser.GetRequirementPrx(nKey);
                rpxReq.GetRequirementLog(out asPerReqHistory);
                listHistory.AddRange(asPerReqHistory);
                for (int i = 0; i < asPerReqHistory.GetLength(0); i++)
                {
                    listTag.Add(rpxReq.Tag);
                    listName.Add(rpxReq.Name);
                }
            }

            nCount = listHistory.Count;

            astrGrid = new string[nCount + 1, 6];
            astrGrid[0, 0] = "Tag";
            astrGrid[0, 1] = "Name";
            astrGrid[0, 2] = "Rev";
            astrGrid[0, 3] = "Date";
            astrGrid[0, 4] = "User";
            astrGrid[0, 5] = "Description";

            for (int i = 1; i <= nCount; i++)
            {
                astrGrid[i, 0] = listTag[i-1];
                astrGrid[i, 1] = listName[i-1];
                astrGrid[i, 2] = listHistory[i - 1].strRevision;
                astrGrid[i, 3] = listHistory[i - 1].date.ToString("u");
                astrGrid[i, 4] = listHistory[i - 1].strUser;
                astrGrid[i, 5] = listHistory[i - 1].strDesc;
            }

            genTable.SetGridContent(astrGrid);
            genTable.Show();
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

            formProgressReqTree = new FormProgressReqTree();
            formProgressReqTree.Show();

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

        public bool ReqMenuAction(int nKey, int nMenuItem, string strMenuText)
        {
            bool bDoImplicitSelect = false;
            switch (nMenuItem)
            {
                case 0:
                    {
                        ArrayList arrReq = new ArrayList();
                        arrReq.Add(nKey);
                        DoRequirementTraces(arrReq, "");
                        bDoImplicitSelect = true;
                    }
                    break;
                case 1:
                    ShowRequirementLog(nKey);
                    break;
                default:
                    MessageBox.Show("Requirement " + strMenuText + " not yet implemented");
                    break;
            }
            return bDoImplicitSelect;
        }

        public void GetPkgCtxMenu(out string[] astrMnuEntry)
        {
            astrMnuEntry = new string[] {
                "Requirement Traces",
                "View Log",
                "View Requirements"
            };
        }

        public bool PkgMenuAction(ArrayList arrReqKeys, ArrayList arrOtherKeys,
            int nMenuItem, string strMenuText, string strTreePathName)
        {
            bool bDoImplicitSelect = false;

            switch (nMenuItem)
            {
                case 0:
                    DoRequirementTraces(arrReqKeys, strTreePathName);
                    bDoImplicitSelect = true;
                    break;
                case 1:
                    ShowRequirementsLog(arrReqKeys, strTreePathName);
                    break;
                default:
                    MessageBox.Show("Package " + strMenuText + " not yet implemented");
                    break;
            }
            return bDoImplicitSelect;
        }
    }
}