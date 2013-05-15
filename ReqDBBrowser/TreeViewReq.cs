using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace ReqDBBrowser
{
    class TreeViewReq: TreeView
    {
        ContextMenuStrip mnuCtxPkg;
        ContextMenuStrip mnuCtxReq;
        ITreeViewReqCb cb;
        TreeNode ActiveNode;

        public interface ITreeViewReqCb
        {
            void GetReqCtxMenu(out string[] astrMnuEntry);
            void GetPkgCtxMenu(out string[] astrMnuEntry);
            bool ReqMenuAction(int nKey, int nMenuItem, string strMenuText);
            bool PkgMenuAction(ArrayList arrReqKeys, ArrayList arrOtherKeys, int nPkgKey,
                int nMenuItem, string strMenuText, string strTreePathName);
        }

        public TreeViewReq(ITreeViewReqCb cb, ImageList imageListTree)
        {
            string [] astrMenuItems;
            ToolStripMenuItem tsMnuItem;
            ToolStripMenuItem tsMnuSubItem;
            int i;

            this.cb = cb;
            this.ActiveNode = null;

            // Create the ContextMenuStrip for the packages
            mnuCtxPkg = new ContextMenuStrip();
            cb.GetPkgCtxMenu (out astrMenuItems);
            i = 0;
            foreach (string str in astrMenuItems)
            {
                if (str.Length > 0)
                {
                    tsMnuItem = new ToolStripMenuItem(str, null, mnuCtxPkg_Click);
                    tsMnuItem.Tag = i;
                    mnuCtxPkg.Items.Add(tsMnuItem);
                }
                else
                    mnuCtxPkg.Items.Add(new ToolStripSeparator()); 
                i++;
            }
            mnuCtxPkg.Items.Add(new ToolStripSeparator());
            tsMnuItem = new ToolStripMenuItem ("collapse all", null, mnuCtxCollapseAll_Click);
            mnuCtxPkg.Items.Add(tsMnuItem);
            tsMnuItem = new ToolStripMenuItem("expand all", null, mnuCtxExpandAll_Click);
            mnuCtxPkg.Items.Add(tsMnuItem);
            tsMnuItem = new ToolStripMenuItem("copy");
            mnuCtxPkg.Items.Add(tsMnuItem);

            tsMnuSubItem = new ToolStripMenuItem("Package Name", null, mnuCtxCpyPkgName_Click);
            tsMnuItem.DropDownItems.Add(tsMnuSubItem);
            tsMnuSubItem = new ToolStripMenuItem("Package Name + Path", null, mnuCtxCpyPkgPath_Click);
            tsMnuItem.DropDownItems.Add(tsMnuSubItem);
            tsMnuSubItem = new ToolStripMenuItem("Recursive Package Name", null, mnuCtxCpyPkgNameRec_Click);
            tsMnuItem.DropDownItems.Add(tsMnuSubItem);
            tsMnuSubItem = new ToolStripMenuItem("Recursive Package Name + Path", null, mnuCtxCpyPkgPathRec_Click);
            tsMnuItem.DropDownItems.Add(tsMnuSubItem);


            // Create the ContextMenuStrip for the requirements
            mnuCtxReq = new ContextMenuStrip();
            cb.GetReqCtxMenu(out astrMenuItems);
            i = 0;
            foreach (string str in astrMenuItems)
            {
                tsMnuItem = new ToolStripMenuItem(str, null, mnuCtxReq_Click);
                tsMnuItem.Tag = i++;
                mnuCtxReq.Items.Add(tsMnuItem);
            }
            mnuCtxReq.Items.Add(new ToolStripSeparator());
            tsMnuItem = new ToolStripMenuItem("collapse all others", null, mnuCtxCollapseAllOthers_Click);
            mnuCtxReq.Items.Add(tsMnuItem);


            // Assign the ImageList to the TreeView.
            ImageList = imageListTree;

            // Set the TreeView control's default image and selected image indexes.
            ImageIndex = 1;
            SelectedImageIndex = 1;
        }

        private void mnuCtxPkg_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mnuItem;
            mnuItem = (ToolStripMenuItem)sender;
            System.Collections.ArrayList arrReqKeys = new System.Collections.ArrayList();
            System.Collections.ArrayList arrOtherKeys = new System.Collections.ArrayList();
            GetNodesTagRecursive(ActiveNode, arrReqKeys, arrOtherKeys);
            if (cb.PkgMenuAction(arrReqKeys, arrOtherKeys, (int) ActiveNode.Tag, (int)mnuItem.Tag, mnuItem.Text, ActiveNode.FullPath))
                this.SelectedNode = ActiveNode;
        }

        private void mnuCtxReq_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mnuItem;
            mnuItem = (ToolStripMenuItem) sender;
            if (cb.ReqMenuAction((int)ActiveNode.Tag, (int)mnuItem.Tag, mnuItem.Text))
                this.SelectedNode = ActiveNode;
        }

        private void mnuCtxCollapseAll_Click(object sender, EventArgs e)
        {
            ActiveNode.Collapse(false);
        }

        private void mnuCtxExpandAll_Click(object sender, EventArgs e)
        {
            ActiveNode.ExpandAll();
        }

        private void mnuCtxCollapseAllOthers_Click(object sender, EventArgs e)
        {
            int nKey = (int) ActiveNode.Tag;
            foreach (TreeNode tn in Nodes)
                tn.Collapse(false);
            ShowNode(nKey);
        }

        private void mnuCtxCpyPkgName_Click(object sender, EventArgs e)
        {
            string str;
            str = System.Windows.Forms.Clipboard.GetText(TextDataFormat.Html);
            str = System.Windows.Forms.Clipboard.GetText(TextDataFormat.Text);
            str = System.Windows.Forms.Clipboard.GetText(TextDataFormat.CommaSeparatedValue);
            str = System.Windows.Forms.Clipboard.GetText(TextDataFormat.Rtf);
            CopyText(ActiveNode.Text);
        }

        private void mnuCtxCpyPkgPath_Click(object sender, EventArgs e)
        {
            CopyText(ActiveNode.FullPath);
        }

        private void mnuCtxCpyPkgNameRec_Click(object sender, EventArgs e)
        {
            string str = "";
            GetNodesNameRecursive(ActiveNode, ref str, false, 0);
            CopyText(str);
        }

        private void mnuCtxCpyPkgPathRec_Click(object sender, EventArgs e)
        {
            string str = "";
            GetNodesNameRecursive(ActiveNode, ref str, true, 0);
            CopyText(str);
        }

        private void CopyText(string str)
        {
            if (str != null)
                if (str.Length > 0)
                    System.Windows.Forms.Clipboard.SetText(str);
        }

        private static void GetNodesNameRecursive(TreeNode tnIn, ref string strOut, bool bWithPath, int nIndent)
        {
            if (tnIn.ImageIndex != 3)
            {
                /* it was a requirement otherwise */
                if (nIndent != 0)
                    strOut += "\r\n";
                if (bWithPath)
                    strOut += tnIn.FullPath;
                else
                {
                    for (int i = 0; i < nIndent; i++)
                        strOut += "\t";
                    strOut += tnIn.Text;
                }
                nIndent++;
                foreach (TreeNode tn in tnIn.Nodes)
                    GetNodesNameRecursive(tn, ref strOut, bWithPath, nIndent);
            }
        }

        private static void GetNodesTagRecursive(TreeNode tnIn, 
            System.Collections.ArrayList arrReqKeys, System.Collections.ArrayList arrOtherKeys)
        {
            if (tnIn.ImageIndex == 3)
                /* if TreeNode represents Requirement */
                arrReqKeys.Add((int)tnIn.Tag);
            else
                arrOtherKeys.Add((int)tnIn.Tag);

            foreach (TreeNode tn in tnIn.Nodes)
                GetNodesTagRecursive(tn, arrReqKeys, arrOtherKeys);
        }

        protected override void OnNodeMouseClick(System.Windows.Forms.TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right) {
                ActiveNode = e.Node;
            }
            base.OnNodeMouseClick(e);
        }

        protected override void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            base.OnBeforeExpand(e);
            if (e.Node.ImageIndex == -1)
            {
                e.Node.ImageIndex = 2;
                e.Node.SelectedImageIndex = 2;
            }
        }
        protected override void OnBeforeCollapse(TreeViewCancelEventArgs e)
        {
            base.OnBeforeCollapse(e);
            if (e.Node.ImageIndex == 2)
            {
                e.Node.ImageIndex = -1;
                e.Node.SelectedImageIndex = -1;
            }
        }

        public void Set(ReqTreeNode reqTreeNode, TreeNode tn)
        {
            tn.Tag = reqTreeNode.Key;
            if (reqTreeNode.IsReq())
            {
                tn.ImageIndex = 3;
                tn.SelectedImageIndex = 3;
                tn.ContextMenuStrip = mnuCtxReq;
            }
            else
                if (reqTreeNode.IsRoot())
                {
                    tn.ImageIndex = 0;
                    tn.SelectedImageIndex = 0;
                } else 
                    tn.ContextMenuStrip = mnuCtxPkg;
        }

        public void CreateTree(ReqTreeNode reqTreeNode)
        {
            int i;
            TreeNode tn = new TreeNode(reqTreeNode.Text);
            Set(reqTreeNode, tn);

            Nodes.Add(tn);
            for (i = 0; i < reqTreeNode.Count; i++)
                CreateTreeNode(ref tn, reqTreeNode[i]);
        }

        public void ReplaceActualTree(ReqTreeNode reqTreeNode)
        {
            Set(reqTreeNode, ActiveNode);
            ActiveNode.Text = reqTreeNode.Text;

            foreach (TreeNode tn in ActiveNode.Nodes)
                tn.Remove();
            ActiveNode.Nodes.Clear();
            for (int i = 0; i < reqTreeNode.Count; i++)
                CreateTreeNode(ref ActiveNode, reqTreeNode[i]);
        }

        private TreeNode CreateTreeNode(ref TreeNode tnParent, ReqTreeNode reqTreeNode)
        {
            int i;
            TreeNode tn;

            tn = new TreeNode(reqTreeNode.Text);
            Set(reqTreeNode, tn);
            tnParent.Nodes.Add(tn);

            for (i = 0; i < reqTreeNode.Count; i++)
            {
                CreateTreeNode(ref tn, reqTreeNode[i]);
            }
            return tn;
        }

        public int GetKeyOfSelected()
        {
            return ((int)SelectedNode.Tag);
        }

        private bool ShowNodeRecursiv (TreeNode tnIn, int nKey, bool bSelect)
        {
            bool bRet = false;
            foreach (TreeNode tn in tnIn.Nodes)
                bRet = bRet || ShowNodeRecursiv (tn, nKey, bSelect);
            if ((int)tnIn.Tag == nKey)
            {
                bRet = true;
                if (bSelect)
                {
                    tnIn.Expand ();
                    SelectedNode = tnIn;
                }
            }
            if (bRet)
                tnIn.Expand ();
            return bRet;
        }

        public bool ShowNode(int nKey)
        {
            Focus();
            return ShowNode(nKey, true);
        }

        private bool ShowNode(int nKey, bool bSelect)
        {
            bool bRet = false;
            foreach (TreeNode tn in Nodes)
                bRet = bRet || ShowNodeRecursiv(tn, nKey, bSelect);
            return bRet;
        }

        public bool ShowNodes(int[] narrKeys)
        {
            bool bRet = true;
            if (narrKeys != null)
                foreach (int nKey in narrKeys)
                    bRet = bRet && ShowNode(nKey, false);
            return bRet;
        }

        public void FindRequirements(string strSearchExpr, out ReqDBBrowser.ReqProProject.SearchResult [] searchResult, out int [] anKeysFound)
        {
            List<ReqDBBrowser.ReqProProject.SearchResult> listSearchResult = new List<ReqProProject.SearchResult>();
            List<int> listnKeys = new List<int>();
            string strSearch = strSearchExpr.ToLower ();

            foreach (TreeNode tn in Nodes)
                FindRequirements(tn, strSearch, ref listSearchResult, ref listnKeys);

            searchResult = listSearchResult.ToArray ();
            anKeysFound = listnKeys.ToArray ();
        }

        private void FindRequirements (TreeNode tn, string strSearchExpr, 
            ref List<ReqDBBrowser.ReqProProject.SearchResult> listSearchResult, ref List<int> listnKeysFound)
        {
            int nIdx = -1;
            string strComp = tn.Text.ToLower();

            nIdx = strComp.IndexOf(strSearchExpr);
            if (nIdx >= 0)
            {
                ReqProProject.SearchResult sResult = new ReqProProject.SearchResult(tn.Text);
                sResult.Index = nIdx;
                sResult.Length = strSearchExpr.Length;
                listSearchResult.Add(sResult);
                listnKeysFound.Add((int)tn.Tag);
            }
            foreach (TreeNode tnChild in tn.Nodes)
                FindRequirements(tnChild, strSearchExpr, ref listSearchResult, ref listnKeysFound);
        }
    }
}
