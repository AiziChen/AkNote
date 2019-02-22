using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AkNote
{
    /**
     * 程序主入口
     * @author QuanyeChen
     * License Under the GPL v2.0
     */ 
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            // 自启动
            string exePath = Application.ExecutablePath;
            RegistryKey rk = Registry.LocalMachine;
            RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
            rk2.SetValue("AkNote", exePath);
            rk2.Close();
            rk.Close();
            
            // 防止启动两个AkNote
            bool ret;
            Mutex mutex = new Mutex(true, Application.ProductName, out ret);
            if (ret)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(GetMainForm());
                mutex.ReleaseMutex();
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                MessageBox.Show("AkNote之前启动过，请按`Alt+Q`即可显示窗口。");
            }
        }

        private static Form1 form1;
        private static Form1 GetMainForm()
        {
            if (form1 == null)
            {
                form1 = new Form1();
                return form1;
            }
            else
            {
                return form1;
            }
        }
    }
}
