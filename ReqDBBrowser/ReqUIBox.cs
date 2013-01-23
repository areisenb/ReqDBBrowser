using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;


namespace ReqDBBrowser
{
    class ReqUIBox
    {
        string strReqName;
        string strReqText;
        Point pOrigin;
        int nKey;
        Font font;
        const int nRectWidth = 200;
        const int nRectHight = 100;

        public ReqUIBox(int nKey, string strReqName, string strReqText, Point pOrigin, Font font)
        {
            this.strReqName = strReqName;
            this.strReqText = strReqText;
            this.pOrigin = pOrigin;
            this.font = font;
        }

        public void Paint(Graphics graphics)
        {
            Pen pen = new System.Drawing.Pen(System.Drawing.Color.Black);
            Point pBorder = new Point (pOrigin.X, pOrigin.Y+nRectHight/3);

            graphics.DrawRectangle(pen, new Rectangle(pOrigin, new Size (nRectWidth, nRectHight)));
            // graphics.DrawString(strReqName, font, Brushes.Black, pOrigin.X, pOrigin.Y);
            // graphics.DrawLine(pen, pBorder, pBorder + new Size(nRectWidth, 0));
            graphics.DrawString(strReqText, font, Brushes.Black, pOrigin.X, pOrigin.Y+2*nRectWidth/3);
            

            pen.Dispose();

        }

    }
}
