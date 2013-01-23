using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ReqDBBrowser
{
    class TreePanel
    {
        TreeViewReq treeView;

        public TreePanel(ref TreeViewReq treeView)
        {
            this.treeView = treeView;
        }

        public void CreateTree (ReqTreeNode reqTreeNode)
        {
            int i;
            TreeNode tn = new TreeNode (reqTreeNode.Text);
            tn.Tag = reqTreeNode.Key;

            treeView.Nodes.Add(tn);
            for (i = 0; i < reqTreeNode.Count; i++ )
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
            tnParent.Nodes.Add(tn);

            for (i = 0; i < reqTreeNode.Count; i++)
            {
                CreateTreeNode(ref tn, reqTreeNode[i]);
            }
        }
    }
}
