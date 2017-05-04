/*
 * 创建人：@谢华良
 * 创建时间:2013年4月8日
 * 目标: Orcle数据库底层数据操作类
 */
using System;
using System.Collections.Generic;
using System.Text;

using System.Data;
using System.Data.OracleClient;
using System.Collections; 
namespace Jep.Database
{
    /// <summary>
    /// Orcle数据库底层数据操作类
    /// </summary>
    public class OrclelHelper
    {
        private readonly string _contentStr =string.Empty;//获取数据库连接字符串

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="contentString">连接字符串</param>
         public OrclelHelper(string contentString)
        {
            this._contentStr = contentString;
        }

         /// <summary>
         /// 获取最大值
         /// </summary>
         /// <param name="FieldName">列名</param>
         /// <param name="TableName">表名</param>
         /// <returns></returns>
         public int GetMaxID(string FieldName, string TableName)
         {
             string strsql = "select max(" + FieldName + ") from " + TableName;
             object obj = GetSingle(strsql);
             if (obj == null)
             {
                 return 1;
             }
             else
             {
                 return int.Parse(obj.ToString());
             }
         }

        /// <summary>
        /// 执行SQL语句，返回影响的记录数
        /// </summary>
        /// <param name="SQLString">SQL语句</param>
        /// <param name="cmdParms">参数</param>
        /// <returns>影响的记录数</returns>
         public int ExecuteSql(string sql, params OracleParameter[] sp)
        {

            using (OracleConnection con = new OracleConnection(_contentStr))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = sql;
                        if (sp != null)
                        {
                            for (int i = 0; i < sp.Length; i++)
                            {
                                cmd.Parameters.Add(sp[i]);
                            }
                        }
                        con.Open();
                        int rows = cmd.ExecuteNonQuery();
                        return rows;
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        throw e;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行存储过程返回影响的行数
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="sp">参数列表</param>
        /// <returns></returns>
         public int ExecuteNonQueryByProc(string procName, params OracleParameter[] sp)
        {
            using (OracleConnection con = new OracleConnection(_contentStr))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = procName;
                        if (sp != null)
                        {
                            for (int i = 0; i < sp.Length; i++)
                            {
                                cmd.Parameters.Add(sp[i]);
                            }
                        }
                        con.Open();
                        int count = cmd.ExecuteNonQuery();
                        return count;
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        cmd.Dispose();
                        con.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 执行Sql语句返回一个DataSet
        /// </summary>
        /// <param name="Sql">Sql语句</param>
        /// <param name="sp">参数</param>
        /// <returns></returns>
         public DataSet GetDataSet(string Sql, params OracleParameter[] sp)
        {
            using (OracleConnection con = new OracleConnection(_contentStr))
            {
                con.Open();
                using (OracleCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = Sql;
                        if (sp != null)
                        {
                            for (int i = 0; i < sp.Length; i++)
                            {
                                cmd.Parameters.Add(sp[i]);
                            }
                        }
                        DataSet ds = new DataSet();
                        OracleDataAdapter da = new OracleDataAdapter(cmd);
                        da.Fill(ds);
                        da.Dispose();
                        return ds;
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        con.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 通过存储过程名来返回一个DataSet
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <returns></returns>
         public DataSet GetDataSetByProc(string procName, params OracleParameter[] sp)
        {
            using (OracleConnection con = new OracleConnection(_contentStr))
            {
                con.Open();
                using (OracleCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandText = procName;
                        if (sp != null)
                        {
                            for (int i = 0; i < sp.Length; i++)
                            {
                                cmd.Parameters.Add(sp[i]);
                            }
                        }
                        DataSet ds = new DataSet();
                        OracleDataAdapter da = new OracleDataAdapter(cmd);
                        da.Fill(ds);
                        da.Dispose();
                        return ds;
                    }
                    catch
                    {
                        throw;
                    }
                    finally {
                        con.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 获取DataTable
        /// </summary>
        /// <param name="Sql">Sql语句</param>
        /// <param name="sp">参数</param>
        /// <returns></returns>
         public DataTable GetTable(string Sql, params OracleParameter[] sp)
        {
            DataTable dt = null;
            using (OracleConnection con = new OracleConnection(_contentStr))
            {
                using (OracleCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = Sql;
                        if (sp != null)
                        {
                            for (int i = 0; i < sp.Length; i++)
                            {
                                cmd.Parameters.Add(sp[i]);
                            }
                        }
                        con.Open();
                        DataSet ds = new DataSet();
                        OracleDataAdapter da = new OracleDataAdapter(cmd);
                        da.Fill(ds);
                        if (ds != null && ds.Tables.Count > 0)
                        {
                            dt = ds.Tables[0];
                            da.Dispose();
                            ds.Dispose();
                        }
                    }
                    catch (Exception e)
                    {
                        throw e;
                    }
                    finally
                    {
                        con.Close();
                    }
                    return dt;
                }
            }
        }

         /// <summary>
         /// 执行存储过程来返回一个DataTable
         /// </summary>
         /// <param name="procName">存储过程名</param>
         /// <param name="sp">参数</param>
         /// <returns>DataTabel</returns>
         public DataTable GetTableByProc(string procName, params OracleParameter[] sp)
         {
             DataTable dt = null;
             using (OracleConnection con = new OracleConnection(_contentStr))
             {
                 using (OracleCommand cmd = con.CreateCommand())
                 {
                     try
                     {
                         con.Open();
                         cmd.CommandType = CommandType.StoredProcedure;
                         cmd.CommandText = procName;
                         if (sp != null)
                         {
                             for (int i = 0; i < sp.Length; i++)
                             {
                                 cmd.Parameters.Add(sp[i]);
                             }
                         }
                         DataSet ds = new DataSet();
                         OracleDataAdapter da = new OracleDataAdapter(cmd);
                         da.Fill(ds);
                         if (ds.Tables != null && ds.Tables.Count > 0)
                         {
                             dt = ds.Tables[0];
                         }
                         da.Dispose();
                     }
                     catch
                     {
                         throw;
                     }
                     finally
                     {
                         con.Close();
                     }
                 }
                 return dt;
             }
         }

      	/// <summary>
      	/// 执行多条Sql语句，实现数据库事务
      	/// </summary>
      	/// <param name="SQLStringList">Sql语句列表</param>
        /// <returns>是否提交完成事务， true-成功，false-失败</returns>
        public bool ExecuteSqlTran(ArrayList SQLStringList)
        {
            bool Flag = false;
            using (OracleConnection conn = new OracleConnection(_contentStr))
            {
                conn.Open();
                OracleCommand cmd = new OracleCommand();
                cmd.Connection = conn;
                OracleTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < SQLStringList.Count; n++)
                    {
                        string strsql = SQLStringList[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    Flag=true;
                }
                catch (System.Data.SqlClient.SqlException e)
                {
                    tx.Rollback();
                    Flag = false;
                    throw e;
                }
                return Flag;
            }
        }

        /// <summary>
        /// 执行多条SQL语句，实现数据库事务。
        /// </summary>
        /// <param name="SQLStringList">SQL语句的哈希表（key为sql语句，value是该语句的OracleParameter[]）</param>
        public bool ExecuteSqlTran(Hashtable SQLStringList)
        {
            bool Flag = false;
            using (OracleConnection conn = new OracleConnection(_contentStr))
            {
                conn.Open();
                using (OracleTransaction trans = conn.BeginTransaction())
                {
                    OracleCommand cmd = new OracleCommand();
                    try
                    {
                        //循环
                        foreach (DictionaryEntry myDE in SQLStringList)
                        {
                            string cmdText = myDE.Key.ToString();
                            OracleParameter[] cmdParms = (OracleParameter[])myDE.Value;
                            PrepareCommand(cmd, conn, trans, cmdText, cmdParms);
                            int val = cmd.ExecuteNonQuery();
                            cmd.Parameters.Clear();
                            
                            trans.Commit();
                            Flag = true;
                        }
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                    return Flag;
                }
            }
        }

        /// <summary>
        /// 获取第一行的第一列
        /// </summary>
        /// <param name="SQLString"></param>
        /// <param name="Times"></param>
        /// <returns></returns>
        public object GetSingle(string Sql, params OracleParameter[] sp)
        {
            using (OracleConnection connection = new OracleConnection(this._contentStr))
            {
                using (OracleCommand cmd = new OracleCommand(Sql, connection))
                {
                    try
                    {
                        connection.Open();
                        if (sp != null)
                        {
                            for (int i = 0; i < sp.Length; i++)
                            {
                                cmd.Parameters.Add(sp[i]);
                            }
                        }
                        object obj = cmd.ExecuteScalar();
                        if ((Object.Equals(obj, null)) || (Object.Equals(obj, System.DBNull.Value)))
                        {
                            return null;
                        }
                        else
                        {
                            return obj;
                        }
                    }
                    catch (System.Data.SqlClient.SqlException e)
                    {
                        connection.Close();
                        throw e;
                    }
                }
            }
        }

        /// <summary>
        /// 关联SqlCommand对象
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="conn"></param>
        /// <param name="trans"></param>
        /// <param name="cmdText"></param>
        /// <param name="cmdParms"></param>
        private static void PrepareCommand(OracleCommand cmd, OracleConnection conn, OracleTransaction trans, string cmdText, OracleParameter[] cmdParms)
        {
            if (conn.State != ConnectionState.Open)
                conn.Open();
            cmd.Connection = conn;
            cmd.CommandText = cmdText;
            if (trans != null)
                cmd.Transaction = trans;
            cmd.CommandType = CommandType.Text;//cmdType;
            if (cmdParms != null)
            {
                foreach (OracleParameter parameter in cmdParms)
                {
                    if ((parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Input) &&
                        (parameter.Value == null))
                    {
                        parameter.Value = DBNull.Value;
                    }
                    cmd.Parameters.Add(parameter);
                }
            }
        }
    }
}
