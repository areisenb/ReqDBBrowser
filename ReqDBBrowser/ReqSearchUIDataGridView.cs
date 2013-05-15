using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DataGridViewExt;


namespace ReqDBBrowser
{
    class ReqSearchUIDataGridView : DataGridView
    {

        ISearchResultViewGridCb cb;
        ContextMenuStrip mnuCtxRow;
        DataGridViewCellEventArgs locMouse;

        public interface ISearchResultViewGridCb
        {
            void GetRowCtxMenu(out string[] astrMnuEntry);
            void RowMenuAction(int nActKey, int[] nSelKeys, int[] nMarkedKeys, int nMenuItem, string strMenuText);
        }

        public ReqSearchUIDataGridView(string [] astrHead, int nRowHeightInLines, int nWidth, ISearchResultViewGridCb cb)
        {
            DataGridViewTextBoxColumn dgvTBCol;
            DataGridViewRichTextBoxColumn dgvRTBCol;
            int nColCount;

            string[] astrMnuEntry;
            ToolStripMenuItem tsMnuItem;
            int nMnuItem;

            this.cb = cb;

            Location = new System.Drawing.Point(0, 0);
            Dock = DockStyle.Fill;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;

            dgvTBCol = new DataGridViewTextBoxColumn();
            dgvTBCol.Name = "Hidden Tag";
            dgvTBCol.Visible = false;
            Columns.Add(dgvTBCol);

            nColCount = astrHead.GetLength(0);
            nWidth -= RowHeadersWidth;

            for (int i = 0; i < nColCount; i++)
            {
                dgvRTBCol = new DataGridViewRichTextBoxColumn();
                dgvRTBCol.Name = astrHead[i];
                dgvRTBCol.Width = nWidth / nColCount;
                Columns.Add(dgvRTBCol);
            }

            DataGridViewRow row = RowTemplate;
            row.Height = Font.Height * (2*nRowHeightInLines+1) / 2; // adding half a line :-)

            ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;

            // Create the ContextMenuStrip.
            mnuCtxRow = new ContextMenuStrip();
            cb.GetRowCtxMenu(out astrMnuEntry);
            nMnuItem = 0;
            foreach (string str in astrMnuEntry)
            {
                if (str.Length > 0)
                {
                    tsMnuItem = new ToolStripMenuItem(str, null, mnuCtxRow_Click);
                    tsMnuItem.Tag = nMnuItem;
                    mnuCtxRow.Items.Add(tsMnuItem);
                }
                else
                    mnuCtxRow.Items.Add(new ToolStripSeparator());                
                nMnuItem++;
            }
            mnuCtxRow.Items.Add(new ToolStripSeparator());
            mnuCtxRow.Items.Add(new ToolStripMenuItem("Copy", null, mnuCpy_Click));
        }

        public void Init ()
        {
            Rows.Clear();
        }

        public void AddRow(ReqProProject.SearchResult[] searchResult, int nKey)
        {
            DataGridViewRow row;
            DataGridViewRichTextBoxCell rtbCell;

            RichTextBox rtbTemp = new RichTextBox ();

            Rows.Add();
            row = Rows[RowCount - 1];
            
            for (int i = 0; i < searchResult.GetLength (0); i++)
            {
                rtbTemp.Clear();
                rtbTemp.Text = searchResult[i].Text;
                if (searchResult[i].Index >= 0)
                {
                    rtbTemp.Select(searchResult[i].Index, searchResult[i].Length);
                    rtbTemp.SelectionBackColor = System.Drawing.Color.Yellow;
                }

                rtbCell = (DataGridViewRichTextBoxCell) row.Cells[i+1];
                rtbCell.Value = rtbTemp.Rtf;
            }

            row.ContextMenuStrip = mnuCtxRow;
            row.HeaderCell.ContextMenuStrip = mnuCtxRow;
            row.Tag = nKey;
        }

        private void mnuCtxRow_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mnuItem;
            List<int> arrSelKeys;

            mnuItem = (ToolStripMenuItem)sender;

            arrSelKeys = new List<int>();
            foreach (DataGridViewRow row in Rows)
                if (row.Selected)
                    arrSelKeys.Add((int)row.Tag);

            cb.RowMenuAction((int)Rows[locMouse.RowIndex].Tag,
                arrSelKeys.ToArray(), null, (int)mnuItem.Tag, mnuItem.Text);
        }

        private void mnuCpy_Click(object sender, EventArgs e)
        {
            if (SelectedCells.Count > 0)
                Clipboard.SetDataObject(GetClipboardContent());
        }

        protected override void OnCellMouseEnter(DataGridViewCellEventArgs e)
        {
            locMouse = e;
            base.OnCellMouseEnter(e);
        }
    }
}
