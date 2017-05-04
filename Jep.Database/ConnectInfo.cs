/*
 * 创建人：@谢华良
 * 创建时间:2013年4月7日
 *目标 数据库连接字符串解析类
 */
using System;
using System.Collections.Generic;
using System.Text;

using Jep.Xml;

namespace Jep.Database
{
    /// <summary>
    ///数据库连接字符串解析类
    /// </summary>
    public class ConnectInfo
    {
        private  string _contentSqlString = string.Empty;//数据库连接字符串
        private readonly string _dbType = string.Empty;//数据库类型
        private readonly string _fileName = string.Empty;//配置文件全名
        private readonly string _xPath = "//System//Config//";
        public string _temp = string.Empty;//数据库连接字符串模版

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbType"></param>
        public ConnectInfo(string fileName)
        {
            try {
                _fileName = fileName;//文件全名
                _dbType = Common.ReadNode(fileName, _xPath + "DbType");//数据库类型
                this._temp = this.GetContentString('[', ']', fileName);
            }
            catch {
                throw;
            }
        }

        #region =获取有效的数据库连接字符串=
        private string GetContentString(char start,char end, string fileName)
        {
            string tempstr = null;
            try
            {
                tempstr = Common.ReadRootNode(fileName, "temp");//读取模版节点
                string[] node = this.IntervalSplit(tempstr, '[', ']');//获取参数数组
                string[] node2 = new string[node.Length];
                for (int i = 0; i < node.Length; i++)
                {
                    //node2[i] = DatabaseEncryption.Decrypt(Common.ReadNode(fileName, _xPath + node[i]));
                    node2[i] =Common.ReadNode(fileName, _xPath + node[i]);
                    tempstr = tempstr.Replace("[" + node[i] + "]", node2[i]);
                }
            }
            catch {
                throw;
            }
            return tempstr;
        }
        #endregion

        /// <summary>
        /// 截取标识串中的字符
        /// </summary>
        /// <param name="str">要截取的字符串</param>
        /// <param name="start">开始串</param>
        /// <param name="end">结束串</param>
        /// <returns></returns>
        private string[] IntervalSplit(string str,char start,char end)
        {
            //Char[] li = new char[2];
            //li[0] = start;
            //li[1] = end;
            string[] temp = str.Split(new Char[2] { start, end });
            //string[] temp = str.Split(li);
            string[] temp2=new string[temp.Length/2];
            for (int i = 0; i < temp2.Length;i++ )
            {
                temp2[i] = temp[2*i + 1].ToString();
            }
            return temp2;
        }
    }
}
