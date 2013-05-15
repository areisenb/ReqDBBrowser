using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ReqDBBrowser
{
    class ReqTraceDotNode: ReqTraceUIRendererNode
    {
        string strDOT;

        public ReqTraceDotNode(int nYOffset,
            ReqTraceNode reqTraceNode): base (nYOffset, reqTraceNode)
        {
            int[] nTraceToX;
            int[] nTraceToY;
            string strCurrent;
            int nTraces;
            bool bAdditionalTraces;
            string strAttribute = "";
            strDOT = "";

            strCurrent = CoordToString (nXPos, nYPos);

            if (reqTraceNode.IsRootNode)
                strAttribute = "style=filled, fillcolor=\"#C8FFC8\", ";

            strDOT = "  " + strCurrent + " [shape = record, " + strAttribute + "label=\"{" +
                DotFormat(reqTraceNode.TagName.Replace("\"", "\\\""), 2) +
                "|" +
                DotFormat(reqTraceNode.Text.Replace("\"", "\\\""), 8) + "}\"];\r\n";

            if (reqTraceNode.AreTooManyTracesTo(out nTraces, out bAdditionalTraces))
            {
                strDOT += "  " + strCurrent +
                    "_To [shape=none, label=\"" +
                    (bAdditionalTraces ? "+" : "") + nTraces.ToString() + " REQs\"];\r\n";
                strDOT += "  " + strCurrent + " -> " + strCurrent + "_To;\r\n";
            }

            if (reqTraceNode.AreTooManyTracesFrom(out nTraces, out bAdditionalTraces))
            {
                strDOT += "  " + strCurrent +
                    "_From [shape=none, label=\"" +
                    (bAdditionalTraces ? "+" : "") + nTraces.ToString() + " REQs\"];\r\n";
                strDOT += "  " + strCurrent + "_From -> " + strCurrent + ";\r\n";
            }

            reqTraceNode.GetTraceToCoord(out nTraceToX, out nTraceToY);

            for (int i = nTraceToX.GetLength(0) - 1; i >= 0; i--)
                if ((nTraceToX[i] != int.MinValue) && (reqTraceNode.X != int.MinValue))
                    strDOT += "  " + strCurrent + " -> " + CoordToString(nTraceToX[i], nYOffset - nTraceToY[i]) + ";\r\n";
        }

        public string Convert()
        {
            return strDOT;
        }

        static string CoordToString(int nX, int nY)
        {
            return string.Format("node{0:00}_{1:00}", nX, nY);
        }

        static string DotFormat(string strIn, int nMaxLines)
        {
            string strOut="";
            string strTemp;
            int nIdxBreak;
            bool bTrunkated = false;

            while ((strIn.Length > 40) && (nMaxLines>0))
            {
                strTemp = strIn.Substring(0, 40);
                nIdxBreak =  strTemp.LastIndexOf(' ');
                if (nIdxBreak <= 3)
                    nIdxBreak = 40;
                if (--nMaxLines > 0)
                    strOut += strIn.Substring(0, nIdxBreak) + "\\l";
                else
                {
                    strOut += strIn.Substring(0, nIdxBreak - 3) + "...\\l";
                    bTrunkated = true;
                }
                strIn = strIn.Substring(nIdxBreak);
            }

            if (!bTrunkated)
                strOut += strIn+"\\l";
            strTemp = strOut.Replace("{", "\\{").Replace("}", "\\}");
            strTemp = strTemp.Replace("\n", "\\l");

            return (strTemp.Replace ("<", "\\<").Replace(">","\\>"));
        }

    }
}
