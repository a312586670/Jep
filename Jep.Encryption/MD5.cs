using System;
using System.Collections.Generic;
using System.Text;

using System.Security.Cryptography; 

namespace Jep.Encryption
{
    /// <summary>
    /// 提供MD5加密方法
    /// </summary>
    public class MD5
    {
        /// <summary> 
        /// md5 16位加密方法
        /// </summary> 
        /// <param name="str">要加密的字符串</param> 
        /// <returns></returns> 
        public static string Md5Str16(string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(str)), 4, 8);
            t2 = t2.Replace("-", "");
            return t2;
        }

        /// <summary> 
        /// md5 32位加密方法
        /// </summary> 
        /// <param name="str">要加密的字符串</param> 
        /// <returns></returns> 
        public static string Md5Str32(string str)
        {
            string cl = str;
            string pwd = "";
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();//实例化一个md5对像 
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　 
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得 
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd = pwd + s[i].ToString("X2");
            }
            return pwd;
        }

        /// <summary>
        /// md5 32位加密方法加强版
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns></returns>
        public static string PlusMd5Str32(string str)
        {
            return Md5Str32(Md5Str32(Md5Str32(str + "$#&465*(F696hjkdjf") + "$&(5465*#Fa5440sfd898SDHf") + "^*&$@*@77Fjhf548778&#*$(");
        }
    }
}
