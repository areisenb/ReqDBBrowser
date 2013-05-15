using System;
using System.Collections.Generic;
using System.Text;

namespace ReqDBBrowser
{
    class ReqTraceGraphDot: IDisposable
    {
        static int nInstCount = 0;

        string strDOT;
        string strSVGFileName;
        string strDescription;

        public ReqTraceGraphDot(string strDescription)
        {
            strDOT = "";
            strSVGFileName = "";
            this.strDescription = strDescription;
            nInstCount++;
        }

        ~ReqTraceGraphDot()
        {
            Dispose();
        }

        public void Add(ReqTraceDotNode reqTraceNode)
        {
            strDOT += reqTraceNode.Convert();
        }

        public string Export2svg()
        {
            if (strSVGFileName.Length == 0)
            {
                System.IO.StreamWriter stw;
                string strDOTFileName;

                // was easier to check during debug time
                // strDOTFileName = String.Format("d:\\temp\\tmp{0:D3}.dot", nInstCount);
                // strSVGFileName = String.Format("d:\\temp\\tmp{0:D3}.svg", nInstCount);
                strDOTFileName = System.IO.Path.GetTempFileName();
                strSVGFileName = strDOTFileName + ".svg";


                stw = new System.IO.StreamWriter(strDOTFileName);
                // too lazy to escape all this special characters of the description
                strDescription = "TraceGraph";
                stw.WriteLine("digraph " + strDescription + " {");
                stw.Write(strDOT);
                stw.WriteLine("}");
                stw.Close();
                ConvertDOT2svg(strDOTFileName, strSVGFileName);
                System.IO.File.Delete(strDOTFileName);
            }
            return strSVGFileName;
        }

        private void ConvertDOT2svg (string strFileNameIn, string strFileNameOut)
        {
            //Create process
            System.Diagnostics.Process pProcess = new System.Diagnostics.Process();

            //strCommand is path and file name of command to run
            pProcess.StartInfo.FileName = "dot.exe";

            //strCommandParameters are parameters to pass to program
            pProcess.StartInfo.Arguments = "-Tsvg " + strFileNameIn + " -o " + strFileNameOut;

            // Do not create the black window.
            pProcess.StartInfo.CreateNoWindow = true;

            pProcess.StartInfo.UseShellExecute = false;

            //Set output of program to be written to process output stream
            pProcess.StartInfo.RedirectStandardOutput = true;
            pProcess.StartInfo.RedirectStandardError = true;

            //Optional
            //pProcess.StartInfo.WorkingDirectory = strWorkingDirectory;

            //Start the process
            try
            {
                pProcess.Start();
                //Get program output
                string strOutput = pProcess.StandardOutput.ReadToEnd();
                string strError = pProcess.StandardError.ReadToEnd();
                //Wait for process to finish
                pProcess.WaitForExit();
                if (strError.Length > 0)
                    System.Windows.Forms.MessageBox.Show(strError, "DOT returned Errors", 
                        System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
            catch (Exception)
            {
                //not the most beautiful solution
                string strErrorSVG = 
                        "<svg xmlns=\"http://www.w3.org/2000/svg\" xmlns:xlink=\"http://www.w3.org/1999/xlink\">"+
                        "<text font-weight=\"bold\" font-size=\"18\" y=\"20\" x=\"10\" "+
                        "stroke-width=\"0\" stroke=\"#000000\" fill=\"#ff0000\">Error!</text>"+
                        "<text y=\"40\" x=\"10\">graphviz does not seem to be installed properly</text>"+
                        "<text y=\"60\" x=\"10\">(cannot find 'dot.exe' within your path)</text>"+
                        "<text y=\"100\" x=\"10\">please install graphviz suite:</text>"+
                        "<a xlink:href=\"http://www.graphviz.org/pub/graphviz/stable/windows/graphviz-2.30.1.msi\">"+
                        "<text x=\"20\" y=\"120\">http://www.graphviz.org/pub/graphviz/stable/windows/graphviz-2.30.1.msi</text>"+
                        "</a></svg>";

                System.IO.StreamWriter stw = new System.IO.StreamWriter(strFileNameOut);
                stw.Write (strErrorSVG);
                stw.Close ();
            }

        }


        #region IDisposable Members
        public void Dispose()
        {
            if (strSVGFileName.Length > 0)
            {
                string strFName = strSVGFileName;
                strSVGFileName = "";
                System.IO.File.Delete(strFName);
            }
        }
        #endregion
    }
}
