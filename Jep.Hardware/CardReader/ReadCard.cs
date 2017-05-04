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

using System.Drawing;
using System.IO;

namespace Jep.Hardware.CardReader
{
    /// <summary>
    /// 提供操作读卡器方法
    /// </summary>
    public class ReadCard
    {
        private int _Port;
        private int _baud;

        /// <summary>
        /// 使用指定参数构造一个读卡器操作对象
        /// </summary>
        /// <param name="port">串口号，串口从1～8 。</param>
        /// <param name="baud">波特率，现使用115200。</param>
        public ReadCard(int port, int baud)
        {
            this._Port = port;
            this._baud = baud;
        }

        /// <summary>
        /// 使用默认波特率构造一个读卡器操作对象
        /// </summary>
        /// <param name="port">串口号，串口从1～8 。</param>
        public ReadCard(int port)
            : this(port, 115200)
        {
        }

        private bool _Open()
        {
            return ThirdInOneReadCard.Init(this._Port, this._baud);
        }

        /// <summary>
        /// 打开设备
        /// </summary>
        public bool Open()
        {
            return _Open();
        }

        private bool _Close()
        {
            return ThirdInOneReadCard.Close(this._Port);
        }

        /// <summary>
        /// 关闭设备
        /// </summary>
        public bool Close()
        {
            return _Close();
        }

        /// <summary>
        /// 寻找卡片
        /// </summary>
        /// <returns></returns>
        private bool LookupCard(byte type, out long serialNo)
        {
            serialNo = 0;
            byte serialLen = 0;
            //关闭一次
            this._Close();
            this._Open();
            ThirdInOneReadCard.GTReset();
            bool flag = ThirdInOneReadCard.LookupCard(type, ref serialNo, ref serialLen) && serialLen > 0;
            return flag;
        }

        private IDCard _ReadIDCard()
        {
            IDCard idcard = null;
            byte[] utmp = new byte[2048];
            int nLen = 0;
            nLen = ThirdInOneReadCard.ReadMefire(255, ref utmp[0]);//读取身份证
            if (nLen > 0)
            {
                idcard = new IDCard();
                char[] trimChars = { ' ', '\t', '\0' };
                idcard.Name = Encoding.Default.GetString(utmp, 0, 30).Trim(trimChars);//姓名
                idcard.Sex = (short)(Encoding.Default.GetString(utmp, 30, 6).Trim(trimChars) == "男" ? 1 : 2);//性别
                idcard.EthnicGroup = Encoding.Default.GetString(utmp, 36, 20).Trim(trimChars);//民族
                idcard.Birthday = Convert.ToDateTime(Encoding.Default.GetString(utmp, 56, 16).Trim(trimChars).Insert(6, "-").Insert(4, "-"));//出生日期
                idcard.Address = Encoding.Default.GetString(utmp, 72, 70).Trim(trimChars);//地址
                idcard.IDCardNumber = Encoding.Default.GetString(utmp, 142, 36).Trim(trimChars);//身份证号
                idcard.IssueUnit = Encoding.Default.GetString(utmp, 178, 30).Trim(trimChars);//发证机关
                idcard.ValidityPeriod = Encoding.Default.GetString(utmp, 208, 32).Trim(trimChars);//有效期
                //读取照片
                string photoPath = AppDomain.CurrentDomain.BaseDirectory + "tmp.bmp";
                if (File.Exists(photoPath))
                {
                    //将照片载入内存
                    MemoryStream ms = new MemoryStream();
                    FileStream fs = new FileStream(photoPath, FileMode.Open, FileAccess.ReadWrite);
                    byte[] buffer = new byte[1024];
                    int count = 0;
                    while ((count = fs.Read(buffer, 0, 1024)) > 0)
                        ms.Write(buffer, 0, count);
                    fs.Close();
                    File.Delete(photoPath);
                    idcard.Photo = Bitmap.FromStream(ms);
#if OnTest
                    idcard.Photo.Save("c:\\abc.bmp");
#endif
                    ms.Close();
                }
            }
            return idcard;
        }

        /// <summary>
        /// 读取身份证
        /// </summary>
        /// <returns></returns>
        public IDCard ReadIDCard()
        {
            return _ReadIDCard();
        }

        private object _Read(out CardType type)
        {
            try
            {
                long serialNo = 0;
                if (LookupCard(2, out serialNo)) //寻卡，读取EM卡)
                {
                    this.Beep(5);
                    type = CardType.EMCard;
                    return serialNo.ToString();
                }
                if (LookupCard(1, out serialNo))
                {
                    this.Beep(5);
                    type = CardType.ICCard;
                    return serialNo.ToString();
                }
                if (LookupCard(3, out serialNo))
                {
                    type = CardType.IDCard;
                    return ReadIDCard();
                }
                type = CardType.Error;
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 使用读卡器读取卡片，并返回读取的数据和卡片类型
        /// </summary>
        /// <param name="type">用于接收返回的卡片类型</param>
        /// <returns></returns>
        public object Read(out CardType type)
        {
            return _Read(out type);
        }

        private void _Beep(byte beep_100mS)
        {
            ThirdInOneReadCard.Beep(beep_100mS);
        }

        /// <summary>
        /// 控制峰鸣器鸣叫。
        /// </summary>
        /// <param name="beep_100mS">峰鸣器鸣叫的持续时间。（100毫秒为1单元）</param>
        public void Beep(byte beep_100mS)
        {
            _Beep(beep_100mS);
        }

        /// <summary>
        /// 控制LED亮、灭。
        /// </summary>
        /// <param name="on">true（灯亮）；false（灯灭）</param>
        private void _Led(Boolean on)
        {
            ThirdInOneReadCard.Led(on);
        }

        /// <summary>
        /// 控制LED亮、灭。
        /// </summary>
        /// <param name="on">true（灯亮）；false（灯灭）</param>
        public void Led(Boolean on)
        {
            _Led(on);
        }
    }
}
