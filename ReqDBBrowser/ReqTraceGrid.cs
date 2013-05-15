using System;
using System.Collections.Generic;
using System.Text;

namespace ReqDBBrowser
{
    class ReqTraceGrid
    {
        int nMaxLevelTo;
        int nMaxLevelFrom;
        int nMaxTraceCount;
        int nMaxFromTraceHops;
        int nMaxToTraceHops;
        List<ReqTraceNode>[] grid;
        List<int> listnReqTypeTracedKeyExcl;
        System.Collections.Generic.Dictionary <int, ReqTraceNode> dictReqKey;
        const ulong ulLevelMultiplier = 20UL;
        public delegate bool ShowProgressReqTraceGrid(int nAddReq, int nReadReq, string strLog);
        ShowProgressReqTraceGrid showProgressReqTraceGrid;

        
        public ReqTraceGrid(int nMaxLevelFrom, int nMaxLevelTo, int nMaxTraceCount, 
            int nMaxFromTraceHops, int nMaxToTraceHops, 
            List<int> listnReqTypeTracedKeyExcl, ShowProgressReqTraceGrid showProgressReqTraceGrid)
        {
            this.nMaxLevelFrom = nMaxLevelFrom;
            this.nMaxLevelTo = nMaxLevelTo;
            this.nMaxTraceCount = nMaxTraceCount;
            this.nMaxFromTraceHops = nMaxFromTraceHops;
            this.nMaxToTraceHops = nMaxToTraceHops;
            this.listnReqTypeTracedKeyExcl = listnReqTypeTracedKeyExcl;
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
                AddReq(reqReqPrx, 0, 0, 0, ulRelDegree, null);
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
            int nTraceLevel, int nTraceFromHopCount, int nTraceToHopCount, ulong ulDegreeOffset, 
            ReqProRequirementPrx reqReqPrxTracesPreceder)
        {
            ReqTraceNode reqTraceNode = null;
            ReqProRequirementPrx.eTraceAbortReason eAbort = ReqProRequirementPrx.eTraceAbortReason.eNoAbort;

            if (dictReqKey.ContainsKey (reqReqPrx.Key))
                /* this requirement was already handled */
            {
                reqTraceNode = dictReqKey[reqReqPrx.Key];
                if (reqReqPrxTracesPreceder == null)
                    reqTraceNode.MakeRootNode();
                if (!(reqTraceNode.TunedUp(nTraceFromHopCount, nTraceToHopCount)))
                    return;
            }

            if (reqReqPrxTracesPreceder != null)
                if (listnReqTypeTracedKeyExcl.Contains (reqReqPrx.ReqTypeKey))
                {
                    eAbort |= ReqProRequirementPrx.eTraceAbortReason.eReqTypeFilter;
                    showProgressReqTraceGrid(0, 0, "Filtered: " + reqReqPrx.Tag);
                    return;
                }

            int nTracesTo;
            int nTracesFrom;
            ReqProRequirementPrx[] aTracesTo;
            ReqProRequirementPrx[] aTracesFrom;
            ulong ulDegreeInc = 1UL;
            
            for (int i=0; i<(nTraceLevel+nMaxLevelTo); i++)
                ulDegreeInc *= ulLevelMultiplier * ((ulong)nTraceLevel + (ulong)nMaxLevelTo);

            Tracer tracer = new Tracer("Traces from/to Req " + reqReqPrx.Tag);
            aTracesTo = reqReqPrx.GetRequirementTracesTo(nMaxTraceCount, ref eAbort, out nTracesTo, reqReqPrxTracesPreceder);
            aTracesFrom = reqReqPrx.GetRequirementTracesFrom(nMaxTraceCount, ref eAbort, out nTracesFrom, reqReqPrxTracesPreceder);
            tracer.Stop("Traces from/to Req " + reqReqPrx.Tag);

            if (reqTraceNode == null)
            {
                reqTraceNode = new ReqTraceNode(reqReqPrx, ulDegreeOffset, reqReqPrxTracesPreceder == null,
                    aTracesFrom, aTracesTo, nTracesFrom, nTracesTo, eAbort, nTraceFromHopCount, nTraceToHopCount);
                dictReqKey.Add(reqReqPrx.Key, reqTraceNode);
                grid[TraceLevel2Index(nTraceLevel)].Add(reqTraceNode);
            }
            else
                reqTraceNode.OnceAgain(ulDegreeOffset, reqReqPrxTracesPreceder == null,
                    aTracesFrom, aTracesTo, nTracesFrom, nTracesTo, eAbort, nTraceFromHopCount, nTraceToHopCount);

            showProgressReqTraceGrid(0, 1, "Adding: " + reqTraceNode.Tag);

            if (aTracesFrom.GetLength(0) > 0)
                if (nTraceLevel < this.nMaxLevelFrom)
                    if (reqTraceNode.TraceFromHopCount < this.nMaxFromTraceHops)
                    {
                        int nNextTraceFromHopCount = nTraceFromHopCount;
                        ulong ulLocOffset = ulDegreeOffset * ulLevelMultiplier;
                        showProgressReqTraceGrid(aTracesFrom.GetLength(0), 0, null);
                        foreach (ReqProRequirementPrx reqReqPrxFrom in aTracesFrom)
                            if (dictReqKey.ContainsKey(reqReqPrxFrom.Key))
                            {
                                ReqTraceNode reqTN = dictReqKey[reqReqPrxFrom.Key];
                                reqTN.SetRelDegree(ulLocOffset);
                                reqTN.AddTraceTo(reqReqPrx);
                            }
                            else
                            {
                                ulLocOffset += ulDegreeInc;
                                AddReq(reqReqPrxFrom, nTraceLevel + 1, ++nNextTraceFromHopCount, nTraceToHopCount,
                                    ulLocOffset, reqReqPrx);
                            }
                    }
                    else
                    {
                        System.Diagnostics.Trace.WriteLine("From Hops exceeded at: " + reqReqPrx.TagName);
                        showProgressReqTraceGrid(0, 0, "tracing from hops exceeded at: " + reqTraceNode.Tag);
                        eAbort |= ReqProRequirementPrx.eTraceAbortReason.eMaxFromHopsExceeded;
                    }
                else
                    eAbort |= ReqProRequirementPrx.eTraceAbortReason.eMaxFromLevelReached;


            if (aTracesTo.GetLength(0) > 0)
                if (nTraceLevel > -this.nMaxLevelTo)
                    if (reqTraceNode.TraceToHopCount+1 <= this.nMaxToTraceHops)
                    {
                        int nNextTraceToHopCount = reqTraceNode.TraceToHopCount + 1;
                        ulong ulLocOffset = ulDegreeOffset;
                        showProgressReqTraceGrid(aTracesTo.GetLength(0), 0, null);
                        foreach (ReqProRequirementPrx reqReqPrxTo in aTracesTo)
                            if (dictReqKey.ContainsKey(reqReqPrxTo.Key))
                            {
                                ReqTraceNode reqTN = dictReqKey[reqReqPrxTo.Key];
                                reqTN.SetRelDegree(ulLocOffset);
                                reqTN.AddTraceFrom(reqReqPrx);
                            }
                            else
                            {
                                ulLocOffset += ulDegreeInc;
                                AddReq(reqReqPrxTo, nTraceLevel - 1, nTraceFromHopCount, nTraceToHopCount,
                                    ulLocOffset, reqReqPrx);
                            }
                    }
                    else
                    {

                        System.Diagnostics.Trace.WriteLine("To Hops exceeded at: " + reqReqPrx.TagName);
                        showProgressReqTraceGrid(0, 0, "tracing to exceeded at: " + reqTraceNode.Tag);
                        eAbort |= ReqProRequirementPrx.eTraceAbortReason.eMaxToHopsExceeded;
                    }
                else
                    eAbort |= ReqProRequirementPrx.eTraceAbortReason.eMaxToLevelReached;
            reqTraceNode.AbortReason = eAbort;
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
