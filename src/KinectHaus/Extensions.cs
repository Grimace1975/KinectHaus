using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace KinectHaus
{
    public static class Extensions
    {
        [DllImport("user32.dll")]
        static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("User32.DLL")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow); 

        public static Process ActivateApp(string processName, string fileName, string args)
        {
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentNullException("processName");
            var ps = Process.GetProcessesByName(processName);
            if (ps.Length > 0)
            {
                SetForegroundWindow(ps[0].MainWindowHandle);
                return ps[0];
            }
            if (string.IsNullOrEmpty(fileName))
                return null;
            var p = new Process();
            p.StartInfo.FileName = fileName;
            if (!string.IsNullOrEmpty(args))
                p.StartInfo.Arguments = args;
            p.Start();
            return p;
        }

        public static Process SwitchApp(string processName, string fileName, string args)
        {
            if (string.IsNullOrEmpty(processName))
                throw new ArgumentNullException("processName");
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException("fileName");
            var ps = Process.GetProcessesByName(processName);
            if (ps.Length > 0)
                ps[0].CloseMainWindow();
            var p = new Process();
            p.StartInfo.FileName = fileName;
            if (!string.IsNullOrEmpty(args))
                p.StartInfo.Arguments = args;
            p.Start();
            return p;
        }

        public static bool CallWithTimeout(this Action action, int timeoutMilliseconds)
        {
            Thread threadToKill = null;
            Action wrappedAction = () => { threadToKill = Thread.CurrentThread; action(); };
            var result = wrappedAction.BeginInvoke(null, null);
            if (result.AsyncWaitHandle.WaitOne(timeoutMilliseconds))
            {
                wrappedAction.EndInvoke(result);
                return true;
            }
            threadToKill.Abort();
            return false;
        }
    }
}
