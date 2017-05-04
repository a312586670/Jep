/*
 * 创建人：@谢华良
 * 创建时间:2013年4月18日
 * 目标:考勤机操作类
 */
using System;
using System.Collections.Generic;
using System.Text;
namespace Jep.Hardware.Attendance
{

    /// <summary>
    /// 考勤机操作类
    /// </summary>
    public class Attendance
    {
        private zkemkeeper.CZKEMClass axCZKEM1 = new zkemkeeper.CZKEMClass();//考勤机操作对象

        //private  OnAttranceTrEventHaldere=new zkemkeeper._IZKEMEvents_OnAttTransactionEventHandler;

        //public zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler OnAtt;

        //public OnAttTransEventHandle onAtt;

        #region 属性
        private int _dwMachineNumber =1;
        private string _DeviceMAC = "";
        private string _DeviceVendor = "";
        private string _DeviceFirmwareVersion = "";//机器固件版本
        private string _DeviceName = "";//机器名称
        private string _DevicePlatName = "";//机器平台名称
        private string _DeviceSerialNumber = "";//机器系列号
        private string _DeviceIp = "";//机器IP地址
        private string _DeviceTime = "";//机器出厂时间
        private string _FingerVersion = "";//指纹算法版本

        /// <summary>
        /// 机器IP地址
        /// </summary>
        public string DeviceIp
        {
            get { return _DeviceIp; }
            private set { _DeviceIp = value; }
            
        }

        /// <summary>
        /// 机器系列号
        /// </summary>
        public string DeviceSerialNumber
        {
            get { return _DeviceSerialNumber; }
            private set { _DeviceSerialNumber = value; }
        }

        /// <summary>
        /// 机器平台名称
        /// </summary>
        public string DevicePlatName
        {
            get { return _DevicePlatName; }
            private set { _DevicePlatName = value; }
        }

        /// <summary>
        /// 机器名称
        /// </summary>
        public string DeviceName
        {
            get { return _DeviceName; }
            private set { _DeviceName = value; }
        }

        /// <summary>
        /// 获取机器固件版本
        /// </summary>
        public string DeviceFirmwareVersion
        {
            get { return _DeviceFirmwareVersion; }
            private set { _DeviceFirmwareVersion = value; }
        }

        /// <summary>
        /// 机器制造商
        /// </summary>
        public string DeviceVendor
        {
            get { return _DeviceVendor; }
            private set { _DeviceVendor = value; }
        }

        /// <summary>
        /// 机器的MAC地址
        /// </summary>
        public string DeviceMAC
        {
            get { return _DeviceMAC; }
            private set { _DeviceMAC = value; }
        }

        /// <summary>
        /// 机器号
        /// </summary>
        public int DwMachineNumber
        {
            get { return _dwMachineNumber; }
            set { _dwMachineNumber = value; }
        }

        /// <summary>
        /// 机器出厂时间
        /// </summary>
        public string DeviceTime
        {
            get { return _DeviceTime; }
            private set { _DeviceTime = value; }
        }

