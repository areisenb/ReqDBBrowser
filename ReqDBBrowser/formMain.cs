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
    public partial class FormMain : Form, 
        ReqProProject.IReqProProjectCb, TreeViewReq.ITreeViewReqCb,
        ReqTraceUIDataGridView.ITraceViewGridCb, ReqTraceUIGraphNode.ITraceViewGraphCb,
        ReqSearchUIDataGridView.ISearchResultViewGridCb
    {
        TreeViewReq treeViewRq;
        ReqProProject reqDBBrowser;

        ReqTraceUIDataGridView dataGridReq;
        ReqTraceGrid reqTraceGrid;
        List<ReqTraceUIGraphNode> arrTraceGraphNode;
        ReqTraceGraphDot reqTraceGraphDot;

        FormProgressReqTree formProgressReqTree;

        ReqSearchUIDataGridView dgvSearchResult;
        ReqSearchUIDataGridView dgvQuickSearchResult;

        TraceParameter tracePar;
        List<int> listnReqTypeTracedKeyExcl;
        List<int> listnReqTypeRootKeyExcl;

        class TraceParameter
        {
            public int nUpCount;
            public int nDownCount;
            public int nMaxTraces;
            public int nMaxUpCount;
            public int nMaxDownCount;

            public TraceParameter ()
            {
                nUpCount = 2;
                nDownCount = 2;
                nMaxTraces = 20;
                nMaxUpCount = 3;
                nMaxDownCount = 3;
            }
        }

        public FormMain()
        {
            InitializeComponent();
            reqDBBrowser = new ReqProProject(this as ReqProProject.IReqProProjectCb);
            arrTraceGraphNode = new List<ReqTraceUIGraphNode>();
            tracePar = new TraceParameter();
            listnReqTypeTracedKeyExcl = new List<int>();
            listnReqTypeRootKeyExcl = new List<int>();
            tabDetails.TabPages.Remove(tabPageQuickSearchResults);
            tabDetails.TabPages.Remove(tabPageSearchResults);
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
                formProgressReqTree.Dispose();
                formProgressReqTree = null;
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

        public void DoRequirementTraces(ArrayList arrKeys, string strTreePathName, bool bConsiderRootReqTypeFilter)
        {
            ReqProRequirementPrx reqReqPrx;
            ArrayList arrReqPrx = new ArrayList();
            bool bContinue;

            bContinue = ToContinueAfterFilter(bConsiderRootReqTypeFilter, true);
            if (!bContinue)
                return;

            Cursor = Cursors.WaitCursor;

            if ((tabDetails.SelectedTab != tabPageTree) && (tabDetails.SelectedTab != tabPageTable))
                tabDetails.SelectedTab = tabPageTable;

            if (strTreePathName.Length == 0)
            {
                reqReqPrx = reqDBBrowser.GetRequirementPrx((int)arrKeys[0]);
                strTreePathName = reqReqPrx.TagName;
            }
            toolStripLabelTraceRoot.Text = strTreePathName;

            foreach (int nKey in arrKeys)
            {
                reqReqPrx = reqDBBrowser.GetRequirementPrx(nKey);
                if (bConsiderRootReqTypeFilter)
                    if (listnReqTypeRootKeyExcl.Contains(reqReqPrx.ReqTypeKey))
                        reqReqPrx = null;
                if (reqReqPrx != null)
                    arrReqPrx.Add (reqReqPrx);
            }

            Cursor = Cursors.Default;
            FillReqTraceGrid(arrReqPrx, tracePar.nUpCount, tracePar.nDownCount, tracePar.nMaxTraces, 
                tracePar.nMaxUpCount, tracePar.nMaxDownCount, listnReqTypeTracedKeyExcl);
            Cursor = Cursors.WaitCursor;
            PrepareDataGrid();
            PopulateDataGrid(tracePar.nUpCount, tracePar.nDownCount);
            PopulateGraph(strTreePathName, tracePar.nUpCount, tracePar.nDownCount);
            Invalidate();
            this.tabPageTree.Invalidate();
            Cursor = Cursors.Default;
        }

        private bool ToContinueAfterFilter(bool bConsiderRootReqTypeFilter, bool bConsiderTracedReqTypeFilter)
        {
            FormReqFilter formReqFilter;
            string strPrjPrefix;
            string[] astrPrjReqType;
            int[] anReqTypeKey;

            string[] astrRelPrjPrefix;
            string[][] aastrRelPrjReqType;
            int[][] aanRelPrjReqTypeKey;

            Cursor = Cursors.WaitCursor;
            reqDBBrowser.GetProjectReqType(out strPrjPrefix, out astrPrjReqType, out anReqTypeKey);
            reqDBBrowser.GetRelProjectReqType(out astrRelPrjPrefix, out aastrRelPrjReqType, out aanRelPrjReqTypeKey);
            Cursor = Cursors.Default;

            formReqFilter = new FormReqFilter(bConsiderRootReqTypeFilter ? strPrjPrefix : null, astrPrjReqType, anReqTypeKey,
                bConsiderTracedReqTypeFilter ? astrRelPrjPrefix : null, aastrRelPrjReqType, aanRelPrjReqTypeKey,
                listnReqTypeRootKeyExcl, listnReqTypeTracedKeyExcl,
                tracePar.nUpCount, tracePar.nDownCount, tracePar.nMaxTraces, tracePar.nMaxUpCount, tracePar.nMaxDownCount);

            if (formReqFilter.ShowDialog() == DialogResult.Cancel)
                return false;

            tracePar.nUpCount = formReqFilter.UpCount;
            tracePar.nDownCount = formReqFilter.DownCount;
            tracePar.nMaxTraces = formReqFilter.MaxTraces;
            tracePar.nMaxUpCount = formReqFilter.MaxUpCount;
            tracePar.nMaxDownCount = formReqFilter.MaxDownCount;

            if (bConsiderTracedReqTypeFilter)
                listnReqTypeTracedKeyExcl = formReqFilter.ReqTypeTracedKeyExcl;

            if (bConsiderRootReqTypeFilter)
                listnReqTypeRootKeyExcl = formReqFilter.ReqTypeRootKeyExcl;

            return true;
        }

        private void RefreshPackage(int nPkgKey, ArrayList arrOldPkgKeys)
        {
            ReqTreeNode reqTreeNode;
            int nPackageCount;

            formProgressReqTree = new FormProgressReqTree();
            formProgressReqTree.Show();

            reqDBBrowser.RemovePkgs(arrOldPkgKeys);
            reqTreeNode = reqDBBrowser.ReadReqTree(out nPackageCount, nPkgKey);
            treeViewRq.ReplaceActualTree(reqTreeNode);
            formProgressReqTree.Dispose();
            formProgressReqTree = null;
        }


        void FillReqTraceGrid(ArrayList arrReqPrx,
            int nUpCount, int nDownCount, int nMaxTraces, int nMaxUpCount, int nMaxDownCount,
            List<int> listnReqTypeTracedKeyExcl)
        {
            FormProgressReqTraceGrid formProgressTraceGrid;
            formProgressTraceGrid = new FormProgressReqTraceGrid();
            formProgressTraceGrid.Show();

            reqTraceGrid = new ReqTraceGrid(nUpCount, nDownCount, nMaxTraces,
                nMaxUpCount, nMaxDownCount, listnReqTypeTracedKeyExcl, formProgressTraceGrid.ShowProgressReqTraceGrid);
            reqTraceGrid.AddReq(arrReqPrx);
        }

        void PrepareDataGrid()
        {
            ReqTraceUIDataGridView dataGridOld = null;

            if (dataGridReq != null)
            {
                dataGridOld = dataGridReq;
                tabPageTable.Controls.Remove(dataGridReq);
            }
            dataGridReq = new ReqTraceUIDataGridView(tabPageTable.Size.Width, dataGridOld, 
                                this as ReqTraceUIDataGridView.ITraceViewGridCb);
            tabPageTable.Controls.Add(dataGridReq);
        }

        void PopulateDataGrid(int nUpCount, int nDownCount)
        {
            ReqTraceNode reqTraceNode;

            for (int i = nUpCount; i >= -nDownCount; i--)
                for (int j = 0; j < reqTraceGrid.GetElementCount (i); j++)
                {
                    reqTraceNode = reqTraceGrid[i, j];
                    if (reqTraceNode != null)
                        dataGridReq.AddRow(reqTraceNode);
                }
        }

        void webBrowserReqTraceGraph_Navigating(object sender, System.Windows.Forms.WebBrowserNavigatingEventArgs e)
        {
            //check for navigating to WebReqPro - cancel in that scenario
            // by e.Cancel = true;
        }

        void PopulateGraph(string strGraphDesc, int nUpCount, int nDownCount)
        {
            reqTraceGraphDot = new ReqTraceGraphDot(strGraphDesc);
            ReqTraceNode reqTraceNode;

            tabPageTree.Controls.Clear();
            arrTraceGraphNode.Clear();

            ReqTraceUIGraphNode.Init(tabPageTree.Controls, this as ReqTraceUIGraphNode.ITraceViewGraphCb);

            for (int i = nUpCount; i >= -nDownCount; i--)
                for (int k = 0; k < reqTraceGrid.GetElementCount(i); k++)
                {
                    reqTraceNode = reqTraceGrid[i, k];
                    if (reqTraceNode != null)
                    {
                        arrTraceGraphNode.Add(new ReqTraceUIGraphNode(nUpCount, reqTraceNode));
                        reqTraceGraphDot.Add(new ReqTraceDotNode(nUpCount, reqTraceNode));
                    }
                }

            webBrowserReqTraceGraph.Url = new Uri (reqTraceGraphDot.Export2svg());
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

            genTable = new FormGenericTable("Log - Requirement: " + rpxReq.Tag, astrHead, afHeadSize, FormGenericTableLayout.eFormGenericTableToken.eReqLog);
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

            genTable.SetGridContent(astrGrid, nCount + " Log Entries");
            genTable.Show();
        }

        private void ShowRequirementsLog(ArrayList arrKeys, string strTreePathName, bool bWithFilter)
        {
            ReqProRequirementPrx rpxReq;
            FormGenericTable genTable;
            ReqProRequirementPrx.sHistEntry[] asPerReqHistory;
            int nCount;
            int nReqCount = 0;
            string[,] astrGrid;
            List<ReqProRequirementPrx.sHistEntry> listHistory;
            List<string> listTag;
            List<string> listName;

            listHistory = new List<ReqProRequirementPrx.sHistEntry>();
            listTag = new List<string>();
            listName = new List<string>();

            if (bWithFilter)
                if (!ToContinueAfterFilter(true, false))
                    return;

            genTable = new FormGenericTable("Log - Package: " + strTreePathName, null, null, FormGenericTableLayout.eFormGenericTableToken.eReqLog);

            foreach (int nKey in arrKeys)
            {
                rpxReq = reqDBBrowser.GetRequirementPrx(nKey);
                if (!listnReqTypeRootKeyExcl.Contains(rpxReq.ReqTypeKey))
                {
                    rpxReq.GetRequirementLog(out asPerReqHistory);
                    listHistory.AddRange(asPerReqHistory);
                    for (int i = 0; i < asPerReqHistory.GetLength(0); i++)
                    {
                        listTag.Add(rpxReq.Tag);
                        listName.Add(rpxReq.Name);
                    }
                    nReqCount++;
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

            genTable.SetGridContent(astrGrid, nCount + " Log Entries for " + nReqCount + " Requirements");
            genTable.Show();
        }

        private void ShowRequirements(ArrayList arrKeys, string strTreePathName, bool bWithFilter)
        {
            ReqProRequirementPrx rpxReq;
            FormGenericTable genTable;
            int nCount;
            string strCell;
            int nTraceCountTo;
            int nTraceCountFrom;
            ReqProRequirementPrx[] arpxReqTracesTo;
            ReqProRequirementPrx[] arpxReqTracesFrom;
            ReqTraceNode reqTN;
            string[] arrStringTrace;
            List<string> listTrace = new List<string>();

            string[,] astrGrid;
            List<string[]> aastrCells;
            List<string> rowHdrCells = new List<string>();
            string[] rowCells;

            string[] astrCol;
            string[] astrValue;
            int nColCount;
            List<string> colTraceTo;
            List<string> colTraceFrom;
            ReqProRequirementPrx.eTraceAbortReason eAbortReason = ReqProRequirementPrx.eTraceAbortReason.eNoAbort;

            if (bWithFilter)
                if (!ToContinueAfterFilter(true, false))
                    return;

            genTable = new FormGenericTable("Requirements - Package: " + strTreePathName, null, null, FormGenericTableLayout.eFormGenericTableToken.eReqDetails);
            nCount = 0;

            /* at least we have: Tag, Name, Text, Version, Date, User,  */
            rowHdrCells.Add ("Tag");
            rowHdrCells.Add ("Name");
            rowHdrCells.Add ("Text");
            rowHdrCells.Add("Version");
            rowHdrCells.Add("Date");
            rowHdrCells.Add("User");
            rowHdrCells.Add("Path");
            rowHdrCells.Add("Package");

            colTraceTo = new List<string>();
            colTraceFrom = new List<string>();

            colTraceTo.Add("Trace To");
            colTraceFrom.Add("Trace From");

            aastrCells = new List<string[]> ();


            foreach (int nKey in arrKeys)
            {
                int nColIdx;
                rowCells = new string [rowHdrCells.Count];

                rpxReq = reqDBBrowser.GetRequirementPrx(nKey);

                if (!listnReqTypeRootKeyExcl.Contains(rpxReq.ReqTypeKey))
                {
                    rowCells[0] = rpxReq.Tag;
                    rowCells[1] = rpxReq.Name;
                    rowCells[2] = rpxReq.Text;
                    rowCells[3] = rpxReq.VersionNumber;
                    rowCells[4] = rpxReq.VersionDateTime;
                    rowCells[5] = rpxReq.VersionUser;
                    rowCells[6] = rpxReq.PackagePathName;
                    rowCells[7] = rpxReq.PackageName;

                    nColCount = rpxReq.GetAttributes(out astrCol, out astrValue);

                    for (int i = 0; i < nColCount; i++)
                    {
                        nColIdx = rowHdrCells.IndexOf(astrCol[i]);
                        if (nColIdx == -1)
                        {
                            int nOldCnt = rowHdrCells.Count;
                            string[] rowCellsOld = rowCells;
                            rowCells = new string[nOldCnt + 1];
                            rowHdrCells.Add(astrCol[i]);
                            for (int j = 0; j < nOldCnt; j++)
                                rowCells[j] = rowCellsOld[j];
                            nColIdx = rowHdrCells.Count - 1;
                        }
                        rowCells[nColIdx] = astrValue[i];
                        if (rowCells[nColIdx] == null)
                            rowCells[nColIdx] = "";
                        System.Diagnostics.Trace.Write(rowHdrCells[nColIdx] + "(" + rowCells[nColIdx] + "); ");
                    }

                    aastrCells.Add(rowCells);
                    arpxReqTracesTo = rpxReq.GetRequirementTracesTo(20, ref eAbortReason, out nTraceCountTo, null);
                    arpxReqTracesFrom = rpxReq.GetRequirementTracesFrom(20, ref eAbortReason, out nTraceCountFrom, null);
                    reqTN = new ReqTraceNode(rpxReq, 0, true, arpxReqTracesFrom, arpxReqTracesTo,
                        nTraceCountFrom, nTraceCountTo, eAbortReason, 0, 0);

                    reqTN.GetTraceFromString(out listTrace);
                    arrStringTrace = listTrace.ToArray();
                    colTraceFrom.Add(string.Join("\n", arrStringTrace));

                    reqTN.GetTraceToString(out listTrace);
                    arrStringTrace = listTrace.ToArray();
                    colTraceTo.Add(string.Join("\n", arrStringTrace));

                    nCount++;
                }
            }

            aastrCells.Insert (0, rowHdrCells.ToArray());

            // we want some additional space for Trace To and From
            astrGrid = new string[aastrCells.Count, rowHdrCells.Count+2];
            for (int i = 0; i < aastrCells.Count; i++)
            {
                int j;
                for (j = 0; j < rowHdrCells.Count; j++)
                {
                    if (j < aastrCells[i].GetLength(0))
                        strCell = aastrCells[i][j];
                    else
                        strCell = null;
                    if (strCell == null)
                        strCell = "n/a";
                    astrGrid[i, j] = strCell;
                }
                astrGrid[i, j] = colTraceTo[i];
                astrGrid[i, j + 1] = colTraceFrom[i];
            }

            genTable.SetGridContent(astrGrid, nCount + " Requirements");
            genTable.Show();
        }

        private void FindRequirement(ArrayList arrKeys, string strTreePathName)
        {
            FormFind formFind = new FormFind();
            if (formFind.ShowDialog() == DialogResult.OK)
            {
                ReqProProject.SearchResult[][] searchResult;
                int[] anKeysFound;

                reqDBBrowser.FindRequirements(arrKeys, formFind.SearchExpr,
                    formFind.UseRegEx, formFind.SearchTag, formFind.SearchName, formFind.SearchText, true,
                    out searchResult, out anKeysFound);
                PrepareSearchResultGrid();
                PopulateSearchResultGrid(searchResult, anKeysFound);
            }
        }

        private void PrepareSearchResultGrid()
        {
            if (dgvSearchResult == null)
            {
                dgvSearchResult = new ReqSearchUIDataGridView(new string [] { "Tag", "Text" , "Name" }, 5,
                    tabPageTable.Size.Width, this as ReqSearchUIDataGridView.ISearchResultViewGridCb);
                tabPageSearchResults.Controls.Add(dgvSearchResult);
                tabDetails.TabPages.Add(tabPageSearchResults);
            }
            else
                dgvSearchResult.Init();
            
            tabDetails.SelectedTab = tabPageSearchResults;
        }

        private void PopulateSearchResultGrid(ReqProProject.SearchResult[][] searchResult, int[] anKeys)
        {
            for (int i = 0; i < searchResult.GetLength(0); i++)
                dgvSearchResult.AddRow(searchResult[i], (anKeys == null) ? 0 : anKeys[i]);
        }

        private void FindRequirement(string strSearchString)
        {
            ReqProProject.SearchResult[] searchResult;
            int[] anKeysFound;

            this.treeViewRq.FindRequirements(strSearchString, out searchResult, out anKeysFound);
            PrepareQuickSearchResultGrid();
            PopulateQuickSearchResultGrid(searchResult, anKeysFound);
        }

        private void PrepareQuickSearchResultGrid()
        {
            if (dgvQuickSearchResult == null)
            {
                dgvQuickSearchResult = new ReqSearchUIDataGridView(new string[] { "Tag: Text" }, 1, 
                    tabPageTable.Size.Width, this as ReqSearchUIDataGridView.ISearchResultViewGridCb);
                tabPageQuickSearchResults.Controls.Add(dgvQuickSearchResult);
                tabDetails.TabPages.Add(tabPageQuickSearchResults);
            }
            else
                dgvQuickSearchResult.Init();

            tabDetails.SelectedTab = tabPageQuickSearchResults;
        }

        private void PopulateQuickSearchResultGrid(ReqProProject.SearchResult[] searchResult, int[] anKeys)
        {
            ReqProProject.SearchResult[] aSearchResult = new ReqProProject.SearchResult[1];
            for (int i = 0; i < searchResult.GetLength(0); i++)
            {
                aSearchResult[0] = searchResult[i];
                dgvQuickSearchResult.AddRow(aSearchResult, (anKeys == null) ? 0 : anKeys[i]);
            }
        }

        private void tabPageTree_Paint(object sender, PaintEventArgs e)
        {
            if (arrTraceGraphNode != null)
            {
                foreach (ReqTraceUIGraphNode reqTraceNode in arrTraceGraphNode)
                    reqTraceNode.DrawBackGround(e.Graphics, tabPageTree.AutoScrollPosition);
                foreach (ReqTraceUIGraphNode reqTraceNode in arrTraceGraphNode)
                    reqTraceNode.DrawForeGround(e.Graphics, tabPageTree.AutoScrollPosition);
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
            reqDBBrowser.RefreshProject();

            Cursor = Cursors.Default;
            CreateReqTree();
            formProgressReqTree.Dispose();
            formProgressReqTree = null;
        }

        private void tabDetails_SizeChanged(object sender, EventArgs e)
        {
            if (dataGridReq != null)
                dataGridReq.ParentSizeChanged(tabPageTable.Size.Width);
        }

        #region TreeViewReq.ITreeViewReqCb
        public void GetReqCtxMenu(out string[] astrMnuEntry)
        {
            astrMnuEntry = new string[] {
                "Requirement Traces",
                "View Log",
                "View Details",
                "Refresh"
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
                        DoRequirementTraces(arrReq, "", false);
                        bDoImplicitSelect = true;
                    }
                    break;
                case 1:
                    ShowRequirementLog(nKey);
                    break;
                case 3:
                    MessageBox.Show("Want to refresh REQ " + nKey);
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
                "Refresh",
                "",
                "View Log",
                "View Requirements",
                "",
                "Find..."
            };
        }

        public bool PkgMenuAction(ArrayList arrReqKeys, ArrayList arrOtherKeys, int nPkgKey,
            int nMenuItem, string strMenuText, string strTreePathName)
        {
            bool bDoImplicitSelect = false;

            switch (nMenuItem)
            {
                case 0:
                    DoRequirementTraces(arrReqKeys, strTreePathName, true);
                    bDoImplicitSelect = true;
                    break;
                case 1:
                    RefreshPackage(nPkgKey, arrOtherKeys);
                    bDoImplicitSelect = true;
                    break;
                case 3:
                    ShowRequirementsLog(arrReqKeys, strTreePathName, true);
                    break;
                case 4:
                    ShowRequirements(arrReqKeys, strTreePathName, true);
                    break;
                case 6:
                    FindRequirement(arrReqKeys, strTreePathName);
                    break;
                default:
                    MessageBox.Show("Package " + strMenuText + " not yet implemented");
                    break;
            }
            return bDoImplicitSelect;
        }
        #endregion

        #region ReqTraceUIDataGridView.ITraceViewGridCb

        public void GetRowCtxMenu(out string[] astrMnuEntry)
        {
            astrMnuEntry = new string[] {
                "Show Entry in Tree",
                "Show Selected Entries in Tree",
                "",
                "View Selected Requirements",
                "View Log of Selected"
            };
        }

        public void RowMenuAction(int nActKey, int [] nSelKeys, int [] nMarkedKeys, int nMenuItem, string strMenuText)
        {
            ArrayList arrSelKeys;
            switch (nMenuItem)
            {
                case 0:
                    SelectTreeNode(nActKey);
                    break;
                case 1:
                    SelectTreeNodes(nSelKeys);
                    break;
                case 3:
                    arrSelKeys = new ArrayList();
                    arrSelKeys.AddRange(nSelKeys);
                    ShowRequirements(arrSelKeys, "", false);
                    break;
                case 4:
                    arrSelKeys = new ArrayList();
                    arrSelKeys.AddRange(nSelKeys);
                    ShowRequirementsLog(arrSelKeys, "", false);
                    break;
                default:
                    SelectDiag(nActKey, nSelKeys, nMarkedKeys, strMenuText);
                    break;
            }
        }

        #endregion

        private void SelectTreeNode(int nKey)
        {
            if (treeViewRq.ShowNode(nKey) == false)
                MessageBox.Show("Could not find key: " + nKey, "Failure",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void SelectTreeNodes(int[] nKeys)
        {
            string strDiag;
            if (treeViewRq.ShowNodes(nKeys) == false)
            {
                strDiag = "";
                foreach (int i in nKeys)
                    strDiag += i + " ";
                MessageBox.Show("Could not find all keys: " + strDiag, "Failure",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SelectDiag(int nKey, int[] nSelKeys, int[] nMarkedKeys, string strMenuText)
        {
            string strDiag;
            strDiag = "Key:            " + nKey;
            if (nSelKeys != null)
            {
                strDiag += "\nSel Keys:    "; ;
                foreach (int i in nSelKeys)
                    strDiag += i + " ";
            }
            else
                strDiag += "\nno selected keys";
            if (nMarkedKeys != null)
            {
                strDiag += "\nMarked Keys: ";
                foreach (int i in nMarkedKeys)
                    strDiag += i + " ";
            }
            else
                strDiag += "\nno marked keys";

            MessageBox.Show(strDiag, strMenuText + " not yet implemented");

        }

        #region ReqTraceUIGraphNode.ITraceViewGraphCb
        public void GetTBTagCtxMenu(out string[] astrMnuEntry)
        {
            astrMnuEntry = new string[] {
                "Show Entry in Tree",
                "Show Selected Entries in Tree"
            };
        }

        public void GetTBNameCtxMenu(out string[] astrMnuEntry)
        {
            astrMnuEntry = new string[] {
                "Show Entry in Tree",
                "Show Selected Entries in Tree"
            };
        }

        public void TBMenuAction (int nActKey, int [] nSelKeys, int [] nMarkedKeys, int nMenuItem, bool bWasTBName, string strMenuText)
        {
            switch (nMenuItem)
            {
                case 0:
                    SelectTreeNode(nActKey);
                    break;
                case 1:
                    SelectTreeNodes(nSelKeys);
                    break;
                default:
                    SelectDiag(nActKey, nSelKeys, nMarkedKeys, strMenuText + (bWasTBName ? " Name" : " Tag"));
                    break;
            }

        }
        #endregion

        #region ISearchResCb Members

        void ReqSearchUIDataGridView.ISearchResultViewGridCb.GetRowCtxMenu
            (out string[] astrMnuEntry)
        {
            astrMnuEntry = new string[] {
                "Show Entry in Tree",
                "Show Selected Entries in Tree",
                "",
                "View Selected Requirements",
                "View Log of Selected"
            };
        }

        void ReqSearchUIDataGridView.ISearchResultViewGridCb.RowMenuAction
            (int nActKey, int[] nSelKeys, int[] nMarkedKeys, int nMenuItem, string strMenuText)
        {
            ArrayList arrSelKeys;

            switch (nMenuItem)
            {
                case 0:
                    SelectTreeNode(nActKey);
                    break;
                case 1:
                    SelectTreeNodes(nSelKeys);
                    break;
                case 3:
                    arrSelKeys = new ArrayList();
                    arrSelKeys.AddRange(nSelKeys);
                    ShowRequirements(arrSelKeys, "", false);
                    break;
                case 4:
                    arrSelKeys = new ArrayList();
                    arrSelKeys.AddRange(nSelKeys);
                    ShowRequirementsLog(arrSelKeys, "", false);
                    break;
                default:
                    SelectDiag(nActKey, nSelKeys, nMarkedKeys, strMenuText);
                    break;
            }
        }
        #endregion

        private void btQuickSearch_Click(object sender, EventArgs e)
        {
            QuickSearch();
        }

        private void tbQuickSearch_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char) Keys.Enter)
                QuickSearch();
        }

        private void QuickSearch()
        {
            string strQuickSearch;
            strQuickSearch = tbQuickSearch.Text;
            if (strQuickSearch.Length > 0)
                FindRequirement(strQuickSearch);
        }

        private void tabPageGraph_Enter(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("tabPageGraph gets Fokus (and Hooks MOUSEWHEEL)");
            webBrowserReqTraceGraph.HookOn();
        }

        private void tabPageGraph_Leave(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("tabPageGraph looses Fokus (and UnHooks MOUSEWHEEL)");
            webBrowserReqTraceGraph.HookOff();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("FormMain closing (and UnHooks MOUSEWHEEL)");
            webBrowserReqTraceGraph.HookOff();
        }

    }
}