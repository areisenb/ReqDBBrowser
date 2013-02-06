using System;
using System.Collections.Generic;
using System.Text;

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
        ReqPro40.CatalogClass rpxCatalog;

        ReqTreeNode reqRootTreeNode;
        Dictionary <int, object> dictPackage;

        eState state;


        public delegate bool RequestCredentialsCallback(string strProjectDesc, ref string strUser, out string strPassword);
        public delegate void ShowOpenReleatedProjectErrorCallback(string strProjectDesc);
        public delegate bool ShowProgressReqPkgTreeCallback(int nSumTreeElements, int nReadTreeElements, int nSumRequirements, int nReadRequirements, string strLogg);

        public ReqProProject ()
        {
            state = eState.stDisc;

            rpxApplication = new ReqPro40.Application ();
            rpxCatalog = (ReqPro40.CatalogClass) rpxApplication.PersonalCatalog;
            arpxRelProjects = new System.Collections.ArrayList();
        }

        public string[] GetProjectCatalog()
        {
            string[] astrProjects;
            int nCount;
            rpxCatalog = (ReqPro40.CatalogClass) rpxApplication.PersonalCatalog;
            nCount = rpxCatalog.Count;
            astrProjects = new string[nCount];

            for (int i = 0; i < nCount; i++)
            {
                astrProjects[i] = rpxCatalog[i+1, ReqPro40.enumCatalogLookups.eCatLookup_Index].get_Name();
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
            RequestCredentialsCallback requestCredentialCallback, 
            ShowOpenReleatedProjectErrorCallback showOpenReleatedProjectErrorCallback, out string strErrDiag)
        {
            strErrDiag = "";
            bool bRet = false;
            try
            {
                if (state != eState.stDisc)
                {
                    CloseProject();
                }
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
                                rpxRelPrj = rpxRelPrjCtx.OpenProject(strRelPrjUser, strRelPrjPassword);
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
                                    bLoadRelPrj = requestCredentialCallback(rpxRelPrjCtx.get_Name(),
                                        ref strRelPrjUser, out strRelPrjPassword);
                                }
                            }
                        }

                    else
                        rpxRelPrj = rpxRelPrjCtx.ThisProject;

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

        public ReqTreeNode ReadReqTree(out int nPackageCount, ShowProgressReqPkgTreeCallback showProgressReqTreeCallback)
        {
            ReqPro40.Package rpxPackage;
            ReqPro40.RootPackage rpxRootPackage;
            int nPackageRead = 0;

            showProgressReqTreeCallback(0, 0, 0, 0, "Identifying Packages");
            rpxRootPackage = rpxProject.GetRootPackage(true);
            object[,] o = (object[,])rpxRootPackage.
                FindPackageElements(ReqPro40.enumElementTypes.eElemType_Package,
                    "*", 0, ReqPro40.enumElementTypes.eElemType_Package, false);
            nPackageCount = o.GetLength(1);
            o = null;
            showProgressReqTreeCallback(nPackageCount, 0, 0, 0, nPackageCount + " Packages identified");
            reqRootTreeNode = new ReqTreeNode
                (rpxRootPackage.Name, rpxRootPackage.key, ReqTreeNode.eReqTreeNodeType.eTreeNodeRoot);

            dictPackage = new Dictionary<int, object>();

            rpxRootPackage.MoveFirst();
            while (!rpxRootPackage.IsEOF)
            {
                rpxPackage = (ReqPro40.Package)rpxRootPackage.GetCurrentElement();
                ReadReqTreeNode(rpxPackage, ref reqRootTreeNode, ref nPackageRead, showProgressReqTreeCallback);
                rpxRootPackage.MoveNext();
            }

            showProgressReqTreeCallback(0, 0, 0, 0, nPackageRead + " Packages read");
            ParseRequirements(showProgressReqTreeCallback);

            state = eState.stParsed;
            return reqRootTreeNode;
        }

        private void ReadReqTreeNode(ReqPro40.Package rpxPackage, ref ReqTreeNode reqParentTreeNode, 
            ref int nPackageRead, ShowProgressReqPkgTreeCallback showProgressReqTreePkgCallback)
        {
            ReqPro40.Package rpxChildPackage;
            ReqTreeNode reqMyTreeNode;

            reqMyTreeNode = new ReqTreeNode(rpxPackage.Name, rpxPackage.key, ReqTreeNode.eReqTreeNodeType.eTreeNodePkg);
            reqParentTreeNode.Add(ref reqMyTreeNode);
            dictPackage.Add(rpxPackage.key, reqMyTreeNode);
            showProgressReqTreePkgCallback(0, ++nPackageRead, 0, 0, null);

            rpxPackage.MoveFirst();
            while (!rpxPackage.IsEOF)
            {
                rpxChildPackage = (ReqPro40.Package)rpxPackage.GetCurrentElement();
                ReadReqTreeNode(rpxChildPackage, ref reqMyTreeNode, ref nPackageRead, showProgressReqTreePkgCallback);
                rpxPackage.MoveNext();
            }
        }

        private void ParseRequirements(ShowProgressReqPkgTreeCallback showProgressReqTreeCallback)
        {
            //Set a_oReqs = a_oProject.GetRequirements("SRS", eReqsLookup_TagPrefix, eReqWeight_Medium, eReqFlag_Empty, 1000, 2)
            //    nItemCount = a_oReqs.Count
            // For Each vReqKey In a_oReqs
            //        Set a_oReq = a_oReqs.Item(vReqKey, eReqLookup_Key)
            //          ActiveSheet.Cells(i, 1).Value = a_oReq.Tag
            //          ActiveSheet.Cells(i, 2).Value = a_oReq.Name
            //          ActiveSheet.Cells(i, 3).Value = a_oReq.Text
        
            ReqPro40.Requirements rpxReqColl;
            ReqPro40.Requirement rpxReq;
            int nReqCount;
            ReqTreeNode tnPackage;
            ReqTreeNode reqMyTreeNode;
            int i = 0;

            showProgressReqTreeCallback(0, 0, 0, 0, "Identifying Requirements");
            rpxReqColl = rpxProject.GetRequirements("*", ReqPro40.enumRequirementsLookups.eReqsLookup_All,
                ReqPro40.enumRequirementsWeights.eReqWeight_Medium, ReqPro40.enumRequirementFlags.eReqFlag_Empty, 1000, 4);
            nReqCount = rpxReqColl.Count;
            showProgressReqTreeCallback(0, 0, nReqCount, 0, nReqCount + " Requirements identified");
            foreach (object o in rpxReqColl)
            {
                rpxReq = rpxReqColl[o, ReqPro40.enumRequirementLookups.eReqLookup_Key];
                try
                {
                    i++;
                    showProgressReqTreeCallback(0, 0, 0, i, null);
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
            showProgressReqTreeCallback(0, 0, 0, 0, i + " Requirements inserted");
            showProgressReqTreeCallback(0, 0, 0, 0, null);
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
    }
}
