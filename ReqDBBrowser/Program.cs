using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ReqDBBrowser
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#if DEBUG
                Application.Run(new FormMain());
#else
            try
            {
                Application.Run(new FormMain());
            }
            catch (ReqProProject.ReqProException e)
            {
                string strMBCaption = "Error";

                switch (e.Severity)
                {
                    case ReqProProject.ReqProException.ESeverity.eFatal:
                        strMBCaption = "Fatal Error";
                        break;
                    case ReqProProject.ReqProException.ESeverity.eError:
                        strMBCaption = "Error";
                        break;
                    case ReqProProject.ReqProException.ESeverity.eWarning:
                        strMBCaption = "Warning";
                        break;
                }
                System.Windows.Forms.MessageBox.Show("Please verify ReqPro Installation\n" + e.Message, 
                    strMBCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.Message + "\n\n" + e.StackTrace, "Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
#endif 
        }
    }
}