using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ReqDBBrowser
{
    class ReqTraceUIArrowDwn: ReqTraceUIArrow
    {

        public ReqTraceUIArrowDwn(int nTraces, bool bAdditionalTraces, int nX, int nY): 
            base (nTraces, bAdditionalTraces, nX, nY, true)
        {
        }
    }
}
