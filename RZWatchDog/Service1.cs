using Cjwdev.WindowsApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RZWatchDog
{
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            var apppath = ConfigurationManager.AppSettings["ProcessPath"];
            var appname = (new FileInfo(apppath)).Name;
            
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    if (!IsProcessExist(appname.Replace(".exe","")))
                    {
                        AppStart(apppath);
                        //Process.Start(AppDomain.CurrentDomain.BaseDirectory+"CompatiblePrintService.exe");
                    }

                    Thread.Sleep(2000);
                }
            });
        }

        /// <summary>
        /// 判断进程是否存在
        /// </summary>
        /// <param name="processName"></param>
        public bool IsProcessExist(string processName)
        {
            Process[] ps = Process.GetProcesses();
            foreach (Process item in ps)
            {
                if (item.ProcessName == processName)
                {
                    return true;
                }
            }

            return false;
        }

        public void AppStart(string appPath)
        {
            try
            {

                string appStartPath = appPath;
                IntPtr userTokenHandle = IntPtr.Zero;
                ApiDefinitions.WTSQueryUserToken(ApiDefinitions.WTSGetActiveConsoleSessionId(), ref userTokenHandle);

                ApiDefinitions.PROCESS_INFORMATION procInfo = new ApiDefinitions.PROCESS_INFORMATION();
                ApiDefinitions.STARTUPINFO startInfo = new ApiDefinitions.STARTUPINFO();
                startInfo.cb = (uint)Marshal.SizeOf(startInfo);

                ApiDefinitions.CreateProcessAsUser(
                    userTokenHandle,
                    appStartPath,
                    "",
                    IntPtr.Zero,
                    IntPtr.Zero,
                    false,
                    0,
                    IntPtr.Zero,
                    null,
                    ref startInfo,
                    out procInfo);

                if (userTokenHandle != IntPtr.Zero)
                    ApiDefinitions.CloseHandle(userTokenHandle);

                int _currentAquariusProcessId = (int)procInfo.dwProcessId;
            }
            catch (Exception ex)
            {
            }
        }

        protected override void OnStop()
        {
        }
    }
}
