using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ReqDBBrowser
{
    abstract class ReqTraceUI
    {
        public abstract void Draw(Graphics gr, Point pOffset);
    }
}
