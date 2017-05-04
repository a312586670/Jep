/*
 * 创建人：@谢华良
 * 创建时间:2013年4月7日
 * 目标: 加密类
 */
using System;
using System.Collections.Generic;
using System.Text;

using Jep.Encryption;
using Jep.Encryption;

namespace Jep.Database
{
    /// <summary>
    /// 加密类
    /// </summary>
    public class DatabaseEncryption
    {
        /// <summary>
        /// 数据库数据加密专用方法
        /// </summary>
        /// <param name="value">要加密的字符串</param>
        /// <returns></returns>
        private static string _Encrypt(string value)
        {
            return DES.EncryptDES(value);
        }

        /// <summary>
        /// 数据库数据解密专用方法
        /// </summary>
        /// <param name="value">要解密的密文</param>
        /// <returns></returns>
        private static string _Decrypt(string value)
        {
            return DES.DecryptDES(value);
        }

        /// <summary>
        /// 数据库数据加密专用方法
        /// </summary>
        /// <param name="value">要加密的字符串</param>
        /// <returns></returns>
        private static string _Encrypt(string value, string key)
        {
            return DES.EncryptDES(value, key);
        }

        /// <summary>
        /// 数据库数据解密专用方法
        /// </summary>
        /// <param name="value">要解密的密文</param>
        /// <param name="key">密钥</param>
        /// <returns></returns>
        private static string _Decrypt(string value, string key)
        {
            return DES.DecryptDES(value, key);
        }

        /// <summary>
        /// 数据库数据加密专用方法
        /// </summary>
        /// <param name="value">要加密的字符串</param>
        /// <param name="key">加密密钥,要求为8位</param>
        /// <returns></returns>
        public static string Encrypt(string value, string key)
        {
            return _Encrypt(value, key);
        }

        /// <summary>
        /// 数据库数据解密专用方法
        /// </summary>
        /// <param name="value">要解密的密文</param>
        /// <param name="key">加密密钥,要求为8位</param>
        /// <returns></returns>
        public static string Decrypt(string value, string key)
        {
            return _Decrypt(value, key);
        }

        /// <summary>
        /// 数据库数据解密专用方法
        /// </summary>
        /// <param name="value">要解密的密文</param>
        /// <returns></returns>
        public static string Decrypt(string value)
        {
            return _Decrypt(value);
        }

        /// <summary>
        /// 数据库数据加密专用方法
        /// </summary>
        /// <param name="value">要加密的明文</param>
        /// <returns></returns>
        public static string Encrypt(string value)
        {
            return _Encrypt(value);
        }
    }
}
