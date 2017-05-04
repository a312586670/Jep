using System;
using System.Collections.Generic;
using System.Text;

namespace Jep.Net
{
    ///<summary>
    /// 地理位置,包括国家和地区
    ///</summary>
    public struct IPLocation
    {
        /// <summary>
        /// 国家
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        public string Area { get; set; }
    }
}
