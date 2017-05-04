using System;

using System.Runtime.InteropServices;

namespace Jep.Encryption
{
    /// <summary>
    /// AES加密算法
    /// </summary>
    public class AES
    {
        /// <summary>
        /// AES加密函数
        /// </summary>
        /// <param name="value">要加密的字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="keyBit">密钥位数</param>
        /// <returns></returns>
        [DllImport(@"SecurityModule.dll")]
        private static extern string EncryptString(string value, string key, int keyBit);

        /// <summary>
        /// AES解密函数
        /// </summary>
        /// <param name="value">要解密的字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="keyBit">密钥位数</param>
        /// <returns></returns>
        [DllImport(@"SecurityModule.dll")]
        private static extern string DecryptString(string value, string key, int keyBit);

        /// <summary>
        /// AES加密函数
        /// </summary>
        /// <param name="text">要加密的字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="keysize">密钥长度</param>
        /// <returns>加密后的字符串</returns>
        public string EntryptString(string text, string key, KeySize keysize = KeySize.B128)
        {
            if (text.Trim().Length == 0)
                throw new Exception("要加密的字符串不能为空。");
            if (key.Trim().Length == 0)
                throw new Exception("加密所需的密钥不能为空。");
            switch (keysize)
            {
                case KeySize.B192:
                    return EncryptString(text, key, 192);
                case KeySize.B256:
                    return EncryptString(text, key, 256);
                default:
                    return EncryptString(text, key, 128);
            }
        }

        /// <summary>
        /// AES加密函数
        /// </summary>
        /// <param name="text">要加密的字符串</param>
        /// <returns></returns>
        public static string EntryptString(string text)
        {
            return new AES().EntryptString(text, "U*#()FDFJ*#54$E**safks83", KeySize.B256);
        }

        /// <summary>
        /// AES解密函数
        /// </summary>
        /// <param name="text">被加密的字符串</param>
        /// <param name="key">密钥</param>
        /// <param name="keysize">密钥长度</param>
        /// <returns>解密后的字符串</returns>
        public string DecryptString(string text, string key, KeySize keysize = KeySize.B128)
        {
            switch (keysize)
            {
                case KeySize.B192:
                    return DecryptString(text, key, 192);
                case KeySize.B256:
                    return DecryptString(text, key, 256);
                default:
                    return DecryptString(text, key, 128);
            }
        }

        /// <summary>
        /// AES解密函数
        /// </summary>
        /// <param name="text">被加密的字符串</param>
        /// <returns></returns>
        public static string DecryptString(string text)
        {
            return new AES().DecryptString(text, "U*#()FDFJ*#54$E**safks83", KeySize.B256);
        }
    }

    /// <summary>
    /// 密钥大小，可以为128位，192位，256位
    /// </summary>
    public enum KeySize
    {
        /// <summary>
        /// 128位密钥
        /// </summary>
        B128 = 128,

        /// <summary>
        /// 192位密钥
        /// </summary>
        B192 = 192,

        /// <summary>
        /// 256位密钥
        /// </summary>
        B256 = 256
    };
}
