/*
 * 创建人：@谢华良
 * 创建时间:2013年4月9日
 *目标: XML文档节点属性类
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Jep.Xml
{
    /// <summary>
    /// EnowitXMLNodeAttribute类，用于操作XML文档节点属性
    /// </summary>
    public class EnowitXmlNodeAttribute : EnowitXmlNodeBase
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public EnowitXmlNodeAttribute()
        { 
        
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">节点名称</param>
        /// <param name="value">节点值</param>
        public EnowitXmlNodeAttribute(string name, string value)
            : base(name, value)
        { }

    }
}
