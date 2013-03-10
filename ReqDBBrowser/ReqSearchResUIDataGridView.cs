using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DataGridViewExt;
using System.Drawing;

namespace ReqDBBrowser
{
    class ReqSearchResUIDataGridView : DataGridView
    {
        ISearchResCb cb;
        ContextMenuStrip mnuCtxRow;
        DataGridViewCellEventArgs locMouse;

        public interface ISearchResCb
        {
            void GetRowCtxMenu(out string[] astrMnuEntry);
            void RowMenuAction(int nActKey, int[] nSelKeys, int[] nMarkedKeys, int nMenuItem, string strMenuText);
        }

        public ReqSearchResUIDataGridView(int nWidth, ReqSearchResUIDataGridView dataGridOld, ISearchResCb cb)
        {
            DataGridViewExt.DataGridViewRichTextBoxColumn lbCol;
            this.cb = cb;
            string[] astrMnuEntry;
            ToolStripMenuItem tsMnuItem;
            int nMnuItem;

            nWidth -= RowHeadersWidth;

            Location = new Point(0, 0);
            Dock = DockStyle.Fill;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;

            lbCol = new DataGridViewRichTextBoxColumn();
            lbCol.Name = "Tag";
            Columns.Add(lbCol);

            lbCol = new DataGridViewRichTextBoxColumn();
            lbCol.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            lbCol.Name = "Name";
            Columns.Add(lbCol);

            lbCol = new DataGridViewRichTextBoxColumn();
            lbCol.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            lbCol.Name = "Text";
            Columns.Add(lbCol);

            if (dataGridOld != null)
                for (int i = 0; i < ColumnCount; i++)
                    Columns[i].Width = dataGridOld.Columns[i].Width;

            DataGridViewRow row = RowTemplate;
            row.Height = Font.Height * 11 / 2;

            ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;

            // Create the ContextMenuStrip.
            mnuCtxRow = new ContextMenuStrip();
            cb.GetRowCtxMenu(out astrMnuEntry);
            nMnuItem = 0;
            foreach (string str in astrMnuEntry)
            {
                tsMnuItem = new ToolStripMenuItem(str, null, mnuCtxRow_Click);
                tsMnuItem.Tag = nMnuItem++;
                mnuCtxRow.Items.Add(tsMnuItem);
            }
            //mnuCtxRow.Items.Add(new ToolStripSeparator());
            //mnuCtxRow.Items.Add(new ToolStripMenuItem("Copy", null, mnuCpy_Click));

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


        
    }
}
