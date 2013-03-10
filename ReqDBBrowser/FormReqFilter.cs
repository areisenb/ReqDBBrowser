using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ReqDBBrowser
{
    public partial class FormReqFilter : Form
    {
        bool bUserChecks;

        public FormReqFilter(string strPrjPrefix, string [] astrRqTypePrj, int [] anReqTypeKey,
            string [] astrRelPrjPrefix, string [][] aastrRqTypeRelPrj, int [][] aanRelPrjReqTypeKey,
            List<int> listnReqTypeRootKeyExcl, List<int> listnReqTypeTracedKeyExcl,
            int nUpCount, int nDownCount, int nMaxTraces, int nMaxUpCount, int nMaxDownCount)
        {
            bUserChecks = false;

            InitializeComponent();
            numUpDwnUpCnt.Value = nUpCount;
            numUpDwnDownCnt.Value = nDownCount;
            numUpDwnMaxTraces.Value = nMaxTraces;
            numUpDwnMaxUpCnt.Value = nMaxUpCount;
            numUpDwnMaxDownCnt.Value = nMaxDownCount;

            treeViewRqRoot.CheckBoxes = true;
            treeViewRqTraced.CheckBoxes = true;
            //tabPageRoot.Name = strPrjPrefix;
            treeViewRqRoot.AfterCheck += new TreeViewEventHandler(treeViewRq_AfterCheck);
            treeViewRqTraced.AfterCheck += new TreeViewEventHandler(treeViewRq_AfterCheck);

            if (strPrjPrefix == null)
                tabControl.TabPages.Remove(tabPageRoot);
            else
                SetReqTypes(strPrjPrefix, astrRqTypePrj, anReqTypeKey, 
                    listnReqTypeRootKeyExcl, treeViewRqRoot);

            if (astrRelPrjPrefix == null)
            {
                tabControl.TabPages.Remove(tabPageTraced);
                tabControl.TabPages.Remove(tabPageParameter);
            }
            else
                for (int i = 0; i < astrRelPrjPrefix.GetLength(0); i++)
                    SetReqTypes(astrRelPrjPrefix[i], aastrRqTypeRelPrj[i], aanRelPrjReqTypeKey[i],
                        listnReqTypeTracedKeyExcl, treeViewRqTraced);

            bUserChecks = true;
        }

        private static void SetReqTypes(string strPrjPrefix, string[] astrRqTypePrj, int[] anReqTypeKey, 
            List<int> listnReqTypeKeyExcl, TreeView tv)
        {
            bool bAllChecked=true;
            bool bAllUnchecked=true;
            TreeNode tn;
            TreeNode tnParent;

            tnParent = new TreeNode(strPrjPrefix);
            tv.Nodes.Add(tnParent);
            for (int i = 0; i < astrRqTypePrj.GetLength(0); i++)
            {
                tn = new TreeNode(astrRqTypePrj[i]);
                tn.Tag = anReqTypeKey[i];
                if (listnReqTypeKeyExcl.Contains((int)tn.Tag))
                {
                    tn.Checked = false;
                    bAllChecked = false;
                }
                else
                {
                    tn.Checked = true;
                    bAllUnchecked = false;
                }
                tnParent.Nodes.Add(tn);
            }
            SetParentCheckState(bAllChecked, bAllUnchecked, tnParent);
        }

        private static void SetParentCheckState(bool bAllChecked, bool bAllUnchecked, TreeNode tnParent)
        {
            tnParent.ForeColor = SystemColors.WindowText;
            if (bAllChecked)
                tnParent.Checked = true;
            else if (bAllUnchecked)
                tnParent.Checked = false;
            else
            {
                tnParent.ForeColor = SystemColors.GrayText;
                tnParent.Checked = true;
            }
        }

        public int UpCount
        {
            get { return (int) numUpDwnUpCnt.Value; }
        }

        public int DownCount
        {
            get { return (int) numUpDwnDownCnt.Value; }
        }

        public int MaxUpCount
        {
            get { return (int) numUpDwnMaxUpCnt.Value; }
        }

        public int MaxDownCount
        {
            get { return (int) numUpDwnMaxDownCnt.Value; }
        }

        public int MaxTraces
        {
            get { return (int) numUpDwnMaxTraces.Value; }
        }

        public List<int> ReqTypeRootKeyExcl
        {
            get { return GetReqTypeKeyExcl(treeViewRqRoot); }
        }

        public List<int> ReqTypeTracedKeyExcl
        {
            get { return GetReqTypeKeyExcl (treeViewRqTraced); }
        }

        private static List<int> GetReqTypeKeyExcl (TreeView tv)
        {
            List<int> ln = new List<int>();
            foreach (TreeNode tnParent in tv.Nodes)
                foreach (TreeNode tn in tnParent.Nodes)
                    if (tn.Checked == false)
                        ln.Add ((int)tn.Tag);
            return (ln);
        }

        void treeViewRq_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (!bUserChecks)
                return;

            bUserChecks = false;

            if (e.Node.Parent == null)
            {
                foreach (TreeNode tn in e.Node.Nodes)
                    tn.Checked = e.Node.Checked;
                e.Node.ForeColor = SystemColors.WindowText;
            }
            else
            {
                bool bAllChecked = true;
                bool bAllUnchecked = true;

                foreach (TreeNode tn in e.Node.Parent.Nodes)
                    if (tn.Checked == true)
                        bAllUnchecked = false;
                    else
                        bAllChecked = false;
                SetParentCheckState(bAllChecked, bAllUnchecked, e.Node.Parent);
            }
            bUserChecks = true;
        }
    }
}