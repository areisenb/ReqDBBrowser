using System;
using System.Collections.Generic;
using System.Text;

namespace ReqDBBrowser
{
    class ReqTreeNode
    {
        public enum eReqTreeNodeType
        {
            eTreeNodeRoot,
            eTreeNodePkg,
            eTreeNodeReq,
            eTreeNodeView
        };

        private System.Collections.ArrayList aReqTreeNodeChilds;
        private string strText;
        private int nKey;
        private int nKeyParent; //do not rely on proper populated member!
        private eReqTreeNodeType reqTreeNodeType;

        public ReqTreeNode (string strText, int nKey, eReqTreeNodeType reqTreeNodeType)
        {
            this.strText = strText;
            this.nKey = nKey;
            this.reqTreeNodeType = reqTreeNodeType;
            aReqTreeNodeChilds = new System.Collections.ArrayList ();
            nKeyParent = -1;
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

        public int Parent
        {
            get { return nKeyParent; }
            set { nKeyParent = value; }
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

        public bool IsReq()
        {
            return (reqTreeNodeType == eReqTreeNodeType.eTreeNodeReq);
        }

        public bool IsRoot()
        {
            return (reqTreeNodeType == eReqTreeNodeType.eTreeNodeRoot);
        }

        public bool IsPackage()
        {
            return (reqTreeNodeType == eReqTreeNodeType.eTreeNodePkg);
        }
    }
}
