using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ReqDBBrowser
{
    public partial class FormProgressReqTraceGrid : Form
    {
        int nSumReq;
        int nSumReadReq;

        public FormProgressReqTraceGrid()
        {
            nSumReq = 0;
            InitializeComponent();
        }

        public bool ShowProgressReqTraceGrid(int nAddReq, int nReadReq, string strLog)
        {
            if (nAddReq > 0)
            {
                nSumReq += nAddReq;
                progressBarReqCnt.Maximum = nSumReq;
            }
            if (nReadReq > 0)
            {
                nSumReadReq += nReadReq;
                if (nSumReadReq <= progressBarReqCnt.Maximum)
                    progressBarReqCnt.Value = nSumReadReq;
                else
                    /* should never happen */
                    progressBarReqCnt.Value = progressBarReqCnt.Maximum;

                textBoxReqCnt.Text = nSumReadReq + " of " + this.nSumReq;
            }
            if (strLog != null)
            {
                textBoxLog.Text += (strLog + "\r\n");
                textBoxLog.Select(textBoxLog.Text.Length, 0);
                textBoxLog.ScrollToCaret();
                textBoxLog.Refresh();
            }
            if ((nAddReq == 0) && (nReadReq == 0) && (strLog == null))
                Close();
            Application.DoEvents();
            return false;
        }
    }
}