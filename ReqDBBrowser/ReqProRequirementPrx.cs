using System;
using System.Collections.Generic;
using System.Text;
using ReqPro40;

namespace ReqDBBrowser
{
    class ReqProRequirementPrx
    {
        string strTag;
        string strName;
        string strText;
        string strPrefix;

        static string strHomePrjPrefix;
        static ReqPro40.Requirements rpxReqColl = null;

        static Dictionary<int, ReqPro40.Requirement> dictReqCache;
        private const int nReqCacheSize = 2000;

        public enum eTraceAbortReason
        {
            eNoAbort,
            eInvalidRequirement =   0x0001, 
            eTooManyTracesFrom =    0x0002,
            eTooManyTracesTo =      0x0004,
            eMaxFromLevelReached =  0x0008,
            eMaxToLevelReached =    0x0010,
            eMaxFromHopsExceeded =  0x0020,
            eMaxToHopsExceeded =    0x0040
        }

        public struct sHistEntry
        {
            public string strRevision;
            public DateTime date;
            public string strUser;
            public string strDesc;
        }

        ReqPro40.Requirement rpxReq;
        int nKey;
        ReqPro40.Project rpxProject;

        static ReqProRequirementPrx()
        {
            dictReqCache = new Dictionary<int,Requirement> ();
        }

        private ReqProRequirementPrx()
        {
            strTag = "";
            strName = "";
            strText = "";
            strPrefix = "";
            rpxReq = null;
            rpxProject = null;
            nKey = -1;
        }

        private ReqProRequirementPrx(string strTag, string strName, string strText)
        {
            Init(strTag, strName, strText);
        }

        public ReqProRequirementPrx(ReqPro40.Requirement rpxReq)
        {
            Init(rpxReq);
        }

        public ReqProRequirementPrx(ReqPro40.Project rpxProject, int nKey)
        {
            if (rpxProject == null)
            {
                Init("Key " + nKey, "unknown Project", "unknown Project");
            }
            else if (!rpxProject.IsProjectOpen)
            {
                Init("Key " + nKey, "not accessible", "not accessible");
            }
            else
            {
                ReqPro40.Requirement rpxReq = null;
                Tracer tracer = new Tracer("Req (Key: " + nKey + ")");
                if (rpxReqColl != null)
                    if (rpxProject.Prefix == strHomePrjPrefix)
                        /* somehow strange - Requirement discovered - but not accessible if it does not belong to the home project */
                        rpxReq = rpxReqColl[nKey, ReqPro40.enumRequirementLookups.eReqLookup_Key];
                if (rpxReq != null)
                    tracer.Stop("Req " + rpxReq.get_Tag(enumTagFormat.eTagFormat_Tag) + " got via rpxColl []");
                else
                {
                    try
                    {
                        rpxReq = dictReqCache[nKey];
                        dictReqCache.Remove(nKey);
                        dictReqCache.Add(nKey, rpxReq);
                        tracer.Stop("Req " + ((rpxReq != null) ? (rpxReq.get_Tag(enumTagFormat.eTagFormat_Tag)) : ("key " + nKey)) +
                            " got via ReqCache");
                    }
                    catch (System.Collections.Generic.KeyNotFoundException e)
                    {
                        rpxReq = rpxProject.GetRequirement(
                             nKey, ReqPro40.enumRequirementLookups.eReqLookup_Key,
                             ReqPro40.enumRequirementsWeights.eReqWeight_Medium,
                             ReqPro40.enumRequirementFlags.eReqFlag_Empty);
                        dictReqCache.Add(nKey, rpxReq);

                        if (dictReqCache.Count > nReqCacheSize)
                        {
                            foreach (KeyValuePair<int, ReqPro40.Requirement> kvp in dictReqCache)
                            {
                                dictReqCache.Remove(kvp.Key);
                                break;
                            }
                        }
                        tracer.Stop("Req " + ((rpxReq != null) ? (rpxReq.get_Tag(enumTagFormat.eTagFormat_Tag)) : ("key " + nKey)) +
                            " got via rpxProject.GetRequirement");
                    }
                }
                Init(rpxReq);
            }

            this.rpxProject = rpxProject;
            this.nKey = nKey;
            strPrefix = "";

            if (rpxProject != null)
                if (rpxProject.Prefix != strHomePrjPrefix)
                    strPrefix = rpxProject.Prefix;
        }

        public ReqProRequirementPrx(ReqProRequirementPrx reqProReqPrx)
        {
            this.nKey = reqProReqPrx.nKey;
            this.rpxProject = reqProReqPrx.rpxProject;
            this.rpxReq = reqProReqPrx.rpxReq;
            this.strName = reqProReqPrx.strName;
            this.strPrefix = reqProReqPrx.strPrefix;
            this.strTag = reqProReqPrx.strTag;
            this.strText = reqProReqPrx.strText;
        }

        private void Init(string strTag, string strName, string strText)
        {
            this.strPrefix = "";
            this.strTag = strTag;
            this.strName = strName;
            this.strText = strText;
            rpxReq = null;
            rpxProject = null;
            nKey = -1;
        }

        public void Init(ReqPro40.Requirement rpxReq)
        {
            if (rpxReq == null)
                Init("", "unknown", "unknown");
            else
                Init(rpxReq.get_Tag(ReqPro40.enumTagFormat.eTagFormat_Tag), rpxReq.Name, rpxReq.Text);
            this.rpxReq = rpxReq;
        }


