/*
 * 创建人：@谢华良
 * 创建时间:2013年4月9日
 *目标: XML文档节点类
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Jep.Xml
{
    /// <summary>
    /// EnowitXMlNode类，用于操作XML文档节点
    /// </summary>
    public class EnowitXmlNode:EnowitXmlNodeBase
    {
        private List<EnowitXmlNodeAttribute> _nodeAttribute;//节点属性

        /// <summary>
        /// 节点属性列表
        /// </summary>
        public List<EnowitXmlNodeAttribute> NodeAttribute
        {
            get { return _nodeAttribute; }
            set { _nodeAttribute = value; }
        }
        /// <summary>
        /// 父节点
        /// </summary>
        private EnowitXmlNode _parentNode;//父节点

        /// <summary>
        /// 父节点
        /// </summary>
        public EnowitXmlNode ParentNode
        {
            get { return _parentNode; }
            set { _parentNode = value; }
        }
        /// <summary>
        /// 孩子节点
        /// </summary>
        private EnowitXmlNodes _childNode = null;//孩子节点

        /// <summary>
        /// 孩子节点
        /// </summary>
        public EnowitXmlNodes ChildNode
        {
            get { return _childNode; }
            set { _childNode = value; }
        }
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public EnowitXmlNode()
        {
            _childNode = new EnowitXmlNodes(this);
            _nodeAttribute = new List<EnowitXmlNodeAttribute>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">节点名称</param>
        public EnowitXmlNode(string name)
            : base(name)
        {
            _childNode = new EnowitXmlNodes(this);
            _nodeAttribute = new List<EnowitXmlNodeAttribute>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">节点名称</param>
        /// <param name="value">节点值</param>
        public EnowitXmlNode(string name, string value)
            : base(name, value)
        {
            _childNode = new EnowitXmlNodes(this);
            _nodeAttribute = new List<EnowitXmlNodeAttribute>();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">节点名称</param>
        /// <param name="value">节点值</param>
        /// <param name="parent">父节点</param>
        public EnowitXmlNode(string name, string value, EnowitXmlNode parent)
            : base(name, value)
        {
            this.ParentNode = parent;
        }
    }
}
