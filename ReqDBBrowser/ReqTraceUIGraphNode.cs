using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace ReqDBBrowser
{
    class ReqTraceUIGraphNode: ReqTraceUIRendererNode
    {
        static Size sizeText = new Size(200, 100);
        static Size sizeTagName = new Size(200, 20);
        const int nXSpacing = 250;
        const int nYSpacing = 200;

        static ContextMenuStrip mnuCtxTag;
        static ContextMenuStrip mnuCtxName;
        static ITraceViewGraphCb cb;
        static Control.ControlCollection ctrls;
        static ReqTraceUIGraphNode actNode;

        List<ReqTraceUI> arrFGUI;
        List<ReqTraceUI> arrBGUI;
        ReqTraceNode reqTraceNode;
        TextBox tbReqTag;
        TextBox tbReqText;

        public static void Init (Control.ControlCollection ctrls, ITraceViewGraphCb cb)
        {
            int nMnuItem;
            ReqTraceUIGraphNode.cb = cb;
            ReqTraceUIGraphNode.ctrls = ctrls;
            string[] astrMnuEntry;
            ToolStripMenuItem tsMnuItem;

            // Create the ContextMenuStrip for TagEditControl
            mnuCtxTag = new ContextMenuStrip ();
            cb.GetTBTagCtxMenu(out astrMnuEntry);
            nMnuItem = 0;
            foreach (string str in astrMnuEntry)
            {
                tsMnuItem = new ToolStripMenuItem(str, null, mnuCtxTag_Click);
                tsMnuItem.Tag = nMnuItem++;
                mnuCtxTag.Items.Add(tsMnuItem);
            }
            mnuCtxTag.Items.Add(new ToolStripSeparator());
            mnuCtxTag.Items.Add(new ToolStripMenuItem("Copy Req Tag", null, mnuCtxCpReqTag_Click));
            mnuCtxTag.Items.Add(new ToolStripMenuItem("Copy Req Name", null, mnuCtxCpReqName_Click));
            mnuCtxTag.Items.Add(new ToolStripMenuItem("Copy Req Tag + Req Name", null, mnuCtxCpReqTagName_Click));
            mnuCtxTag.Items.Add(new ToolStripMenuItem("Copy", null, mnuCtxCpTag_Click));

            // Create the ContextMenuStrip for TagEditControl
            mnuCtxName = new ContextMenuStrip();
            cb.GetTBNameCtxMenu(out astrMnuEntry);
            nMnuItem = 0;
            foreach (string str in astrMnuEntry)
            {
                tsMnuItem = new ToolStripMenuItem(str, null, mnuCtxText_Click);
                tsMnuItem.Tag = nMnuItem++;
                mnuCtxName.Items.Add(tsMnuItem);
            }
            mnuCtxName.Items.Add(new ToolStripSeparator());
            mnuCtxName.Items.Add(new ToolStripMenuItem("Copy Req Text", null, mnuCtxCpReqText_Click));
            mnuCtxName.Items.Add(new ToolStripMenuItem("Copy Req Trace To", null, mnuCtxCpReqTrcTo_Click));
            mnuCtxName.Items.Add(new ToolStripMenuItem("Copy Req Trace From", null, mnuCtxCpReqTrcFrom_Click));
            mnuCtxName.Items.Add(new ToolStripMenuItem("Copy", null, mnuCtxCpName_Click));
        }

        private static void mnuCtxTag_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mnuItem;
            mnuItem = (ToolStripMenuItem)sender;
            cb.TBMenuAction(actNode.reqTraceNode.Key, null, null, (int)mnuItem.Tag, false, mnuItem.Text);
        }

        private static void mnuCtxText_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mnuItem;
            mnuItem = (ToolStripMenuItem)sender;
            cb.TBMenuAction(actNode.reqTraceNode.Key, null, null, (int)mnuItem.Tag, true, mnuItem.Text);
        }

        private static void mnuCtxCpReqTag_Click(object sender, EventArgs e)
        {
            actNode.CopyReqTag();
        }

        private static void mnuCtxCpReqName_Click(object sender, EventArgs e)
        {
            actNode.CopyReqName();
        }

        private static void mnuCtxCpReqTagName_Click(object sender, EventArgs e)
        {
            actNode.CopyReqTagName();
        }

        private static void mnuCtxCpTag_Click(object sender, EventArgs e)
        {
            actNode.CopyFromTag();
        }

        private static void mnuCtxCpReqText_Click(object sender, EventArgs e)
        {
            actNode.CopyReqText();
        }

        private static void mnuCtxCpReqTrcTo_Click(object sender, EventArgs e)
        {
            actNode.CopyReqTraceTo();
        }

        private static void mnuCtxCpReqTrcFrom_Click(object sender, EventArgs e)
        {
            actNode.CopyReqTraceFrom();
        }

        private static void mnuCtxCpName_Click(object sender, EventArgs e)
        {
            actNode.CopyFromName();
        }

        public ReqTraceUIGraphNode(int nYOffset,
            ReqTraceNode reqTraceNode): base (nYOffset, reqTraceNode)
        {
            TextBox tb;
            ToolTip tt;
            int nTraces;
            bool bAdditionalTraces;
            int[] nTraceToX;
            int[] nTraceToY;

            this.reqTraceNode = reqTraceNode;

            tb = new TextBox();
            tb.Location = new Point(nXPos * nXSpacing + nXSpacing / 2, nYPos * nYSpacing + nYSpacing / 2);
            tb.Size = sizeTagName;
            tb.Multiline = true;
            tb.ReadOnly = true;
            tb.Text = reqTraceNode.TagName;
            if (reqTraceNode.IsRootNode)
                tb.BackColor = Color.FromArgb(200, 255, 200);
            tt = new ToolTip();
            tt.BackColor = Color.Yellow;
            tt.SetToolTip(tb, reqTraceNode.TagName);
            tb.ContextMenuStrip = mnuCtxTag;
            tb.MouseEnter += tb_MouseEnter;
            ctrls.Add(tb);
            tbReqTag = tb;

            tb = new TextBox();
            tb.Location = new Point(nXPos * nXSpacing + nXSpacing / 2, nYPos * nYSpacing + sizeTagName.Height + nYSpacing / 2);
            tb.Size = sizeText;
            tb.Multiline = true;
            tb.ReadOnly = true;
            tb.ScrollBars = ScrollBars.Vertical;
            tb.Text = reqTraceNode.Text;
            tb.ContextMenuStrip = mnuCtxName;
            tb.MouseEnter += tb_MouseEnter;
            ctrls.Add(tb);
            tbReqText = tb;

            arrFGUI = new List<ReqTraceUI>();
            arrBGUI = new List<ReqTraceUI>();

            if (reqTraceNode.AreTooManyTracesFrom(out nTraces, out bAdditionalTraces))
                arrBGUI.Add(new ReqTraceUIArrowDwn(nTraces, bAdditionalTraces,
                    nXPos * nXSpacing + nXSpacing / 2 + sizeTagName.Width / 2,
                    nYPos * nYSpacing + nYSpacing / 2));

            if (reqTraceNode.AreTooManyTracesTo(out nTraces, out bAdditionalTraces))
                arrBGUI.Add(new ReqTraceUIArrowUp(nTraces, bAdditionalTraces,
                    nXPos * nXSpacing + nXSpacing / 2 + sizeTagName.Width / 2,
                    nYPos * nYSpacing + sizeTagName.Height + sizeText.Height + nYSpacing / 2));

            reqTraceNode.GetTraceToCoord(out nTraceToX, out nTraceToY);

            for (int i = nTraceToX.GetLength(0) - 1; i >= 0; i--)
                if ((nTraceToX[i] != int.MinValue) && (reqTraceNode.X != int.MinValue))
                    arrFGUI.Add(new ReqTraceUITraceArrow(
                        nXPos * nXSpacing + sizeText.Width / 2 + nXSpacing / 2,
                        nYPos * nYSpacing + sizeTagName.Height + sizeText.Height + nYSpacing / 2,
                        nTraceToX[i] * nXSpacing + sizeText.Width / 2 + nXSpacing / 2,
                        (nYOffset - nTraceToY[i]) * nYSpacing + nYSpacing / 2));
        }

        private static void Draw(Graphics graphics, Point pScrollPosition, List<ReqTraceUI> arrUI)
        {
            foreach (ReqTraceUI reqTraceUI in arrUI)
                reqTraceUI.Draw(graphics, pScrollPosition);
        }

        public void DrawBackGround(Graphics graphics, Point pScrollPosition)
        {
            Draw(graphics, pScrollPosition, arrBGUI);
        }

        public void DrawForeGround(Graphics graphics, Point pScrollPosition)
        {
            Draw(graphics, pScrollPosition, arrFGUI);
        }

        private void CopyText (string str)
        {
            if (str != null)
                if (str.Length > 0)
                    System.Windows.Forms.Clipboard.SetText(str);
            //MessageBox.Show ("Copy to Clipboard:\n"+str, "Please help yourself");
        }

        private void CopyReqTag ()
        {
            CopyText (reqTraceNode.Tag);
        }

        private void CopyReqName()
        {
            CopyText (reqTraceNode.Name);
        }

        private void CopyReqTagName()
        {
            CopyText (reqTraceNode.TagName);
        }

        private void CopyFromTag()
        {
            tbReqTag.Copy();
        }

        private void CopyReqText ()
        {
            CopyText (reqTraceNode.Text);
        }

        private void CopyReqTraceTo ()
        {
            List<string> lString;
            string[] arrString;
            reqTraceNode.GetTraceToString (out lString);
            arrString = lString.ToArray();
            CopyText (string.Join ("\n", arrString));
        }

        private void CopyReqTraceFrom ()
        {
            List<string> lString;
            string[] arrString;
            reqTraceNode.GetTraceFromString (out lString);
            arrString = lString.ToArray();
            CopyText (string.Join ("\n", arrString));
        }

        private void CopyFromName()
        {
            tbReqText.Copy();
        }

        void tb_MouseEnter(object sender, EventArgs e)
        {
            actNode = this;
        }

    }
}
