/*
 * 提供三合一读卡器的操作方法
 * 作者：尹俊
 * 完成日期：2013年4月16日
 */
//#define OnTest
#undef OnTest

using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;

namespace Jep.Hardware.CardReader
{
    /// <summary>
    /// 提供三合一读卡器的操作方法
    /// </summary>
    public class ThirdInOneReadCard
    {
        /// <summary>
        /// 初始化读卡器通信端口
        /// </summary>
        /// <param name="port">串口号，串口从1～8 。</param>
        /// <param name="baud">波特率，现使用115200。</param>
        /// <returns>true成功；false失败</returns>
        [DllImport("ThirdInOne Drive\\ThirdInOne.dll", EntryPoint = "XZX_Init")]//打开读卡器
        public static extern Boolean Init(int port, int baud);

        /// <summary>
        /// 关闭读卡器通信端口
        /// </summary>
        /// <param name="port">串口号，串口从1～8 。</param>
        /// <returns>true成功；false失败</returns>
        [DllImport("ThirdInOne Drive\\ThirdInOne.dll", EntryPoint = "XZX_Close")]//关闭读卡器
        public static extern Boolean Close(int port);

        /// <summary>
        /// 控制峰鸣器鸣叫。
        /// </summary>
        /// <param name="beep_100mS">峰鸣器鸣叫的持续时间。（100毫秒为1单元）</param>
        [DllImport("ThirdInOne Drive\\ThirdInOne.dll", EntryPoint = "XZX_Beep2")]//蜂鸣
        public static extern void Beep(byte beep_100mS);

        /// <summary>
        /// 控制LED亮、灭。
        /// </summary>
        /// <param name="on">true（灯亮）；false（灯灭）</param>
        [DllImport("ThirdInOne Drive\\ThirdInOne.dll", EntryPoint = "XZX_Led")]//LED开关
        public static extern void Led(Boolean on);

        /// <summary>
        /// 使M1卡进入中止模式。
        /// </summary>
        /// <returns>true成功；false失败</returns>
        [DllImport("ThirdInOne Drive\\ThirdInOne.dll", EntryPoint = "XZX_GT_Halt")]//挂起卡片
        public static extern Boolean GTHalt();

        /// <summary>
        /// 重置读卡器M1卡。
        /// </summary>
        /// <returns>true成功；false失败</returns>
        [DllImport("ThirdInOne Drive\\ThirdInOne.dll", EntryPoint = "XZX_GT_Reset")]//卡片复位
        public static extern Boolean GTReset();

        /// <summary>
        /// 搜寻读卡器上的M1/ID/二代证卡，并返回卡号或厂家码。
        /// </summary>
        /// <param name="nType">卡片类型，1(M1)；2(ID)；3(二代证)</param>
        /// <param name="SerialNum">M1和EM返回序列号,二代证返回厂家码</param>
        /// <param name="nSerialLen">返回SerialNum的长度</param>
        /// <returns>false无卡；true有卡</returns>
        [DllImport("ThirdInOne Drive\\ThirdInOne.dll", EntryPoint = "XZX_LookupCard")]//寻卡
        public static extern Boolean LookupCard(byte nType, ref long SerialNum, ref byte nSerialLen);

        /// <summary>
        /// M1卡密钥预装到阅读器，下载第sectionNum扇区的密码。
        /// </summary>
        /// <param name="mode">0(keyA),1(keyB)</param>
        /// <param name="sectionNum">扇区索引号（0~15）</param>
        /// <param name="key">写入读卡器密码（6字节大小）</param>
        /// <returns>false失败；true成功</returns>
        [DllImport("ThirdInOne Drive\\ThirdInOne.dll", EntryPoint = "XZX_LoadKey")]//M1卡密钥预装到阅读器
        public static extern Boolean LoadKey(byte mode, byte sectionNum, ref byte key);

        /// <summary>
        /// 用已预先存储的密码认证M1卡。
        /// </summary>
        /// <param name="mode">0：(keyA),1：(keyB)</param>
        /// <param name="sectionNum">扇区索引号（0~15）</param>
        /// <returns>true认证成功；false认证失败</returns>
        [DllImport("ThirdInOne Drive\\ThirdInOne.dll", EntryPoint = "XZX_Authenticate")]//用预装的密钥认证卡
        public static extern Boolean Authenticate(byte mode, byte sectionNum);

        /// <summary>
        /// 读卡上信息（已通过认证）
        /// </summary>
        /// <param name="Block_Adr">如当前为 M1卡，则为起始块索引号（0～63）。如当前为身份证卡，则为读身份证卡的输出信息可选项参数。</param>
        /// <param name="_Data">返回数据缓冲地址。</param>
        /// <returns>0：读卡失败，否则为返回的数据长度。</returns>
        [DllImport("ThirdInOne Drive\\ThirdInOne.dll", EntryPoint = "XZX_ReadMefire")]//读卡（已通过认证）
        public static extern int ReadMefire(byte Block_Adr, ref byte _Data);

        /// <summary>
        /// 写入从第blockNum块开始地址的M1卡的数据。
        /// </summary>
        /// <param name="Start_Block">起始块索引号（0~63）</param>
        /// <param name="_Data">起始块索引号（0~63）</param>
        /// <returns>True：成功；false: 失败</returns>
        [DllImport("ThirdInOne Drive\\ThirdInOne.dll", EntryPoint = "XZX_WriteMefire", CharSet = CharSet.Ansi)]//Write 命令
        public static extern Boolean WriteMefire(byte Start_Block, ref byte _Data);
    }
}
