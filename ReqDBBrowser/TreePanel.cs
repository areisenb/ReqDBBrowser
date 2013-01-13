using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ReqDBBrowser
{
    class TreePanel
    {
        TreeView treeView;

        public TreePanel(ref TreeView treeView)
        {
            this.treeView = treeView;
        }

        public void CreateTree (ReqTreeNode reqTreeNode)
        {
            int i;
            TreeNode tn = new TreeNode (reqTreeNode.Text);

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
            tnParent.Nodes.Add(tn);

            for (i = 0; i < reqTreeNode.Count; i++)
            {
                CreateTreeNode(ref tn, reqTreeNode[i]);
            }
        }
    }
}
