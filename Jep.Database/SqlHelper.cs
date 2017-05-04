/*
 * 创建人：@谢华良
 * 创建时间:2013年4月8日
 * 目标: 数据库底层数据操作类(集合5大数据库1-Access,2-MySql,3-Orcle,4-SqlServer,5-Sqlite)
 */
using System;
using System.Collections.Generic;
using System.Text;

using Jep.Xml;
using System.Collections;
using System.Data;

using System.Data.SQLite;
using System.Data.SqlClient;
using MySql.Data.MySqlClient;
using System.Data.OracleClient;
using System.Data.OleDb;
using System.Data.Common;
namespace Jep.Database
{
    /// <summary>
    /// 数据库底层数据操作类(集合5大数据库1-Access,2-MySql,3-Orcle,4-SqlServer,5-Sqlite)
    /// </summary>
    public class SqlHelper
    {
        private readonly string _dbType = string.Empty;//数据库类型
        private readonly string _contentString = string.Empty;//数据库连接字符串

        private SqlServerHelper _sqlServerHelper = null;//SqlServer数据库帮助类
        private SqliteHelper _sqLiteHelper = null;     //Sqlite数据库帮助类
        private OrclelHelper _orcleHelper = null;      //Orcle数据库帮助类
        private MySqlHelper _mySqlHelper = null;       //MySql数据库帮助类
        private AccessHelper _accessHelper = null;     //Access数据库帮助类

        #region =构造函数=
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fileName">配置文件路径</param>
        /// <param name="fileDir">数据库文件全路径!针对Sqlite数据库有用</param>
        ///  <param name="xPath">配置文件数据库类型节点表达式,例如://节点1//节点2</param>
        public SqlHelper(string fileName, string fileDir = null, string xPath = "//System//Config//DbType")
        {
            this._dbType = Common.ReadNode(fileName, xPath);//获取数据库类型
            if (this._dbType.Equals(DataType.SQLITE.ToString()) && fileDir != null)//如果是Sqlite数据库怎直接传一个数据库路径给连接字符串
                this._contentString = fileDir;
            this._contentString = new ConnectInfo(fileName)._temp.ToString();//数据库连接字符串

            //实例化各数据库操作类
            _sqlServerHelper = new SqlServerHelper(this._contentString);
            _orcleHelper = new OrclelHelper(this._contentString);
            _sqLiteHelper = new SqliteHelper(this._contentString);
            _mySqlHelper = new MySqlHelper(this._contentString);
            _accessHelper = new AccessHelper(this._contentString);
        }
        #endregion

        #region =获取最大值=
        /// <summary>
        /// 获取最大值
        /// </summary>
        /// <param name="FieldName">列名</param>
        /// <param name="TableName">表名</param>
        /// <returns></returns>
        public int GetMaxID(string FieldName, string TableName)
        {
            int count = 0;
            switch (_dbType)
            {
                case "SERVERSQL":
                    count = _sqlServerHelper.GetMaxID(FieldName, TableName);
                    break;
                case "MYSQL":
                    count = _mySqlHelper.GetMaxID(FieldName, TableName);
                    break;
                case "ORCLE":
                    count = _orcleHelper.GetMaxID(FieldName, TableName);
                    break;
                case "ACCESS":
                    count = _accessHelper.GetMaxID(FieldName, TableName);
                    break;
                case "SQLITE":
                    count = _sqLiteHelper.GetMaxID(FieldName, TableName);
                    break;
            }
            return count;
        }
        #endregion

