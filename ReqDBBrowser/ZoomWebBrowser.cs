using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace ReqDBBrowser
{
    class ZoomWebBrowser: System.Windows.Forms.WebBrowser
    {
        private int nIdxZoom;
        static readonly int [] aZoomLevel = { 5, 10, 20, 50, 70, 100, 125, 150, 200, 300, 400, 800 };

        private IntPtr _hookID_LLMouse = IntPtr.Zero;
        private HOOKProc callBackDelegate = null;

        public ZoomWebBrowser()
            : base()
        {
            _hookID_LLMouse = IntPtr.Zero;
            callBackDelegate = new HOOKProc(LLMouseCallback);
            nIdxZoom = 5;
        }

        public int ZoomIn()
        {
            if (nIdxZoom < aZoomLevel.GetLength (0)-1)
                nIdxZoom++;
            return (Zoom());
        }

        public int ZoomOut()
        {
            if (nIdxZoom > 0)
                nIdxZoom--;
            return (Zoom());
        }

        public int SetZoom(int nZoom)
        {
            //this.nZoom = nZoom;
            //Zoom();
            return (aZoomLevel[nIdxZoom]);
        }

        private int Zoom()
        {
            SHDocVw.IWebBrowser2 browserInst = ((SHDocVw.IWebBrowser2)(this.ActiveXInstance));
            object pvaIn = aZoomLevel[nIdxZoom];
            object pvaOut = 0;

            while (browserInst.Busy) ;

            browserInst.ExecWB(SHDocVw.OLECMDID.OLECMDID_OPTICAL_ZOOM,
                SHDocVw.OLECMDEXECOPT.OLECMDEXECOPT_DODEFAULT, ref pvaIn, ref pvaOut);
            return (aZoomLevel[nIdxZoom]);
        }

        public void HookOn()
        {
            if (_hookID_LLMouse == IntPtr.Zero)
                _hookID_LLMouse = SetHook(callBackDelegate, (int)HookId.WH_MOUSE_LL);
        }

        public void HookOff()
        {
            if (_hookID_LLMouse != IntPtr.Zero)
            {
                _hookID_LLMouse = IntPtr.Zero;
                UnhookWindowsHookEx(_hookID_LLMouse);
            }
        }

        private IntPtr SetHook(HOOKProc proc, int hookID)
        {
            using (System.Diagnostics.Process curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (System.Diagnostics.ProcessModule curModule = curProcess.MainModule)
            {
                //return SetWindowsHookEx(hookID, proc,
                //    GetModuleHandle(curModule.ModuleName), 0);
                //uint ui = (uint)System.Threading.Thread.CurrentThread.ManagedThreadId;
                //ui = (uint)AppDomain.GetCurrentThreadId();

                return SetWindowsHookEx(hookID, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr HOOKProc (int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr LLMouseCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
                if (MouseMessages.WM_MOUSEWHEEL == (MouseMessages)wParam)
                    if (ModifierKeys == System.Windows.Forms.Keys.Control)
                    {
                        if (Bounds.Contains(PointToClient(MousePosition)))
                        {
                            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                            short sDelta = (short)(hookStruct.mouseData / 0x10000);
                            if (sDelta > 0)
                                /* scrolling up (=away from the user) zooms in */
                                ZoomIn();
                            else
                                ZoomOut();
                            System.Diagnostics.Debug.WriteLine("ZoomBrowser MouseWheelLL Delta: " + sDelta);
                        }
                        else
                            System.Diagnostics.Debug.WriteLine("ZoomBrowser MouseWheelLL out of Client Area");
                    }
                    else
                        System.Diagnostics.Debug.WriteLine("ZoomBrowser MouseWheelLL without <CONTROL>");
            return CallNextHookEx(_hookID_LLMouse, nCode, wParam, lParam);
        }

        private enum HookId
        {
            WH_MOUSE        = 7,
            WH_MOUSE_LL     = 14
        }


        private enum MouseMessages
        {
            WM_LBUTTONDOWN = 0x0201,
            WM_LBUTTONUP = 0x0202,
            WM_MOUSEMOVE = 0x0200,
            WM_MOUSEWHEEL = 0x020A,
            WM_RBUTTONDOWN = 0x0204,
            WM_RBUTTONUP = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            HOOKProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
