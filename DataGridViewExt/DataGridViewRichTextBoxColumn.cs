using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;


namespace DataGridViewExt
{
    public class DataGridViewRichTextBoxColumn: DataGridViewColumn
    {
        public DataGridViewRichTextBoxColumn()
            : base(new DataGridViewRichTextBoxCell())
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
                if (!(value is DataGridViewRichTextBoxCell))
                    throw new InvalidCastException("CellTemplate must be a DataGridViewRichTextBoxCell");

                base.CellTemplate = value;  
            }
        }
    }

}