        #region =执行Sql语句，返回影响行数=
        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="cmdParms">参数</param>
        /// <returns>影响的记录数</returns>
        public int ExecuteSql(string sql, params DbParameter[] sp)
        {
            int count = 0;
            switch (_dbType)
            {
                case "SERVERSQL":
                    count = _sqlServerHelper.ExecuteSql(sql, this.ConvertSqlParameter(sp));
                    break;
                case "MYSQL":
                    count = _mySqlHelper.ExecuteSql(sql, this.ConvertMySqlParameter(sp));
                    break;
                case "ORCLE":
                    count = _orcleHelper.ExecuteSql(sql, this.ConvertOrcleParameter(sp));
                    break;
                case "ACCESS":
                    count = _accessHelper.ExecuteSql(sql, this.ConvertOleDbParameter(sp));
                    break;
                case "SQLITE":
                    count = _sqLiteHelper.ExecuteSql(sql, this.ConvertSQLiteParameter(sp));
                    break;
            }
            return count;
        }
        #endregion

        #region =执行存储过程，返回影响行数=
        /// <summary>
        /// 执行存储过程返回影响的行数
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="sp">参数列表</param>
        /// <returns></returns>
        public int ExecuteNonQueryByProc(string procName, params DbParameter[] sp)
        {
            int count = 0;
            switch (_dbType)
            {
                case "SERVERSQL":
                    count = _sqlServerHelper.ExecuteNonQueryByProc(procName, this.ConvertSqlParameter(sp));
                    break;
                case "MYSQL":
                    count = _mySqlHelper.ExecuteNonQueryByProc(procName, this.ConvertMySqlParameter(sp));
                    break;
                case "ORCLE":
                    count = _orcleHelper.ExecuteNonQueryByProc(procName, this.ConvertOrcleParameter(sp));
                    break;
                case "ACCESS":
                    count = _accessHelper.ExecuteNonQueryByProc(procName, this.ConvertOleDbParameter(sp));
                    break;
                case "SQLITE":
                    count = _sqLiteHelper.ExecuteNonQueryByProc(procName, this.ConvertSQLiteParameter(sp));
                    break;
            }
            return count;
        }
        #endregion

        #region =执行Sql语句，返回一个DataSet=
        /// <summary>
        /// 执行Sql语句返回一个DataSet
        /// </summary>
        /// <param name="Sql">Sql语句</param>
        /// <param name="sp">参数</param>
        /// <returns></returns>
        public DataSet GetDataSet(string sql, params DbParameter[] sp)
        {
            DataSet ds = null;
            switch (_dbType)
            {
                case "SERVERSQL":
                    ds = _sqlServerHelper.GetDataSet(sql, this.ConvertSqlParameter(sp));
                    break;
                case "MYSQL":
                    ds = _mySqlHelper.GetDataSet(sql, this.ConvertMySqlParameter(sp));
                    break;
                case "ORCLE":
                    ds = _orcleHelper.GetDataSet(sql, this.ConvertOrcleParameter(sp));
                    break;
                case "ACCESS":
                    ds = _accessHelper.GetDataSet(sql, this.ConvertOleDbParameter(sp));
                    break;
                case "SQLITE":
                    ds = _sqLiteHelper.GetDataSet(sql, this.ConvertSQLiteParameter(sp));
                    break;
            }
            return ds;
        }
        #endregion

        #region =执行存储过程，返回一个DataSet=
        /// <summary>
        /// 通过存储过程名来返回一个DataSet
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="sp">参数</param>
        /// <returns></returns>
        public DataSet GetDataSetByProc(string procName, params DbParameter[] sp)
        {
            DataSet ds = null;
            switch (_dbType)
            {
                case "SERVERSQL":
                    ds = _sqlServerHelper.GetDataSetByProc(procName, this.ConvertSqlParameter(sp));
                    break;
                case "MYSQL":
                    ds = _mySqlHelper.GetDataSetByProc(procName, this.ConvertMySqlParameter(sp));
                    break;
                case "ORCLE":
                    ds = _orcleHelper.GetDataSetByProc(procName, this.ConvertOrcleParameter(sp));
                    break;
                case "ACCESS":
                    ds = _accessHelper.GetDataSetByProc(procName, this.ConvertOleDbParameter(sp));
                    break;
                case "SQLITE":
                    ds = _sqLiteHelper.GetDataSetByProc(procName, this.ConvertSQLiteParameter(sp));
                    break;
            }
            return ds;
        }
        #endregion

