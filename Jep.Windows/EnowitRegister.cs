/*
 * 创建人：@谢华良
 * 创建时间：2013年4月12日
 * 目标:注册表操作类
 */
using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Win32;
namespace Jep.Windows
{
    /// <summary>
    /// 注册表操作类
    /// </summary>
    public class EnowitRegister
    {
        #region 公共方法
        #region =写入注册表,如果指定项已经存在,则修改指定项的值=
        /// <summary>
        /// 写入注册表,如果指定项已经存在,则修改指定项的值
        /// </summary>
        /// <param name="keyType">注册表基项枚举</param>
        /// <param name="key">注册表项,不包括基项</param>
        /// <param name="name">值名称</param>
        /// <param name="values">值</param>
        /// <returns>返回布尔值,指定操作是否成功</returns>
        public bool setValue(Jep.Windows.EnowitRegister.keyType keyType, string key, string name, string values)
        {
            try
            {
                RegistryKey rk = (RegistryKey)getRegistryKey(keyType);
                RegistryKey rkt = rk.CreateSubKey(key);
                if (rkt != null)
                    rkt.SetValue(name, values);
                else
                {
                    throw (new Exception("要写入的项不存在"));
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region =读取注册表=
        /// <summary>
        /// 读取注册表
        /// </summary>
        /// <param name="keyType">注册表基项枚举</param>
        /// <param name="key">注册表项,不包括基项</param>
        /// <param name="name">值名称</param>
        /// <returns>返回字符串</returns>
        public string getValue(Jep.Windows.EnowitRegister.keyType keyType, string key, string name)
        {
            try
            {
                RegistryKey rk = (RegistryKey)getRegistryKey(keyType);
                RegistryKey rkt = rk.OpenSubKey(key);
                if (rkt != null)
                    return rkt.GetValue(name).ToString();
                else
                    throw (new Exception("无法找到指定项"));
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        #region =删除注册表中的值=
        /// <summary>
        /// 删除注册表中的值
        /// </summary>
        /// <param name="keyType">注册表基项枚举</param>
        /// <param name="key">注册表项名称,不包括基项</param>
        /// <param name="name">值名称</param>
        /// <returns>返回布尔值,指定操作是否成功</returns>
        public bool deleteValue(Jep.Windows.EnowitRegister.keyType keyType, string key, string name)
        {
            try
            {
                RegistryKey rk = (RegistryKey)getRegistryKey(keyType);
                RegistryKey rkt = rk.OpenSubKey(key, true);
                if (rkt != null)
                    rkt.DeleteValue(name, true);
                else
                    throw (new Exception("无法找到指定项"));
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region =删除注册表中的指定项=
        /// <summary>
        /// 删除注册表中的指定项
        /// </summary>
        /// <param name="keytype">注册表基项枚举</param>
        /// <param name="key">注册表中的项,不包括基项</param>
        /// <returns>返回布尔值,指定操作是否成功</returns>
        public bool deleteSubKey(Jep.Windows.EnowitRegister.keyType keytype, string key)
        {
            try
            {
                RegistryKey rk = (RegistryKey)getRegistryKey(keytype);
                rk.DeleteSubKeyTree(key);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region =判断指定项是否存在=
        /// <summary>
        /// 判断指定项是否存在
        /// </summary>
        /// <param name="keyType">基项枚举</param>
        /// <param name="key">指定项字符串</param>
        /// <returns>返回布尔值,说明指定项是否存在</returns>
        public bool isExist(Jep.Windows.EnowitRegister.keyType keyType, string key)
        {
            RegistryKey rk = (RegistryKey)getRegistryKey(keyType);

            if (rk.OpenSubKey(key) == null)
                return false;
            else
                return true;
        }
        #endregion

        #region =检索指定项关联的所有值=
        /// <summary>
        /// 检索指定项关联的所有值
        /// </summary>
        /// <param name="keyType">基项枚举</param>
        /// <param name="key">指定项字符串</param>
        /// <returns>返回指定项关联的所有值的字符串数组</returns>
        public string[] getValues(Jep.Windows.EnowitRegister.keyType keyType, string key)
        {
            RegistryKey rk = (RegistryKey)getRegistryKey(keyType);
            RegistryKey rkt = rk.OpenSubKey(key);
            if (rkt != null)
            {
                string[] names = rkt.GetValueNames();
                if (names.Length == 0)
                    return names;
                else
                {
                    string[] values = new string[names.Length];
                    int i = 0;
                    foreach (string name in names)
                    {
                        values[i] = rkt.GetValue(name).ToString();
                        i++;
                    }
                    return values;
                }
            }
            else
            {
                throw (new Exception("指定项不存在"));
            }
        }
        #endregion
        #endregion

        #region 私有方法
        /// <summary>
        /// 返回RegistryKey对象
        /// </summary>
        /// <param name="keytype">注册表基项枚举</param>
        /// <returns></returns>
        private object getRegistryKey(Jep.Windows.EnowitRegister.keyType keytype)
        {
            RegistryKey rk = null;

            switch (keytype)
            {
                case keyType.HKEY_CLASS_ROOT:
                    rk = Registry.ClassesRoot;
                    break;
                case keyType.HKEY_CURRENT_USER:
                    rk = Registry.CurrentUser;
                    break;
                case keyType.HKEY_LOCAL_MACHINE:
                    rk = Registry.LocalMachine;
                    break;
                case keyType.HKEY_USERS:
                    rk = Registry.Users;
                    break;
                case keyType.HKEY_CURRENT_CONFIG:
                    rk = Registry.CurrentConfig;
                    break;
            }
            return rk;
        }
        #endregion

        #region 枚举
        /// <summary>
        /// 注册表基项枚举
        /// </summary>
        public enum keyType : int
        {
            /// <summary>
            /// 注册表基项 HKEY_CLASSES_ROOT
            /// </summary>
            HKEY_CLASS_ROOT,
            /// <summary>
            /// 注册表基项 HKEY_CURRENT_USER
            /// </summary>
            HKEY_CURRENT_USER,
            /// <summary>
            /// 注册表基项 HKEY_LOCAL_MACHINE
            /// </summary>
            HKEY_LOCAL_MACHINE,
            /// <summary>
            /// 注册表基项 HKEY_USERS
            /// </summary>
            HKEY_USERS,
            /// <summary>
            /// 注册表基项 HKEY_CURRENT_CONFIG
            /// </summary>
            HKEY_CURRENT_CONFIG
        }
        #endregion
    }
}
