using System;
using System.Collections.Generic;
using System.Text;

namespace ReqDBBrowser
{
    class ReqTraceNode : ReqProRequirementPrx
    {
        ReqProRequirementPrx[] aReqProReqPrxTracesTo;
        ReqProRequirementPrx[] aReqProReqPrxTracesFrom;

        int nTraceFromCount;
        int nTraceToCount;
        ulong ulDegreeRel;
        bool bIsRootNode;
        int nTraceFromHopCount;
        int nTraceToHopCount;

        eTraceAbortReason eAbort;

        int x;
        int y;
        System.Collections.Generic.Dictionary<int, ReqTraceNode> dictReqKey;

        public static int CompareReqTraceNodebyRelationDegree(ReqTraceNode x, ReqTraceNode y)
        {
            if (x == null)
            {
                if (y == null)
                    // If x is null and y is null, they're equal. 
                    return 0;
                else
                    // If x is null and y is not null, x is greater. 
                    return 1;
            }
            else
            {
                // If x is not null...
                if (y == null)
                    // ...and y is null, y is greater.
                    return -1;
                else
                {
                    // ...and y is not null, compare the lengths of the two strings.
                    if (x.ulDegreeRel == y.ulDegreeRel)
                        return 0;
                    else
                        if (x.ulDegreeRel > y.ulDegreeRel)
                            return 1;
                        else
                            return -1;
                }
            }
        }

        public ReqTraceNode(ReqProRequirementPrx reqProReqPrx, ulong ulDegreeRel, bool bIsRootNode,
            ReqProRequirementPrx[] aReqProReqPrxTracesFrom, ReqProRequirementPrx[] aReqProReqPrxTracesTo,
            int nTraceFromCount, int nTraceToCount, eTraceAbortReason eAbort, int nTraceFromHopCount, int nTraceToHopCount)
            : base(reqProReqPrx)
        {
            x = int.MinValue;
            y = int.MinValue;
            this.nTraceFromHopCount = int.MaxValue;
            this.nTraceToHopCount = int.MaxValue;

            OnceAgain(ulDegreeRel, bIsRootNode, aReqProReqPrxTracesFrom, aReqProReqPrxTracesTo,
                nTraceFromCount, nTraceToCount, eAbort, nTraceFromHopCount, nTraceToHopCount);
        }

        public void OnceAgain(ulong ulDegreeRel, bool bIsRootNode,
            ReqProRequirementPrx[] aReqProReqPrxTracesFrom, ReqProRequirementPrx[] aReqProReqPrxTracesTo,
            int nTraceFromCount, int nTraceToCount, eTraceAbortReason eAbort, int nTraceFromHopCount, int nTraceToHopCount)
        {
            this.bIsRootNode = bIsRootNode;
            this.ulDegreeRel = ulDegreeRel;
            this.aReqProReqPrxTracesFrom = aReqProReqPrxTracesFrom;
            this.aReqProReqPrxTracesTo = aReqProReqPrxTracesTo;
            this.nTraceFromCount = nTraceFromCount;
            this.nTraceToCount = nTraceToCount;
            this.eAbort = eAbort;

            if (nTraceFromHopCount < this.nTraceFromHopCount)
                this.nTraceFromHopCount = nTraceFromHopCount;
            if (nTraceToHopCount < this.nTraceToHopCount)
                this.nTraceToHopCount = nTraceToHopCount;
        }

        public eTraceAbortReason AbortReason
        {
            set { this.eAbort = value; }
            get { return this.eAbort; }
        }

        public void SetRelDegree(ulong ulDegreeRel)
        {
            if (ulDegreeRel < this.ulDegreeRel)
                this.ulDegreeRel = ulDegreeRel;
        }

        public void AddTraceTo(ReqProRequirementPrx reqReqPrxTraceTo)
        {
            int nTraceToCountEnumerated = aReqProReqPrxTracesTo.GetLength(0);
            if (nTraceToCount != nTraceToCountEnumerated)
            {
                /* just if we have already satured the Traces to - the number already contains all traces */
                ReqProRequirementPrx[] aReqProReqPrxTracesToOld;
                aReqProReqPrxTracesToOld = aReqProReqPrxTracesTo;

                aReqProReqPrxTracesTo = new ReqProRequirementPrx[nTraceToCountEnumerated + 1];
                for (int i = 0; i < nTraceToCountEnumerated; i++)
                    aReqProReqPrxTracesTo[i] = aReqProReqPrxTracesToOld[i];
                aReqProReqPrxTracesTo[nTraceToCountEnumerated] = reqReqPrxTraceTo;
            }
        }

