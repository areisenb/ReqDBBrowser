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
        ReqTreeNode reqRootTreeNode;
        Dictionary <int, object> dictPackage;

        eState state;

        public ReqProProject ()
        {
            rpxApplication = new ReqPro40.Application ();

            state = eState.stDisc;
        }

        public void OpenProject(string strProject, string strUser, string strPassword)
        {
            if (state != eState.stDisc) {
                CloseProject ();
            }
            rpxProject = rpxApplication.OpenProject(strProject,
                ReqPro40.enumOpenProjectOptions.eOpenProjOpt_RQSFile,
                strUser,
                strPassword,
                ReqPro40.enumProjectFlags.eProjFlag_Normal,
                ReqPro40.enumRelatedProjectOptions.eRelatedProjOption_ConnectAll);
            state = eState.stConn;
        }

        public void CloseProject()
        {
            if (state != eState.stDisc)
            {
                rpxProject.CloseProject();
                state = eState.stDisc;
            }
        }

        public ReqTreeNode ReadReqTree(out int nPackageCount)
        {
            ReqPro40.Package rpxPackage;
            ReqPro40.RootPackage rpxRootPackage;

            rpxRootPackage = rpxProject.GetRootPackage(true);
            object[,] o = (object[,])rpxRootPackage.
                FindPackageElements(ReqPro40.enumElementTypes.eElemType_Package,
                    "*", 0, ReqPro40.enumElementTypes.eElemType_Package, false);
            nPackageCount = o.GetLength(1);
            o = null;
            reqRootTreeNode = new ReqTreeNode(rpxRootPackage.Name);

            dictPackage = new Dictionary<int, object>();

            rpxRootPackage.MoveFirst();
            while (!rpxRootPackage.IsEOF)
            {
                rpxPackage = (ReqPro40.Package)rpxRootPackage.GetCurrentElement();
                ReadReqTreeNode(rpxPackage, ref reqRootTreeNode);
                rpxRootPackage.MoveNext();
            }

            ParseRequirements();

            state = eState.stParsed;
            return reqRootTreeNode;
        }

        private void ReadReqTreeNode(ReqPro40.Package rpxPackage, ref ReqTreeNode reqParentTreeNode)
        {
            ReqPro40.Package rpxChildPackage;
            ReqTreeNode reqMyTreeNode;

            reqMyTreeNode = new ReqTreeNode(rpxPackage.Name + " (" + rpxPackage.key+")");
            reqParentTreeNode.Add(ref reqMyTreeNode);
            dictPackage.Add(rpxPackage.key, reqMyTreeNode);

            rpxPackage.MoveFirst();
            while (!rpxPackage.IsEOF)
            {
                rpxChildPackage = (ReqPro40.Package)rpxPackage.GetCurrentElement();
                ReadReqTreeNode(rpxChildPackage, ref reqMyTreeNode);
                rpxPackage.MoveNext();
            }
        }

        private void ParseRequirements()
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

            rpxReqColl = rpxProject.GetRequirements("*", ReqPro40.enumRequirementsLookups.eReqsLookup_All,
                ReqPro40.enumRequirementsWeights.eReqWeight_Medium, ReqPro40.enumRequirementFlags.eReqFlag_Empty, 1000, 4);
            nReqCount = rpxReqColl.Count;
            foreach (object o in rpxReqColl)
            {
                rpxReq = rpxReqColl[o, ReqPro40.enumRequirementLookups.eReqLookup_Key];
                try
                {
                    tnPackage = (ReqTreeNode)dictPackage[rpxReq.PackageKey];

                    reqMyTreeNode = new ReqTreeNode(rpxReq.get_Tag(ReqPro40.enumTagFormat.eTagFormat_Tag)+": "+rpxReq.Name);
                    if (tnPackage != null)
                        tnPackage.Add(ref reqMyTreeNode);
                }
                catch (Exception e)
                {
                }

            }

        }
    }
}
