using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Collections;

namespace ReqDBBrowser
{
    class ReqTraceUIDataGridView: DataGridView
    {
        ArrayList arrLBTracesTo;
        ArrayList arrLBTracesFrom;
        int nOldWidth;

        public ReqTraceUIDataGridView(int nWidth, ReqTraceUIDataGridView dataGridOld)
        {
            arrLBTracesFrom = new ArrayList();
            arrLBTracesTo = new ArrayList();

            nWidth -= RowHeadersWidth;

            ColumnCount = 5;
            Location = new Point(0, 0);
            Dock = DockStyle.Fill;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;

            Columns[0].Name = "Tag";
            Columns[0].Visible = false;
            Columns[0].Width = 0;
            Columns[1].Name = "Name";
            Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            Columns[1].Width = nWidth / 10;
            Columns[2].Name = "Text";
            Columns[2].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            Columns[2].Width = nWidth / 2;
            Columns[3].Name = "Trace to";
            Columns[3].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            Columns[3].Width = nWidth / 5;
            Columns[4].Name = "Trace from";
            Columns[4].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            Columns[4].Width = nWidth / 5;

            if (dataGridOld != null)
                for (int i = 0; i < ColumnCount; i++)
                    Columns[i].Width = dataGridOld.Columns[i].Width;

            DataGridViewRow row = RowTemplate;
            row.Height = Font.Height * 11 / 2;

        }

        public void AddRow(ReqTraceGrid.ReqTraceNode reqTraceNode)
        {
            ListBox lb;
            Rectangle rect;
            DataGridViewRow row;
            string[] astrReq;

            string strTraceTo = "";
            string strTraceFrom = "";
            List<string> arrTracesTo;
            List<string> arrTracesFrom;

            reqTraceNode.GetTraceToString(out arrTracesTo);
            foreach (string str in arrTracesTo)
                strTraceTo += str + "\r\n";

            reqTraceNode.GetTraceFromString(out arrTracesFrom);
            foreach (string str in arrTracesFrom)
                strTraceFrom += str + "\r\n";

            astrReq = new string[] 
                        {
                            reqTraceNode.Tag,
                            reqTraceNode.TagName,
                            reqTraceNode.Text,
                        };

            row = new DataGridViewRow();
            row.CreateCells(this, astrReq);
            if (reqTraceNode.IsRootNode)
                row.Cells[1].Style.BackColor = Color.PaleGreen;
            row.Height = Font.Height * 11 / 2;
            Rows.Add(row);
            
            lb = new ListBox();
            foreach (string str in arrTracesTo)
                lb.Items.Add(str);
            lb.Tag = row.Index;
            Controls.Add(lb);
            arrLBTracesTo.Add(lb);
            lb.BorderStyle = BorderStyle.None;
            lb.SelectedIndexChanged += lbTraces_SelectedIndexChanged;

            lb = new ListBox();
            foreach (string str in arrTracesFrom)
                lb.Items.Add(str);
            lb.Tag = row.Index;
            Controls.Add(lb);
            arrLBTracesFrom.Add(lb);
            lb.BorderStyle = BorderStyle.None;
            lb.SelectedIndexChanged += lbTraces_SelectedIndexChanged;
        }

        public void Populated ()
        {
            AdaptListBoxLayout();
        }

        private void AdaptListBoxLayout()
        {
            SuspendLayout();
            AdaptListBoxLayout (arrLBTracesFrom, 4);
            AdaptListBoxLayout (arrLBTracesTo, 3);
            ResumeLayout();
        }

        private void AdaptListBoxLayout(ArrayList arrLBTraces, int nCol)
        {
            Rectangle rect;
            foreach (ListBox lb in arrLBTraces)
            {
                rect = GetCellDisplayRectangle(nCol, (int)lb.Tag, true);
                lb.Location = rect.Location;
                rect.Width -= 2;
                lb.Size = rect.Size;
            }
        }

        protected override void OnScroll(ScrollEventArgs e)
        {
            base.OnScroll(e);
            AdaptListBoxLayout();
        }

        protected override void OnColumnWidthChanged(DataGridViewColumnEventArgs e)
        {
            base.OnColumnWidthChanged(e);
            AdaptListBoxLayout();
        }

        public void ParentSizeChanged(int nNewWidth)
        {
            int nInc = nNewWidth - nOldWidth;
            if (nInc > 0)
            {
                int nCurrentWidth = 0;
                for (int i = 0; i < Columns.Count; i++)
                    nCurrentWidth += Columns[i].Width;
                nCurrentWidth += RowHeadersWidth;

                if (nCurrentWidth < nOldWidth)
                {
                    Columns[2].Width += nInc / 2;
                    Columns[1].Width += nInc / 10;
                    Columns[3].Width += nInc / 5;
                    Columns[4].Width += nInc / 5;
                    AdaptListBoxLayout();
                }
            }
            nOldWidth = nNewWidth;
        }

        private void lbTraces_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            ListBox lb;
            string strTagReqTrace;
            int nFoundIndex = -1;


            lb = (ListBox)sender;
            if (lb.SelectedIndex == -1)
                return;

            strTagReqTrace = (string)(lb.SelectedItem);

            foreach (DataGridViewRow row in Rows)
            {
                if ((string)(row.Cells[0].Value) == strTagReqTrace)
                {
                    nFoundIndex = row.Index;
                    break;
                }
            }

            if (nFoundIndex != -1)
            {
                foreach (DataGridViewCell cell in SelectedCells)
                {
                    cell.Selected = false;
                }

                Rows[nFoundIndex].Selected = true;
                FirstDisplayedScrollingRowIndex = nFoundIndex;
                /* don't know why - but Scrolled Event is not fired... */
                OnScroll(null);
            }
            else
                MessageBox.Show("Could not select " + strTagReqTrace);


            lb.SelectedIndex = -1;
        }

    }
}
