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
        int nOldWidth;
        ITraceViewGridCb cb;
        ContextMenuStrip mnuCtxRow;
        DataGridViewCellEventArgs locMouse;

        class DataGridViewListBoxColumn : DataGridViewColumn
        {
            public DataGridViewListBoxColumn()
                : base(new DataGridViewListBoxCell())
            {
            }
            public override DataGridViewCell CellTemplate
            {
                get
                {
                    return base.CellTemplate;
                }
                set
                {
                    // Ensure that the cell used for the template is a DataGridViewListBoxCell. 
                    if (value != null &&
                        !value.GetType().IsAssignableFrom(typeof(DataGridViewListBoxCell)))
                    {
                        throw new InvalidCastException("Must be a DataGridViewListBoxCell");
                    }
                    base.CellTemplate = value;
                }
            }
        }

        class MyListBox : ListBox
        {
            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
            }

            protected override void OnMouseDown(MouseEventArgs e)
            {
                base.OnMouseDown(e);
            }

            protected override void OnLayout(LayoutEventArgs levent)
            {
                base.OnLayout(levent);
            }
        }

        class DataGridViewListBoxCell : DataGridViewCell
        {
            MyListBox lb;
            bool bIsCreated;
            Rectangle rectOldCellBounds;
            Point pOldScrollPos;

            public DataGridViewListBoxCell():base ()
            {
                lb = new MyListBox();
                lb.MaximumSize = new Size(1000, 1000);
                bIsCreated = false;
            }

            public override object Clone()
            {
                //lb.Clone ();
                return base.Clone();
            }

            public ListBox.ObjectCollection Items
            { get { return lb.Items; } }

            public override Type ValueType
            { get { return typeof(MyListBox); } }

            protected override object GetFormattedValue(object value, int rowIndex, 
                ref DataGridViewCellStyle cellStyle, 
                System.ComponentModel.TypeConverter valueTypeConverter, 
                System.ComponentModel.TypeConverter formattedValueTypeConverter, 
                DataGridViewDataErrorContexts context)
            {
                return null;
            }

            protected override void Paint(Graphics graphics, Rectangle clipBounds, 
                Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, 
                object value, object formattedValue, string errorText, 
                DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, 
                DataGridViewPaintParts paintParts)
            {
                Point pScrollPos = new Point (DataGridView.HorizontalScrollingOffset, DataGridView.VerticalScrollingOffset);
                // Calculate the area in which to draw the Listbox.
                if (!bIsCreated)
                {
                    DataGridView.Controls.Add(lb);
                    lb.SelectedIndexChanged += ((ReqTraceUIDataGridView)DataGridView).lbTraces_SelectedIndexChanged;
                    bIsCreated = true;
                }

                System.Diagnostics.Trace.Write
                    ("Updating Cell (" + ColumnIndex + "/" + RowIndex + ") - rowIdx: " + rowIndex + 
                     " | New (" + cellBounds.X + "/" + cellBounds.Y + ")(" + cellBounds.Width + "/" + cellBounds.Height + ")" +
                     " | Old (" + rectOldCellBounds.X + "/" + rectOldCellBounds.Y + ")" +
                     " |  Scroll (" + pScrollPos.X + "/" + pScrollPos.Y + ")" );

                if ((cellBounds != rectOldCellBounds) || (pScrollPos != pOldScrollPos))
                {
                    //int nScrollBarCnt;

                    lb.Location = cellBounds.Location;
                    lb.Size = cellBounds.Size;
                    // this construct does not bring needed functionality
                    //nScrollBarCnt = ((ReqTraceUIDataGridView)DataGridView).GetVisibleScrollBarCount();
                    //lb.Parent.Controls.SetChildIndex(lb, nScrollBarCnt);

                    // still does not work - maybe better than nothing
                    ((ReqTraceUIDataGridView)DataGridView).BringCtrlToFront(lb);

                    System.Diagnostics.Trace.WriteLine(" ==> redrawn (" + lb.Location.X + "/" + lb.Location.Y + ")(" + 
                        lb.Size.Width + "/" + lb.Size.Height + ")");
                    rectOldCellBounds = cellBounds;
                    pOldScrollPos = pScrollPos;
                }
                else
                    System.Diagnostics.Trace.WriteLine("");

                base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
            }
        }

        public interface ITraceViewGridCb
        {
            void GetRowCtxMenu(out string[] astrMnuEntry);
            void RowMenuAction(int nActKey, int[] nSelKeys, int[] nMarkedKeys, int nMenuItem, string strMenuText);
        }

        public ReqTraceUIDataGridView(int nWidth, ReqTraceUIDataGridView dataGridOld, ITraceViewGridCb cb)
        {
            DataGridViewListBoxColumn lbCol;
            this.cb = cb;
            string[] astrMnuEntry;
            ToolStripMenuItem tsMnuItem;
            int nMnuItem;

            nWidth -= RowHeadersWidth;

            ColumnCount = 3;
            Location = new Point(0, 0);
            Dock = DockStyle.Fill;
            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;

            lbCol = new DataGridViewListBoxColumn();
            lbCol.Name = "Trace to";
            Columns.Add(lbCol);

            lbCol = new DataGridViewListBoxColumn();
            lbCol.Name = "Trace from";
            Columns.Add(lbCol);

            Columns[0].Name = "Tag";
            Columns[0].Visible = false;
            Columns[0].Width = 0;
            Columns[1].Name = "Name";
            Columns[1].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            Columns[1].Width = nWidth / 10;
            Columns[2].Name = "Text";
            Columns[2].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            Columns[2].Width = nWidth / 2;
            Columns[3].Width = nWidth / 5;
            Columns[4].Width = nWidth / 5;

            if (dataGridOld != null)
                for (int i = 0; i < ColumnCount; i++)
                    Columns[i].Width = dataGridOld.Columns[i].Width;

            DataGridViewRow row = RowTemplate;
            row.Height = Font.Height * 11 / 2;

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
            //mnuCtxPkg.Items.Add(new ToolStripSeparator());

        }

        public void AddRow(ReqTraceGrid.ReqTraceNode reqTraceNode)
        {
            DataGridViewListBoxCell lbCell;
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
                row.Cells[1].Style.BackColor = Color.FromArgb(200, 255, 200);
            row.Height = Font.Height * 11 / 2;
            row.ContextMenuStrip = mnuCtxRow;
            row.Tag = reqTraceNode.Key;
            Rows.Add(row);

            lbCell = (DataGridViewListBoxCell) row.Cells[3];
            foreach (string str in arrTracesTo)
                lbCell.Items.Add(str);

            lbCell = (DataGridViewListBoxCell)row.Cells[4];
            foreach (string str in arrTracesFrom)
                lbCell.Items.Add (str);
        }

        public void PopulationDone ()
        {
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
                }
            }
            nOldWidth = nNewWidth;
        }

        public int GetVisibleScrollBarCount()
        {
            int nRet = 0;
            if (HorizontalScrollBar.Visible)
                nRet++;
            if (VerticalScrollBar.Visible)
                nRet++;
            return (nRet);
        }

        public void BringCtrlToFront(Control ctrl)
        {
            ctrl.BringToFront();
            if (HorizontalScrollBar.Visible)
                HorizontalScrollBar.BringToFront();
            if (VerticalScrollBar.Visible)
                VerticalScrollBar.BringToFront();
        }

        private void mnuCtxRow_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem mnuItem;
            List<int> arrSelKeys;

            mnuItem = (ToolStripMenuItem)sender;

            arrSelKeys = new List<int> ();
            foreach (DataGridViewRow row in Rows)
                if (row.Selected)
                    arrSelKeys.Add ((int)row.Tag);

            cb.RowMenuAction ((int)Rows[locMouse.RowIndex].Tag, 
                arrSelKeys.ToArray (), null, (int)mnuItem.Tag, mnuItem.Text);
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

        protected override void OnCellMouseEnter(DataGridViewCellEventArgs e)
        {
            locMouse = e;
            base.OnCellMouseEnter(e);
        }

        protected override void OnDataError(bool displayErrorDialogIfNoHandler, DataGridViewDataErrorEventArgs e)
        {
            base.OnDataError(displayErrorDialogIfNoHandler, e);
        }

    }
}
