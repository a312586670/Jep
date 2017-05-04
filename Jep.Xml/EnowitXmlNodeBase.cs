/*
 * 创建人：@谢华良
 * 创建时间:2013年4月9日
 *目标: XML文档节点基类
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Jep.Xml
{
    /// <summary>
    /// EnowitXmlNodeBase基类，用于操作XML文档节点的基类
    /// </summary>
    public class EnowitXmlNodeBase
    {
       
        private string _Name;

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
      
        private string _Value;

        /// <summary>
        /// 值
        /// </summary>
        public string Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        /// <summary>
        /// 构造EnowitXmlNodeBase对象
        /// </summary>
        public EnowitXmlNodeBase() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        public EnowitXmlNodeBase(string name,string value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// 构造EnowitXmlNodeBase对象
        /// </summary>
        /// <param name="name"></param>
        public EnowitXmlNodeBase(string name)
        {
            this.Name = name;
        }
    }
}
