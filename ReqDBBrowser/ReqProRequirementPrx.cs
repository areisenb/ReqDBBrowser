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

        public enum eTraceAbortReason
        {
            eNoAbort,
            eInvalidRequirement =   0x0001, 
            eTooManyTracesFrom =    0x0002,
            eTooManyTracesTo =      0x0004,
            eMaxFromLevelReached =  0x0008,
            eMaxToLevelReached =    0x0010
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
            this.strPrefix = "";
            this.strTag = strTag;
            this.strName = strName;
            this.strText = strText;
            rpxReq = null;
            rpxProject = null;
            nKey = -1;
        }

        public ReqProRequirementPrx(ReqPro40.Requirement rpxReq):
            this((rpxReq!=null) ? rpxReq.get_Tag(ReqPro40.enumTagFormat.eTagFormat_Tag):"",
                (rpxReq != null) ? rpxReq.Name : "unknown", (rpxReq != null) ? rpxReq.Text : "unknown")
        {
            this.rpxReq = rpxReq;
        }

        public ReqProRequirementPrx(ReqPro40.Project rpxProject, int nKey):
            this((rpxProject!=null) ? rpxProject.GetRequirement
                (nKey, ReqPro40.enumRequirementLookups.eReqLookup_Key,
                 ReqPro40.enumRequirementsWeights.eReqWeight_Medium,
                 ReqPro40.enumRequirementFlags.eReqFlag_Empty):null)
        {
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
            


        public ReqProRequirementPrx[] GetRequirementTraces
            (ReqPro40.Relationships rpxRelations, int nMaxTraceCount, out int nTraceCount)
        {
            int nCount = rpxRelations.Count;
            if (nCount > nMaxTraceCount)
                nCount = 0;

            nTraceCount = nCount;
            ReqProRequirementPrx[] aReqPrx = new ReqProRequirementPrx[nCount];
            rpxRelations.MoveFirst();
            for (int i = 0; i < nCount; i++)
            {
                try
                {
                    if (rpxRelations.Direction == enumRelationshipDirections.eRelDirection_To)
                    {
                        aReqPrx[i] = new ReqProRequirementPrx
                            (rpxRelations.ItemCurrent.DestinationProject, rpxRelations.ItemCurrent.DestinationKey);
                    }
                    else
                    {
                        aReqPrx[i] = new ReqProRequirementPrx
                            (rpxRelations.ItemCurrent.SourceProject, rpxRelations.ItemCurrent.SourceKey);
                    }
                }
                catch (Exception)
                {
                    aReqPrx [i] = new ReqProRequirementPrx (null, -1);
                }

                rpxRelations.MoveNext();
            }
            return aReqPrx;
        }

        public ReqProRequirementPrx[] GetRequirementTracesFrom(int nMaxTraceCount, ref eTraceAbortReason eAbort, out int nTraceCount)
        {
            if (rpxReq != null)
            {
                eTraceAbortReason eLocAbort = eTraceAbortReason.eNoAbort;
                ReqProRequirementPrx[] aReqPrx = GetRequirementTraces(rpxReq.TracesFrom, nMaxTraceCount, out nTraceCount);
                if ((nTraceCount > 0) && (aReqPrx.GetLength(0) == 0))
                    eAbort = eAbort | eTraceAbortReason.eTooManyTracesFrom;
                return aReqPrx;
            }
            nTraceCount = 0;
            return new ReqProRequirementPrx[0];
        }

        public ReqProRequirementPrx[] GetRequirementTracesTo(int nMaxTraceCount, ref eTraceAbortReason eAbort, out int nTraceCount)
        {
            if (rpxReq != null)
            {
                eTraceAbortReason eLocAbort = eTraceAbortReason.eNoAbort;
                ReqProRequirementPrx[] aReqPrx = GetRequirementTraces(rpxReq.TracesTo, nMaxTraceCount, out nTraceCount);
                if ((nTraceCount > 0) && (aReqPrx.GetLength(0) == 0))
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
