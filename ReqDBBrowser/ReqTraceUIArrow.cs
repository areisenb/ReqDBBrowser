using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ReqDBBrowser
{
    class ReqTraceUIArrow : ReqTraceUI
    {
        const int nWidth = 30;
        const int nLength = 30;
        int nTraces;
        bool bAdditionalTraces;
        Point [] arrPoint;

        public ReqTraceUIArrow(int nTraces, bool bAdditionalTraces, int nX, int nY, bool bIsTip)
        {
            this.nTraces = nTraces;
            this.bAdditionalTraces = bAdditionalTraces;
            arrPoint = new Point[7];

            if (bIsTip)
                nY -= nLength;

            arrPoint[0] = new Point(nX - nWidth / 3, nY);
            arrPoint[1] = new Point(nX + nWidth / 3, nY);
            arrPoint[2] = new Point(nX + nWidth / 3, nY + 2 * nLength / 3);
            arrPoint[3] = new Point(nX + nWidth / 2, nY + 2 * nLength / 3);
            arrPoint[4] = new Point(nX, nY + nLength);
            arrPoint[5] = new Point(nX - nWidth / 2, nY + 2 * nLength / 3);
            arrPoint[6] = new Point(nX - nWidth / 3, nY + 2 * nLength / 3);
        }

        public override void Draw(Graphics gr, Point pOffset)
        {
            string strCaption;
            gr.SmoothingMode = SmoothingMode.HighQuality;
            SolidBrush brush = new SolidBrush(Color.LightBlue);
            SolidBrush brText = new SolidBrush(Color.Black);
            FillMode fm = FillMode.Winding;
            Point[] arrDrwPoint = new Point[arrPoint.GetLength(0)];

            for (int i=0; i<arrDrwPoint.GetLength(0); i++)
                arrDrwPoint[i] = 
                    new Point (arrPoint[i].X + pOffset.X, arrPoint[i].Y + pOffset.Y);

            gr.FillPolygon(brush, arrDrwPoint, fm);

            strCaption = nTraces.ToString() + " REQs";
            if (bAdditionalTraces)
                strCaption = "+" + strCaption;
            gr.DrawString(strCaption, System.Drawing.SystemFonts.DialogFont, brText, arrDrwPoint[1]);

            brText.Dispose();
            brush.Dispose();
        }

    }
}
