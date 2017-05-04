/*
 * 创建人：@谢华良
 * 创建时间:2013年4月11日 9:10
 * 目标:地址栏参数帮助类
 */
using System;
using System.Collections.Generic;
using System.Text;

using System.Text.RegularExpressions;
using System.Web;
namespace Jep.Web
{
    /// <summary>
    /// 地址栏参数
    /// </summary>
    public class EnowitQuery
    {
        #region =等于Request.QueryString;如果为null 返回 空“” ，否则返回Request.QueryString[name]=
        /// <summary>
        ///获取地址栏参数值
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <returns>string</returns>
        public static string QueryString(string name)
        {
            return Request.QueryString[name] == null ? "" : Request.QueryString[name];
        }
        #endregion

        #region =等于Request.Form  如果为null 返回 空“” ，否则返回 Request.Form[name]=
        /// <summary>
        /// 等于  Request.Form  如果为null 返回 空“” ，否则返回 Request.Form[name]
        /// </summary>
        /// <param name="name">参数名</param>
        /// <returns>string</returns>
        public static string QueryFormString(string name)
        {
            return Request.Form[name] == null ?"" : Request.Form[name].ToString();
        }
        #endregion

        #region =检查一个字符串是否是纯数字构成的=
        /// <summary>
        /// 检查一个字符串是否是纯数字构成的
        /// </summary>
        /// <param name="input">需验证的字符串。。</param>
        /// <returns>是否合法的bool值。</returns>
        public static bool IsNumberId(string input)
        {
            return IsMatch(input,"^[1-9]*[0-9]*$");
        }
        #endregion

        #region =指定的正则表达式在输入的字符串中是否匹配=
        /// <summary>
        /// 定的正则表达式在输入的字符串中是否匹配
        /// </summary>
        /// <param name="input">字符串</param>
        /// <param name="pattern">正则表达式。</param>
        /// <returns>是否合法的bool值</returns>
        public static bool IsMatch(string input, string pattern)
        {
            if (input == null) return false;
            Regex myRegex = new Regex(pattern);
            if (input.Length == 0)
            {
                return false;
            }
            return myRegex.IsMatch(input);
        }
        #endregion

        #region 类内部调用
        /// <summary>
        /// HttpContext Current
        /// </summary>
        public static HttpContext Current
        {
            get { return HttpContext.Current; }
        }
        /// <summary>
        /// HttpContext Current  HttpRequest Request   get { return Current.Request;
        /// </summary>
        public static HttpRequest Request
        {
            get { return Current.Request; }
        }
        /// <summary>
        ///  HttpContext Current  HttpRequest Request   get { return Current.Request; HttpResponse Response  return Current.Response;
        /// </summary>
        public static HttpResponse Response
        {
            get { return Current.Response; }
        }
        #endregion
    }
}
