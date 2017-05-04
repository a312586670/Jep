using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Jep.IO;

namespace Test
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form2());

            int a = 0;
            int b = 2;
            try
            {
                int count= b/a;
            }
            catch(Exception ex) {
                RuntimeLog.WriteRuntimeErrorLog(ex);
            }
        }
    }
}
