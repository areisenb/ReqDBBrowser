using System;
using System.Collections.Generic;
using System.Text;

namespace ReqDBBrowser
{
    class ReqTraceUIRendererNode
    {
        protected int nXPos;
        protected int nYPos;

        public interface ITraceViewGraphCb
        {
            void GetTBTagCtxMenu(out string[] astrMnuEntry);
            void GetTBNameCtxMenu(out string[] astrMnuEntry);
            void TBMenuAction(int nActKey, int[] nSelKeys, int[] nMarkedKeys, int nMenuItem, bool bWasTBName, string strMenuText);
        }

        public ReqTraceUIRendererNode(int nYOffset,
            ReqTraceNode reqTraceNode)
        {
            nXPos = reqTraceNode.X;
            nYPos = nYOffset - reqTraceNode.Y;
        }

    }
}
