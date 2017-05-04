using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Win32;
using System.Diagnostics;

namespace Jep.IO
{
    /// <summary>
    /// 提供Zip压缩解压方法
    /// </summary>
    public class ZipHelper
    {
        #region 私有变量
        String the_rar;
        RegistryKey the_Reg;
        Object the_Obj;
        String the_Info;
        ProcessStartInfo the_StartInfo;
        Process the_Process;
        #endregion

        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="zipname">要解压的文件名</param>
        /// <param name="zippath">要压缩的文件目录</param>
        /// <param name="dirpath">初始目录</param>
        public void EnZip(string zipname, string zippath, string dirpath)
        {
            try
            {
                the_Reg = Registry.ClassesRoot.OpenSubKey(@"Applications\WinRAR.exe\Shell\Open\Command");
                the_Obj = the_Reg.GetValue("");
                the_rar = the_Obj.ToString();
                the_Reg.Close();
                the_rar = the_rar.Substring(1, the_rar.Length - 7);
                the_Info = " a    " + zipname + "  " + zippath;
                the_StartInfo = new ProcessStartInfo();
                the_StartInfo.FileName = the_rar;
                the_StartInfo.Arguments = the_Info;
                the_StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                the_StartInfo.WorkingDirectory = dirpath;
                the_Process = new Process();
                the_Process.StartInfo = the_StartInfo;
                the_Process.Start();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="zipname">要解压的文件名</param>
        /// <param name="zippath">要解压的文件路径</param>
        public void DeZip(string zipname, string zippath)
        {
            try
            {
                the_Reg = Registry.ClassesRoot.OpenSubKey(@"Applications\WinRar.exe\Shell\Open\Command");
                the_Obj = the_Reg.GetValue("");
                the_rar = the_Obj.ToString();
                the_Reg.Close();
                the_rar = the_rar.Substring(1, the_rar.Length - 7);
                the_Info = " X " + zipname + " " + zippath;
                the_StartInfo = new ProcessStartInfo();
                the_StartInfo.FileName = the_rar;
                the_StartInfo.Arguments = the_Info;
                the_StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                the_Process = new Process();
                the_Process.StartInfo = the_StartInfo;
                the_Process.Start();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