        #region =执行Sql语句，返回一个DataTable
        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="Sql">Sql语句</param>
        /// <param name="sp">参数</param>
        /// <returns></returns>
        public DataTable GetTable(string sql, params DbParameter[] sp)
        {
            DataTable dt = null;
            switch (_dbType)
            {
                case "SERVERSQL":
                    dt = _sqlServerHelper.GetTable(sql, this.ConvertSqlParameter(sp));
                    break;
                case "MYSQL":
                    dt = _mySqlHelper.GetTable(sql, this.ConvertMySqlParameter(sp));
                    break;
                case "ORCLE":
                    dt = _orcleHelper.GetTable(sql, this.ConvertOrcleParameter(sp));
                    break;
                case "ACCESS":
                    dt = _accessHelper.GetTable(sql, this.ConvertOleDbParameter(sp));
                    break;
                case "SQLITE":
                    dt = _sqLiteHelper.GetTable(sql, this.ConvertSQLiteParameter(sp));
                    break;
            }
            return dt;
        }
        #endregion

        #region =执行存储过程，返回一个DataTable=
        /// <summary>
        /// 执行存储过程来返回一个DataTable
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="sp">参数</param>
        /// <returns>DataTabel</returns>
        public DataTable GetTableByProc(string procName, params DbParameter[] sp)
        {
            DataTable dt = null;
            switch (_dbType)
            {
                case "SERVERSQL":
                    dt = _sqlServerHelper.GetTableByProc(procName, this.ConvertSqlParameter(sp));
                    break;
                case "MYSQL":
                    dt = _mySqlHelper.GetTableByProc(procName, this.ConvertMySqlParameter(sp));
                    break;
                case "ORCLE":
                    dt = _orcleHelper.GetTableByProc(procName, this.ConvertOrcleParameter(sp));
                    break;
                case "ACCESS":
                    dt = _accessHelper.GetTableByProc(procName, this.ConvertOleDbParameter(sp));
                    break;
                case "SQLITE":
                    dt = _sqLiteHelper.GetTableByProc(procName, this.ConvertSQLiteParameter(sp));
                    break;
            }
            return dt;
        }
        #endregion

