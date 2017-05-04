using System;
using System.Collections.Generic;
using System.Text;

namespace Jep.Base.DataStructure
{
    /// <summary>
    /// 队列节点
    /// </summary>
    public class ETQueueNode<T>
    {
        public ETQueueNode()
        {
        }

        public ETQueueNode(T value, ETQueueNode<T> nextNode, ETQueueNode<T> prevNode)
        {
            this._Value = value;
            this._NextNode = nextNode;
            this._PrevNode = prevNode;
        }

        public ETQueueNode(T value, ETQueueNode<T> prevNode)
            : this(value, default(ETQueueNode<T>), prevNode)
        {

        }

        public ETQueueNode(T value)
            : this(value, default(ETQueueNode<T>), default(ETQueueNode<T>))
        {

        }

        private T _Value = default(T);

        /// <summary>
        /// 节点存放的数据
        /// </summary>
        public T Value
        {
            get { return this._Value; }
            set { this._Value = value; }
        }

        private ETQueueNode<T> _NextNode = default(ETQueueNode<T>);

        /// <summary>
        /// 下一个节点
        /// </summary>
        public ETQueueNode<T> NextNode
        {
            get { return _NextNode; }
            set { _NextNode = value; }
        }

        private ETQueueNode<T> _PrevNode = default(ETQueueNode<T>);

        /// <summary>
        /// 上一个节点
        /// </summary>
        public ETQueueNode<T> PrevNode
        {
            get { return _PrevNode; }
            set { _PrevNode = value; }
        }
    }
}