        public string Tag
        { get 
            {
                if (strPrefix.Length > 0)
                    return this.strPrefix + "." + strTag;
                else
                    return this.strTag; 
            } 
        }
        public string Name
        { get { return this.strName; } }
        public string TagName
        { get { return (this.Tag+": "+this.strName); } }
        public string Text
        { get { return this.strText; } }

        public int Key
        { get { return ((rpxReq!=null)? rpxReq.key : -1); } }

        public static string HomePrjPrefix
        {
            get { return strHomePrjPrefix; }
            set { strHomePrjPrefix = value; }
        }

        public static ReqPro40.Requirements RPXReqColl
        {
            set { rpxReqColl = value; }
        }

        public ReqProRequirementPrx[] GetRequirementTraces
            (ReqPro40.Relationships rpxRelations, int nMaxTraceCount, out int nTraceCount, ReqProRequirementPrx reqReqPrxTracesPrecedor)
        {
            bool bDoNotRead;
            ReqProRequirementPrx[] aReqPrx;

            int nCount = rpxRelations.Count;
            nTraceCount = nCount;
            if (nCount > nMaxTraceCount)
            {
                if (reqReqPrxTracesPrecedor == null)
                    aReqPrx = new ReqProRequirementPrx[0];
                else
                {
                    aReqPrx = new ReqProRequirementPrx[1];
                    aReqPrx[0] = reqReqPrxTracesPrecedor;
                }
            }
            else
            {
                aReqPrx = new ReqProRequirementPrx[nCount];
                rpxRelations.MoveFirst();
                for (int i = 0; i < nCount; i++)
                {
                    try
                    {
                        if ((rpxRelations.ItemCurrent.Permissions == enumPermissions.ePerm_None) ||
                            (rpxRelations.ItemCurrent.Permissions == enumPermissions.ePermission_None))
                            bDoNotRead = true;
                        else
                            bDoNotRead = false;

                        if (rpxRelations.Direction == enumRelationshipDirections.eRelDirection_To)
                        {
                            if (bDoNotRead)
                                aReqPrx[i] = new ReqProRequirementPrx("Key " + rpxRelations.ItemCurrent.DestinationKey, "no permission", "no permission");
                            else
                                aReqPrx[i] = new ReqProRequirementPrx
                                    (rpxRelations.ItemCurrent.DestinationProject, rpxRelations.ItemCurrent.DestinationKey);
                        }
                        else
                        {
                            if (bDoNotRead)
                                aReqPrx[i] = new ReqProRequirementPrx("Key " + rpxRelations.ItemCurrent.SourceKey, "no permission", "no permission");
                            else
                                aReqPrx[i] = new ReqProRequirementPrx
                                    (rpxRelations.ItemCurrent.SourceProject, rpxRelations.ItemCurrent.SourceKey);
                        }
                    }
                    catch (Exception)
                    {
                        aReqPrx[i] = new ReqProRequirementPrx(null, -1);
                    }

                    rpxRelations.MoveNext();
                }
            }
            return aReqPrx;
        }

        public ReqProRequirementPrx[] GetRequirementTracesFrom(int nMaxTraceCount, ref eTraceAbortReason eAbort,
            out int nTraceCount, ReqProRequirementPrx reqReqPrxTracesFrom)
        {
            if (rpxReq != null)
            {
                eTraceAbortReason eLocAbort = eTraceAbortReason.eNoAbort;
                ReqProRequirementPrx[] aReqPrx = GetRequirementTraces(rpxReq.TracesFrom, nMaxTraceCount,
                    out nTraceCount, reqReqPrxTracesFrom);
                if (nTraceCount != aReqPrx.GetLength(0))
                    eAbort = eAbort | eTraceAbortReason.eTooManyTracesFrom;
                return aReqPrx;
            }
            nTraceCount = 0;
            return new ReqProRequirementPrx[0];
        }

        public ReqProRequirementPrx[] GetRequirementTracesTo(int nMaxTraceCount, ref eTraceAbortReason eAbort,
            out int nTraceCount, ReqProRequirementPrx reqReqPrxTracesTo)
        {
            if (rpxReq != null)
            {
                eTraceAbortReason eLocAbort = eTraceAbortReason.eNoAbort;
                ReqProRequirementPrx[] aReqPrx = GetRequirementTraces(rpxReq.TracesTo, nMaxTraceCount,
                    out nTraceCount, reqReqPrxTracesTo);
                if (nTraceCount != aReqPrx.GetLength(0))
                    eAbort = eAbort | eTraceAbortReason.eTooManyTracesTo;
                return aReqPrx;
            }
            nTraceCount = 0;
            return new ReqProRequirementPrx[0];
        }

        public void GetRequirementLog(out ReqProRequirementPrx.sHistEntry[] asHistory)
        {
            ReqPro40.Revision rpxRev;
            int nCount;

            nCount = rpxReq.Revisions.Count;
            asHistory = new ReqProRequirementPrx.sHistEntry[nCount];

            for (int i = 0; i < nCount; i++)
            {
                rpxRev = rpxReq.Revisions[i + 1, ReqPro40.enumRevisionLookups.eRevLookup_Index];
                asHistory[i].strRevision = rpxRev.VersionNumber;
                asHistory[i].date = DateTime.Parse (rpxRev.VersionDateTime);
                asHistory[i].strUser = rpxRev.VersionUser.FullName;
                asHistory[i].strDesc = rpxRev.VersionReason;
            }

        }

    }
}
