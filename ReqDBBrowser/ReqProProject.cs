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

        ReqPro40.Application rpxApplication;
        ReqPro40.Project rpxProject;
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

        public ReqProProject (IReqProProjectCb cb)
        {
            state = eState.stDisc;
            this.cb = cb;

            try
            {
                //MessageBox.Show("Ctor of ReqProProject");
                arpxRelProjects = new System.Collections.ArrayList();
                //MessageBox.Show("Empty List of Related Projects created");
                rpxApplication = new ReqPro40.Application();
                //MessageBox.Show("rpxApplication created");
                rpxCatalog = rpxApplication.PersonalCatalog;
                //MessageBox.Show("RPX Catalog created");
            }
            catch (System.Runtime.InteropServices.COMException e)
            {
                MessageBox.Show(e.Message + "\r\n\r\nStack: " + e.StackTrace + "\r\n\r\nin: " + e.TargetSite, 
                    "COM Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message + "\r\n\r\nStack: " + e.StackTrace + "\r\n\r\nin: " + e.TargetSite, 
                    "Error", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
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
                rpxProject = rpxApplication.OpenProject(strProject,
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
                ReqPro40.Project rpxRelPrj;

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
                                rpxRelPrj = rpxRelPrjCtx.OpenProject(strRelPrjUser, strRelPrjPassword);
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
                        rpxRelPrj = rpxRelPrjCtx.ThisProject;
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
            ReqPro40.Package rpxPackage;
            ReqPro40.RootPackage rpxRootPackage;
            ReqTreeNode reqRootTreeNode;

            int nPackageRead = 0;

            cb.ShowProgressReqPkgTree(0, 0, 0, 0, "Identifying Packages");
            rpxRootPackage = rpxProject.GetRootPackage(true);
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
                rpxPackage = (ReqPro40.Package)rpxRootPackage.GetCurrentElement();
                ReadReqTreeNode(rpxPackage, ref reqRootTreeNode, ref nPackageRead);
                rpxRootPackage.MoveNext();
            }

            cb.ShowProgressReqPkgTree(0, 0, 0, 0, nPackageRead + " Packages read");
            ParseRequirements();

            state = eState.stParsed;
            return reqRootTreeNode;
        }

        private void ReadReqTreeNode(ReqPro40.Package rpxPackage, ref ReqTreeNode reqParentTreeNode, 
            ref int nPackageRead)
        {
            ReqPro40.Package rpxChildPackage;
            ReqTreeNode reqMyTreeNode;

            reqMyTreeNode = new ReqTreeNode(rpxPackage.Name, rpxPackage.key, ReqTreeNode.eReqTreeNodeType.eTreeNodePkg);
            reqParentTreeNode.Add(ref reqMyTreeNode);
            dictPackage.Add(rpxPackage.key, reqMyTreeNode);
            cb.ShowProgressReqPkgTree(0, ++nPackageRead, 0, 0, null);

            rpxPackage.MoveFirst();
            while (!rpxPackage.IsEOF)
            {
                rpxChildPackage = (ReqPro40.Package)rpxPackage.GetCurrentElement();
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
            ReqPro40.Package rpxPkg;
            // root is just a dummy - will not be used at the end
            ReqTreeNode reqDummy = new ReqTreeNode ("", 0, ReqTreeNode.eReqTreeNodeType.eTreeNodeRoot);

            nPackageCount = 0;
            rpxPkg = rpxProject.GetPackage (nStartKey, ReqPro40.enumPackageWeights.ePackageWeight_Empty);
            rpxPkg.Refresh(ReqPro40.enumPackageWeights.ePackageWeight_Package, true);
            ReadReqTreeNode(rpxPkg, ref reqDummy, ref nPackageCount);
            ParseRequirements(rpxPkg, reqDummy[0]);
            return reqDummy[0];
        }

        private void ParseRequirements()
        {
            ReqPro40.Requirements rpxReqColl;
            ReqPro40.Requirement rpxReq;
            int nReqCount;
            ReqTreeNode tnPackage;
            ReqTreeNode reqMyTreeNode;
            int i = 0;

            cb.ShowProgressReqPkgTree(0, 0, 0, 0, "Identifying Requirements");
            rpxReqColl = rpxProject.GetRequirements("*", ReqPro40.enumRequirementsLookups.eReqsLookup_All,
                ReqPro40.enumRequirementsWeights.eReqWeight_Medium, ReqPro40.enumRequirementFlags.eReqFlag_Empty, 1000, 8);
            ReqProRequirementPrx.RPXReqColl = rpxReqColl;

            nReqCount = rpxReqColl.Count;
            cb.ShowProgressReqPkgTree(0, 0, nReqCount, 0, nReqCount + " Requirements identified");
            foreach (object o in rpxReqColl)
            {
                Tracer tracer = new Tracer("Req (Key: " + o + ") from Req Coll");
                rpxReq = rpxReqColl[o, ReqPro40.enumRequirementLookups.eReqLookup_Key];
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
                    if (tnPackage != null)
                        tnPackage.Add(ref reqMyTreeNode);
                }
                catch (Exception)
                {
                }

            }
            cb.ShowProgressReqPkgTree(0, 0, 0, 0, i + " Requirements inserted");
            cb.ShowProgressReqPkgTree(0, 0, 0, 0, null);
        }

        private void ParseRequirements(ReqPro40.Package rpxPkg, ReqTreeNode tn)
        {
            ReqPro40.Requirement rpxReq;
            ReqPro40.Package rpxPkgChild;
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
                }
            }

            for (i = 0; i < tn.Count; i++)
            {
                tnPkgChild = tn[i];
                if (tnPkgChild.IsPackage())
                {
                    rpxPkgChild = rpxProject.GetPackage(tnPkgChild.Key, ReqPro40.enumPackageWeights.ePackageWeight_Empty);
                    //tnPkgChild = (ReqTreeNode)dictPackage[rpxPkgChild.PackageKey];
                    ParseRequirements(rpxPkgChild, tnPkgChild);
                }
            }
        }

        public ReqPro40.Requirement GetRequirement(int nKey)
        {
            if (this.state == eState.stParsed)
            {
                return rpxProject.GetRequirement
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
                GetProjectReqType((ReqPro40.Project)arpxRelProjects[i], 
                    out astrPrjPrefix[i], out aastrReqType[i], out aanReqTypeKey[i]);
        }

        private void GetProjectReqType(ReqPro40.Project rpxPrj, out string strPrjPrefix,
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
                    out SearchResult [] [] searchResult)
        {
            ReqProRequirementPrx reqPrx;
            bool bFound;
            SearchResult [] res;
            string strCmpStr;
            ArrayList anKeysOut = new ArrayList();
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
                        anKeysOut.Add (nKey);
                        listSResult.Add (res);
                        res = new SearchResult[3];
                    }
                }
            }
            searchResult = listSResult.ToArray();
        }

        private static bool FindInText (ref SearchResult searchResult, string strSearchExpr, 
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
