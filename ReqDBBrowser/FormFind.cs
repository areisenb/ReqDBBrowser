using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ReqDBBrowser
{
    public partial class FormFind : Form
    {
        public FormFind()
        {
            InitializeComponent();
        }

        public string SearchExpr
        {
            get { return tbFindString.Text ; }
        }

        public bool UseRegEx
        {
            get { return rbRegex.Checked; }
        }

        public bool SearchTag
        {
            get { return cbTag.Checked; }
        }

        public bool SearchName
        {
            get { return cbName.Checked; }
        }

        public bool SearchText
        {
            get { return cbText.Checked; }
        }



    }
}