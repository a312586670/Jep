/*
 * 创建人：@谢华良
 * 创建时间:2013年4月9日
 *目标:XML文档节点列表类
 */
using System;
using System.Collections.Generic;
using System.Text;

using System.Collections;

namespace Jep.Xml
{
    /// <summary>
    /// EnowitXmlNodes基类，用于操作XML文档节点的一个集合
    /// </summary>
    public class EnowitXmlNodes : IEnumerable
    {
        private EnowitXmlNode _rootNode = null;
        private List<EnowitXmlNode> _nodes = null;

        /// <summary>
        /// 节点集合
        /// </summary>
        /// <param name="node">节点</param>
        public EnowitXmlNodes(EnowitXmlNode node)
        {
            _rootNode = node;
        }

        /// <summary>
        /// 增加方法
        /// </summary>
        /// <param name="node">EnowitXmlNode节点</param>
        public void Add(EnowitXmlNode node)
        {
            if (_nodes == null)
                _nodes = new List<EnowitXmlNode>();
            node.ParentNode = _rootNode;
            _nodes.Add(node);
        }

        /// <summary>
        /// 返回一个IEnumerator对象
        /// </summary>
        /// <returns></returns>
        public IEnumerator GetEnumerator()
        {
            return new EnowitXmlNodeEnum(_nodes);
        }
    }

    /// <summary>
    /// 继承ITnumerator接口，实现迭代
    /// </summary>
    public class EnowitXmlNodeEnum : IEnumerator
    {
        private List<EnowitXmlNode> _nodes;
        int position = -1;

        /// <summary>
        /// 构造一个EnowitXmlNodeEnum对象
        /// </summary>
        /// <param name="list">集合</param>
        public EnowitXmlNodeEnum(List<EnowitXmlNode> list)
        {
            _nodes = list;
        }

        /// <summary>
        /// 将枚举数推进到集合的下一个元素。
        /// </summary>
        /// <returns>如果枚举数成功地推进到下一个元素，则为 true；如果枚举数越过集合的结尾，则为 false。</returns>
        public bool MoveNext()
        {
            if (_nodes == null)
                return false;
            position++;
            return (position < _nodes.Count);
        }

        /// <summary>
        /// 将枚举数设置为其初始位置，该位置位于集合中第一个元素之前。
        /// </summary>
        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        /// <summary>
        /// 获取当前节点
        /// </summary>
        public EnowitXmlNode Current
        {
            get
            {
                try
                {
                    return _nodes[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}
