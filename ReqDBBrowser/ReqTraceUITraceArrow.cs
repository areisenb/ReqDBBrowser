using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ReqDBBrowser
{
    class ReqTraceUITraceArrow: ReqTraceUI
    {
        int nFromX;
        int nFromY;
        int nToX;
        int nToY;

        const int nDotRadius = 0;

        public ReqTraceUITraceArrow(int nFromX, int nFromY, int nToX, int nToY)
        {
            this.nFromX = nFromX;
            this.nFromY = nFromY;
            this.nToX = nToX;
            this.nToY = nToY;
        }

        public override void Draw(Graphics gr, Point pOffset)
        {
            AdjustableArrowCap bigArrow = new AdjustableArrowCap(3, 5);
            Pen pen = new Pen(Color.Blue);
            SolidBrush blueBrush = new SolidBrush(Color.Blue);
            pen.StartCap = LineCap.RoundAnchor;

            pen.CustomEndCap = bigArrow;


            gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            gr.DrawLine(pen, nFromX + pOffset.X, nFromY + nDotRadius + pOffset.Y, 
                             nToX + pOffset.X, nToY + pOffset.Y);

            blueBrush.Dispose();
            pen.Dispose();
        }
    }
}
