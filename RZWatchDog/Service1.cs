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
            var apppaths = ConfigurationManager.AppSettings["ProcessPath"].Split('|');
            foreach (var apppath in apppaths)
            {
                var appname = (new FileInfo(apppath.Split(',')[1])).Name;
                Task.Factory.StartNew(() =>
                {
                    while (true)
                    {
                        if (!IsProcessExist(appname.Replace(".exe", "")))
                        {
                            Thread.Sleep(Convert.ToInt32(apppath.Split(',')[0]));
                            AppStart(apppath.Split(',')[1]);
                            //Process.Start(AppDomain.CurrentDomain.BaseDirectory+"CompatiblePrintService.exe");
                        }

                        Thread.Sleep(2000);
                    }
                });
            }

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
                //Process.Start(appPath);
                //return;
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

        public string RunCmd(string cmd)
        {
            var proc = new Process();
            proc.StartInfo.CreateNoWindow = false;
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.Start();
            proc.StandardInput.WriteLine(cmd);
            // proc.StandardInput.WriteLine("exit");
            // string outStr = proc.StandardOutput.ReadToEnd();
            proc.Close();
            return string.Empty;
        }
        protected override void OnStop()
        {
        }
    }
}
