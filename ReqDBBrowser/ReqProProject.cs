using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;
using System.Collections;

namespace ReqDBBrowser
{
    class ReqProProject
    {
        enum eState
        {
            stIdle,
            stDisc,
            stConn,
            stParsed
        };

        ReqPro40.Application___v4 rpxApplication;
        ReqPro40.Project___v7 rpxProject;
        private System.Collections.ArrayList arpxRelProjects;
        ReqPro40.Catalog rpxCatalog;

        Dictionary <int, object> dictPackage;

        eState state;
        IReqProProjectCb cb;

        public interface IReqProProjectCb
        {
            bool RequestCredentials(string strProjectDesc, ref string strUser, out string strPassword);
            void ShowOpenRelatedProjectError(string strProjectDesc);
            bool ShowProgressReqPkgTree(int nSumTreeElements, int nReadTreeElements, 
                int nSumRequirements, int nReadRequirements, string strLogg);
            bool ShowProgressOpenProject(string strLogg);
        }

        public class SearchResult
        {
            string strText;
            int nIdx;
            int nLen;

            public SearchResult(string strText, int nIdx, int nLen)
            {
                this.strText = strText;
                this.nIdx = nIdx;
                this.nLen = nLen;
            }

            public SearchResult(string strText)
            {
                this.strText = strText;
                nIdx = -1;
                nLen = 0;
            }

            public string Text
            {
                get { return strText; }
            }
            public int Index
            {
                get { return nIdx; }
                set { nIdx = value; }
            }
            public int Length
            {
                get { return nLen; }
                set { nLen = value; }
            }
        }

        public class ReqProException : Exception
        {
            public enum ESeverity
            {
                eWarning,
                eError,
                eFatal
            }

            ESeverity eSeverity;

            public ReqProException(ESeverity eSeverity, string strMessage, Exception InnerException):
                base(strMessage, InnerException)
            {
                this.eSeverity = eSeverity;
            }

            public ESeverity Severity
            {
                get { return eSeverity; }
            }
        }

