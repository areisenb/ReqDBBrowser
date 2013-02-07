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
            bool PkgMenuAction(ArrayList arrReqKeys, ArrayList arrOtherKeys, 
                int nMenuItem, string strMenuText);
        }

        public TreeViewReq(ITreeViewReqCb cb, ImageList imageListTree)
        {
            string [] astrMenuItems;
            ToolStripMenuItem tsMnuItem;
            int i;

            this.cb = cb;
            this.ActiveNode = null;

            // Create the ContextMenuStrip.
            mnuCtxPkg = new ContextMenuStrip();
            cb.GetPkgCtxMenu (out astrMenuItems);
            i = 0;
            foreach (string str in astrMenuItems)
            {
                tsMnuItem = new ToolStripMenuItem(str, null, mnuCtxPkg_Click);
                tsMnuItem.Tag = i++;
                mnuCtxPkg.Items.Add(tsMnuItem);
            }
            mnuCtxPkg.Items.Add(new ToolStripSeparator());
            tsMnuItem = new ToolStripMenuItem ("collapse all", null, mnuCtxCollapseAll_Click);
            mnuCtxPkg.Items.Add(tsMnuItem);
            tsMnuItem = new ToolStripMenuItem("expand all", null, mnuCtxExpandAll_Click);
            mnuCtxPkg.Items.Add(tsMnuItem);

            mnuCtxReq = new ContextMenuStrip();
            cb.GetReqCtxMenu(out astrMenuItems);
            i = 0;
            foreach (string str in astrMenuItems)
            {
                tsMnuItem = new ToolStripMenuItem(str, null, mnuCtxReq_Click);
                tsMnuItem.Tag = i++;
                mnuCtxReq.Items.Add(tsMnuItem);
            }


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
            if (cb.PkgMenuAction(arrReqKeys, arrOtherKeys, (int)mnuItem.Tag, mnuItem.Text))
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

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);
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

        public void CreateTree(ReqTreeNode reqTreeNode)
        {
            int i;
            TreeNode tn = new TreeNode(reqTreeNode.Text);
            tn.Tag = reqTreeNode.Key;
            tn.ImageIndex = 0;
            tn.SelectedImageIndex = 0;

            Nodes.Add(tn);
            for (i = 0; i < reqTreeNode.Count; i++)
            {
                CreateTreeNode(ref tn, reqTreeNode[i]);
            }
        }

        public void CreateTreeNode(ref TreeNode tnParent, ReqTreeNode reqTreeNode)
        {
            int i;
            TreeNode tn;
            tn = new TreeNode(reqTreeNode.Text);
            tn.Tag = reqTreeNode.Key;
            if (reqTreeNode.IsReq())
            {
                tn.ImageIndex = 3;
                tn.SelectedImageIndex = 3;
                tn.ContextMenuStrip = mnuCtxReq;
            }
            else
            {
                tn.ContextMenuStrip = mnuCtxPkg;
            }

            tnParent.Nodes.Add(tn);

            for (i = 0; i < reqTreeNode.Count; i++)
            {
                CreateTreeNode(ref tn, reqTreeNode[i]);
            }
        }

        public int GetKeyOfSelected()
        {
            return ((int)SelectedNode.Tag);
        }

    }
}
