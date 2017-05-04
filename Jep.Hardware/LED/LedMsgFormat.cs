/*Enowit科技有限公司
 * 创建人:@曹江波
 * 创建时间:2013年7月26日
 * 描述： LED数据包格式类
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Jep.Hardware.LED
{
    /// <summary>
    /// LED数据包格式类
    /// </summary>
    public class LedMsgFormat
    {
        #region 字段

        private bool _isSave = false;               //是否预留
        private int _style = 0;                     //显示花样 0-6
        private int _speed = 0;                     //显示速度 0-9
        private int _stopTime = 0;                  //停留时间 0-9
        private string _content = string.Empty;     //显示内容

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public LedMsgFormat() { }

        /// <summary>
        /// 以指定的参数构造LED数据包信息对象
        /// </summary>
        public LedMsgFormat(bool isSave, int style, int speed, int stopTime, string content)
        {
            _isSave = isSave;
            _style = style;
            _speed = speed;
            _stopTime = stopTime;
            _content = content;
        }

        #endregion

        #region 属性

        /// <summary>
        /// 是否预留
        /// </summary>
        public bool IsSave
        {
            get { return _isSave; }
            set { _isSave = value; }
        }

        /// <summary>
        /// 显示花样 范围：0-6档
        /// </summary>
        public int Style
        {
            get { return _style; }
            set { _style = value; }
        }

        /// <summary>
        /// 显示速度 范围：0-9档
        /// </summary>
        public int Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        /// <summary>
        /// 停留时间 范围：0-9档
        /// </summary>
        public int StopTime
        {
            get { return _stopTime; }
            set { _stopTime = value; }
        }

        /// <summary>
        /// 显示内容 有校验位的内容长度最大为：249，无校验位的最大长度为93.
        /// </summary>
        public string Content
        {
            get { return _content; }
            set { _content = value; }
        }



        #endregion
    }
}
