using System;
using System.Collections.Generic;
using System.Text;

namespace ReqDBBrowser
{
    class ReqTreeNode
    {
        private System.Collections.ArrayList aReqTreeNodeChilds;
        private string strText;
        private int nKey;

        public ReqTreeNode (string strText, int nKey)
        {
            this.strText = strText;
            this.nKey = nKey;
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

        public int Key
        {
            get { return this.nKey; }
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
