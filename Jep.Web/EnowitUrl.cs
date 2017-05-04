/*
 * 创建人：@谢华良
 * 创建时间:2013年4月11日
 * 目标：Url操作类
 */
using System;
using System.Collections.Generic;
using System.Text;

using System.Web;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
namespace Jep.Web
{
    public class EnowitUrl
    {
        static System.Text.Encoding encoding = System.Text.Encoding.UTF8;

        #region =URL的64位编码=
        /// <summary>
        /// Url的64位编码
        /// </summary>
        /// <param name="url">url</param>
        /// <returns>64位编码后的字符串</returns>
        public static string Base64Encrypt(string url)
        {
            string eurl = HttpUtility.UrlEncode(url);
            eurl = Convert.ToBase64String(encoding.GetBytes(eurl));
            return eurl;
        }
        #endregion

        #region =URL的64位解码=
        /// <summary>
        /// URL的64位解码
        /// </summary>
        /// <param name="url">64位编码的字符串</param>
        /// <returns>string</returns>
        public static string Base64Decrypt(string url)
        {
            if (!IsBase64(url))
            {
                return url;
            }
            byte[] buffer = Convert.FromBase64String(url);
            string sourthUrl = encoding.GetString(buffer);
            sourthUrl = HttpUtility.UrlDecode(sourthUrl);
            return sourthUrl;
        }
        #endregion

        #region =是否是Base64字符串=
        /// <summary>
        /// 是否是Base64字符串
        /// </summary>
        /// <param name="input">要判断的字符串</param>
        /// <returns>bool</returns>
        public static bool IsBase64(string input)
        {
            if ((input.Length % 4) != 0)
            {
                return false;
            }
            if (!Regex.IsMatch(input, "^[A-Z0-9/+=]*$", RegexOptions.IgnoreCase))
            {
                return false;
            }
            return true;
        }
        #endregion

        #region =对URL增加参数信息=
        /// <summary>
        /// 对url增加参数信息
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="paramName">参数名称</param>
        /// <param name="value">参数值</param>
        /// <returns>string</returns>
        public static string AddParam(string url, string paramName, string value)
        {
            Uri uri = new Uri(url);
            if (string.IsNullOrEmpty(uri.Query))
            {
                //string eval = HttpContext.Current.Server.UrlEncode(value);
                return String.Concat(url, "?" + paramName + "=" + value);
            }
            else
            {
                //string eval = HttpContext.Current.Server.UrlEncode(value);
                return String.Concat(url, "&" + paramName + "=" + value);
            }
        }
        #endregion

        #region =修改Url参数值=
        /// <summary>
        /// 修改Url参数值
        /// </summary>
        /// <param name="url">要修改的url</param>
        /// <param name="paramName">参数名称</param>
        /// <param name="value">参数值</param>
        /// <returns>string</returns>
        public static string UpdateParam(string url, string paramName, string value)
        {
            string keyWord = paramName + "=";
            int indexQueryStart = url.IndexOf('?');
            string Query = "";
            if (indexQueryStart == -1)
                return url;//获取参数字符串
            Query = url.Substring(indexQueryStart);

            string frontUrl = url.Substring(0, indexQueryStart);
            if (Query.IndexOf(keyWord) == -1)
                return url;
            int index = Query.IndexOf(keyWord) + keyWord.Length;
            int index1 = Query.IndexOf("&", index);
            if (index1 == -1)
            {
                //url = url.Remove(index, url.Length - index);
                //url = string.Concat(url, value);
                return url;
            }
            Query = Query.Remove(index, index1 - index);
            url =frontUrl+Query.Remove(index, index1 - index);
            url =frontUrl+Query.Insert(index, value);
            return url;
        }
        #endregion

        #region =分析url字符串中的参数信息=
        /// <summary>
        /// 分析 url 字符串中的参数信息
        /// </summary>
        /// <param name="url">输入的 URL</param>
        /// <param name="baseUrl">输出 URL 的基础部分</param>
        /// <param name="nvc">输出分析后得到的 (参数名,参数值) 的集合</param>
        public static void ParseUrl(string url, out string baseUrl, out NameValueCollection nvc)
        {
            if (url == null)
                throw new ArgumentNullException("url");
            nvc = new NameValueCollection();
            baseUrl = "";
            if (url == "")
                return;
            int questionMarkIndex = url.IndexOf('?');
            if (questionMarkIndex == -1)
            {
                baseUrl = url;
                return;
            }
            baseUrl = url.Substring(0, questionMarkIndex);
            if (questionMarkIndex == url.Length - 1)
                return;
            string ps = url.Substring(questionMarkIndex + 1);

            // 开始分析参数对    
            Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
            MatchCollection mc = re.Matches(ps);

            foreach (Match m in mc)
            {
                nvc.Add(m.Result("$2").ToLower(), m.Result("$3"));
            }
        }
        #endregion
    }
}
