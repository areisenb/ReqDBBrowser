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
            


        public ReqProRequirementPrx[] GetRequirementTraces(ReqPro40.Relationships rpxRelations, int nMaxCount)
        {
            int nCount = rpxRelations.Count;
            if (nCount > nMaxCount)
                nCount = 0;

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
                        int n;
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

        public ReqProRequirementPrx[] GetRequirementTracesFrom(int nMaxTraces)
        {
            if (rpxReq != null)
                return GetRequirementTraces(rpxReq.TracesFrom, nMaxTraces);
            return new ReqProRequirementPrx[0];
        }

        public ReqProRequirementPrx[] GetRequirementTracesTo(int nMaxTraces)
        {
            if (rpxReq != null)
                return GetRequirementTraces(rpxReq.TracesTo, nMaxTraces);
            return new ReqProRequirementPrx[0];
        }


    }
}
