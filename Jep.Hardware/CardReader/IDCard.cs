/*
 * 身份证实体类
 * 作者：尹俊
 * 完成日期：2013年4月16日
 */
using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;

namespace Jep.Hardware.CardReader
{
    /// <summary>
    /// 身份证
    /// </summary>
    public class IDCard
    {
        /// <summary>
        /// 构造一个无数据的IDCard对象
        /// </summary>
        public IDCard()
        {
        }

        /// <summary>
        /// 使用指定数据构造一个IDCard对象
        /// </summary>
        /// <param name="name">姓名</param>
        /// <param name="sex">性别（1为男性，2为女性）</param>
        /// <param name="ethnicGroup">民族</param>
        /// <param name="birthday">生日</param>
        /// <param name="address">住址</param>
        /// <param name="idCardNumber">身份证号码</param>
        /// <param name="issueUnit">签发机关</param>
        /// <param name="validityPeriod">有效期限</param>
        /// <param name="Photo">照片</param>
        public IDCard(string name, short sex, string ethnicGroup, DateTime birthday, string address, string idCardNumber, string issueUnit, string validityPeriod, Image photo)
        {
            this.Name = name;
            this.Sex = sex;
            this.EthnicGroup = ethnicGroup;
            this.Birthday = birthday;
            this.Address = address;
            this.IDCardNumber = idCardNumber;
            this.IssueUnit = issueUnit;
            this.ValidityPeriod = validityPeriod;
            this.Photo = photo;
        }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 性别。1为男性，2为女性。
        /// </summary>
        public short Sex { get; set; }
        /// <summary>
        /// 民族
        /// </summary>
        public string EthnicGroup { get; set; }
        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime Birthday { get; set; }
        /// <summary>
        /// 住址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 公民身份号码
        /// </summary>
        public string IDCardNumber { get; set; }
        /// <summary>
        /// 签发机关
        /// </summary>
        public string IssueUnit { get; set; }
        /// <summary>
        /// 有效期限
        /// </summary>
        public string ValidityPeriod { get; set; }
        /// <summary>
        /// 照片
        /// </summary>
        public Image Photo { get; set; }
    }
}
