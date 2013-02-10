using System;
using System.Collections.Generic;
using System.Text;

namespace ReqDBBrowser
{
    class ReqTraceGrid
    {
        public class ReqTraceNode: ReqProRequirementPrx
        {
            ReqProRequirementPrx[] aReqProReqPrxTracesTo;
            ReqProRequirementPrx[] aReqProReqPrxTracesFrom;

            int nTraceFromCount;
            int nTraceToCount;
            ulong ulDegreeRel;

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

            public ReqTraceNode(ReqProRequirementPrx reqProReqPrx, ulong ulDegreeRel,
                ReqProRequirementPrx[] aReqProReqPrxTracesFrom, ReqProRequirementPrx[] aReqProReqPrxTracesTo,
                int nTraceFromCount, int nTraceToCount)
                : base(reqProReqPrx)
            {
                this.aReqProReqPrxTracesFrom = aReqProReqPrxTracesFrom;
                this.aReqProReqPrxTracesTo = aReqProReqPrxTracesTo;
                this.nTraceFromCount = nTraceFromCount;
                this.nTraceToCount = nTraceToCount;
                this.ulDegreeRel = ulDegreeRel;
                x = int.MinValue;
                y = int.MinValue;
            }

            public void SetRelDegree(ulong ulDegreeRel)
            {
                if (ulDegreeRel < this.ulDegreeRel)
                    this.ulDegreeRel = ulDegreeRel;
            }

            public string GetTraceToString()
            {
                return GetTraceString(aReqProReqPrxTracesTo, nTraceToCount);
            }

            public string GetTraceFromString()
            {
                return GetTraceString(aReqProReqPrxTracesFrom, nTraceFromCount);
            }

            private static string GetTraceString(ReqProRequirementPrx[] aReqProReqPrxTraces, int nTraceCount)
            {
                string strRet;
                if ((aReqProReqPrxTraces.GetLength (0) == 0) && (nTraceCount > 0))
                    strRet = "(" + nTraceCount + ")\n"; 
                else
                    strRet = "";
                foreach (ReqProRequirementPrx reqReqPrx in aReqProReqPrxTraces)
                    strRet += reqReqPrx.Tag + "\n";
                return strRet;
            }

            public void SetCoordinates (int x, int y, Dictionary<int, ReqTraceNode> dict)
            {
                this.dictReqKey = dict;
                this.x = x;
                this.y = y;
            }

            public int X
            { get { return x; } }
            public int Y
            { get { return y; } }

