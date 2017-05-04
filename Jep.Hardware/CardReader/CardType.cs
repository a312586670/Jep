/*
 * 卡片类型
 * 作者：尹俊
 * 完成日期：2013年4月16日
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Jep.Hardware.CardReader
{
    /// <summary>
    /// 卡片类型
    /// </summary>
    public enum CardType
    {
        /// <summary>
        /// EM卡片
        /// </summary>
        EMCard = 1,
        /// <summary>
        /// IC卡片
        /// </summary>
        ICCard = 2,
        /// <summary>
        /// 二代身份证卡片
        /// </summary>
        IDCard = 3,
        /// <summary>
        /// 错误卡片类型
        /// </summary>
        Error = 0
    }
}
