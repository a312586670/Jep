/*
 * 创建人：@谢华良
 * 创建时间:2013年4月10 16:00
 * 目标：字符串验证类
 */
using System;
using System.Collections.Generic;
using System.Text;

using System.Text.RegularExpressions;

namespace Jep.Text
{
    /// <summary>
    /// 验证字符串类
    /// </summary>
    public class CheckString
    {
        #region =身份证验证=
        /// <summary>
        /// 身份证验证
        /// </summary>
        /// <param name="IdCard">身份证号</param>
        /// <returns>bool值</returns>
        public static bool CheckIDCard(string IdCard)
        {
            if (IdCard.Length == 18)
            {
                bool check = CheckIDCard18(IdCard);
                return check;
            }
            else if (IdCard.Length == 15)
            {
                bool check = CheckIDCard15(IdCard);
                return check;
            }
            else
            {
                return false;
            }
        }

        #region 18为身份证验证
        /// <summary>
        /// 18位身份证验证
        /// </summary>
        /// <param name="Id">身份证号</param>
        /// <returns></returns>
        private static bool CheckIDCard18(string Id)
        {
            long n = 0;
            if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = Id.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower())
            {
                return false;//校验码验证
            }
            return true;//符合GB11643-1999标准
        }
        #endregion

        #region 15位身份证验证
        /// <summary>
        /// 15位身份证验证
        /// </summary>
        /// <param name="Id">身份证号</param>
        /// <returns></returns>
        private static bool CheckIDCard15(string Id)
        {
            long n = 0;
            if (long.TryParse(Id, out n) == false || n < Math.Pow(10, 14))
            {
                return false;//数字验证
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(Id.Remove(2)) == -1)
            {
                return false;//省份验证
            }
            string birth = Id.Substring(6, 6).Insert(4, "-").Insert(2, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证
            }
            return true;//符合15位身份证标准
        }
        #endregion
        #endregion

        #region =验证电话号码=
        /// <summary>
        /// 验证是否是电话号码
        /// </summary>
        /// <param name="input">要验证的电话号码(除手机号码)</param>
        /// <returns>bool</returns>
        public static bool IsTellPhone(string input)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(input, @"^(\d{3,4}-)?\d{6,8}$");
        }

        /// <summary>
        /// 验证是否是手机号码
        /// </summary>
        /// <param name="input">要验证的手机号码</param>
        /// <returns>bool</returns>
        public static bool IsMobilePhone(string input)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^1\\d{10}$");
            return regex.IsMatch(input);
        }

        /// <summary>
        /// 验证电话号码
        /// </summary>
        /// <param name="input"></param>
        /// <returns>bool</returns>
        public static bool IsTell(string input)
        {
            if (IsMobilePhone(input))
            {
                return true;
            }
            if (IsTellPhone(input))
            {
                return true;
            }
            return false;
        }
        #endregion

        #region =邮编验证=
        /// <summary>
        /// 验证是否是邮编
        /// </summary>
        /// <param name="input">要验证的邮编号码</param>
        /// <returns>bool</returns>
        public static bool IsPostalCode(string input)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(input, @"^\d{6}$");
        }
        #endregion

        #region =验证是否是有效网址=
        /// <summary>
        /// 验证是否是有效网址
        /// </summary>
        /// <param name="url">要验证的网址</param>
        /// <returns>bool</returns>
        public static bool HasUrl(string source)
        {
            return Regex.IsMatch(source,
            @"(((file|gopher|news|nntp|telnet|http|ftp|https|ftps|sftp)://)|(www\.))+(([a-zA-Z0-9\._-]+\.[
a-zA-Z]{2,6})|([0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}))(/[a-zA-Z0-9\&amp;%_\./-~-]*)?",
            RegexOptions.IgnoreCase);
        }
        #endregion

        #region =检测客户的输入中是否有危险字符串=
        /// <summary>
        /// 检测客户输入的字符串是否有效,并将原始字符串修改为有效字符串或空字符串。
        /// 当检测到客户的输入中有攻击性危险字符串,则返回false,有效返回true。
        /// </summary>
        /// <param name="input">要检测的字符串</param>
        /// <returns>bool</returns>
        private static bool _IsValidInput(ref string input)
        {
            try
            {
                if (string.IsNullOrEmpty(input))
                {
                    //如果是空值,则跳出
                    return true;
                }
                else
                {
                    //替换单引号
                    input = input.Replace("'", "''").Trim();
                    //检测攻击性危险字符串
                    string testString = "and |or |exec |insert |select |delete |update |count |chr |mid |master |truncate |char |declare ";
                    string[] testArray = testString.Split('|');
                    foreach (string testStr in testArray)
                    {
                        if (input.ToLower().IndexOf(testStr) != -1)
                        {
                            //检测到攻击字符串,清空传入的值
                            input = "";
                            return false;
                        }
                    }
                    //未检测到攻击字符串
                    return true;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 检测客户输入的字符串是否有效,并将原始字符串修改为有效字符串或空字符串。
        /// 当检测到客户的输入中有攻击性危险字符串,则返回false,有效返回true。
        /// </summary>
        /// <param name="input">要检测的字符串</param>
        /// <returns>bool</returns>
        public static bool IsValidInput(ref string input)
        {
            return _IsValidInput(ref input);
        }
        #endregion
    }
}
