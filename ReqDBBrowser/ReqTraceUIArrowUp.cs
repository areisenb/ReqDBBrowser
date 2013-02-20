using System;
using System.Collections.Generic;
using System.Text;

namespace ReqDBBrowser
{
    class ReqTraceUIArrowUp: ReqTraceUIArrow
    {
        public ReqTraceUIArrowUp(int nTraces, bool bAdditionalTraces, int nX, int nY)
            : base(nTraces, bAdditionalTraces, nX, nY, false)
        {
        }
    }
}
