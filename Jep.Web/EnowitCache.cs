/*
 * 创建人：@谢华良
 * 创建时间：2013年4月11日 11:15
 * 目标：web缓存类
 */
using System;
using System.Collections.Generic;
using System.Text;

using System.Web;
using System.Collections;

namespace Jep.Web
{
    /// <summary>
    /// 缓存操作类
    /// </summary>
    public class EnowitCache
    {
        #region =获取数据缓存=
        /// <summary>
        /// 获取数据缓存
        /// </summary>
        /// <param name="key">键</param>
        public static object GetCache(string key)
        {
            object obj=null;
            try{
                System.Web.Caching.Cache objCache = HttpRuntime.Cache;
                obj = objCache[key];
            }
            catch{
                throw new HttpException("GetCache");
            }
            return null;
        }
        #endregion

        #region =设置缓存=
        /// <summary>
        /// 设置缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetCache(string key, object value)
        {
            try
            {
                System.Web.Caching.Cache objCache = HttpRuntime.Cache;
                objCache.Insert(key, value);
            }
            catch {
                throw new HttpException("SetCache");
            }
        }
        
        /// <summary>
        /// 设置数据缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="Timeout">间隔时间</param>
        public static void SetCache(string key, object value, TimeSpan Timeout)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(key, value, null, DateTime.MaxValue,Timeout, System.Web.Caching.CacheItemPriority.NotRemovable, null);
        }

        /// <summary>
        /// 设置暑假缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="absoluteExpiration">到期时间</param>
        /// <param name="slidingExpiration">间隔时间</param>
        public static void SetCache(string CacheKey, object objObject, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            try
            {
                System.Web.Caching.Cache objCache = HttpRuntime.Cache;
                objCache.Insert(CacheKey, objObject, null, absoluteExpiration, slidingExpiration);
            }
            catch {
                throw new HttpException("SetCache"); 
            }
        }
        #endregion

        #region =移除指定数据缓存=
        /// <summary>
        /// 移除指定数据缓存
        /// </summary>
        /// <param name="key">键</param>
        public static void RemoveAllCache(string key)
        {
            try
            {
                System.Web.Caching.Cache _cache = HttpRuntime.Cache;
                _cache.Remove(key);
            }
            catch {
                throw new HttpException("RemoveAllCache"); 
            }
        }
        #endregion

        #region =移除全部数据缓存缓存=
        /// <summary>
        /// 移除全部数据缓存缓存
        /// </summary>
        public static void RemoveAllCache()
        {
            try
            {
                System.Web.Caching.Cache _cache = HttpRuntime.Cache;
                IDictionaryEnumerator CacheEnum = _cache.GetEnumerator();
                while (CacheEnum.MoveNext())
                {
                    _cache.Remove(CacheEnum.Key.ToString());
                }
            }
            catch {
                throw new HttpException("RemoveAllCache"); 
            }
        }
        #endregion
    }
}
