using System;
using System.Collections.Generic;
using System.Text;

using System.Web;

namespace Jep.IO
{
    /// <summary>
    /// 提供Web和Winform应用程序路径获取方法。
    /// </summary>
    public class AppPath
    {
        private static string _GetWebPath(string localPath)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory; //HttpContext.Current.Request.ApplicationPath;
            string thisPath;
            string thisLocalPath;
            //如果不是根目录就加上"/" 根目录自己会加"/"
            if (path != "/")
            {
                thisPath = path + "/";
            }
            else
            {
                thisPath = path;
            }
            if (localPath.StartsWith("~/"))
            {
                thisLocalPath = localPath.Substring(2);
            }
            else
            {
                return localPath;
            }
            return thisPath + thisLocalPath;
        }

        /// <summary>
        /// 根据给出的相对地址获取网站绝对地址
        /// </summary>
        /// <param name="localPath">相对地址</param>
        /// <returns>绝对地址</returns>
        public static string GetWebPath(string localPath)
        {
            return _GetWebPath(localPath);
        }

        private static string _GetWebPath()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory; //System.Web.HttpContext.Current.Request.ApplicationPath;
            string thisPath;
            //如果不是根目录就加上"/" 根目录自己会加"/"
            if (path != "/")
            {
                thisPath = path + "/";
            }
            else
            {
                thisPath = path;
            }
            return thisPath;
        }

        /// <summary>
        ///  获取网站绝对地址
        /// </summary>
        /// <returns></returns>
        public static string GetWebPath()
        {
            return _GetWebPath();
        }

        /// <summary>
        /// 获取Winform程序的根路径。
        /// </summary>
        /// <returns></returns>
        private static string _GetPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// 获取Winform程序的根路径。
        /// </summary>
        /// <returns></returns>
        public static string GetPath()
        {
            return _GetPath();
        }
    }
}
