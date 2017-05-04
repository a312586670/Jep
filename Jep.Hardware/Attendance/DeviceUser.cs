/*
 * 创建人:@谢华良
 * 创建时间:2013年4月18日
 * 目标:存储机器的用户信息
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace Jep.Hardware.Attendance
{
    /// <summary>
    /// 存储机器的用户信息
    /// </summary>
    public class DeviceUser
    {
        private string dwEnrollNumber;//用户号
        private string name;//姓名
        private string passWord;//密码
        private int privilege;//权限
        private bool enabled;//用户启用标志
        private int verifyMode;//验证模式
        private List<WorkLog> workLog;//考勤记录

        /// <summary>
        /// 考勤列表
        /// </summary>
        public List<WorkLog> WorkLog
        {
            get { return workLog; }
            set { workLog = value; }
        }
       
        /// <summary>
        /// 用户考勤模式：0 为密码验证，1 为指纹验证，2 为卡验证
        /// </summary>
        public int VerifyMode
        {
            get { return verifyMode; }
            set { verifyMode = value; }
        }

        /// <summary>
        /// 用户启用标志,true为启用，false为禁用
        /// </summary>
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>
        /// 用户权限，3 为管理员，0 为普通用户
        /// </summary>
        public int Privilege
        {
            get { return privilege; }
            set { privilege = value; }
        }

        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord
        {
            get { return passWord; }
            set { passWord = value; }
        }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// 用户号
        /// </summary>
        public string DwEnrollNumber
        {
            get { return dwEnrollNumber; }
            set { dwEnrollNumber = value; }
        }
    }

    /// <summary>
    /// 考勤记录
    /// </summary>
    public class WorkLog
    {
       
        private DateTime dt;
       
        private int InOutMode;
        
        private int Workcode;

        /// <summary>
        /// WorkCode值
        /// </summary>
        public int Workcode1
        {
            get { return Workcode; }
            set { Workcode = value; }
        }

        /// <summary>
        /// 考勤状态0—Check-In，1—Check-Out， 2—Break-Out，3—Break-In， 4—OT-In, 5—OT-Out
        /// </summary>
        public int InOutMode1
        {
            get { return InOutMode; }
            set { InOutMode = value; }
        }

        /// <summary>
        /// 考勤时间
        /// </summary>
        public DateTime Dt
        {
            get { return dt; }
            set { dt = value; }
        }
      
      
        /// <summary>
        /// 构造函数
        /// </summary>
       /// <param name="dt">考勤时间</param>
       /// <param name="outMode">考勤状态0—Check-In，1—Check-Out， 2—Break-Out，3—Break-In， 4—OT-In, 5—OT-Out</param>
       /// <param name="workCode">WorkCode值</param>
       public WorkLog(DateTime dt,int outMode,int workCode)
       {
           this.dt = dt;
           this.InOutMode = outMode;
           this.Workcode = workCode;
       }
    }
}