        public void AddTraceFrom(ReqProRequirementPrx reqReqPrxTraceFrom)
        {
            int nTraceFromCountEnumerated = aReqProReqPrxTracesFrom.GetLength(0);
            if (nTraceFromCount != nTraceFromCountEnumerated)
            {
                /* just if we have already satured the Traces to - the number already contains all traces */
                ReqProRequirementPrx[] aReqProReqPrxTracesFromOld;
                aReqProReqPrxTracesFromOld = aReqProReqPrxTracesFrom;

                aReqProReqPrxTracesFrom = new ReqProRequirementPrx[nTraceFromCountEnumerated + 1];
                for (int i = 0; i < nTraceFromCountEnumerated; i++)
                    aReqProReqPrxTracesFrom[i] = aReqProReqPrxTracesFromOld[i];
                aReqProReqPrxTracesFrom[nTraceFromCountEnumerated] = reqReqPrxTraceFrom;
            }
        }

        public void GetTraceToString(out List<string> arrString)
        {
            GetTraceString(aReqProReqPrxTracesTo, nTraceToCount, out arrString);
            if ((eAbort & eTraceAbortReason.eMaxToLevelReached) != 0)
                arrString.Add("<Level Reached>");
            if ((eAbort & eTraceAbortReason.eMaxToHopsExceeded) != 0)
                arrString.Add("<Hops Exceeded>");
        }

        public void GetTraceFromString(out List<string> arrString)
        {
            GetTraceString(aReqProReqPrxTracesFrom, nTraceFromCount, out arrString);
            if ((eAbort & eTraceAbortReason.eMaxFromLevelReached) != 0)
                arrString.Add("<Level Reached>");
            if ((eAbort & eTraceAbortReason.eMaxFromHopsExceeded) != 0)
                arrString.Add("<Hops Exceeded>");
        }

        private static void GetTraceString(ReqProRequirementPrx[] aReqProReqPrxTraces,
            int nTraceCount, out List<string> arrString)
        {
            arrString = new List<string>();

            foreach (ReqProRequirementPrx reqReqPrx in aReqProReqPrxTraces)
                arrString.Add(reqReqPrx.Tag);
            if (aReqProReqPrxTraces.GetLength(0) != nTraceCount)
                if (aReqProReqPrxTraces.GetLength(0) == 0)
                    arrString.Add("(" + nTraceCount + " REQs)");
                else
                    arrString.Add("( +" + (nTraceCount - aReqProReqPrxTraces.GetLength(0)) + " REQs)");
        }

        public void SetCoordinates(int x, int y, Dictionary<int, ReqTraceNode> dict)
        {
            this.dictReqKey = dict;
            this.x = x;
            this.y = y;
        }

        public int X
        { get { return x; } }
        public int Y
        { get { return y; } }

        public bool IsRootNode
        { get { return bIsRootNode; } }

        public void MakeRootNode()
        {
            bIsRootNode = true;
        }

        public int TraceFromHopCount
        { get { return nTraceFromHopCount; } }
        public int TraceToHopCount
        { get { return nTraceToHopCount; } }

        public void GetTraceToCoord(out int[] x, out int[] y)
        {
            int nKeyTo;
            ReqTraceNode reqTN;
            int nTraceCount = aReqProReqPrxTracesTo.GetLength(0);
            x = new int[nTraceCount];
            y = new int[nTraceCount];

            for (int i = 0; i < nTraceCount; i++)
            {
                nKeyTo = aReqProReqPrxTracesTo[i].Key;
                try
                {
                    reqTN = dictReqKey[nKeyTo];
                    x[i] = reqTN.X;
                    y[i] = reqTN.Y;
                }
                catch (Exception)
                {
                    x[i] = int.MinValue;
                    y[i] = int.MinValue;
                }
            }
        }

        public bool AreTooManyTracesTo(out int nAdditionalTraces, out bool bAdditionals)
        {
            nAdditionalTraces = nTraceToCount - aReqProReqPrxTracesTo.GetLength(0);
            bAdditionals = (aReqProReqPrxTracesTo.GetLength(0) != 0);
            return ((eAbort & eTraceAbortReason.eTooManyTracesTo) != 0);
        }

        public bool AreTooManyTracesFrom(out int nAdditionalTraces, out bool bAdditionals)
        {
            nAdditionalTraces = nTraceFromCount - aReqProReqPrxTracesFrom.GetLength(0);
            bAdditionals = (aReqProReqPrxTracesFrom.GetLength(0) != 0);
            return ((eAbort & eTraceAbortReason.eTooManyTracesFrom) != 0);
        }

        public bool TunedUp(int nTraceFromHopCount, int nTraceToHopCount)
        {
            if ((nTraceFromHopCount < this.nTraceFromHopCount) || (nTraceToCount < this.nTraceToHopCount))
                return true;
            return false;
        }
    }

}
