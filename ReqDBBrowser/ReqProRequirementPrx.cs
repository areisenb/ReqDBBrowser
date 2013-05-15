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

        static Dictionary<int, ReqPro40.Requirement___v6> dictReqCache;
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
            eMaxToHopsExceeded =    0x0040,
            eReqTypeFilter =        0x0080
        }

        public struct sHistEntry
        {
            public string strRevision;
            public DateTime date;
            public string strUser;
            public string strDesc;
        }

        ReqPro40.Requirement___v6 rpxReq;
        int nKey;
        ReqPro40.Project___v7 rpxProject;

        static ReqProRequirementPrx()
        {
            InitCache();
        }

        public static void InitCache()
        {
            dictReqCache = new Dictionary<int, Requirement___v6>();
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

        public ReqProRequirementPrx(ReqPro40.Requirement___v6 rpxReq)
        {
            Init(rpxReq);
        }

        public ReqProRequirementPrx(ReqPro40.Project___v7 rpxProject, int nKey)
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
                ReqPro40.Requirement___v6 rpxReq = null;
                Tracer tracer = new Tracer("Req (Key: " + nKey + ")");
                if (rpxReqColl != null)
                    if (rpxProject.Prefix == strHomePrjPrefix)
                        /* somehow strange - Requirement discovered - but not accessible if it does not belong to the home project */
                        rpxReq = (Requirement___v6)rpxReqColl[nKey, ReqPro40.enumRequirementLookups.eReqLookup_Key];
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
                    catch (System.Collections.Generic.KeyNotFoundException)
                    {
                        rpxReq = (Requirement___v6)rpxProject.GetRequirement(
                             nKey, ReqPro40.enumRequirementLookups.eReqLookup_Key,
                             ReqPro40.enumRequirementsWeights.eReqWeight_Medium,
                             ReqPro40.enumRequirementFlags.eReqFlag_Empty);
                        dictReqCache.Add(nKey, rpxReq);

                        if (dictReqCache.Count > nReqCacheSize)
                        {
                            foreach (KeyValuePair<int, ReqPro40.Requirement___v6> kvp in dictReqCache)
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

        public void Init(ReqPro40.Requirement___v6 rpxReq)
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

        public int ReqTypeKey
        { get { return ((rpxReq != null) ? rpxReq.ReqTypeKey : -1); } }

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
            ReqProRequirementPrx[] aReqPrx;

            int nCount = rpxRelations.Count;
            nTraceCount = nCount;
            if ((nMaxTraceCount != -1) && (nCount > nMaxTraceCount))
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
                    aReqPrx[i] = GetRequirementTrace (rpxRelations.ItemCurrent, rpxRelations.Direction);
                    rpxRelations.MoveNext();
                }
            }
            return aReqPrx;
        }

        public ReqProRequirementPrx GetRequirementTrace (ReqPro40.Relationship rpxRelation, enumRelationshipDirections dir)
        {
            ReqProRequirementPrx reqPrx;
            bool bDoNotRead;
            try
            {
                if ((rpxRelation.Permissions == enumPermissions.ePerm_None) ||
                    (rpxRelation.Permissions == enumPermissions.ePermission_None))
                    bDoNotRead = true;
                else
                    bDoNotRead = false;

                if ((dir == enumRelationshipDirections.eRelDirection_To) ||
                    (dir == enumRelationshipDirections.eRelDirection_Child))
                {
                    if (bDoNotRead)
                        reqPrx = new ReqProRequirementPrx("Key " + rpxRelation.DestinationKey, "no permission", "no permission");
                    else
                        reqPrx = new ReqProRequirementPrx
                            ((ReqPro40.Project___v7)rpxRelation.DestinationProject, rpxRelation.DestinationKey);
                }
                else
                {
                    if (bDoNotRead)
                        reqPrx = new ReqProRequirementPrx("Key " + rpxRelation.SourceKey, "no permission", "no permission");
                    else
                        reqPrx = new ReqProRequirementPrx
                            ((ReqPro40.Project___v7)rpxRelation.SourceProject, rpxRelation.SourceKey);
                }
            }
            catch (Exception)
            {
                reqPrx = new ReqProRequirementPrx(null, -1);
            }

            return reqPrx;
        }

        public ReqProRequirementPrx[] GetRequirementTracesFrom(int nMaxTraceCount, ref eTraceAbortReason eAbort,
            out int nTraceCount, ReqProRequirementPrx reqReqPrxTracesFrom)
        {
            if (rpxReq != null)
            {
                ReqProRequirementPrx[] aReqPrx = GetRequirementTraces(rpxReq.TracesFrom, nMaxTraceCount,
                    out nTraceCount, reqReqPrxTracesFrom);
                if (nTraceCount != aReqPrx.GetLength(0))
                    eAbort = eAbort | eTraceAbortReason.eTooManyTracesFrom;
                if (!rpxReq.IsRoot)
                {
                    ReqProRequirementPrx[] aOut;
                    int i;

                    ReqProRequirementPrx reqParent = GetRequirementTrace(
                        rpxReq.get_Parent(enumRequirementsWeights.eReqWeight_Medium),
                        enumRelationshipDirections.eRelDirection_Parent);
                    aOut = new ReqProRequirementPrx[aReqPrx.GetLength(0) + 1];
                    for (i = 0; i < aReqPrx.GetLength(0); i++)
                        aOut[i] = aReqPrx[i];
                    aOut[i] = reqParent;
                    nTraceCount++;
                }

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
                int i=0;
                int nTraceCountTo;
                int nTraceCountChildren;

                ReqProRequirementPrx[] aReqPrxTo = GetRequirementTraces(rpxReq.TracesTo, nMaxTraceCount,
                    out nTraceCountTo, reqReqPrxTracesTo);
                if (nTraceCountTo != aReqPrxTo.GetLength(0))
                    eAbort = eAbort | eTraceAbortReason.eTooManyTracesTo;
                ReqProRequirementPrx[] aReqPrxChildren = GetRequirementTraces(rpxReq.Children, -1,
                    out nTraceCountChildren, reqReqPrxTracesTo);

                ReqProRequirementPrx[] aReqPrx;
                aReqPrx = new ReqProRequirementPrx[aReqPrxTo.GetLength(0) + aReqPrxChildren.GetLength(0)];
                foreach (ReqProRequirementPrx reqPrx in aReqPrxTo)
                    aReqPrx[i++] = reqPrx;
                foreach (ReqProRequirementPrx reqPrx in aReqPrxChildren)
                    aReqPrx[i++] = reqPrx;
                nTraceCount = nTraceCountTo + nTraceCountChildren;

                return aReqPrx;
            }
            nTraceCount = 0;
            return new ReqProRequirementPrx[0];
        }

        public void GetRequirementLog(out ReqProRequirementPrx.sHistEntry[] asHistory)
        {
            Revision___v1 rpxRev;
            int nCount;

            nCount = rpxReq.Revisions.Count;
            asHistory = new ReqProRequirementPrx.sHistEntry[nCount];

            for (int i = 0; i < nCount; i++)
            {
                rpxRev = (Revision___v1)rpxReq.Revisions[i + 1, ReqPro40.enumRevisionLookups.eRevLookup_Index];
                asHistory[i].strRevision = rpxRev.VersionNumber;
                asHistory[i].date = DateTime.Parse (rpxRev.VersionDateTime);
                asHistory[i].strUser = rpxRev.VersionUser.FullName;
                asHistory[i].strDesc = rpxRev.VersionReason;
            }
        }

        public int GetAttributes(out string[] astrCol, out string[] astrValue)
        {
            ReqPro40.AttrValue rpxAttr;
            int nCount;

            nCount = rpxReq.AttrValues.Count;
            astrCol = new string [nCount];
            astrValue = new string [nCount];

            for (int i = 0; i < nCount; i++)
            {
                rpxAttr = rpxReq.AttrValues[i + 1, enumAttrValueLookups.eAttrValueLookup_Index];
                astrCol[i] = rpxAttr.Label;
                switch (rpxAttr.DataType) {
                    case enumAttrDataTypes.eAttrDataTypes_Text:
                    case enumAttrDataTypes.eAttrDataTypes_List:
                        astrValue[i] = rpxAttr.Text;
                        break;
                    case enumAttrDataTypes.eAttrDataTypes_MultiSelect:
                        {
                            int nListCount;
                            ReqPro40.ListItemValue liVal;
                            nListCount = rpxAttr.ListItemValues.Count;
                            astrValue[i] = "";
                            for (int j=0; j<nListCount; j++) 
                            {
                                liVal = rpxAttr.ListItemValues[j+1, enumListItemValueLookups.eListItemValueLookup_Index];
                                if (liVal.Selected == true)
                                    astrValue[i] += liVal.Text + "\r\n";
                            }
                            break;
                        }
                    case enumAttrDataTypes.eAttrDataTypes_Integer:
                    case enumAttrDataTypes.eAttrDataTypes_Date:
                    default:
                        astrValue[i] = rpxAttr.DataTypeName + " not yet implemented";
                        break;
                }

            }
            return nCount;
        }

        public string PackageName
        {
            get { return rpxReq.Package.Name; }
        }

        public string PackagePathName
        {
            get
            {
                ReqPro40._iPackage rpxPkg = rpxReq.Package;
                return (rpxPkg.GetHierarchyPathName());
            }
        }

        public string VersionDateTime
        {
            get { return rpxReq.VersionDateTime; }
        }

        public string VersionNumber
        {
            get { return rpxReq.VersionNumber; }
        }

        public string VersionUser
        {
            get { return rpxReq.VersionUser.FullName; }
        }

        public override string ToString()
        {
            return this.TagName;
        }

    }
}
