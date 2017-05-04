
/*Enowit科技有限公司
 * 创建人:@曹江波
 * 创建时间:2013年7月26日
 * 描述：四字LED显示屏操作类
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace Jep.Hardware.LED
{
    /// <summary>
    /// 四字LED显示屏操作类
    /// </summary>
    public class LED
    {
        #region 字段

        private SerialPort serialPort = new SerialPort(); //COM端口实例

        #endregion

        #region 构造函数

        public LED() { }      //构造函数

        #endregion

        #region 数据的产生与发送

        /// <summary>
        /// 向LED显示屏发送数据包
        /// </summary>
        /// <param name="port">COM端口号</param>
        /// <param name="ledMsg">需要发送的数据包</param>
        /// <returns></returns>
        public bool SendLedMsg(int port, byte[] ledMsg)
        {
            try
            {
                serialPort.PortName = "COM" + port.ToString();
                serialPort.Open();
                serialPort.Write(ledMsg, 0, ledMsg.Length);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                serialPort.Close();
            }

        }

        /// <summary>
        /// 根据指定的信息格式产生数据包
        /// </summary>
        /// <param name="hasCheckFlag">LED显示屏是否存在校验位</param>
        /// <param name="msgFormat">数据包格式</param>
        /// <returns></returns>
        public byte[] GenerateMsgPacket(bool hasCheckFlag, LedMsgFormat msgFormat)
        {
            if (hasCheckFlag)
            {
                return GenerateMsgPacket_Check(msgFormat);
            }
            else
            {
                return GenerateMsgPacket_NoCheck(msgFormat);
            }
        }

        /// <summary>
        /// 不存在校验位的数据包生成
        /// </summary>
        private byte[] GenerateMsgPacket_NoCheck(LedMsgFormat msgFormat)
        {
            byte header = 0x0A;
            byte ender = 0x0D;

            //超过最大长度进行截取
            int maxLength = 99 - 6;
            if (msgFormat.Content.Trim().Length > maxLength)
            {
                msgFormat.Content = msgFormat.Content.Substring(0, maxLength);
            }

            byte[] contentBytes = ASCIIEncoding.Default.GetBytes(msgFormat.Content);
            string lengths = string.Format("{0:00}", contentBytes.Length + 6);
            byte lengthFirst = Convert.ToByte(lengths[0]);
            byte lengthSecord = Convert.ToByte(lengths[1]);

            byte address = 0x30;
            byte cmd = msgFormat.IsSave ? Convert.ToByte(0x59) : Convert.ToByte(0x44);    //Y：预存 D：显示
            byte style = Convert.ToByte(Convert.ToChar(msgFormat.Style.ToString()));
            byte speed = Convert.ToByte(Convert.ToChar(msgFormat.Speed.ToString()));
            byte stopTime = Convert.ToByte(Convert.ToChar(msgFormat.StopTime.ToString()));
            List<byte> msgList = new List<byte>();

            //添加数据
            msgList.Add(header);
            msgList.Add(lengthFirst);
            msgList.Add(lengthSecord);
            msgList.Add(address);
            msgList.Add(address);
            msgList.Add(cmd);
            msgList.Add(style);
            msgList.Add(speed);
            msgList.Add(stopTime);
            msgList.AddRange(contentBytes);
            msgList.Add(ender);

            return msgList.ToArray();
        }

        /// <summary>
        /// 存在校验位的数据包生成
        /// </summary>
        private byte[] GenerateMsgPacket_Check(LedMsgFormat msgFormat)
        {
            byte header = 0x0A;
            byte ender = 0x0D;

            int maxLength = 256 - 7;
            if (msgFormat.Content.Trim().Length > maxLength)
            {
                msgFormat.Content = msgFormat.Content.Substring(0, maxLength);
            }

            byte[] contentBytes = ASCIIEncoding.Default.GetBytes(msgFormat.Content);
            byte length = Convert.ToByte(contentBytes.Length + 7);
            byte option = 0x00;     //预留
            byte address = 0x30;
            byte cmd = msgFormat.IsSave ? Convert.ToByte(0x59) : Convert.ToByte(0x44);    //Y：预存 D：显示
            byte style = Convert.ToByte(Convert.ToChar(msgFormat.Style.ToString()));
            byte speed = Convert.ToByte(Convert.ToChar(msgFormat.Speed.ToString()));
            byte stopTime = Convert.ToByte(Convert.ToChar(msgFormat.StopTime.ToString()));
            List<byte> msgList = new List<byte>();

            //添加数据
            msgList.Add(header);
            msgList.Add(length);
            msgList.Add(option);
            msgList.Add(address);
            msgList.Add(address);
            msgList.Add(cmd);
            msgList.Add(style);
            msgList.Add(speed);
            msgList.Add(stopTime);
            msgList.AddRange(contentBytes);

            //计算校验位
            byte chk = 0x00;
            for (int i = 1; i < msgList.Count; i++)
            {
                chk += msgList[i];
            }

            msgList.Add(chk);
            msgList.Add(ender);

            return msgList.ToArray();
        }

        #endregion

        #region 格式转换

        /// <summary>
        /// 将字节数组转换为16进制格式的字符串
        /// </summary>
        /// <param name="srcStr">需要转换的字符串</param>
        /// <param name="separator">转换后的字符分割符，默认为一个空格</param>
        /// <returns></returns>
        public string BytesToHexString(byte[] scrBytes, char separator = ' ')
        {
            char[] lookup = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
            int i = 0, p = 0, l = scrBytes.Length;
            char[] c = new char[l * 3];
            byte d;
            while (i < l)
            {
                d = scrBytes[i++];
                c[p++] = lookup[d / 0x10];
                c[p++] = lookup[d % 0x10];
                c[p++] = separator;
            }
            return new string(c, 0, c.Length);
        }

        /// <summary>
        /// 将16进制格式的字符串转换为字节数组
        /// </summary>
        /// <param name="hexStr">16进制格式的字符串</param>
        /// <param name="separator">分隔符，默认为一个空格</param>
        /// <returns></returns>
        public byte[] HexStringToBytes(string hexStr, char separator = ' ')
        {
            string[] chars = hexStr.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
            byte[] bytes = new byte[chars.Length];

            for (int i = 0; i < chars.Length; i++)
            {
                bytes[i] = Convert.ToByte(chars[i], 16);
            }

            return bytes;
        }

        #endregion

    }
}
