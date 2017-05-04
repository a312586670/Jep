using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Jep.Hardware.Patrol
{
    /// <summary>
    /// FYComm类型的巡更棒操作类
    /// </summary>
    public class Patrol : IHardware
    {
        [DllImport("FYComm.ocx")]//打开读卡器
        public static extern int FY_Comminit(Int16 port, Int32 rate, Int32 databit, Int32 StopBits, Int32 Parity);

        [DllImport("FYComm.ocx")]//
        public static extern int FY_IsMachineComm();

        [DllImport("FYComm.ocx")]//释放设备
        public static extern int FY_CommDestroy();

        [DllImport("FYComm.ocx")]//读取指定数据
        public static extern int FY_ReadSiteRecord(Int16 iDiRecord, StringBuilder sRet);

        [DllImport("FYComm.ocx")]//清除数据
        public static extern int FY_ClearPatrolEvent();

        [DllImport("FYComm.ocx", CharSet = CharSet.Ansi)]//获取巡更记录的总数
        public static extern int FY_GetMachineEvent(StringBuilder sRet, Int32 iOpt);

        [DllImport("FYComm.ocx", CharSet = CharSet.Ansi)]//获取巡更记录的总数
        public static extern int FY_SetMachineTime(string sRet);

        /// <summary>
        /// 清除巡更数据
        /// </summary>
        /// <returns>返回值:返回 1 为成功，否则 0 为失败</returns>
        public bool ClearData()
        {
            bool Flag = false;
            try
            {
                Flag = FY_ClearPatrolEvent() == 1;
            }
            catch
            {
            }
            return Flag;
        }

        /// <summary>
        /// 连接设备
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="Rate">波特率</param>
        /// <returns></returns>
        public bool Open(int port, int Rate)
        {
            Int16 ports = Convert.ToInt16(port);
            bool Flag = false;
            try
            {
                Flag = FY_Comminit(ports, Rate, 8, 1, 0) == 1;
                Flag = FY_IsMachineComm() == 1;
            }
            catch
            {
                throw new Exception("Open Patrol Exception");
            }
            return Flag;
        }

        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            bool Flag = false;
            try
            {
                Flag = FY_CommDestroy() == 1;
            }
            catch
            {

            }
            return Flag;
        }

        /// <summary>
        /// 清空数据
        /// </summary>
        /// <returns></returns>
        public bool Clear()
        {
            bool Flag = false;
            try
            {
                Flag = FY_ClearPatrolEvent() == 1;
            }
            catch
            {

            }
            return Flag;
        }

        /// <summary>
        /// 读取指定记录的巡更记录
        /// </summary>
        /// <param name="recordID">记录ID，例如第一条记录</param>
        /// <returns></returns>
        public Record Read(int recordID)
        {
            Record record = null;
            StringBuilder str = new StringBuilder(1024);
            int index = 0;
            try
            {
                index = FY_ReadSiteRecord(Convert.ToInt16(recordID), str);
                if (index == 1)
                {
                    string records = str.ToString();
                    if (!string.IsNullOrEmpty(records) && records.Length > 16)
                    {
                        record = new Record();
                        record.ID = Convert.ToInt32(records.Substring(0, 4), 16);
                        string timeStr = records.Substring(4, 12).ToString();
                        timeStr = timeStr.Insert(2, "-");
                        timeStr = timeStr.Insert(5, "-");
                        timeStr = timeStr.Insert(8, " ");
                        timeStr = timeStr.Insert(11, ":");
                        timeStr = timeStr.Insert(14, ":");
                        record.Time = Convert.ToDateTime(timeStr);
                        record.AddressID = records.Substring(16);
                    }
                }
            }
            catch
            {

            }
            return record;
        }

        /// <summary>
        /// 读取全部巡更记录
        /// </summary>
        /// <returns></returns>
        public Record[] Reads()
        {
            Record[] record = null;

            int count = this.GetTotal();
            if (count > 0)
            {
                record = new Record[count];
                for (int i = 0; i < count; i++)
                {
                    try
                    {
                        record[i] = this.Read(i + 1);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            return record;
        }

        /// <summary>
        /// 获取巡更记录总条数
        /// </summary>
        /// <returns></returns>
        public int GetTotal()
        {
            StringBuilder str = new StringBuilder(1024);
            int num = 0;
            try
            {
                FY_GetMachineEvent(str, 1);
                num = Convert.ToInt32(str.ToString(), 16);
            }
            catch
            {

            }
            return num;
        }

        /// <summary>
        /// 测试数据方法(在模拟测试的时候可以用这个方法来代替)
        /// </summary>
        /// <returns></returns>
        public Record[] ReadsTest()
        {
            Record[] record = new Record[10];
            for (int i = 0; i < 10; i++)
            {
                record[i] = new Record();
            }

            record[0].ID = 1;
            record[0].Time = DateTime.Now.AddHours(1);
            record[0].AddressID = "0062E8CD";
            record[1].ID = 2;
            record[1].Time = DateTime.Now.AddHours(2);
            record[1].AddressID = "0062E5CD";
            record[2].ID = 3;
            record[2].Time = DateTime.Now.AddHours(-1);
            record[2].AddressID = "0062E9CD";
            record[3].ID = 4;
            record[3].Time = DateTime.Now.AddHours(-1);
            record[3].AddressID = "0062E9FD";
            record[4].ID = 5;
            record[4].Time = DateTime.Now.AddHours(1);
            record[4].AddressID = "0062E9AD";
            record[5].ID = 6;
            record[5].Time = DateTime.Now.AddHours(1);
            record[5].AddressID = "0062E9BD";
            record[6].ID = 7;
            record[6].Time = DateTime.Now.AddHours(3);
            record[6].AddressID = "0062E9AD";
            record[7].ID = 8;
            record[7].Time = DateTime.Now.AddHours(6);
            record[7].AddressID = "0063E9AD";
            record[8].ID = 9;
            record[8].Time = DateTime.Now.AddHours(1);
            record[8].AddressID = "0066E9AD";
            record[9].ID = 10;
            record[9].Time = DateTime.Now.AddHours(2);
            record[9].AddressID = "0026E9AD";
            return record;
        }

        /// <summary>
        /// 格式化时间
        /// </summary>
        /// <returns></returns>
        public bool SetTime()
        {
            bool Flag = false;
            string time = DateTime.Now.ToString("yy") +this.GetWeekNumber() + DateTime.Now.ToString("MMddHHmmss");
            if (FY_SetMachineTime(time) != 0)
            {
                Flag = true;
            }
            return Flag;
        }

        /// <summary>
        /// 获取数字星期几（1-7）
        /// </summary>
        /// <returns></returns>
        private string GetWeekNumber()
        {
            string str = DateTime.Now.DayOfWeek.ToString();
            string  week ="";
            switch (str)
            {
                case "Monday":
                    week = "01";
                    break;
                case "Tuesday":
                    week = "02";
                    break;
                case "Wednesday":
                    week = "03";
                    break;
                case "Thursday":
                    week = "04";
                    break;
                case "Friday":
                    week = "05";
                    break;
                case "Saturday":
                    week = "06";
                    break;
                case "Sunday":
                    week = "07";
                    break;

            }
            return week;
        }
    }
}

