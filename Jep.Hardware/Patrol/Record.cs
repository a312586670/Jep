using System;
using System.Collections.Generic;
using System.Text;

namespace Jep.Hardware.Patrol
{
    /// <summary>
    /// 巡更设备记录实体类
    /// </summary>
    public class Record
    {
        public Record()
        { 
            
        }

        private int _ID;

        /// <summary>
        /// 记录ID
        /// </summary>
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        private string addressID;

        /// <summary>
        /// 巡更地点卡号
        /// </summary>
        public string AddressID
        {
            get { return addressID; }
            set { addressID = value; }
        }
        private DateTime time;

        /// <summary>
        /// 巡更时间
        /// </summary>
        public DateTime Time
        {
            get { return time; }
            set { time = value; }
        }
    }
}
