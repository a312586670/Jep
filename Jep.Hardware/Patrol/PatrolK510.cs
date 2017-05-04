/*Enowit科技有限公司
 * 创建人:@谢华良
 * 创建时间:2013年4月17日
 * 目标:巡更棒操作类
 */
using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;

namespace Jep.Hardware.Patrol
{
    /// <summary>
    /// 巡更棒操作类
    /// </summary>
    public class PatrolK510 : IHardware
    {
        #region 属性
        /// <summary>
        /// 巡更机器号
        /// </summary>
        private string number;
        /// <summary>
        /// 记录总条数
        /// </summary>
        private int Total;
        #endregion

        #region =构造函数=
        /// <summary>
        /// 构造函数并未打开连接
        /// </summary>
        public PatrolK510()
        {

        }

        #endregion

        [DllImport("DLLK500.dll")]
        private static extern bool OpenCommPort(char[] ComNum);

        [DllImport("DLLK500.dll")]
        private static extern bool CloseCommPort();

        [DllImport("DLLK500.dll")]
        public static extern int GetMachineNumber(Byte[] getchar);

        //get records number
        [DllImport("DLLK500.dll")]
        private static extern int GetRecordNumbers(Byte[] getchar, char[] MachineId);

        [DllImport("DLLK500.dll")]
        private static extern int GetFristRecordEx(Byte[] getchar, char[] MachineId, int RecNo);

        //get the next record
        [DllImport("DLLK500.dll")]
        private static extern int GetNextRecordEx(Byte[] getchar, char[] MachineId, int RecNo);

        //delete records
        [DllImport("DLLK500.dll")]
        private static extern int DeleteAllRecord(Byte[] getchar, char[] MachineId);

        #region 将字节数组转换位字符串
        // <summary>
        /// 将字节数组转换位字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private string BytesToString(byte[] bytes)
        {
            string str = string.Empty;
            foreach (byte bt in bytes)
            {
                str += Convert.ToChar(bt).ToString();
            }
            return str;
        }

        #endregion

