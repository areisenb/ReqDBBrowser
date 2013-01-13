using System;
using System.Collections.Generic;
using System.Text;

namespace ReqDBBrowser
{
    class ReqTreeNode
    {
        private System.Collections.ArrayList aReqTreeNodeChilds;
        private string strText;

        public ReqTreeNode (string strText)
        {
            this.strText = strText;
            aReqTreeNodeChilds = new System.Collections.ArrayList ();
        }

        public void Add (ref ReqTreeNode  child)
        {
            aReqTreeNodeChilds.Add (child);
        }

        public string Text
        {
            get { return strText; }
        }

        public int Count
        {
            get { return aReqTreeNodeChilds.Count; }
        }

        public ReqTreeNode this [int idx]
        {
            get
            {
                if ((idx > -1 ) && (idx < Count))
                    return ((ReqTreeNode)aReqTreeNodeChilds[idx]);
                else
                    return null;
            }
        }

    }
}
