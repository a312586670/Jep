using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Drawing;
namespace Jep.Hardware.CardReader
{
    /// <summary>
    /// IDR210身份证阅读器操作类
    /// </summary>
    public class IDR210
    {
        [DllImport("IDR210\\sdtapi.dll")]//打开读卡器
        private static extern int InitComm(Int32 port);

        [DllImport("IDR210\\sdtapi.dll")]//关闭设备
        private static extern int CloseComm();

        [DllImport("IDR210\\sdtapi.dll")]//卡认证接口
        private static extern int Authenticate();

        [DllImport("IDR210\\sdtapi.dll", CharSet = CharSet.Ansi)]//读卡信息
        private static extern int ReadBaseInfos([MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder Name, [MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder Gender,
            [MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder Folk, [MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder BirthDay,
            [MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder Code, [MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder Address,
            [MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder Agency, [MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder ExpireStart,
            [MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder ExpireEnd);

        [DllImport("IDR210\\sdtapi.dll", CharSet = CharSet.Ansi)]//寻卡
        private static extern int Routon_IC_FindCard(StringBuilder cardNo);

        [DllImport("IDR210\\sdtapi.dll", CharSet = CharSet.Ansi)]//设置蜂鸣声
        private static extern int HID_BeepLED(bool BeepON, bool LEDON, int duration);

        [DllImport("IDR210\\sdtapi.dll", CharSet = CharSet.Ansi)]//读IC卡序列号
        private static extern int Routon_IC_HL_ReadCardSN(StringBuilder cardNo);

        [DllImport("IDR210\\sdtapi.dll", CharSet = CharSet.Ansi)]//读卡信息
        private static extern int ReadBaseInfosPhoto([MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder Name,
           [MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder Gender,
           [MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder Folk,
           [MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder BirthDay,
           [MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder Code,
           [MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder Address,
           [MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder Agency,
           [MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder ExpireStart,
           [MarshalAsAttribute(UnmanagedType.LPStr)] StringBuilder ExpireEnd,
           string directory);

        /// <summary>
        /// 打开设备
        /// </summary>
        /// <returns></returns>
        public static bool Open()
        {
            bool Flag = false;
            try
            {
                Flag = InitComm(1001) == 1;
            }
            catch
            {

            }
            return Flag;
        }

        /// <summary>
        /// 关闭设备
        /// </summary>
        /// <returns></returns>
        public static bool Close()
        {
            bool Flag = false;
            try
            {
                Flag = CloseComm() == 1;
            }
            catch
            {

            }
            return Flag;
        }

        /// <summary>
        /// 读取身份证信息
        /// </summary>
        /// <returns></returns>
        public static IDCard Read()
        {
            IDCard idCard =null;
            StringBuilder name = new StringBuilder(1024);
            StringBuilder sex = new StringBuilder(1024);
            StringBuilder Folk = new StringBuilder(1024);
            StringBuilder BirthDay = new StringBuilder(1024);
            StringBuilder CodeID = new StringBuilder(1024);
            StringBuilder Address = new StringBuilder(1024);
            StringBuilder Agency = new StringBuilder(1024);
            StringBuilder Start = new StringBuilder(1024);
            StringBuilder End = new StringBuilder(1024);
            try
            {
                Authenticates();
                int i = ReadBaseInfos(name, sex, Folk, BirthDay, CodeID, Address, Agency, Start, End);
                //string appPath = AppDomain.CurrentDomain.BaseDirectory + "photo\\";
                //int i = ReadBaseInfosPhoto(name, sex, Folk, BirthDay, CodeID, Address, Agency, Start, End, appPath);
                if (i == 1)
                {
                    idCard = new IDCard();
                    if (Address.ToString().Contains(CodeID.ToString()))
                    {
                        idCard.Address = Address.ToString().Substring(0, Address.ToString().Length - CodeID.ToString().Length);
                    }
                    else
                    {
                        idCard.Address = Address.ToString();
                    }
                    idCard.Birthday = Convert.ToDateTime(BirthDay.ToString().Insert(4,"-").Insert(7,"-"));
                    idCard.EthnicGroup = Folk.ToString();
                    idCard.IDCardNumber = CodeID.ToString();
                    idCard.IssueUnit = Agency.ToString();
                    idCard.Name = name.ToString();
                    string photoPath = AppDomain.CurrentDomain.BaseDirectory + "photo.bmp";
                    string photo1 = AppDomain.CurrentDomain.BaseDirectory + "1.jpg";
                    string photo2 = AppDomain.CurrentDomain.BaseDirectory + "2.jpg";
                    if (File.Exists(photo1))
                    {
                        File.Delete(photo1);
                    }
                    if (File.Exists(photo2))
                    {
                        File.Delete(photo2);
                    }
//                    if (File.Exists(photoPath))
//                    {
//                        //将照片载入内存
//                        MemoryStream ms = new MemoryStream();
//                        FileStream fs = new FileStream(photoPath, FileMode.Open, FileAccess.ReadWrite);
//                        byte[] buffer = new byte[1024];
//                        int count = 0;
//                        while ((count = fs.Read(buffer, 0, 1024)) > 0)
//                            ms.Write(buffer, 0, count);
//                        fs.Close();
//                        File.Delete(photoPath);
//                        idCard.Photo = Bitmap.FromStream(ms);
//#if OnTest
//                    idcard.Photo.Save("c:\\abc.bmp");
//#endif
//                        ms.Close();
//                    }
                    idCard.Sex = sex.ToString() == "男" ? Convert.ToInt16(1) : Convert.ToInt16(0);
                    idCard.ValidityPeriod = Start.ToString() + "-" + End.ToString();
                }
            }
            catch
            {
            }
            return idCard;
        }

        /// <summary>
        /// 读取身份证信息
        /// </summary>
        /// <returns></returns>
        public static string[] Reads()
        {
            string[] IDstr =null;
            try
            {
                IDCard idcard = Read();
                if (idcard != null)
                {
                    IDstr=new string[8];
                    IDstr[0] = idcard.Name.ToString();//姓名
                    IDstr[1] = Convert.ToInt32(idcard.Sex) == 1 ? "男" : "女";//性别
                    IDstr[2] = idcard.EthnicGroup.ToString();//民族
                    IDstr[3] = idcard.Birthday.ToString("yyyy-MM-dd");//出生日期
                    IDstr[4] = idcard.Address.ToString();//地址
                    IDstr[5] = idcard.IDCardNumber.ToString();//身份证号
                    IDstr[6] = idcard.IssueUnit.ToString();//发证机关
                    IDstr[7] = idcard.ValidityPeriod.ToString();//有效期
                }
            }
            catch
            {
                return null;
            }
            return IDstr;
        }

//        public static string[] ReadsUrl()
//        {
//            string[] IDstr = null;
//            StringBuilder name = new StringBuilder(1024);
//            StringBuilder sex = new StringBuilder(1024);
//            StringBuilder Folk = new StringBuilder(1024);
//            StringBuilder BirthDay = new StringBuilder(1024);
//            StringBuilder CodeID = new StringBuilder(1024);
//            StringBuilder Address = new StringBuilder(1024);
//            StringBuilder Agency = new StringBuilder(1024);
//            StringBuilder Start = new StringBuilder(1024);
//            StringBuilder End = new StringBuilder(1024);
//            string path = AppDomain.CurrentDomain.BaseDirectory + "photo";
//            try
//            {
//                Authenticates();
//                int i = ReadBaseInfosPhoto(name, sex, Folk, BirthDay, CodeID, Address, Agency, Start, End,path);
//                //string appPath = AppDomain.CurrentDomain.BaseDirectory+"photo";
//                //int i = ReadBaseInfosPhoto(name, sex, Folk, BirthDay, CodeID, Address, Agency, Start, End, appPath);
//                if (i == 1)
//                {
//                    IDstr = new string[8];
//                    if (Address.ToString().Contains(CodeID.ToString()))
//                    {
//                        IDstr[4]  = Address.ToString().Substring(0, Address.ToString().Length - CodeID.ToString().Length);
//                    }
//                    else
//                    {
//                        IDstr[4] = Address.ToString();
//                    }
//                    IDstr[0] = name.ToString();
//                    IDstr[1] = sex.ToString();
//                    IDstr[2] = Folk.ToString();
//                    IDstr[3] = Convert.ToDateTime(BirthDay.ToString().Insert(4, "-").Insert(7, "-")).ToString("yyyy-MM-dd");
//                    IDstr[5] = CodeID.ToString();
//                    IDstr[6] = Agency.ToString();
//                    IDstr[7] = Start.ToString() + "-" + End.ToString();
                   
//                    string photoPath = AppDomain.CurrentDomain.BaseDirectory + "photo.bmp";
//                    string photo1 = AppDomain.CurrentDomain.BaseDirectory + "1.jpg";
//                    string photo2 = AppDomain.CurrentDomain.BaseDirectory + "2.jpg";
//                    if (File.Exists(photo1))
//                    {
//                        File.Delete(photo1);
//                    }
//                    if (File.Exists(photo2))
//                    {
//                        File.Delete(photo2);
//                    }
//                    if (File.Exists(photoPath))
//                    {
//                        //将照片载入内存
//                        MemoryStream ms = new MemoryStream();
//                        FileStream fs = new FileStream(photoPath, FileMode.Open, FileAccess.ReadWrite);
//                        byte[] buffer = new byte[1024];
//                        int count = 0;
//                        while ((count = fs.Read(buffer, 0, 1024)) > 0)
//                            ms.Write(buffer, 0, count);
//                        fs.Close();
//                        File.Delete(photoPath);
//                        idCard.Photo = Bitmap.FromStream(ms);
//#if OnTest
//                    idcard.Photo.Save("c:\\abc.bmp");
//#endif
//                        ms.Close();
//                    }
//                }
//            }
//            catch
//            {
//            }
//            //try
//            //{
//            //    IDCard idcard = Read();
//            //    if (idcard != null)
//            //    {
//            //        IDstr = new string[8];
//            //        IDstr[0] = idcard.Name.ToString();//姓名
//            //        IDstr[1] = Convert.ToInt32(idcard.Sex) == 1 ? "男" : "女";//性别
//            //        IDstr[2] = idcard.EthnicGroup.ToString();//民族
//            //        IDstr[3] = idcard.Birthday.ToString("yyyy-MM-dd");//出生日期
//            //        IDstr[4] = idcard.Address.ToString();//地址
//            //        IDstr[5] = idcard.IDCardNumber.ToString();//身份证号
//            //        IDstr[6] = idcard.IssueUnit.ToString();//发证机关
//            //        IDstr[7] = idcard.ValidityPeriod.ToString();//有效期
//            //    }
//            //}
//            //catch
//            //{
//            //    return null;
//            //}
//            return IDstr;
//        }

        /// <summary>
        /// 认证身份证卡并选择卡
        /// </summary>
        /// <returns></returns>
        private static bool Authenticates()
        {
            bool Flag = false;
            try
            {
                Flag = Authenticate() == 1;
            }
            catch
            {

            }
            return Flag;
        }

        /// <summary>
        /// 寻卡
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns></returns>
        public static bool FindCard(out string cardID)
        {
            bool Flag = false;
            cardID = "";
            try
            {
                StringBuilder str=new StringBuilder();
                //Flag = Routon_IC_FindCard(str) == 1;
                Flag = Routon_IC_HL_ReadCardSN(str) == 1;
                cardID = Convert.ToInt32(str.ToString(), 16).ToString() ;
            }
            catch { 
            
            }
            return Flag;
        }

        /// <summary>
        /// 控制峰鸣器鸣叫。
        /// </summary>
        /// <param name="beep_100mS"></param>
        public static void Beep(int beep_100mS)
        {
            HID_BeepLED(true, true, beep_100mS);
        }
    }
}