        #region =执行Sql事务=
        /// <summary>
        /// 执行多条Sql语句，实现数据库事务
        /// </summary>
        /// <param name="SQLStringList">Sql语句列表</param>
        /// <returns>是否提交完成事务， true-成功，false-失败</returns>
        public bool ExecuteSqlTran(ArrayList sqlStringList)
        {
            bool Flag = false;
            switch (_dbType)
            {
                case "SERVERSQL":
                    Flag = _sqlServerHelper.ExecuteSqlTran(sqlStringList);
                    break;
                case "MYSQL":
                    Flag = _mySqlHelper.ExecuteSqlTran(sqlStringList);
                    break;
                case "ORCLE":
                    Flag = _orcleHelper.ExecuteSqlTran(sqlStringList);
                    break;
                case "ACCESS":
                    Flag = _accessHelper.ExecuteSqlTran(sqlStringList);
                    break;
                case "SQLITE":
                    Flag = _sqLiteHelper.ExecuteSqlTran(sqlStringList);
                    break;
            }
            return Flag;
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的OracleParameter[]）</param>
        public bool ExecuteSqlTran(Hashtable sqlStringList)
        {
            bool Flag = false;
            switch (_dbType)
            {
                case "SERVERSQL":
                    Flag = _sqlServerHelper.ExecuteSqlTran(sqlStringList);
                    break;
                case "MYSQL":
                    Flag = _mySqlHelper.ExecuteSqlTran(sqlStringList);
                    break;
                case "ORCLE":
                    Flag = _orcleHelper.ExecuteSqlTran(sqlStringList);
                    break;
                case "ACCESS":
                    Flag = _accessHelper.ExecuteSqlTran(sqlStringList);
                    break;
                case "SQLITE":
                    Flag = _sqLiteHelper.ExecuteSqlTran(sqlStringList);
                    break;
            }
            return Flag;
        }
        #endregion

        #region =执行Sql语句，返回记录集中的第一行第一列
        /// <summary>
        /// 获取第一行的第一列
        /// </summary>
        /// <param name="SQLString"></param>
        /// <param name="Times"></param>
        /// <returns></returns>
        public object GetSingle(string sql, params Object[] sp)
        {
            object obj = null;
            switch (_dbType)
            {
                case "SERVERSQL":
                    obj = _sqlServerHelper.GetTable(sql, (SqlParameter[])sp);
                    break;
                case "MYSQL":
                    obj = _mySqlHelper.GetTable(sql, (MySqlParameter[])sp);
                    break;
                case "ORCLE":
                    obj = _orcleHelper.GetTable(sql, (OracleParameter[])sp);
                    break;
                case "ACCESS":
                    obj = _accessHelper.GetTable(sql, (OleDbParameter[])sp);
                    break;
                case "SQLITE":
                    obj = _sqLiteHelper.GetTable(sql, (SQLiteParameter[])sp);
                    break;
            }
            return obj;
        }
        #endregion

        #region 转换类
        /// <summary>
        /// SqlParameter转换格式
        /// </summary>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        private SqlParameter[] ConvertSqlParameter(DbParameter[] commandParameters)
        {
            if (commandParameters == null || commandParameters.Length == 0)
            {
                return null;
            }
            SqlParameter[] _SqlParameters = new SqlParameter[commandParameters.Length];
            for (int i = 0; i < commandParameters.Length; i++)
            {
                _SqlParameters[i] = commandParameters[i] as SqlParameter;
            }
            return _SqlParameters;
        }

        /// <summary>
        /// MySqlParameter转换格式
        /// </summary>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        private MySqlParameter[] ConvertMySqlParameter(DbParameter[] commandParameters)
        {
            if (commandParameters == null || commandParameters.Length == 0)
            {
                return null;
            }
            MySqlParameter[] _SqlParameters = new MySqlParameter[commandParameters.Length];
            for (int i = 0; i < commandParameters.Length; i++)
            {
                _SqlParameters[i] = commandParameters[i] as MySqlParameter;
            }
            return _SqlParameters;
        }

        /// <summary>
        /// OleDbParameter转换格式
        /// </summary>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        private OleDbParameter[] ConvertOleDbParameter(DbParameter[] commandParameters)
        {
            if (commandParameters == null || commandParameters.Length == 0)
            {
                return null;
            }
            OleDbParameter[] _SqlParameters = new OleDbParameter[commandParameters.Length];
            for (int i = 0; i < commandParameters.Length; i++)
            {
                _SqlParameters[i] = commandParameters[i] as OleDbParameter;
            }
            return _SqlParameters;
        }

        /// <summary>
        /// OracleParameter转换格式
        /// </summary>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        private OracleParameter[] ConvertOrcleParameter(DbParameter[] commandParameters)
        {
            if (commandParameters == null || commandParameters.Length == 0)
            {
                return null;
            }
            OracleParameter[] _SqlParameters = new OracleParameter[commandParameters.Length];
            for (int i = 0; i < commandParameters.Length; i++)
            {
                _SqlParameters[i] = commandParameters[i] as OracleParameter;
            }
            return _SqlParameters;
        }

        /// <summary>
        /// SQLiteParameter转换格式
        /// </summary>
        /// <param name="commandParameters"></param>
        /// <returns></returns>
        private SQLiteParameter[] ConvertSQLiteParameter(DbParameter[] commandParameters)
        {
            if (commandParameters == null || commandParameters.Length == 0)
            {
                return null;
            }
            SQLiteParameter[] _SqlParameters = new SQLiteParameter[commandParameters.Length];
            for (int i = 0; i < commandParameters.Length; i++)
            {
                _SqlParameters[i] = commandParameters[i] as SQLiteParameter;
            }
            return _SqlParameters;
        }
        #endregion
    }
}
