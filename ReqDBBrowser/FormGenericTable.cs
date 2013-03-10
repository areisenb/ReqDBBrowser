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
        static List<FormGenericTableLayout> listFormTemplate;

        static FormGenericTable()
        {
            listFormTemplate = new List<FormGenericTableLayout>();
        }

        DataGridView dataGrid;
        int nYOffsetDataGrid;
        int nCntHiddenCols;
        private bool bSelfMoving;

        private DataGridViewCellEventArgs mouseLocation;
        private FormGenericTableLayout formTemplate;

        public FormGenericTable(string strCaption, string [] astrHead, float [] afLines, 
            FormGenericTableLayout.eFormGenericTableToken eToken)
        {
            TextBox tb;
            int ntbHeigth;

            bSelfMoving = true;
            nCntHiddenCols = 0;

            InitializeComponent();
            Text = strCaption;

            AttachTemplate(eToken);

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
            bSelfMoving = false;
        }

        private void AttachTemplate(FormGenericTableLayout.eFormGenericTableToken eToken)
        {
            this.formTemplate = null;
            foreach (FormGenericTableLayout formTemplate in listFormTemplate)
                if (formTemplate.IsType(eToken))
                {
                    this.formTemplate = formTemplate;
                    break;
                }
            if (this.formTemplate != null)
            {
                Location = this.formTemplate.Location;
                Size = this.formTemplate.Size;
            }
            else
            {
                this.formTemplate = new FormGenericTableLayout(eToken);
                listFormTemplate.Add(this.formTemplate);
            }
        }

        public void SetGridContent(string[,] astrContent, string strStatusLineLabel)
        {
            int nCols;
            int nRows;
            ContextMenuStrip mnuCtxCell;
            FormGenericTableLayout.ColDefinition sColDef;

            if (dataGrid != null)
                Controls.Remove (dataGrid);
            dataGrid = new DataGridView();
            string [] strRow;

            dataGrid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            dataGrid.ColumnWidthChanged += new DataGridViewColumnEventHandler(dataGrid_ColumnWidthChanged);

            nCols = astrContent.GetLength (1);
            nRows = astrContent.GetLength (0);

            SetGridLayout();
            //dataGrid.Dock = DockStyle.Top;
            dataGrid.AllowUserToAddRows = false;
            dataGrid.AllowUserToDeleteRows = false;
            dataGrid.ColumnCount = nCols;
            DataGridViewRow row = dataGrid.RowTemplate;
            row.Height = dataGrid.Font.Height * 11 / 2;

            mnuCtxCell = new ContextMenuStrip();
            mnuCtxCell.Items.Add(new ToolStripMenuItem("Copy", null, mnuCpy_Click));
            row.ContextMenuStrip = mnuCtxCell;
            row.HeaderCell.ContextMenuStrip = mnuCtxCell;

            for (int i=0; i<nCols; i++)
            {
                dataGrid.Columns[i].Name = astrContent[0,i];
                dataGrid.Columns[i].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                sColDef = formTemplate[astrContent[0,i]];
                if (sColDef != null)
                {
                    dataGrid.Columns[i].Width = sColDef.Width;
                    dataGrid.Columns[i].Visible = sColDef.Visible;
                    if (sColDef.Visible == false)
                        nCntHiddenCols++;
                }
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
            UpdateHiddenColsText();
        }

        private void FormGenericTable_Resize(object sender, EventArgs e)
        {
            SetGridLayout();
            foreach (Control ct in Controls)
                if (ct is TextBox)
                    ct.Width = ClientSize.Width;
            if (!bSelfMoving)
                formTemplate.UpdateSize(Location, Size);
        }

        private void FormGenericTable_Move(object sender, EventArgs e)
        {
            if (!bSelfMoving)
                formTemplate.UpdateSize(Location, Size);
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
            ContextMenuStrip mnuCtxColHeader;

            mnuCtxColHeader = new ContextMenuStrip();

            mnuCtxColHeader.Items.Add(new ToolStripMenuItem("Hide Column", null, mnuCtxRowHeaderHideCol_Click));
            mnuCtxColHeader.Items.Add(new ToolStripMenuItem("Show all Columns", null, mnuCtxRowHeaderShowAllCols_Click));
            mnuCtxColHeader.Items.Add(new ToolStripMenuItem("Columns...", null, mnuCtxRowHeaderCols_Click));
            foreach (DataGridViewColumn dgvCol in dataGrid.Columns)
                dgvCol.HeaderCell.ContextMenuStrip = mnuCtxColHeader;
        }

        private void UpdateHiddenColsText()
        {
            if (nCntHiddenCols == 0)
                toolStripStatusLabelHiddenCols.Text = "";
            else
                toolStripStatusLabelHiddenCols.Text = "| " + nCntHiddenCols + " Hidden Columns";
        }

        // Deal with hovering over a cell.
        private void dataGrid_CellMouseEnter(object sender,
            DataGridViewCellEventArgs location)
        {
            mouseLocation = location;
        }

        void dataGrid_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            DataGridViewColumn dvgCol = e.Column;
            formTemplate.UpdateCol(new FormGenericTableLayout.ColDefinition(dvgCol.Name, dvgCol.Width, dvgCol.Visible));
        }

        private void mnuCtxRowHeaderHideCol_Click(object sender, EventArgs e)
        {
            DataGridViewColumn dvgCol = dataGrid.Columns[mouseLocation.ColumnIndex];
            dvgCol.Visible = false;
            formTemplate.UpdateCol(new FormGenericTableLayout.ColDefinition(dvgCol.Name, dvgCol.Width, false));
            nCntHiddenCols++;
            UpdateHiddenColsText();
        }

        private void mnuCtxRowHeaderShowAllCols_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewColumn dvgCol in dataGrid.Columns)
            {
                dvgCol.Visible = true;
                formTemplate.UpdateCol(new FormGenericTableLayout.ColDefinition (dvgCol.Name, dvgCol.Width, true));
            }
            nCntHiddenCols = 0;
            UpdateHiddenColsText();
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
            DataGridViewColumn dvgCol;
            nCntHiddenCols = 0;

            formColPicker.GetColumns(out astrCols, out abVisible, out anColIdx);
            for (int i = 0; i < anColIdx.GetLength(0); i++)
            {
                dvgCol = dataGrid.Columns[anColIdx[i]];
                dvgCol.Visible = abVisible[i];
                if (abVisible[i] == false)
                    nCntHiddenCols++;
                formTemplate.UpdateCol(new FormGenericTableLayout.ColDefinition(dvgCol.Name, dvgCol.Width, dvgCol.Visible));
            }
            UpdateHiddenColsText();
        }

        private void mnuCpy_Click(object sender, EventArgs e)
        {
            if (dataGrid.SelectedCells.Count > 0)
                Clipboard.SetDataObject(dataGrid.GetClipboardContent());
        }
    }
}