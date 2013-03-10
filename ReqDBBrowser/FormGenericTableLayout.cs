using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ReqDBBrowser
{
    public class FormGenericTableLayout
    {
        public enum eFormGenericTableToken
        {
            eReqDetails,
            eReqLog
        }

        public class ColDefinition
        {
            string sColHead;
            int nWidth;
            bool bVisible;

            public ColDefinition (string sColHead, int nWidth, bool bVisible)
            {
                this.sColHead = sColHead;
                this.nWidth = nWidth;
                this.bVisible = bVisible;
            }

            public void CopyFrom(ColDefinition src)
            {
                sColHead = src.sColHead;
                nWidth = src.nWidth;
                bVisible = src.bVisible;
            }

            public string ColHead
            {
                get { return sColHead; }
            }

            public int Width
            {
                get { return nWidth; }
            }

            public bool Visible
            {
                get { return bVisible; }
            }
        }

        eFormGenericTableToken eToken;
        Rectangle rect;
        Dictionary<string, ColDefinition> dictColDefinition;

        public FormGenericTableLayout(eFormGenericTableToken eToken)
        {
            this.eToken = eToken;
            dictColDefinition = new Dictionary<string, ColDefinition>();
        }

        public bool IsType(eFormGenericTableToken eToken)
        {
            return this.eToken == eToken;
        }

        public void UpdateSize(Point location, Size size)
        {
            this.rect = new Rectangle(location, size);
        }

        public void UpdateSize(Rectangle rect)
        {
            this.rect = rect;
        }

        public void UpdateCol(ColDefinition colNew)
        {
            if (dictColDefinition.ContainsKey(colNew.ColHead))
            {
                ColDefinition col;
                col = dictColDefinition[colNew.ColHead];
                col.CopyFrom(colNew);
            }
            else
                dictColDefinition.Add(colNew.ColHead, colNew);
        }

        public Point Location
        {
            get { return rect.Location; }
        }

        public Size Size
        {
            get { return rect.Size; }
        }

        public ColDefinition this[string strColHead]
        {
            get
            {
                if (dictColDefinition.ContainsKey(strColHead))
                    return (dictColDefinition[strColHead]);
                else
                    return null;
            }
        }
    }
}