            public void GetTraceToCoord(out int[] x, out int[] y)
            {
                int nKeyTo;
                ReqTraceNode reqTN;
                int nTraceCount = aReqProReqPrxTracesTo.GetLength(0);
                x = new int[nTraceCount];
                y = new int[nTraceCount];

                for (int i = 0; i < nTraceCount; i++)
                {
                    nKeyTo = aReqProReqPrxTracesTo [i].Key;
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
        }

        int nMaxLevelTo;
        int nMaxLevelFrom;
        int nMaxTraceCount;
        int nMaxFromTraceHops;
        int nMaxToTraceHops;
        List<ReqTraceNode>[] grid;
        System.Collections.Generic.Dictionary <int, ReqTraceNode> dictReqKey;
        const ulong ulLevelMultiplier = 20UL;
        public delegate bool ShowProgressReqTraceGrid(int nAddReq, int nReadReq, string strLog);
        ShowProgressReqTraceGrid showProgressReqTraceGrid;

        
        public ReqTraceGrid(int nMaxLevelFrom, int nMaxLevelTo, int nMaxTraceCount, 
            int nMaxFromTraceHops, int nMaxToTraceHops, ShowProgressReqTraceGrid showProgressReqTraceGrid)
        {
            this.nMaxLevelFrom = nMaxLevelFrom;
            this.nMaxLevelTo = nMaxLevelTo;
            this.nMaxTraceCount = nMaxTraceCount;
            this.nMaxFromTraceHops = nMaxFromTraceHops;
            this.nMaxToTraceHops = nMaxToTraceHops;
            grid = new List<ReqTraceNode>[nMaxLevelFrom + nMaxLevelTo + 1];
            for (int i=0; i<(nMaxLevelFrom+nMaxLevelTo+1); i++)
                grid[i] = new List<ReqTraceNode>();
            dictReqKey = new System.Collections.Generic.Dictionary<int, ReqTraceNode>();
            this.showProgressReqTraceGrid = showProgressReqTraceGrid;
        }

        public void AddReq(System.Collections.ArrayList arrReqPrx)
        {
            ulong ulRelDegree = 1;
            
            /* First let us step through the requirements tree */
            showProgressReqTraceGrid(1, 0, "starting Requirement Trace Analysis");

            foreach (ReqProRequirementPrx reqReqPrx in arrReqPrx)
            {
                AddReq(reqReqPrx, 0, 0, 0, ulRelDegree);
                ulRelDegree += 10000;
            }

            /* then sort according to the relation degree */
            showProgressReqTraceGrid(0, 0, "sorting Requirements based on the Relationship");
            for (int i = nMaxLevelFrom; i >= -nMaxLevelTo; i--)
                grid[TraceLevel2Index(i)].Sort(ReqTraceNode.CompareReqTraceNodebyRelationDegree);

            /* finally publish the position to each node */
            showProgressReqTraceGrid(0, 0, "Publish Position to the Requirements");
            for (int i = -nMaxLevelTo; i <= nMaxLevelFrom; i++)
                for (int j = 0; j < GetElementCount(i); j++)
                    grid[TraceLevel2Index(i)][j].SetCoordinates(j, i, dictReqKey);
            showProgressReqTraceGrid(0, 0, null);
        }

        private int TraceLevel2Index(int nTraceLevel)
        {
            return (nMaxLevelFrom - nTraceLevel);
        }

        private void AddReq(ReqProRequirementPrx reqReqPrx, 
            int nTraceLevel, int nTraceFromHopCount, int nTraceToHopCount, ulong ulDegreeOffset)
        {
            if (dictReqKey.ContainsKey (reqReqPrx.Key))
                return;

            ReqProRequirementPrx.eTraceAbortReason eAbort = ReqProRequirementPrx.eTraceAbortReason.eNoAbort;
            int nTracesTo;
            int nTracesFrom;
            ReqProRequirementPrx[] aTracesTo;
            ReqProRequirementPrx[] aTracesFrom;
            ulong ulDegreeInc = 1UL;
            ReqTraceNode reqTraceNode;
            
            for (int i=0; i<(nTraceLevel+nMaxLevelTo); i++)
                ulDegreeInc *= ulLevelMultiplier * ((ulong)nTraceLevel + (ulong)nMaxLevelTo);

            Tracer tracer = new Tracer("Traces from/to Req " + reqReqPrx.Tag);
            aTracesTo = reqReqPrx.GetRequirementTracesTo(nMaxTraceCount, ref eAbort, out nTracesTo);
            aTracesFrom = reqReqPrx.GetRequirementTracesFrom(nMaxTraceCount, ref eAbort, out nTracesFrom);
            tracer.Stop("Traces from/to Req " + reqReqPrx.Tag);

            reqTraceNode = new ReqTraceNode(reqReqPrx, ulDegreeOffset, aTracesFrom, aTracesTo, nTracesFrom, nTracesTo);
            dictReqKey.Add(reqReqPrx.Key, reqTraceNode);

            grid[TraceLevel2Index(nTraceLevel)].Add(reqTraceNode);

            showProgressReqTraceGrid(0, 1, "Adding: " + reqTraceNode.Tag);

            if ((aTracesFrom.GetLength(0) > 0) && (nTraceLevel < this.nMaxLevelFrom))
                if (nTraceFromHopCount < this.nMaxFromTraceHops)
                {
                    int nNextTraceFromHopCount = nTraceFromHopCount;
                    ulong ulLocOffset = ulDegreeOffset * ulLevelMultiplier;
                    showProgressReqTraceGrid(aTracesFrom.GetLength(0), 0, null);
                    foreach (ReqProRequirementPrx reqReqPrxFrom in aTracesFrom)
                        if (dictReqKey.ContainsKey(reqReqPrxFrom.Key))
                        {
                            ReqTraceNode reqTN = dictReqKey[reqReqPrxFrom.Key];
                            reqTN.SetRelDegree(ulLocOffset);
                        }
                        else
                        {
                            ulLocOffset += ulDegreeInc;
                            AddReq(reqReqPrxFrom, nTraceLevel + 1, ++nNextTraceFromHopCount, nTraceToHopCount,
                                ulLocOffset);
                        }
                }
                else
                {
                    System.Diagnostics.Trace.WriteLine("From Hops exceeded at: " + reqReqPrx.TagName);
                    showProgressReqTraceGrid(0, 0, "tracing from hops exceeded at: " + reqTraceNode.Tag);
                }


            if ((aTracesTo.GetLength(0) > 0) && (nTraceLevel > -this.nMaxLevelTo))
                if (++nTraceToHopCount <= this.nMaxToTraceHops)
                {
                    int nNextTraceToHopCount = nTraceToHopCount;
                    ulong ulLocOffset = ulDegreeOffset;
                    showProgressReqTraceGrid(aTracesTo.GetLength(0), 0, null);
                    foreach (ReqProRequirementPrx reqReqPrxTo in aTracesTo)
                        if (dictReqKey.ContainsKey(reqReqPrxTo.Key))
                        {
                            ReqTraceNode reqTN = dictReqKey[reqReqPrxTo.Key];
                            reqTN.SetRelDegree(ulLocOffset);
                        }
                        else
                        {
                            ulLocOffset += ulDegreeInc;
                            AddReq(reqReqPrxTo, nTraceLevel - 1, nTraceFromHopCount, nTraceToHopCount, ulLocOffset);
                        }
                }
                else
                {

                    System.Diagnostics.Trace.WriteLine("To Hops exceeded at: " + reqReqPrx.TagName);
                    showProgressReqTraceGrid(0, 0, "tracing to exceeded at: " + reqTraceNode.Tag);
                }
        }

        public int GetElementCount(int nLevel)
        {
            if ((nLevel <= nMaxLevelFrom) && (nLevel >= -nMaxLevelTo))
                return (grid[TraceLevel2Index(nLevel)].Count);
            return 0;
        }

        public ReqTraceNode this[int nLevel, int nIdx]
        {
            get
            {
                if ((nLevel <= nMaxLevelFrom) && (nLevel >= -nMaxLevelTo))
                    if ((nIdx < grid[TraceLevel2Index(nLevel)].Count) && (nIdx >= 0))
                        return (grid[TraceLevel2Index(nLevel)][nIdx]);
                return null;
            }
        }
    }
}
