/*
 * 创建人：@谢华良
 * 创建时间:2013年4月11日 11:35
 * 目标: Cookie操作类
 */
using System;
using System.Collections.Generic;
using System.Text;

using System.Web;
namespace Jep.Web
{
    /// <summary>
    /// Cookie操作类
    /// </summary>
    public class EnowitCookie
    {
        #region =清除指定Cookie=
        /// <summary>
        /// 清除指定Cookie
        /// </summary>
        /// <param name="cookieName">cookieName</param>
        public static void ClearCookie(string cookieName)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            if (cookie != null)
            {
                cookie.Expires = DateTime.Now.AddYears(-3);
                HttpContext.Current.Response.Cookies.Add(cookie);
            }
        }
        #endregion

        #region =获取指定Cookie的值=
        /// <summary>
        /// 获取指定Cookie值
        /// </summary>
        /// <param name="cookieName">cookieName</param>
        /// <returns>string</returns>
        public static string GetCookieValue(string cookieName)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[cookieName];
            string str = string.Empty;
            if (cookie != null)
            {
                str = cookie.Value;
            }
            return str;
        }
        #endregion

        #region =添加一个Cookie（24小时过期）=
        /// <summary>
        /// 添加一个Cookie（24小时过期）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void SetCookie(string name, string value)
        {
            SetCookie(name, value, DateTime.Now.AddDays(1.0));
        }
        #endregion

        #region =添加一个Cookie=
        /// <summary>
        /// 添加一个Cookie
        /// </summary>
        /// <param name="name">cookie名</param>
        /// <param name="key">cookie值</param>
        /// <param name="expires">过期时间 DateTime</param>
        public static void SetCookie(string name, string value, DateTime expires)
        {
            HttpCookie cookie = new HttpCookie(name)
            {
                Value = value,
                Expires = expires
            };
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
        #endregion


    }
}
