using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ReqDBBrowser
{
    class TreeViewReq: TreeView
    {
        NodeSelectedCallback nodeSelectedCallback;

        public TreeViewReq(NodeSelectedCallback nodeSelectedCallback)
        {
            this.nodeSelectedCallback = nodeSelectedCallback;
        }

        public delegate void NodeSelectedCallback(int nKey, bool doubleClick, MouseButtons mButton);

        protected override void OnNodeMouseClick(System.Windows.Forms.TreeNodeMouseClickEventArgs e)
        {
            base.OnNodeMouseClick(e);
            if (e.Button == MouseButtons.Right) {
                nodeSelectedCallback((int)e.Node.Tag, false, MouseButtons.Right);
                //MessageBox.Show ("Requirement: " + e.Node.Text + " selected left mouse");
            }
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);
            nodeSelectedCallback((int)e.Node.Tag, false, MouseButtons.Right);
        }
    }
}
