using System;
using System.Collections.Generic;
using System.Text;

namespace Jep.Hardware.Patrol
{
    /// <summary>
    /// 巡更棒类型
    /// </summary>
    public enum PatrolType
    { 
        /// <summary>
        /// K510型巡更棒
        /// </summary>
        K510,
        /// <summary>
        /// FyComm接口型巡更棒
        /// </summary>
        FyComm
    }

    /// <summary>
    /// 巡更棒设备操作类
    /// </summary>
    public class PatrolPublic
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public PatrolPublic()
        { 
        }

        private PatrolK510 patrolK510;

        private Patrol patrol;

        /// <summary>
        /// 连接设备
        /// </summary>
        /// <param name="port">端口</param>
        /// <param name="rate">波特率</param>
        /// <returns></returns>
        public bool Open(int port,int rate,PatrolType type)
        {
            bool Flag = false;
            try
            {
                if (type == PatrolType.K510)
                {
                    patrolK510 = new PatrolK510();
                    Flag=patrolK510.Open(port,rate);
                }
                if (type == PatrolType.FyComm)
                {
                    patrol = new Patrol();
                    Flag = patrol.Open(port, rate);
                }
            }
            catch
            {
                throw new Exception(" Patrol Content  Exception");
            }
            return Flag;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="type">设备类型</param>
        /// <returns></returns>
        public bool Close(PatrolType type)
        {
            bool Flag = false;
            try
            {
                if (type == PatrolType.K510)
                {
                   Flag=this.patrolK510.Close();
                }
                if (type == PatrolType.FyComm)
                {
                    Flag = this.patrol.Close();
                }
            }
            catch
            {
                throw new Exception("Patrol Close Exception");
            }
            return Flag;
        }

        /// <summary>
        /// 读取指定记录的巡更记录
        /// </summary>
        /// <param name="type">设备类型</param>
        /// <returns></returns>
        public Record Read(int recordID,PatrolType type)
        {
            Record record = null;
            try
            {
                if (type == PatrolType.K510)
                {
                    record = this.patrolK510.Read(recordID);
                }
                if(type==PatrolType.FyComm)
                {
                    record = this.patrol.Read(recordID);
                }
            }
            catch
            {
                throw new Exception("Read Patrol Data Exception");
            }
            return record;
        }

        /// <summary>
        /// 读取全部巡更记录
        /// </summary>
        /// <param name="type">设备类型</param>
        /// <returns></returns>
        public Record[] Reads(PatrolType type)
        {
            Record[] record = null;
            try
            {
                if (type == PatrolType.K510)
                {
                    record = this.patrolK510.Reads();
                }
                if (type == PatrolType.FyComm)
                {
                    record = this.patrol.Reads();
                }
            }
            catch
            {
                throw new Exception("Reads Patrol Records Exception");
            }
            return record;
        }

        /// <summary>
        /// 读取巡更记录总数
        /// </summary>
        /// <param name="type">设备类型</param>
        /// <returns></returns>
        public int GetRecordTotal(PatrolType type)
        {
            int count = 0;
            try
            {
                if (type == PatrolType.K510)
                {
                    count = this.patrolK510.GetTotal();
                }
                if(type==PatrolType.FyComm)
                {
                    count = this.patrol.GetTotal();
                }
            }
            catch {
                throw new Exception("Read Patrol RecordTotal Exception");
            }
            return count;
        }

        /// <summary>
        /// 清空巡更记录
        /// </summary>
        /// <param name="type">设备类型</param>
        /// <returns></returns>
        public bool Clear(PatrolType type)
        {
            bool Flag = false;
            try
            {
                if (type == PatrolType.K510)
                {
                    Flag = this.patrolK510.Clear();
                }
                if (type == PatrolType.FyComm)
                {
                    Flag = this.patrol.Clear();
                }
            }
            catch
            {
                throw new Exception("Clear Patrol Record Exception");
            }
            return Flag;
        }

        /// <summary>
        /// 格式化时间
        /// </summary>
        /// <returns></returns>
        public bool SetTime(PatrolType type)
        {
            bool Flag = false;
            try
            {
                if (type == PatrolType.K510)
                {
                    Flag = this.patrolK510.SetTime();
                }
                else if (type == PatrolType.FyComm)
                {
                    Flag = this.patrol.SetTime();
                }
            }
            catch
            {
                throw new Exception("k510 Patrol SetTime Exception");
            }
            return Flag;
        }
    }
}
