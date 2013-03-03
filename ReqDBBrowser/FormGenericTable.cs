using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ReqDBBrowser
{
    public partial class FormGenericTable : Form
    {
        DataGridView dataGrid;
        int nYOffsetDataGrid;
        ContextMenuStrip mnuCtxRowHeader;
        private DataGridViewCellEventArgs mouseLocation;

        public FormGenericTable(string strCaption, string [] astrHead, float [] afLines)
        {
            TextBox tb;
            int ntbHeigth;
            
            InitializeComponent();
            Text = strCaption;

            nYOffsetDataGrid = toolStripMain.Location.Y + toolStripMain.Size.Height;

            if (astrHead != null)
                for (int i = 0; i < astrHead.GetLength(0); i++)
                {
                    tb = new TextBox();
                    tb.Text = astrHead[i];
                    tb.ReadOnly = true;
                    tb.Multiline = true;
                    tb.ScrollBars = ScrollBars.Vertical;
                    ntbHeigth = (int)(tb.Font.Height * afLines[i]);
                    tb.Location = new Point(0, nYOffsetDataGrid);
                    tb.Size = new Size(ClientSize.Width, ntbHeigth);
                    nYOffsetDataGrid += tb.Size.Height;
                    Controls.Add(tb);
                }
        }

        public void SetGridContent(string[,] astrContent, string strStatusLineLabel)
        {
            int nCols;
            int nRows;

            if (dataGrid != null)
                Controls.Remove (dataGrid);
            dataGrid = new DataGridView();
            string [] strRow;

            nCols = astrContent.GetLength (1);
            nRows = astrContent.GetLength (0);

            SetGridLayout();
            //dataGrid.Dock = DockStyle.Top;
            dataGrid.AllowUserToAddRows = false;
            dataGrid.AllowUserToDeleteRows = false;
            dataGrid.ColumnCount = nCols;
            DataGridViewRow row = dataGrid.RowTemplate;
            row.Height = dataGrid.Font.Height * 11 / 2;

            for (int i=0; i<nCols; i++)
            {
                dataGrid.Columns[i].Name = astrContent[0,i];
                dataGrid.Columns[i].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
            }

            dataGrid.Columns[0].Frozen = true;

            for (int j = 1; j < nRows; j++)
            {
                strRow = new string[nCols];
                for (int i = 0; i < nCols; i++)
                    strRow[i] = astrContent[j, i];
                dataGrid.Rows.Add(strRow);
            }

            Controls.Add (dataGrid);
            toolStripStatusLabel1.Text = strStatusLineLabel;
            dataGrid.CellMouseEnter += dataGrid_CellMouseEnter;
            AddContextMenu();
        }

        private void FormGenericTable_Resize(object sender, EventArgs e)
        {
            SetGridLayout();
            foreach (Control ct in Controls)
                if (ct is TextBox)
                    ct.Width = ClientSize.Width;
        }

        private void SetGridLayout()
        {
            if (dataGrid != null)
            {
                dataGrid.Location = new Point (0, nYOffsetDataGrid);
                dataGrid.Size = new Size(ClientSize.Width, ClientSize.Height - nYOffsetDataGrid - statusStrip.Height);
            }
        }

        private void AddContextMenu()
        {
            mnuCtxRowHeader = new ContextMenuStrip();

            mnuCtxRowHeader.Items.Add(new ToolStripMenuItem ("Hide Column", null, mnuCtxRowHeaderHideCol_Click));
            mnuCtxRowHeader.Items.Add(new ToolStripMenuItem("Show all Columns", null, mnuCtxRowHeaderShowAllCols_Click));
            mnuCtxRowHeader.Items.Add(new ToolStripMenuItem("Columns...", null, mnuCtxRowHeaderCols_Click));
            foreach (DataGridViewColumn dgvCol in dataGrid.Columns)
                dgvCol.HeaderCell.ContextMenuStrip = mnuCtxRowHeader;
        }



        // Deal with hovering over a cell.
        private void dataGrid_CellMouseEnter(object sender,
            DataGridViewCellEventArgs location)
        {
            mouseLocation = location;
        }

        private void mnuCtxRowHeaderHideCol_Click(object sender, EventArgs e)
        {
            dataGrid.Columns[mouseLocation.ColumnIndex].Visible = false;
        }

        private void mnuCtxRowHeaderShowAllCols_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn dvgCol in dataGrid.Columns)
                dvgCol.Visible = true;
        }

        private void mnuCtxRowHeaderCols_Click(object sender, EventArgs e)
        {
            string[] astrCols;
            bool[] abSelected;
            int[] anColIdx;
            FormColPicker formColPicker;
            DataGridViewColumn dvgCol;
            int nColIdx;

            int nColCount = dataGrid.Columns.Count;

            astrCols = new string[nColCount];
            abSelected = new bool[nColCount];
            anColIdx = new int[nColCount];

            for (int i = 0; i < nColCount; i++)
            {
                dvgCol = dataGrid.Columns[i];
                nColIdx = dvgCol.DisplayIndex;
                astrCols[nColIdx] = (string) dvgCol.HeaderCell.Value;
                abSelected[nColIdx] = dvgCol.Visible;
                anColIdx[nColIdx] = i;
            }

            formColPicker = new FormColPicker(astrCols, abSelected, anColIdx);
            formColPicker.ApplyCols += formColPicker_Apply;
            formColPicker.ShowDialog();
        }

        public void formColPicker_Apply(FormColPicker formColPicker)
        {
            string [] astrCols;
            bool [] abVisible;
            int [] anColIdx;
            int nColIdx;

            formColPicker.GetColumns(out astrCols, out abVisible, out anColIdx);
            for (int i = 0; i < anColIdx.GetLength(0); i++)
            {
                nColIdx = anColIdx[i];
                dataGrid.Columns[nColIdx].Visible = abVisible[i];
            }
        }
    }
}