        public ReqProProject (IReqProProjectCb cb)
        {
            state = eState.stDisc;
            this.cb = cb;

            try
            {
                arpxRelProjects = new System.Collections.ArrayList();
                rpxApplication = (ReqPro40.Application___v4)new ReqPro40.Application();
                rpxCatalog = rpxApplication.PersonalCatalog;
            }
            catch (System.Runtime.InteropServices.COMException e)
            {
                MessageBox.Show(e.Message + "\r\n\r\nStack: " + e.StackTrace + "\r\n\r\nin: " + e.TargetSite, 
                    "COM Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            catch (Exception e)
            {
                throw new ReqProException (ReqProException.ESeverity.eFatal, 
                    "Compiled against ReqPro40 Version 7.1.1.5\n\n" + e.Message, e);
            }
        }

        public string[] GetProjectCatalog()
        {
            string[] astrProjects;
            int nCount;

            if (rpxCatalog == null)
                astrProjects = new string[0];
            else
            {
                nCount = rpxCatalog.Count;
                astrProjects = new string[nCount];

                for (int i = 0; i < nCount; i++)
                    astrProjects[i] = rpxCatalog[i + 1, ReqPro40.enumCatalogLookups.eCatLookup_Index].get_Name();
            }
            return astrProjects;
        }

        public string ProjectFileFromProject(string strProject)
        {
            try
            {
                ReqPro40.CatalogItem rpxCatItem = rpxCatalog[strProject, ReqPro40.enumCatalogLookups.eCatLookup_Name];
                return (rpxCatItem.get_FileDirectory() + rpxCatItem.get_Filename());
            }
            catch (Exception)
            {
                return "";
            }
        }

        public bool OpenProject(string strProject, string strUser, string strPassword,
            out string strErrDiag)
        {
            strErrDiag = "";
            bool bRet = false;
            try
            {
                if (state != eState.stDisc)
                {
                    CloseProject();
                }
                cb.ShowProgressOpenProject("Opening " + strProject);
                rpxProject = (ReqPro40.Project___v7) rpxApplication.OpenProject(strProject,
                    ReqPro40.enumOpenProjectOptions.eOpenProjOpt_RQSFile,
                    strUser,
                    strPassword,
                    ReqPro40.enumProjectFlags.eProjFlag_Normal,
                    ReqPro40.enumRelatedProjectOptions.eRelatedProjOption_ConnectAll);
                state = eState.stConn;
                bRet = true;

                ReqProRequirementPrx.HomePrjPrefix = rpxProject.Prefix;

                int nCountRelPrj = rpxProject.RelatedProjectContexts.Count;
                bool bLoadRelPrj;
                string strRelPrjPassword;
                string strRelPrjUser;

                cb.ShowProgressOpenProject("Related Projects to be opened: " + nCountRelPrj);

                ReqPro40.RelatedProjectContext rpxRelPrjCtx;
                ReqPro40.Project___v7 rpxRelPrj;

                for (int i = 0; i < nCountRelPrj; i++)
                {
                    rpxRelPrjCtx = rpxProject.RelatedProjectContexts[i + 1, ReqPro40.enumRelatedProjectLookups.eRelProjLookup_Index];
                    bLoadRelPrj = true;
                    strRelPrjPassword = strPassword;
                    strRelPrjUser = strUser;
                    rpxRelPrj = null;
                    if (!rpxRelPrjCtx.IsOpen)
                        while (bLoadRelPrj)
                        {
                            try
                            {
                                cb.ShowProgressOpenProject("Opening: " + rpxRelPrjCtx.get_Name());
                                rpxRelPrj = (ReqPro40.Project___v7)rpxRelPrjCtx.OpenProject(strRelPrjUser, strRelPrjPassword);
                                cb.ShowProgressOpenProject("Opened: " + rpxRelPrj.Name);
                            }
                            catch (System.Runtime.InteropServices.COMException e)
                            {
                                bool bPasswordError;
                                unchecked
                                {
                                    bPasswordError = (e.ErrorCode == (int)0x8004088b);
                                }
                                if (bPasswordError)
                                {
                                    bLoadRelPrj = cb.RequestCredentials(rpxRelPrjCtx.get_Name(),
                                        ref strRelPrjUser, out strRelPrjPassword);
                                }
                                else
                                    bLoadRelPrj = false;
                            }
                        }
                    else
                    {
                        rpxRelPrj = (ReqPro40.Project___v7)rpxRelPrjCtx.ThisProject;
                        cb.ShowProgressOpenProject("Already opened: " + rpxRelPrj.Name);
                    }

                    if (rpxRelPrj != null)
                        arpxRelProjects.Add(rpxRelPrj);
                }
            }
            catch (System.Runtime.InteropServices.COMException e)
            {
                strErrDiag = e.Message;
            }
            return bRet;
        }

        public void CloseProject()
        {
            if (state != eState.stDisc)
            {
                rpxProject.CloseProject();
                state = eState.stDisc;

                foreach (ReqPro40.Project rpxRelPrj in arpxRelProjects)
                    rpxRelPrj.CloseProject();
                arpxRelProjects.Clear();
            }
        }

        public void RefreshProject()
        {
            if ((state == eState.stConn) || (state == eState.stParsed))
            {
                ReqProRequirementPrx.InitCache();
                rpxProject.Refresh(true);
                rpxProject.RefreshAllRelatedProjectContexts();
            }

        }

        public ReqTreeNode ReadReqTree(out int nPackageCount)
        {
            ReqPro40.Package___v1 rpxPackage;
            ReqPro40.RootPackage___v1 rpxRootPackage;
            ReqTreeNode reqRootTreeNode;

            int nPackageRead = 0;

            cb.ShowProgressReqPkgTree(0, 0, 0, 0, "Identifying Packages");
            rpxRootPackage = (ReqPro40.RootPackage___v1)rpxProject.GetRootPackage(true);
            object[,] o = (object[,])rpxRootPackage.
                FindPackageElements(ReqPro40.enumElementTypes.eElemType_Package,
                    "*", 0, ReqPro40.enumElementTypes.eElemType_Package, false);
            nPackageCount = o.GetLength(1);
            o = null;
            cb.ShowProgressReqPkgTree(nPackageCount, 0, 0, 0, nPackageCount + " Packages identified");
            reqRootTreeNode = new ReqTreeNode
                (rpxRootPackage.Name, rpxRootPackage.key, ReqTreeNode.eReqTreeNodeType.eTreeNodeRoot);

            dictPackage = new Dictionary<int, object>();

            rpxRootPackage.Refresh(ReqPro40.enumPackageWeights.ePackageWeight_Package, true);

            rpxRootPackage.MoveFirst();
            while (!rpxRootPackage.IsEOF)
            {
                rpxPackage = (ReqPro40.Package___v1)rpxRootPackage.GetCurrentElement();
                ReadReqTreeNode(rpxPackage, ref reqRootTreeNode, ref nPackageRead);
                rpxRootPackage.MoveNext();
            }

            cb.ShowProgressReqPkgTree(0, 0, 0, 0, nPackageRead + " Packages read");
            ParseRequirements();

            state = eState.stParsed;
            return reqRootTreeNode;
        }

        private void ReadReqTreeNode(ReqPro40.Package___v1 rpxPackage, ref ReqTreeNode reqParentTreeNode, 
            ref int nPackageRead)
        {
            ReqPro40.Package___v1 rpxChildPackage;
            ReqTreeNode reqMyTreeNode;

            reqMyTreeNode = new ReqTreeNode(rpxPackage.Name, rpxPackage.key, ReqTreeNode.eReqTreeNodeType.eTreeNodePkg);
            reqParentTreeNode.Add(ref reqMyTreeNode);
            dictPackage.Add(rpxPackage.key, reqMyTreeNode);
            cb.ShowProgressReqPkgTree(0, ++nPackageRead, 0, 0, null);

            rpxPackage.MoveFirst();
            while (!rpxPackage.IsEOF)
            {
                rpxChildPackage = (ReqPro40.Package___v1)rpxPackage.GetCurrentElement();
                ReadReqTreeNode(rpxChildPackage, ref reqMyTreeNode, ref nPackageRead);
                rpxPackage.MoveNext();
            }
        }

        public void RemovePkgs(System.Collections.ArrayList arrPkgKeys)
        {
            foreach (int nPkgKey in arrPkgKeys)
                dictPackage.Remove (nPkgKey);
        }

        public ReqTreeNode ReadReqTree(out int nPackageCount, int nStartKey)
        {
            ReqPro40.Package___v1 rpxPkg;
            // root is just a dummy - will not be used at the end
            ReqTreeNode reqDummy = new ReqTreeNode ("", 0, ReqTreeNode.eReqTreeNodeType.eTreeNodeRoot);

            nPackageCount = 0;
            rpxPkg = (ReqPro40.Package___v1)rpxProject.GetPackage(nStartKey, ReqPro40.enumPackageWeights.ePackageWeight_Empty);
            rpxPkg.Refresh(ReqPro40.enumPackageWeights.ePackageWeight_Package, true);
            ReadReqTreeNode(rpxPkg, ref reqDummy, ref nPackageCount);
            ParseRequirements(rpxPkg, reqDummy[0]);
            return reqDummy[0];
        }

        private void ParseRequirements()
        {
            ReqPro40.Requirements rpxReqColl;
            ReqPro40.Requirement___v6 rpxReq;
            ReqPro40.Relationship reqRelParent;

            int nReqCount;
            ReqTreeNode tnPackage;
            ReqTreeNode reqMyTreeNode;
            List<ReqTreeNode> reqUnassigned;
            Dictionary<int, ReqTreeNode> dictReq;
            int i = 0;

            reqUnassigned = new List<ReqTreeNode>();
            dictReq = new Dictionary<int, ReqTreeNode>();

            cb.ShowProgressReqPkgTree(0, 0, 0, 0, "Identifying Requirements");
            rpxReqColl = rpxProject.GetRequirements("*", ReqPro40.enumRequirementsLookups.eReqsLookup_All,
                ReqPro40.enumRequirementsWeights.eReqWeight_Medium, ReqPro40.enumRequirementFlags.eReqFlag_Empty, 1000, 8);
            ReqProRequirementPrx.RPXReqColl = rpxReqColl;

            nReqCount = rpxReqColl.Count;
            cb.ShowProgressReqPkgTree(0, 0, nReqCount, 0, nReqCount + " Requirements identified");
            foreach (object o in rpxReqColl)
            {
                Tracer tracer = new Tracer("Req (Key: " + o + ") from Req Coll");
                rpxReq = (ReqPro40.Requirement___v6)rpxReqColl[o, ReqPro40.enumRequirementLookups.eReqLookup_Key];
                tracer.Stop("Req " + rpxReq.get_Tag(ReqPro40.enumTagFormat.eTagFormat_Tag) +
                    " got via eReqLookup_Key from Requirements Collection");
                try
                {
                    i++;
                    cb.ShowProgressReqPkgTree(0, 0, 0, i, null);
                    tnPackage = (ReqTreeNode)dictPackage[rpxReq.PackageKey];

                    reqMyTreeNode = new ReqTreeNode
                        (rpxReq.get_Tag(ReqPro40.enumTagFormat.eTagFormat_Tag)+": "+rpxReq.Name,
                         rpxReq.key, ReqTreeNode.eReqTreeNodeType.eTreeNodeReq);
                    if (rpxReq.IsRoot)
                    {
                        if (tnPackage != null)
                            tnPackage.Add(ref reqMyTreeNode);
                        dictReq.Add(rpxReq.key, reqMyTreeNode);
                    }
                    else
                    {
                        //Tracer tr = new Tracer("Parent of " + reqMyTreeNode.Text + " discovering");

                        // performance is too low for that step :-(
                        //reqRelParent = rpxReq.get_Parent(ReqPro40.enumRequirementsWeights.eReqWeight_Medium);
                        //reqMyTreeNode.Parent = reqRelParent.SourceKey;
                        //reqUnassigned.Add(reqMyTreeNode);
                        //tr.Stop("Parent of " + reqMyTreeNode.Text + " discovered");

                        if (tnPackage != null)
                            tnPackage.Add(ref reqMyTreeNode);
                        dictReq.Add(rpxReq.key, reqMyTreeNode);
                    }

                }
                catch (Exception)
                {
                }
            }

            Tracer trCleanup = new Tracer("Cleanup Children Requirements");
            while (reqUnassigned.Count > 0)
                for (int j=0; j<reqUnassigned.Count; j++)
                {
                    reqMyTreeNode = reqUnassigned[j];
                    if (dictReq.ContainsKey(reqMyTreeNode.Parent))
                    {
                        tnPackage = dictReq[reqMyTreeNode.Parent];

                        tnPackage.Add(ref reqMyTreeNode);
                        dictReq.Add(reqMyTreeNode.Key, reqMyTreeNode);
                        reqUnassigned.Remove(reqMyTreeNode);
                        j--;
                    }
                }
            trCleanup.Stop("Cleanup Children Requirements");

            cb.ShowProgressReqPkgTree(0, 0, 0, 0, i + " Requirements inserted");
            cb.ShowProgressReqPkgTree(0, 0, 0, 0, null);
        }

        private void ParseRequirements(ReqPro40.Package___v1 rpxPkg, ReqTreeNode tn)
        {
            ReqPro40.Requirement___v6 rpxReq;
            ReqPro40.Package___v1 rpxPkgChild;
            ReqTreeNode tnPkgChild;
            ReqTreeNode tnReqNew;
            int nKey;
            int i;

            object[,] o = (object[,])rpxPkg.KeyList (ReqPro40.enumElementTypes.eElemType_Requirement);
            for (i=0; i<o.GetLength(1); i++)
            {
                if (o[0, i] != null)
                {
                    nKey = (int)o[0, i];
                    rpxReq = GetRequirement(nKey);
                    tnReqNew = new ReqTreeNode
                        (rpxReq.get_Tag(ReqPro40.enumTagFormat.eTagFormat_Tag) + ": " + rpxReq.Name,
                         rpxReq.key, ReqTreeNode.eReqTreeNodeType.eTreeNodeReq);
                    tn.Add(ref tnReqNew);
                    ParseReqChildren (rpxReq, tnReqNew);
                }
            }

            for (i = 0; i < tn.Count; i++)
            {
                tnPkgChild = tn[i];
                if (tnPkgChild.IsPackage())
                {
                    rpxPkgChild = (ReqPro40.Package___v1)rpxProject.GetPackage(tnPkgChild.Key, ReqPro40.enumPackageWeights.ePackageWeight_Empty);
                    //tnPkgChild = (ReqTreeNode)dictPackage[rpxPkgChild.PackageKey];
                    ParseRequirements(rpxPkgChild, tnPkgChild);
                }
            }
        }

        private void ParseReqChildren(ReqPro40.Requirement___v6 rpxReq, ReqTreeNode tn)
        {
            ReqPro40.Relationships rpxReqChildrenRelation;
            ReqPro40.Requirement___v6 rpxReqChild;
            ReqTreeNode tnReqNew;
            int nChildCount;
            
            rpxReqChildrenRelation = rpxReq.Children;
            nChildCount = rpxReqChildrenRelation.Count;
            rpxReqChildrenRelation.MoveFirst ();

            for (int i = 0; i < nChildCount; i++)
            {
                rpxReqChild = GetRequirement(rpxReqChildrenRelation.ItemCurrent.DestinationKey);
                tnReqNew = new ReqTreeNode
                    (rpxReqChild.get_Tag(ReqPro40.enumTagFormat.eTagFormat_Tag) + ": " + rpxReqChild.Name,
                     rpxReqChild.key, ReqTreeNode.eReqTreeNodeType.eTreeNodeReq);
                ParseReqChildren(rpxReqChild, tnReqNew);
                tn.Add(ref tnReqNew);
                rpxReqChildrenRelation.MoveNext();
            }
        }

        public ReqPro40.Requirement___v6 GetRequirement(int nKey)
        {
            //if (this.state == eState.stParsed)
            {
                return (ReqPro40.Requirement___v6)rpxProject.GetRequirement
                    (nKey, ReqPro40.enumRequirementLookups.eReqLookup_Key,
                     ReqPro40.enumRequirementsWeights.eReqWeight_Medium,
                     ReqPro40.enumRequirementFlags.eReqFlag_Empty);
            }
            return null;
        }

        public ReqProRequirementPrx GetRequirementPrx(int nKey)
        {
            return (new ReqProRequirementPrx(rpxProject, nKey));
        }

        public void GetProjectReqType(out string strPrjPrefix, out string[] astrReqType, out int[] anReqTypeKey)
        {
            GetProjectReqType (rpxProject, out strPrjPrefix, out astrReqType, out anReqTypeKey);
        }

        public void GetRelProjectReqType(out string[] astrPrjPrefix, out string[][] aastrReqType, out int[][] aanReqTypeKey)
        {
            int nRelPrjCnt = arpxRelProjects.Count;

            astrPrjPrefix = new string[nRelPrjCnt];
            aastrReqType = new string[nRelPrjCnt][];
            aanReqTypeKey = new int[nRelPrjCnt][];

            for (int i=0; i < nRelPrjCnt; i++)
                GetProjectReqType((ReqPro40.Project___v7)arpxRelProjects[i], 
                    out astrPrjPrefix[i], out aastrReqType[i], out aanReqTypeKey[i]);
        }

        private void GetProjectReqType(ReqPro40.Project___v7 rpxPrj, out string strPrjPrefix,
            out string[] astrReqType, out int[] anReqTypeKey)
        {
            int nCount;
            ReqPro40.ReqType rpxReqType;
            ReqPro40.ReqTypes rpxReqTypes = rpxPrj.ReqTypes;
            strPrjPrefix = rpxPrj.Prefix;

            nCount = rpxReqTypes.Count;
            astrReqType = new string [nCount];
            anReqTypeKey = new int [nCount];

            for (int i=0; i<nCount; i++)
            {
                rpxReqType = rpxReqTypes[i+1, ReqPro40.enumReqTypesLookups.eReqTypesLookups_Index];
                astrReqType[i] = rpxReqType.ReqPrefix + ": " + rpxReqType.Name;
                anReqTypeKey[i] = rpxReqType.key;
            }
        }

        public void FindRequirements(ArrayList anKeys, string strSearchExpr, bool bUseRegEx,
                    bool bSearchTag, bool bSearchName, bool bSearchText, 
                    bool bIgnoreCase,
                    out SearchResult [] [] searchResult, out int [] anKeysOut)
        {
            ReqProRequirementPrx reqPrx;
            bool bFound;
            SearchResult [] res;
            string strCmpStr;
            ArrayList listKeysOut = new ArrayList();
            List <SearchResult[]> listSResult = new List<SearchResult[]>();

            res = new SearchResult [3];

            if (!bUseRegEx)
                if (bIgnoreCase)
                    strCmpStr = strSearchExpr.ToLower();
                else
                    strCmpStr = strSearchExpr;
            else
                strCmpStr = strSearchExpr;
            
            foreach (int nKey in anKeys)
            {
                bFound = false;
                reqPrx = GetRequirementPrx (nKey);
                if (reqPrx != null)
                {
                    res[0] = new SearchResult (reqPrx.Tag);
                    if (bSearchTag)
                        bFound |= FindInText (ref res[0], strCmpStr, bUseRegEx, bIgnoreCase);
                    res[1] = new SearchResult (reqPrx.Name);
                    if (bSearchTag)
                        bFound |= FindInText (ref res[1], strCmpStr, bUseRegEx, bIgnoreCase);
                    res[2] = new SearchResult (reqPrx.Text);
                    if (bSearchText)
                        bFound |= FindInText (ref res[2], strCmpStr, bUseRegEx, bIgnoreCase);
                    if (bFound)
                    {
                        listKeysOut.Add (nKey);
                        listSResult.Add (res);
                        res = new SearchResult[3];
                    }
                }
            }
            searchResult = listSResult.ToArray();

            anKeysOut = new int[listKeysOut.Count];
            for (int i = 0; i < listKeysOut.Count; i++)
                anKeysOut[i] = (int)listKeysOut[i];
        }

        private static bool FindInText(ref SearchResult searchResult, string strSearchExpr, 
            bool bUseRegEx, bool bIgnoreCase)
        {
            int nIdx=-1;
            string strCmpText;

            if (bIgnoreCase) 
                strCmpText = searchResult.Text.ToLower();
            else
                strCmpText = searchResult.Text;

            if (bUseRegEx)
                MessageBox.Show ("not yet implemented");
            else
                nIdx = strCmpText.IndexOf (strSearchExpr);
            searchResult.Index = nIdx;

            if (nIdx == -1)
            {
                searchResult.Length = 0;
                return false;
            }
            else
            {
                searchResult.Length = strSearchExpr.Length;
                return true;
            }
        }
    }
}