        /// <summary>
        /// 连接K510巡更棒操作
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="Rate">波特率(此波特率不用)</param>
        /// <returns></returns>
        public bool Open(int port, int Rate)
        {
            string port2 = "COM" + port.ToString();
            bool Flag = false;
            try
            {
                Flag = OpenCommPort(port2.ToCharArray());
                if (Flag)
                {
                    this.number = this.GetMachineNumber();
                    this.Total = this.GetRecordTotal();
                }
                else
                {
                    this.Close();
                }
            }
            catch
            {
                this.Close();
                Flag = false;
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
                Flag = CloseCommPort();
            }
            catch
            {
                this.Close();
            }
            return Flag;
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        /// <returns></returns>
        public bool Clear()
        {
            bool Flag = false;
            try
            {
                Byte[] GetChar = new Byte[12];
                string number = this.number;
                if (DeleteAllRecord(GetChar, number.ToCharArray()) != 0)
                {
                    Flag = true;
                }
            }
            catch
            {
                this.Close();
            }
            return Flag;
        }

        /// <summary>
        /// 读取指定巡更记录
        /// </summary>
        /// <param name="recordID">记录ID,例如:第1条记录</param>
        /// <returns></returns>
        public Record Read(int recordID)
        {
            Record record = null;
            string str = "";
            if (recordID > this.Total)
            {
                throw new ArgumentOutOfRangeException("index Out Of Exception");
            }

            byte[] getChar = new byte[100];
            char[] machineNum = this.number.Trim().ToCharArray();
            if (GetNextRecordEx(getChar, machineNum, recordID) != 0)
            {
                str = BytesToString(getChar);
                //str转换成Record对象
                string[] strPatrol = str.Split(',');
                if (strPatrol != null && strPatrol.Length > 0)
                {
                    record = new Record();
                    //转换成Record[]对象
                    record.AddressID = strPatrol[2];
                    record.ID = 1;
                    record.Time = Convert.ToDateTime(DateTime.Today.Year.ToString().Substring(0, 2) + strPatrol[1]);
                }
            }
            else
            {
                this.Close();
                throw new Exception("Get new record fail.");
            }
            return record;
        }

        /// <summary>
        /// 读取全部巡更记录
        /// </summary>
        /// <returns></returns>
        public Record[] Reads()
        {
            Record[] recordArray = null;

            char[] machineNum = this.number.Trim().ToCharArray();
            int totalCount = this.Total;
            string[] records = new string[this.Total];
            int index = 0;

            for (int recNo = 1; recNo <= totalCount; recNo++)
            {
                string record = string.Empty;
                try
                {
                    if (recNo != 1)
                    {
                        record = GetNextRecord(recNo);
                    }
                    else        //第一条记录
                    {
                        record = GetFirstRecord();
                    }
                    if (!record.Equals(""))
                    {
                        records[index] = record;
                        index++;
                    }
                }
                catch
                {
                    continue;
                }
            }
            if (records != null && records.Length > 0)
            {
                recordArray = new Record[records.Length];
                for (int i = 0; i < records.Length; i++)
                {
                    string[] strPatrol = records[i].Split(',');
                    recordArray[i] = new Record();
                    //转换成Record[]对象
                    recordArray[i].AddressID = strPatrol[2];
                    recordArray[i].ID = Convert.ToInt32(strPatrol[0].Substring(0, 5));
                    recordArray[i].Time = Convert.ToDateTime(DateTime.Today.Year.ToString().Substring(0, 2) + strPatrol[1]);
                }
            }
            return recordArray;
        }


        /// <summary>
        /// 获取记录总条数
        /// </summary>
        /// <returns></returns>
        private int GetRecordTotal()
        {
            string str2 = "";
            Byte[] GetChar = new Byte[32];
            if (GetRecordNumbers(GetChar, this.number.ToCharArray()) != 0)
            {
                for (int i = 0; i < GetChar.Length; i++)
                {
                    str2 += Convert.ToChar(GetChar[i]).ToString();
                }
                str2 = int.Parse(str2, System.Globalization.NumberStyles.HexNumber).ToString();
            }
            return int.Parse(str2);
        }

        /// <summary>
        /// 获取巡更记录总数
        /// </summary>
        /// <returns></returns>
        public int GetTotal()
        {
            return this.Total;
        }

        #region 读取第一条记录
        /// <summary>
        /// 获取第一条数据记录
        /// </summary>
        /// <returns></returns>
        private string GetFirstRecord()
        {
            Byte[] getChar = new Byte[100];
            Char[] machineNum = this.number.Trim().ToCharArray();
            int recNo = 1;

            if (GetFristRecordEx(getChar, machineNum, recNo) != 0)
            {
                return BytesToString(getChar).Trim();
            }
            else
            {
                this.Close();
                throw new Exception("Get First Exception");
            }
        }
        #endregion

        #region =读取下一条记录=
        /// <summary>
        /// 获取知道索引的下一条记录
        /// </summary>
        /// <param name="recNo">索引</param>
        /// <returns></returns>
        private string GetNextRecord(int recNo)
        {
            if (recNo > this.Total)
            {
                throw new ArgumentOutOfRangeException("index Out Of Exception");
            }
            Byte[] getChar = new Byte[100];
            Char[] machineNum = this.number.Trim().ToCharArray();

            if (GetNextRecordEx(getChar, machineNum, recNo) != 0)
            {
                return BytesToString(getChar);
            }
            else
            {
                throw new Exception("Get Next Excepton");
            }
        }

        #endregion

        #region =获取巡检机号=
        /// <summary>
        /// 获取巡检机号
        /// </summary>
        /// <returns></returns>
        public string GetMachineNumber()
        {
            string machineId = string.Empty;
            Byte[] GetChar = new Byte[12];
            if (GetMachineNumber(GetChar) == 0)
            {
                throw new Exception("get MachineID Exception");
            }
            else
            {
                for (int i = 0; i < GetChar.Length; i++)
                {
                    machineId += Convert.ToChar(GetChar[i]).ToString();
                }
            }
            return machineId;
        }
        #endregion

        #region =设置时间=
        //set reader time
        [DllImport("DLLK500.dll")]
        private static extern int SetTime(Byte[] getchar, char[] MachineId, char[] pStrTime, char[] PStrWeek);

        /// <summary>
        /// 设置时间
        /// </summary>
        /// <returns></returns>
        public bool SetTime()
        {
            bool Flag = false;
            Byte[] GetChar = new Byte[12];
            if (SetTime(GetChar, this.number.ToCharArray(), DateTime.Now.ToString("yyMMddHHmm").ToCharArray(), Convert.ToInt32(DateTime.Now.DayOfWeek).ToString().ToCharArray()) != 0)
            {
                Flag = true;
            }
            return Flag;
        }
        #endregion

    }
}
