/*
 * 创建人：@谢华良
 * 创建时间:2013年4月11日
 * 目标 ：Session操作类
 */
using System;
using System.Collections.Generic;
using System.Text;

using System.Web;

namespace Jep.Web
{
    /// <summary>
    /// Session对象操作类
    /// </summary>
    public class EnowitSession
    {
        #region =根据会话值的键名获取session对象=
        /// <summary>
        /// 根据会话值的键名获取session对象
        /// </summary>
        /// <param name="name">会话值的键名</param>
        /// <returns></returns>
        public static object GetSession(string name)
        {
            return HttpContext.Current.Session[name];
        }
        #endregion

        #region =根据会话值的数字索引获取session对象=
        /// <summary>
        /// 根据会话值的数字索引获取session对象
        /// </summary>
        /// <param name="index">会话值的数字索引</param>
        /// <returns>object</returns>
        public static object GetSession(int index)
        { 
            return HttpContext.Current.Session[index];
        }
        #endregion

        #region =设置session=
        /// <summary>
        /// 设置session
        /// </summary>
        /// <param name="name">会话状态集合项的名称</param>
        /// <param name="val">会话状态集合项的名称</param>
        public static void SetSession(string name, object val)
        {
            HttpContext.Current.Session.Remove(name);
            HttpContext.Current.Session.Add(name, val);
        }
        #endregion

        #region =添加Session，Session有效期为20分钟=
        /// <summary>
        /// 添加Session，Session有效期为20分钟
        /// </summary>
        /// <param name="name">会话值的键名</param>
        /// <param name="value">会话值的键值</param>
        public static void Add(string name, string value)
        {
            HttpContext.Current.Session[name] = value;
            HttpContext.Current.Session.Timeout = 20;
        }

        /// <summary>
        /// 添加Session，Session有效期为20分钟
        /// </summary>
        /// <param name="name">会话值的键名</param>
        /// <param name="value">会话值的键值数组</param>
        public static void Adds(string name, string[] values)
        {
            HttpContext.Current.Session[name] = values;
            HttpContext.Current.Session.Timeout = 20;
        }
        #endregion

        #region =添加Session对象，并指定有效时间=
        /// <summary>
        /// 添加Session
        /// </summary>
        /// <param name="name">会话值的键名</param>
        /// <param name="value">会话值的键值</param>
        /// <param name="iExpires">调动有效期（分钟）</param>
        public static void Add(string name, string value, int iExpires)
        {
            HttpContext.Current.Session[name] = value;
            HttpContext.Current.Session.Timeout = iExpires;
        }

        /// <summary>
        /// 添加Session对象，并指定有效时间
        /// </summary>
        /// <param name="name">会话值的键名</param>
        /// <param name="values">会话值的键值数组</param>
        /// <param name="iExpires">调动有效期（分钟）</param>
        public static void Adds(string name, string[] values, int iExpires)
        {
            HttpContext.Current.Session[name] = values;
            HttpContext.Current.Session.Timeout = iExpires;
        }
        #endregion

        #region =读取某个Session对象值=
        /// <summary>
        /// 读取某个Session对象值
        /// </summary>
        /// <param name="name">会话值的键名</param>
        /// <returns>Session对象值</returns>
        public static string GetSessionValue(string name)
        {
            if (HttpContext.Current.Session[name] == null)
            {
                return null;
            }
            else
            {
                return HttpContext.Current.Session[name].ToString();
            }
        }
        #endregion

        #region =读取某个Session对象值数组=
        /// <summary>
        /// 读取某个Session对象值数组
        /// </summary>
        /// <param name="name">会话值的键名</param>
        /// <returns>Session对象值数组</returns>
        public static string[] GetSessionValues(string name)
        {
            if (HttpContext.Current.Session[name] == null)
            {
                return null;
            }
            else
            {
                return (string[])HttpContext.Current.Session[name];
            }
        }
        #endregion

        #region =删除某个Session对象=
        /// <summary>
        /// 删除某个Session对象
        /// </summary>
        /// <param name="name">会话状态键值名称</param>
        public static void Del(string name)
        {
            HttpContext.Current.Session[name] = null;
        }
        #endregion
    }
}
