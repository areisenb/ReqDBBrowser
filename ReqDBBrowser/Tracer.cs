using System;
using System.Collections.Generic;
using System.Text;

namespace ReqDBBrowser
{
    class Tracer
    {
        long lStart;
        string strPreDescription;
        bool bStopped;

        public Tracer(string strPreDescription)
        {
            lStart = DateTime.Now.Ticks;
            this.strPreDescription = strPreDescription;
            bStopped = false;
        }

        ~Tracer()
        {
            if (!bStopped)
                Stop("WARNING: Did not Stop - " + strPreDescription);
        }

        public void Stop (string strDescription)
        {
            bStopped = true;
            long lDuration = (DateTime.Now.Ticks - lStart) / 10L;
            long lSecs = lDuration / 1000000L;
            long lmSecs = lDuration % 1000000L;
            long luSecs = lmSecs % 1000L;
            lmSecs /= 1000L;
            System.Diagnostics.Trace.WriteLine (
                String.Format ("{0:T} lasts {1},{2:000} {3:000}s ", DateTime.Now, lSecs, lmSecs, luSecs) + 
                strDescription);

        }
    }
}
