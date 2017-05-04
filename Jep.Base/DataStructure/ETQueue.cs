using System;
using System.Collections.Generic;
using System.Text;

using System.Collections;

namespace Jep.Base.DataStructure
{
    /// <summary>
    /// 队列结构
    /// </summary>
    public class ETQueue<T> : IEnumerable
    {
        /// <summary>
        /// 头部节点
        /// </summary>
        private ETQueueNode<T> _Header = default(ETQueueNode<T>);
        /// <summary>
        /// 最后一个节点
        /// </summary>
        private ETQueueNode<T> _LastNode = default(ETQueueNode<T>);
        private void _Add(ETQueueNode<T> node)
        {
            if (_Header == default(ETQueueNode<T>))
            {
                //建立首个节点
                node.NextNode = default(ETQueueNode<T>);
                node.PrevNode = default(ETQueueNode<T>);
                _Header = node;
                _LastNode = _Header;
            }
            else
            {
                //建立非首个节点
                node.PrevNode = _LastNode;
                _LastNode.NextNode = node;
                _LastNode = node;
            }
            _Count++;
        }

        /// <summary>
        /// 添加一个节点到队列
        /// </summary>
        /// <param name="node">要添加到队列的节点</param>
        public void Add(ETQueueNode<T> node)
        {
            _Add(node);
        }

        /// <summary>
        /// 添加数据并构建一个节点到队列
        /// </summary>
        /// <param name="data">要添加的数据</param>
        public void Add(T data)
        {
            _Add(new ETQueueNode<T>(data));
        }

        private T _Peek()
        {
            if (_Header == default(ETQueueNode<T>))
            {
                throw new EmptyNodeException("当前队列为空。");
            }
            else
            {
                return _Header.Value;
            }
        }

        /// <summary>
        /// 获取队列首个节点的数据，但不移除该节点。
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            return _Peek();
        }

        private T _Pop()
        {
            if (_Header == default(ETQueueNode<T>))
            {
                throw new EmptyNodeException("当前队列为空。");
            }
            else
            {
                T value = _Header.Value;
                _Header = _Header.NextNode;
                _Count--;
                return value;
            }
        }

        /// <summary>
        /// 获取队列首个节点的数据，并移除该节点。
        /// </summary>
        /// <returns></returns>
        public T Pop()
        {
            return _Pop();
        }

        private bool _IsEmpty()
        {
            return _Header == default(ETQueueNode<T>);
        }

        /// <summary>
        /// 获取一个bool值，该值指示当前队列是否为空。
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return _IsEmpty();
        }

        private int _Count = 0;

        /// <summary>
        /// 获取队列中元素的个数。
        /// </summary>
        public int Count
        {
            get { return _Count; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator<T>)GetEnumerator();
        }

        public ETEnumerator GetEnumerator()
        {
            return new ETEnumerator(_Header);
        }

        public class ETEnumerator : IEnumerator
        {
            ETQueueNode<T> header = default(ETQueueNode<T>);
            ETQueueNode<T> node = default(ETQueueNode<T>);
            public ETEnumerator(ETQueueNode<T> header)
            {
                this.header = header;
            }

            /// <summary>
            /// 获取枚举数当前位置的元素。
            /// </summary>
            public object Current
            {
                get
                {
                    return (ETQueueNode<T>)node;
                }
            }

            /// <summary>
            /// 使枚举数前进到 ETQueue<T> 的下一个元素。
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                if (node == default(ETQueueNode<T>))
                {
                    node = header;
                }
                else
                {
                    node = node.NextNode;
                }
                return node != null;
            }

            /// <summary>
            /// 重置位置
            /// </summary>
            public void Reset()
            {
                node = default(ETQueueNode<T>);
            }

            /// <summary>
            /// 释放由 ETEnumerator 使用的所有资源。
            /// </summary>
            public void Dispose()
            {
                node = null;
                header = null;
            }
        }
    }
}
