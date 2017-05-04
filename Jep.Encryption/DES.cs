using System;
using System.Collections.Generic;
using System.Text;

using System.Security.Cryptography;
using System.IO;

namespace Jep.Encryption
{
    /// <summary>
    /// 提供DES加密方法
    /// </summary>
    public class DES
    {
        #region 默认密钥向量
        /// <summary>
        /// 默认密钥向量
        /// </summary>
        private static byte[] Keys = {  0x4E, 0x3E, 0x54, 0x53, 0x56, 0x55, 0x4A, 0x6C, 0x77, 0x64, 0x63, 0x42, 0x4D, 0x46,
                                        0x6A, 0x6E, 0x41, 0x4B, 0x62, 0x3F, 0x71, 0x78, 0x76, 0x79, 0x65, 0x47, 0x7A, 0x66,
                                        0x4C, 0x50, 0x3D, 0x5F, 0x45, 0x52, 0x40, 0x5A, 0x5C, 0x61, 0x6D, 0x5D, 0x43, 0x68,
                                        0x67, 0x6F, 0x57, 0x44, 0x3B, 0x51, 0x75, 0x58, 0x5B, 0x3C, 0x74, 0x6B, 0x70, 0x72,
                                        0x5E, 0x60, 0x69, 0x49, 0x48, 0x4F, 0x59, 0x73, 0x2E, 0x33, 0x1E, 0x01, 0x13, 0x22,
                                        0x10, 0x0B, 0x28, 0x2D, 0x20, 0x0D, 0x19, 0x3C, 0x3B, 0x06, 0x11, 0x1C, 0x0C, 0x00,
                                        0x3D, 0x1D, 0x2F, 0x21, 0x03, 0x02, 0x05, 0x04, 0x2C, 0x31, 0x3E, 0x23, 0x32, 0x24,
                                        0x27, 0x38, 0x1F, 0x39, 0x25, 0x12, 0x0A, 0x09, 0x18, 0x1B, 0x2A, 0x29, 0x3A, 0x0E,
                                        0x35, 0x07, 0x26, 0x0F, 0x2B, 0x36, 0x14, 0x37, 0x3F, 0x34, 0x30, 0x16, 0x08, 0x15,
                                        0x17, 0x1A 
                                     };
        #endregion

        #region DES加密字符串
        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="encryptKey">加密密钥,要求为8位</param>
        /// <returns>加密成功返回加密后的字符串，失败返回null</returns>
        public static string EncryptDES(string encryptString, string encryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 8));
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray());
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region DES解密字符串
        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>
        /// <returns>解密成功返回解密后的字符串，失败返回null</returns>
        public static string DecryptDES(string decryptString, string decryptKey)
        {
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(decryptKey);
                byte[] rgbIV = Keys;
                byte[] inputByteArray = Convert.FromBase64String(decryptString);
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region DES加密字符串，默认加密钥匙
        /// <summary>
        /// DES加密字符串，默认加密钥匙
        /// </summary>
        /// <param name="encryptString"></param>
        /// <returns></returns>
        public static string EncryptDES(string encryptString)
        {
            return EncryptDES(encryptString, "^$#&*^$&*D21KFJlalsfj013");
        }
        #endregion

        #region DES解密字符串，默认加密钥匙
        /// <summary>
        /// DES解密字符串，默认加密钥匙
        /// </summary>
        /// <param name="decryptString"></param>
        /// <returns></returns>
        public static string DecryptDES(string decryptString)
        {
            return DecryptDES(decryptString, "^$#&*^$&*D21KFJlalsfj013");
        }
        #endregion
    }
}
