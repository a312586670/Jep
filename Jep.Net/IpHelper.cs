/*
 * IP操作辅助类
 * 作者：谢华良
 * 完成时间：2013年4月9日
 */
using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.Web;

namespace Jep.Net
{
    /// <summary>
    /// 提供IP操作辅助类
    /// </summary>
    public class IpHelper
    {
        private static IPAddress _GetClientIp()
        {
            string ip;
            string[] temp;
            bool isErr = false;
            if (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_ForWARDED_For"] == null)
                ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"].ToString();
            else
                ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_ForWARDED_For"].ToString();
            if (ip.Length > 15)
                isErr = true;
            else
            {
                temp = ip.Split('.');
                if (temp.Length == 4)
                {
                    for (int i = 0; i < temp.Length; i++)
                    {
                        if (temp[i].Length > 3) isErr = true;
                    }
                }
                else
                    isErr = true;
            }

            if (isErr)
                return IPAddress.Parse("1.1.1.1");
            else
                return IPAddress.Parse(ip); ;
        }

        /// <summary>
        /// 获得客户端IP地址
        /// </summary>
        public static IPAddress GetClientIp()
        {
            return _GetClientIp();
        }
    }
}