        /// <summary>
        /// 指纹算法版本
        /// </summary>
        public string FingerVersion
        {
            get { return _FingerVersion; }
            private set { _FingerVersion = value; }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        public Attendance()
        {
            //OnAtt=new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(OnAttTranTransactionEx);
        }
        #endregion

        /// <summary>
        /// 事件对象(用用这个方法则要引用Interop.zkemkeeper)
        /// </summary>
        public zkemkeeper.CZKEMClass EventObject
        {
            get { return this.axCZKEM1; }
        }

   

        #region 连接网络考勤机
        /// <summary>
        /// 连接网络考勤机
        /// </summary>
        /// <param name="iPaddress">IP地址</param>
        /// <param name="port">端口号</param>
        /// <returns>连接成功返回true，连接不成功返回false</returns>
        public bool ContentNet(string iPaddress, int port, int dwMachineNumber)
        {
            bool Flag = axCZKEM1.Connect_Net(iPaddress, port);
            if (Flag)
            {
                axCZKEM1.RegEvent(dwMachineNumber, 65535);

                this.DwMachineNumber = dwMachineNumber;
                this._DeviceTime = this.GetDeviceTimeInfo(this.DwMachineNumber);
                this.FingerVersion = this.GetSysFingerVersion();
                this.DeviceMAC = this.GetDeviceMAC();
                this.DeviceVendor = this.GetVendor();
                this.DeviceFirmwareVersion = this.GetFirmwareVersion();
                this.DeviceName = this.GetProductName();
                this.DevicePlatName = this.GetPlatform();
                this.DeviceSerialNumber = this.GetSerialNumber();
                this.DeviceIp = this.GetDeviceIP();
            }
            return Flag;
        }
        #endregion

        #region 断开连接
        /// <summary>
        /// 断开连接
        /// </summary>
        public void CloseContent()
        {
            axCZKEM1.Disconnect();
        }
        #endregion

        #region 获取最后一次错误
        /// <summary>
        /// 获取最后一次错误
        /// </summary>
        /// <param name="errorId">错误代号</param>
        /// <returns></returns>
        public string GetLastErrorInfo(int errorId)
        {
            string errorInfo = "";
            axCZKEM1.GetLastError(ref errorId);
            switch (errorId)
            {
                case 0:
                    errorInfo = "找不到数据或重复数据";
                    break;
                case -100:
                    errorInfo = "不支持或数据不存在";
                    break;
                case -10:
                    errorInfo = "传输的数据长度不对";
                    break;
                case -5:
                    errorInfo = "数据已经存在";
                    break;
                case -4:
                    errorInfo = "空间不足";
                    break;
                case -3:
                    errorInfo = "错误的大小";
                    break;
                case -2:
                    errorInfo = "文件读写错误";
                    break;
                case -1:
                    errorInfo = "SDK未初始化，需要重新连接";
                    break;
                case 1:
                    errorInfo = "操作正确";
                    break;
                case 4:
                    errorInfo = "参数错误";
                    break;
                case 101:
                    errorInfo = "分配缓冲区错误";
                    break;
            }
            return errorInfo;
        }
        #endregion

        #region 获取机器是否支持射频卡功能
        /// <summary>
        /// 获取机器是否支持射频卡功能
        /// </summary>
        /// <returns>当返回值为1 时，机器仅支持射频卡，为2 是即支持射频卡也支持指纹，为0 不支持射频卡</returns>
        public int GetCardFun()
        {
            int index = 0;
            axCZKEM1.GetCardFun(this.DwMachineNumber, ref index);
            return index;
        }
        #endregion

        #region 获取SDK版本号
        /// <summary>
        /// 获取SDK版本号
        /// </summary>
        /// <returns></returns>
        public string GetSDKVersion()
        {
            string str = "";
            axCZKEM1.GetSDKVersion(ref str);
            return str;
        }
        #endregion

        #region 获取当前机器状态
        /// <summary>
        /// 获取当前机器状态
        /// </summary>
        /// <returns>0-等待状态，1，登记指纹状态2，识别指纹状态4，进入菜单状态忙状态（正在处理其他工作）5，等待写卡状态</returns>
        public int QueryState()
        {
            int index = 0;
            axCZKEM1.QueryState(ref index);
            return index;
        }
        #endregion

        #region 私有方法
        #region 获取机器的出厂时间
        /// <summary>
        /// 获取机器的出厂时间
        /// </summary>
        /// <param name="dwMachineNumber"></param>
        /// <returns></returns>
        private string GetDeviceTimeInfo(int dwMachineNumber)
        {
            string str = "";
            axCZKEM1.GetDeviceStrInfo(dwMachineNumber, 1, out str);
            return str;
        }
        #endregion

        #region 获取机器指纹算法版本号
        /// <summary>
        /// 获取机器的指纹算法版本号
        /// </summary>
        /// <returns></returns>
        private string GetSysFingerVersion()
        {
            string str = "";
            axCZKEM1.GetSysOption(this.DwMachineNumber, "~ZKFPVersion", out str);
            if (str.Equals("10"))
                return "10.0";
            return "9.0";
        }
        #endregion

        #region 获取机器的MAC地址
        /// <summary>
        /// 获取机器的MAC地址
        /// </summary>
        /// <returns></returns>
        private string GetDeviceMAC()
        {
            string str = "";
            axCZKEM1.GetDeviceMAC(this.DwMachineNumber, ref str);
            return str;
        }
        #endregion

        #region 获取机器制造商名
        /// <summary>
        /// 获取机器制造商名
        /// </summary>
        /// <returns></returns>
        private string GetVendor()
        {
            string str = "";
            axCZKEM1.GetVendor(ref str);
            return str;
        }
        #endregion

        #region 获取机器的固件版本
        /// <summary>
        /// 获取机器的固件版本
        /// </summary>
        /// <returns></returns>
        private string GetFirmwareVersion()
        {
            string str = "";
            axCZKEM1.GetFirmwareVersion(this.DwMachineNumber, ref str);
            return str;
        }
        #endregion

        #region 获取机器名称
        /// <summary>
        /// 获取机器名称
        /// </summary>
        /// <returns></returns>
        private string GetProductName()
        {
            string str = "";
            axCZKEM1.GetProductCode(this.DwMachineNumber, out str);
            return str;
        }
        #endregion

        #region 获取机器平台名称
        /// <summary>
        /// 获取机器平台名称
        /// </summary>
        /// <returns></returns>
        private string GetPlatform()
        {
            string str = "";
            axCZKEM1.GetPlatform(this.DwMachineNumber, ref str);
            return str;
        }
        #endregion

        #region 获取机器序列号
        /// <summary>
        /// 获取机器序列号
        /// </summary>
        /// <returns></returns>
        private string GetSerialNumber()
        {
            string str = "";
            axCZKEM1.GetSerialNumber(this.DwMachineNumber, out str);
            return str;
        }
        #endregion

        #region 获取机器IP地址
        /// <summary>
        /// 获取机器IP地址
        /// </summary>
        /// <returns></returns>
        private string GetDeviceIP()
        {
            string str = "";
            axCZKEM1.GetDeviceIP(this.DwMachineNumber, ref str);
            return str;
        }
        #endregion

        #region 读取所有的用户信息到PC 内存中
        /// <summary>
        /// 读取所有的用户信息到PC 内存中，包括用户编号，密码，姓名，卡号等，指纹模板除外。
        /// 在该函数执行完成后，可调用函数GetAllUserID 取出用户信息
        /// </summary>
        /// <returns></returns>
        public bool ReadAllUserID()
        {
            return axCZKEM1.ReadAllUserID(this.DwMachineNumber);
        }
        #endregion

        #endregion

        #region 考勤工作方面

        #region 设置工作标号记录
        /// <summary>
        /// 设置工作标号记录
        /// </summary>
        /// <param name="workCode">编号</param>
        /// <param name="name">编号描述，一般为名字</param>
        /// <returns>成功返回true,失败返回false</returns>
        public bool SetWorkCode(int workCode, string name)
        {
            bool Flag = axCZKEM1.SSR_SetWorkCode(workCode, name);
            axCZKEM1.RefreshData(this.DwMachineNumber);
            return Flag;
        }
        #endregion

        #region 删除指定编号的记录
        /// <summary>
        /// 删除指定编号的考勤记录
        /// </summary>
        /// <param name="workCode">编号</param>
        /// <returns></returns>
        public bool DeleteWorkCode(int workCode)
        {
            bool Flag = axCZKEM1.SSR_DeleteWorkCode(workCode);
            axCZKEM1.RefreshData(this.DwMachineNumber);
            return Flag;
        }
        #endregion

        #region 清空所以自定义的工作编号
        /// <summary>
        /// 清空所以自定义的工作编号
        /// </summary>
        /// <returns></returns>
        public bool ClearWorkCode()
        {
            bool Flag = axCZKEM1.SSR_ClearWorkCode();
            axCZKEM1.RefreshData(this.DwMachineNumber);
            return Flag;
        }
        #endregion

        #region 获取指定WorkCode 编号的名字
        /// <summary>
        /// 获取指定WorkCode 编号的名字
        /// </summary>
        /// <param name="workCode">编号</param>
        /// <returns></returns>
        public string GetWorkCodeName(int workCode)
        {
            string str = "";
            axCZKEM1.SSR_GetWorkCode(workCode, out str);
            return str;
        }
        #endregion

        #endregion

        #region 节假日操作

        #region 设置节假日
        /// <summary>
        /// 设置节假日
        /// </summary>
        /// <param name="holidayId">假日编号</param>
        /// <param name="beginMonth">假日开始月</param>
        /// <param name="beginDay">假日开始日</param>
        /// <param name="endMonth">假日结束月</param>
        /// <param name="endDay">假日结束日</param>
        /// <param name="timeID">节假日使用的时间段编号</param>
        /// <returns>成功返回true，失败返回false</returns>
        public bool SetHoliday(int holidayId, int beginMonth, int beginDay, int endMonth, int endDay, int timeID)
        {
            bool Flag = axCZKEM1.SSR_SetHoliday(this.DwMachineNumber, holidayId, beginMonth, beginDay, endMonth, endDay, timeID);
            axCZKEM1.RefreshData(this.DwMachineNumber);
            return Flag;
        }
        #endregion

        #region 根据节假日编号获取机器上的节假日设置
        /// <summary>
        /// 根据节假日编号获取机器上的节假日设置
        /// </summary>
        /// <param name="holidayId">节假日编号</param>
        /// <param name="beginMonth">开始月</param>
        /// <param name="beginDay">开始日</param>
        /// <param name="endMonth">结束月</param>
        /// <param name="endDay">结束日</param>
        /// <param name="timeID">假日的时间段编号</param>
        /// <returns></returns>
        public bool GetHoliday(int holidayId, ref int beginMonth, ref int beginDay, ref int endMonth, ref int endDay, ref int timeID)
        {
            return axCZKEM1.SSR_GetHoliday(this.DwMachineNumber, holidayId, ref beginMonth, ref beginDay, ref endMonth, ref endDay, ref timeID);
        }
        #endregion

        #endregion

        #region 用户

        #region 获取全部用户信息
        /// <summary>
        /// 获取全部用户信息
        /// </summary>
        /// <returns>返回用户列表</returns>
        public List<DeviceUser> GetAllUserInfo()
        {
            List<DeviceUser> userList = null;
            if (!this.ReadAllUserID())
                return null;
            userList = new List<DeviceUser>();
            string dwEnrollNumber = "";
            string name = "";
            string pwd = "";
            int privilege = 0;
            bool enabled = false;
            bool Flag = true;
            do{
                Flag = axCZKEM1.SSR_GetAllUserInfo(this.DwMachineNumber, out dwEnrollNumber, out name, out pwd, out privilege, out enabled);
                DeviceUser user = new DeviceUser();
                user.DwEnrollNumber = dwEnrollNumber;
                user.Name = name;
                user.PassWord = pwd;
                user.Privilege = privilege;
                user.Enabled = enabled;
                userList.Add(user);
            }
            while(Flag);
            return userList;
        }
        #endregion

        #region 获取指定用户的用户信息
        /// <summary>
        /// 获取指定用户的用户信息
        /// </summary>
        /// <param name="dwEnrollNumber">用户号</param>
        /// <returns>DeviceUser，返回用户对象</returns>
        public DeviceUser GetUserInfo(string dwEnrollNumber)
        {
            DeviceUser user = null;
            string name = "";
            string pwd = "";
            int privatege = 0;
            bool enable = false;
            bool Flag = axCZKEM1.SSR_GetUserInfo(this.DwMachineNumber, dwEnrollNumber, out name, out pwd, out privatege, out enable);
            if (Flag)
            {
                user = new DeviceUser();
                user.DwEnrollNumber = dwEnrollNumber;
                user.Name = name;
                user.PassWord = pwd;
                user.Privilege = privatege;
                user.Enabled = enable;
            }
            return user;
        }
        #endregion

        #region 设置用户是否可用
        /// <summary>
        /// 设置用户是否可用
        /// </summary>
        /// <param name="dwEnrollNumber">用户号</param>
        /// <param name="bFlag">用户启用标志，True 为启用，False 为禁用</param>
        /// <returns>true-设置成功,false-设置失败</returns>
        public bool SetEnableUser(string dwEnrollNumber,bool bFlag)
        {
            return axCZKEM1.SSR_EnableUser(this.DwMachineNumber,dwEnrollNumber, bFlag);
        }
        #endregion

        #region 设置用户信息
        /// <summary>
        /// 设置用户信息
        /// </summary>
        /// <param name="dwEnorollNumber">用户号</param>
        /// <param name="name">姓名</param>
        /// <param name="pwd">密码</param>
        /// <param name="privilage">权限</param>
        /// <param name="eabled">用户是否启用</param>
        /// <returns>true设置成功,false设置失败</returns>
        public bool SetUserInfo(string dwEnorollNumber,string name,string pwd,int privilage,bool eabled)
        {
           return axCZKEM1.SSR_SetUserInfo(this.DwMachineNumber, dwEnorollNumber, name, pwd, privilage, eabled);
        }

        /// <summary>
        /// 设置用户信息
        /// </summary>
        /// <param name="user">用户对象</param>
        /// <returns>true设置成功,false设置失败</returns>
        public bool SetUserInfo(DeviceUser user)
        {
            bool falg = false;
            try 
	        {	        
		         falg = axCZKEM1.SSR_SetUserInfo(this.DwMachineNumber, user.DwEnrollNumber, user.Name, user.PassWord, user.Privilege, user.Enabled);
                 return falg;
	        }
	        catch (Exception )
	        {

                throw;
	        }
        }
        #endregion

        #region 获取指纹模版
        /// <summary>
        /// 以字符串方式获取用户ZKFinger 9.0 指纹模板
        /// </summary>
        /// <param name="dwEnrollNumber">用户号</param>
        /// <param name="dwFingerIndex">指纹索引号，一般范围为0-9</param>
        /// <returns></returns>
        public string GetUserTmpStr(string dwEnrollNumber,int dwFingerIndex)
        {
            string tmpData="";
            int tmpLength = 0;
            axCZKEM1.SSR_GetUserTmpStr(this.DwMachineNumber, dwEnrollNumber, dwFingerIndex, out tmpData, out tmpLength);
            return tmpData;
        }

        /// <summary>
        /// 以二进制方式下载用户普通指纹模板或者胁迫指纹模板(从缓存中获取所有指纹信息)
        /// </summary>
        /// <param name="dwEnrollNumber">用户号</param>
        /// <param name="dwFingerIndex">指纹索引号，一般范围为0-9</param>
        /// <param name="iFlag">标示指纹模板是否有效或者是否为胁迫指纹， 0 表示指纹模板无效，1 表示指纹模板有效，3 表示为胁迫指纹</param>
        /// <param name="tmpData">指纹模板数据</param>
        /// <param name="tmpLength">指纹模板长度</param>
        /// <returns></returns>
        public bool GetUserTmpExStr(string dwEnrollNumber, int dwFingerIndex, out int iFlag, out string tmpData,out int tmpLength)
        {
            bool flag = false;

            try
            {
                flag = axCZKEM1.GetUserTmpExStr(this.DwMachineNumber, dwEnrollNumber, dwFingerIndex, out iFlag, out tmpData, out tmpLength);

            }
            catch (Exception)
            {
                
                throw;
            }
            return flag;
        }
        /// <summary>
        /// 以二进制方式获取用户ZKFinger 9.0 指纹模板
        /// </summary>
        /// <param name="dwEnrollNumber">用户号</param>
        /// <param name="dwFingerIndex">指纹索引号，一般范围为0-9</param>
        /// <returns></returns>
        public byte GetUserTmp(string dwEnrollNumber, int dwFingerIndex)
        {
            byte tmpData =0;
            int tmpLength = 0;
            axCZKEM1.SSR_GetUserTmp(this.DwMachineNumber, dwEnrollNumber, dwFingerIndex, out tmpData, out tmpLength);
            return tmpData;
        }

        #region 准备以批处理模式上传数据
        /// <summary>
        /// 准备以批处理模式上传数据，如在上传用户模板、用户信息等数据前使用该函数
        /// </summary>
        /// <param name="updateFlagl">模板，当该参数为1 时，为强制覆盖，为0 时，不覆盖</param>
        /// <returns></returns>
        public bool BeginBatchUpdate(int updateFlagl)
        {
            bool falg = axCZKEM1.BeginBatchUpdate(this.DwMachineNumber, updateFlagl);
            return falg;
        }
        #endregion

        #region 释放为批处理上传准备的缓冲区

        /// <summary>
        /// 释放为批处理上传准备的缓冲区
        /// </summary>
        /// <returns></returns>
        public bool BatchUpdate()
        {
            return axCZKEM1.BatchUpdate(this.DwMachineNumber);

        }

        #endregion

        #region 刷新机内数据
        /// <summary>
        /// 刷新机内数据
        /// </summary>
        /// <returns></returns>
        public bool RefreshData()
        {
            return axCZKEM1.RefreshData(this.DwMachineNumber);
        }
        #endregion

        /// <summary>
        /// 读取所有指纹模版到缓存
        /// </summary>
        /// <returns></returns>
        public bool ReadAllTemplate()
        {
            return axCZKEM1.ReadAllTemplate(this.DwMachineNumber);
        }
        

        #endregion

        #region 以二进制方式上传用户普通指纹模板或者胁迫指纹模板
        /// <summary>
        /// 以二进制方式上传用户普通指纹模板或者胁迫指纹模板
        /// </summary>
        /// <param name="dwEnrollNumber">用户号</param>
        /// <param name="dwFingerIndex">指纹索引</param>
        /// <param name="Flag">标示指纹模板是否有效或者是否为胁迫指纹， 0 表示指纹模板无效，1 表示指纹模板有效，3 表示为胁迫指纹</param>
        /// <param name="temData">指纹模板数据</param>
        /// <returns>成功返回True，否则返回False</returns>
        public bool SetUserTmpExStr(string dwEnrollNumber,int dwFingerIndex,int Flag,string temData)
        {
            return axCZKEM1.SetUserTmpExStr(this.DwMachineNumber, dwEnrollNumber, dwFingerIndex, Flag, temData);
        }
        #endregion
		
        #region 清除机器内所有管理员权限
        /// <summary>
        /// 清除机器内所有管理员权限
        /// </summary>
        /// <returns>成功返回True，否则返回False</returns>
        public bool ClearAdministrators()
        {
            return axCZKEM1.ClearAdministrators(this.DwMachineNumber);
        }
        #endregion

        #region 启用或者禁用机器
        /// <summary>
        /// 启用或者禁用机器，禁用即意味着关闭指纹头，键盘，卡模块等
        /// </summary>
        /// <param name="enagled">true表示启用,false表示禁用</param>
        /// <returns></returns>
        public bool EnableDevice(bool enagled)
        {
            return axCZKEM1.EnableDevice(this.DwMachineNumber, enagled);
        }
        #endregion

        #region 机器操作

        #region 关机
        /// <summary>
        /// 关机
        /// </summary>
        /// <returns></returns>
        public bool PowerOffDevice()
        {
            return axCZKEM1.PowerOffDevice(this.DwMachineNumber);
        }
        #endregion

        #region 重启机器
        /// <summary>
        /// 重启机器
        /// </summary>
        /// <returns></returns>
        public bool RestartDevice()
        {
            return axCZKEM1.RestartDevice(this.DwMachineNumber);
        }
        #endregion

        #endregion

        #region 数据操作

        #region 从机器获取指定数据文件
        /// <summary>
        /// 从机器获取指定数据文件
        /// </summary>
        /// <param name="dataFlag">需要获取的数据文件类型:
        /// 1,考勤记录数据文件.
        /// 2,指纹模板数据文件
        /// 3,无
        /// 4,操作记录数据文件
        /// 5,用户信息数据文件
        /// 6,短消息数据文件
        /// 7,短消息与用户关系的数据文件
        /// 8,扩展用户信息数据文件
        /// 9,Workcode 信息数据文件</param>
        /// <param name="fileName">保存文件名</param>
        /// <returns></returns>
        public bool GetDataFile(int dataFlag,string fileName)
        {
            return axCZKEM1.GetDataFile(this.DwMachineNumber, dataFlag, fileName);
        }
        #endregion

        #region 清楚机器中全部考勤记录
        /// <summary>
        /// 清楚机器中全部考勤记录
        /// </summary>
        /// <returns></returns>
        public bool ClearGLog()
        {
            return axCZKEM1.ClearGLog(this.DwMachineNumber);
        }
        #endregion

        #region 获取含有考勤记录的DeviceUser对象
        /// <summary>
        /// 获取含有考勤记录的DeviceUser对象
        /// </summary>
        /// <param name="dwEnrollNumber">用户号</param>
        /// <param name="dwVerifyMode">用户考勤模式：0 为密码验证，1 为指纹验证，2 为卡验证</param>
        /// <param name="dwInOutMode">考勤状态0—Check-In，1—Check-Out， 2—Break-Out，3—Break-In， 4—OT-In, 5—OT-Out></param>
        /// <returns></returns>
        public DeviceUser GetGeneralLogData(string dwEnrollNumber,int dwVerifyMode,int dwInOutMode)
        {
            if (!axCZKEM1.ReadGeneralLogData(this.DwMachineNumber))
                return null;
            DeviceUser user = this.GetUserInfo(dwEnrollNumber);//获取用户基本信息
            List<WorkLog> workList = new List<WorkLog>();
            int year=1999;
            int month=1;
            int day=1;
            int hours=0;
            int min=0;
            int second=0;
            int dwWorkCode=0;
            bool Flag = true;
            do{
                
                Flag=axCZKEM1.SSR_GetGeneralLogData(this.DwMachineNumber, out dwEnrollNumber, out dwVerifyMode, out dwInOutMode, out year, out month, out day, out hours, out min, out second, ref dwWorkCode);
                DateTime dt = new DateTime(year, month, day, hours, min, second);
                WorkLog worklog = new WorkLog(dt, dwInOutMode, dwWorkCode);
                workList.Add(worklog);
            }
            while(Flag);
            user.WorkLog = workList;
            return user;
        }
        #endregion

        #region 考勤记录数据
        /// <summary>
        /// 考勤记录数据
        /// </summary>
        /// <param name="dwEnrollNumber">用户号</param>
        /// <returns></returns>
        public bool ReadGeneralLogData(int dwEnrollNumber)
        {
            
           return  axCZKEM1.ReadGeneralLogData(dwEnrollNumber);
        }
        #endregion

        #region 从内部缓冲区中逐一读取考勤记录
        /// <summary>
        /// 从内部缓冲区中逐一读取考勤记录
       /// </summary>
       /// <param name="dwMachineNumber"></param>
       /// <param name="dwEnrollNumber"></param>
       /// <param name="dwVerifyMode"></param>
       /// <param name="dwInOutMode"></param>
       /// <param name="dwYear"></param>
       /// <param name="dwMonth"></param>
       /// <param name="dwDay"></param>
       /// <param name="dwHour"></param>
       /// <param name="dwMinute"></param>
       /// <param name="dwSecond"></param>
       /// <param name="dwWorkCode"></param>
       /// <returns></returns>
        public bool SSR_GetGeneralLogData(int dwMachineNumber, out string dwEnrollNumber, out int dwVerifyMode, out int dwInOutMode, out int dwYear, out int dwMonth, out int dwDay, out int dwHour, out int dwMinute, out int dwSecond, ref int dwWorkCode)
        {
            return axCZKEM1.SSR_GetGeneralLogData(dwMachineNumber, out dwEnrollNumber, out dwVerifyMode, out dwInOutMode, out dwYear, out dwMonth, out dwDay, out dwHour, out dwMinute, out dwSecond, ref dwWorkCode);
        }

        #region 获取最后一次错误信息
        /// <summary>
        /// 获取最后一次错误信息
        /// </summary>
        /// <param name="dwErrorCode">用户号</param>
        public void GetLastError(ref int dwErrorCode)
        {
            axCZKEM1.GetLastError(ref dwErrorCode);
        }
        #endregion 
        #endregion

        #endregion

        #region 获取实时事件方法

        //#region 注册需要触发的实时事件
        ///// <summary>
        ///// 注册实时事件
        ///// </summary>
        ///// <param name="eventMask">
        ///// 事件代号。具体含义如下
        ///// 1 OnAttTransaction，OnAttTransactionEx
        ///// 2 OnFinger
        ///// 4 OnNewUser
        ///// 8 OnEnrollFinger，OnEnrollFingerEx
        ///// 16 OnKeyPress 
        ///// 256 OnVerify
        ///// 512 OnFingerFeature
        ///// 1024 OnDoor，OnAlarm
        ///// 2048 OnHIDNum
        ///// 4096 OnWriteCard
        ///// 8192 OnEmptyCard
        ///// 16384 OnDeleteTemplate
        ///// 65535注册全部事件
        ///// </param>
        ///// <returns></returns>
        //public bool RegEvent(int eventMask)
        //{
        //    bool Flag = false;
        //    try
        //    {
        //       Flag=axCZKEM1.RegEvent(DwMachineNumber, eventMask);
        //       switch(eventMask)
        //       {
        //        //this.axCZKEM1.OnFinger -= new zkemkeeper._IZKEMEvents_OnFingerEventHandler(axCZKEM1_OnFinger);
        //        //this.axCZKEM1.OnVerify -= new zkemkeeper._IZKEMEvents_OnVerifyEventHandler(axCZKEM1_OnVerify);
        //        //this.axCZKEM1.OnFingerFeature -= new zkemkeeper._IZKEMEvents_OnFingerFeatureEventHandler(axCZKEM1_OnFingerFeature);
        //        //this.axCZKEM1.OnEnrollFingerEx -= new zkemkeeper._IZKEMEvents_OnEnrollFingerExEventHandler(axCZKEM1_OnEnrollFingerEx);
        //        //this.axCZKEM1.OnDeleteTemplate -= new zkemkeeper._IZKEMEvents_OnDeleteTemplateEventHandler(axCZKEM1_OnDeleteTemplate);
        //        //this.axCZKEM1.OnNewUser -= new zkemkeeper._IZKEMEvents_OnNewUserEventHandler(axCZKEM1_OnNewUser);
        //        //this.axCZKEM1.OnHIDNum -= new zkemkeeper._IZKEMEvents_OnHIDNumEventHandler(axCZKEM1_OnHIDNum);
        //        //this.axCZKEM1.OnAlarm -= new zkemkeeper._IZKEMEvents_OnAlarmEventHandler(axCZKEM1_OnAlarm);
        //        //this.axCZKEM1.OnDoor -= new zkemkeeper._IZKEMEvents_OnDoorEventHandler(axCZKEM1_OnDoor);
        //        //this.axCZKEM1.OnWriteCard -= new zkemkeeper._IZKEMEvents_OnWriteCardEventHandler(axCZKEM1_OnWriteCard);
        //        //this.axCZKEM1.OnEmptyCard -= new zkemkeeper._IZKEMEvents_OnEmptyCardEventHandler(axCZKEM1_OnEmptyCard);
        //           case 1:
        //              this.axCZKEM1.OnAttTransactionEx -= new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(OnAttTranTransactionEx);
        //              ////this.axCZKEM1.OnAttTransaction-=new zkemkeeper._IZKEMEvents_OnAttTransactionEventHandler();
        //              this.axCZKEM1.OnAttTransactionEx += new zkemkeeper._IZKEMEvents_OnAttTransactionExEventHandler(OnAttTranTransactionEx);
        //              //this.axCZKEM1.OnAttTransaction+=ne zkemkeeper._IZKEMEvents_OnAttTransactionEventHandler
        //              break;
        //       }
        //    }
        //    catch { 
            
        //    }
        //    return Flag;
            
        //    //return axCZKEM1.RegEvent(DwMachineNumber,eventMask);
        //}
        //#endregion

        /// <summary>
        /// 播放指定序号语音，具体序号视机器而定，用户可在机器内声音测试内查看到序号，一般为0-11
        /// </summary>
        /// <param name="Index">需要播放的语音序号</param>
        /// <returns>成功返回True，否则返回False</returns>
        public virtual bool PlayVoiceByIndex(int Index)
        {
            return axCZKEM1.PlayVoiceByIndex(Index);
        }
        #endregion

        #endregion
     
    }
}
