using System;
using System.Collections.Generic;
using System.Text;

namespace Jep.Hardware.Patrol
{
    public interface IHardware
    {
        /// <summary>
        /// 打开设备
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="Rate">波特率</param>
        /// <returns>返回值:True-连接成功,False-连接失败</returns>
        bool Open(int port, int Rate);

        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <returns></returns>
        bool Close();

        /// <summary>
        /// 清空数据
        /// </summary>
        /// <returns></returns>
        bool Clear();

        /// <summary>
        /// 读取指定巡更记录
        /// </summary>
        /// <param name="recordID">记录ID</param>
        /// <returns></returns>
        Record Read(int recordID);

        /// <summary>
        /// 读取全部内容
        /// </summary>
        /// <returns></returns>
        Record[] Reads();

        /// <summary>
        /// 获取数据总数
        /// </summary>
        /// <returns></returns>
        int GetTotal();

        /// <summary>
        /// 更改时间
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        bool SetTime();
    }
}
