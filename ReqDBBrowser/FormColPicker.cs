using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ReqDBBrowser
{
    public partial class FormColPicker : Form
    {
        string[] astrCols;
        bool[] abVisible;
        int[] anColIdx;

        public delegate void ApplyColsHandler(FormColPicker formColPicker); 
        public event ApplyColsHandler ApplyCols;

        public FormColPicker(string [] astrCols, bool [] abVisible, int [] anColIdx)
        {
            InitializeComponent();
            this.astrCols = astrCols;
            this.abVisible = abVisible;
            this.anColIdx = anColIdx;
        }

        private void FormColPicker_Load(object sender, EventArgs e)
        {
            ListViewItem lvItem;

            listView.Scrollable = true;
            
            listView.Columns.Add("Column", -2, HorizontalAlignment.Left);
            for (int i=0; i<astrCols.GetLength (0); i++)
            {
                lvItem = new ListViewItem(astrCols[i],0);
                lvItem.Checked = abVisible[i];
                lvItem.Tag = anColIdx[i];
                listView.Items.Add (lvItem);
            }
        }

        public void GetColumns(out string[] astrCols, out bool[] abVisible, out int[] anColIdx)
        {
            ListViewItem lvItem;
            int nItems = listView.Items.Count;

            astrCols = new string [nItems];
            abVisible = new bool [nItems];
            anColIdx = new int [nItems];

            for (int i = 0; i < nItems; i++)
            {
                lvItem = listView.Items[i];
                astrCols[i] = lvItem.Text;
                abVisible[i] = lvItem.Checked;
                anColIdx[i] = (int) lvItem.Tag;
            }
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            if (ApplyCols != null)
                ApplyCols(this);
        }

    }
}