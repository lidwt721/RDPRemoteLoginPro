using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using RDPRemoteLoginPro;

namespace RDPRemoteLoginPro
{
    static class Program
    {
        /// <summary>  
        /// window form show count  
        /// </summary>  
        private const int WS_SHOWNORMAL = 1;

        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string sLibName);

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            //Application.EnableVisualStyles();
            //Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());

            Process instance = RunningInstance();
            if (instance == null)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            else
            {
                HandleRunningInstance(instance);
            }
        }
        /// <summary>  
        /// application repeat start  
        /// </summary>  
        public static void ApplicationRepeatStart()
        {
            Process instance = RunningInstance();
            if (instance != null)
            {
                HandleRunningInstance(instance);
            }
        }

        /// <summary>  
        /// Running process instance  
        /// </summary>  
        /// <returns>the running process</returns>  
        public static Process RunningInstance()
        {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            ////Loop through the running processes in with the same name  
            foreach (Process process in processes)
            {
                ////Ignore the current process  
                if (process.Id != current.Id)
                {
                    ////Make sure that the process is running from the exe file.  
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName)
                    {
                        ////Return the other process instance.  
                        return process;
                    }
                }
            }
            ////No other instance was found, return null.  
            return null;
        }

        /// <summary>  
        /// handle running instance  
        /// </summary>  
        /// <param name="instance">the running process</param>  
        public static void HandleRunningInstance(Process instance)
        {
            ////Make sure the window is not minimized or maximized  
            ShowWindowAsync(instance.MainWindowHandle, WS_SHOWNORMAL);
            
            ////Set the real instance to foreground window  
            SetForegroundWindow(instance.MainWindowHandle);
        }
    }
}
