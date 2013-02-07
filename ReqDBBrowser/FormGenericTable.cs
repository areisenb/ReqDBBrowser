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

        public void SetGridContent(string[,] astrContent)
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

            for (int j = 1; j < nRows; j++)
            {
                strRow = new string[nCols];
                for (int i = 0; i < nCols; i++)
                    strRow[i] = astrContent[j, i];
                dataGrid.Rows.Add(strRow);
            }

            Controls.Add (dataGrid);
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
                dataGrid.Size = new Size(ClientSize.Width, ClientSize.Height - nYOffsetDataGrid);
            }
        }
    }
}