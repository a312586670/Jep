/*
 * 创建人：@谢华良
 * 创建时间:2013年4月9日 15:15
 * 目标: Excel操作帮助类
 */
using System;
using System.Collections.Generic;
using System.Text;


using System.IO;
using System.Data;

using System.Collections;
using System.Data.OleDb;
using System.Diagnostics;
using System.Web;
using Aspose.Cells;
namespace Jep.Office
{
    public enum TotalType
    {
        none, sum, count
    }
    /// <summary>
    /// Excel操作帮助类
    /// </summary>
    public class ExcelHelper
    {

        public TotalType totaltype = TotalType.none;

        /// <summary>
        /// 工作薄
        /// </summary>
        public Workbook book;
        /// <summary>
        /// 单元格对象
        /// </summary>
        public Worksheet sheet;
        /// <summary>
        /// 样式
        /// </summary>
        public Aspose.Cells.Style st;

        /// <summary>
        /// 构造函数
        /// </summary>
        public ExcelHelper()
        {
            book = new Workbook();
            sheet = book.Worksheets[0];
            st = book.Styles[book.Styles.Add()];
            st.Borders.SetStyle(CellBorderType.Thin);
            st.Borders.DiagonalStyle = CellBorderType.None;
        }

        #region =导出到Excel表=
        /// <summary>
        /// 把DataTable导出Excel表
        /// </summary>
        /// <param name="dataTable">DataTable</param>
        /// <returns>true为成功，false为失败</returns>
        private static bool _ExportExcel(DataTable dataTable, string fileName)
        {
            bool Flag = false;
            //Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.ApplicationClass excel = new Microsoft.Office.Interop.Excel.ApplicationClass();
            try
            {
                //没有数据的话就不往下执行
                if (dataTable.Rows.Count == 0)
                    return false;
                excel.Application.Workbooks.Add();
                excel.Visible = false;//要不要显示Excel表
                //判断电脑是否有Excel
                if (excel == null)
                {
                    return false;
                }
                Microsoft.Office.Interop.Excel.Worksheet sheet = (Microsoft.Office.Interop.Excel.Worksheet)excel.ActiveSheet;//
                // 生成字段名称
                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    excel.Cells[1, i + 1] = dataTable.Columns[i].ColumnName;
                    ((Microsoft.Office.Interop.Excel.Range)sheet.Cells[1, i + 1]).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                }

                //填充数据
                for (int i = 0; i < dataTable.Rows.Count - 1; i++)
                {
                    for (int j = 0; j < dataTable.Columns.Count; j++)
                    {
                        excel.Cells[i + 2, j + 1] = "" + dataTable.Rows[i][j].ToString();
                        ((Microsoft.Office.Interop.Excel.Range)sheet.Cells[i + 2, j + 1]).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
                    }
                }
                SetColumnFit(sheet);//设置自适应宽度
                //保存Excel表
                excel.Save(fileName);
                Flag = true;
            }
            catch
            {
                throw;
            }
            finally
            {
                //设置禁止弹出保存和覆盖的询问提示框  
                excel.DisplayAlerts = true;
                excel.AlertBeforeOverwriting = true;
                excel.Workbooks.Close();
                //确保Excel进程关闭  
                excel.Quit();
                excel = null;
                //垃圾回收
                GC.Collect();
            }
            return Flag;
        }

        /// <summary>
        /// 把DataTable导出到Excel表中
        /// </summary>
        /// <param name="dataTable">DataTable</param>
        /// <param name="fileName">文件名</param>
        /// <returns></returns>
        public static bool ExportExcel(DataTable dataTable, string fileName)
        {
            return _ExportExcel(dataTable, fileName);
        }

        #endregion

        #region =通过连接导出到Excel表=
        /// <summary>
        /// 将数据导出至Excel文件
        /// </summary>
        /// <param name="Table">DataTable对象</param>
        /// <param name="ExcelFilePath">Excel文件路径</param>
        private static bool _ExportDbExcel(DataTable Table, string fileName)
        {
            if (File.Exists(fileName))
            {
                throw new Exception("该文件已经存在！");
            }

            if ((Table.TableName.Trim().Length == 0) || (Table.TableName.ToLower() == "table"))
            {
                Table.TableName = "Sheet1";
            }

            //数据表的列数
            int ColCount = Table.Columns.Count;

            //用于记数，实例化参数时的序号
            int i = 0;

            //创建参数
            OleDbParameter[] para = new OleDbParameter[ColCount];

            //创建表结构的SQL语句
            string TableStructStr = @"Create Table " + Table.TableName + "(";

            //连接字符串
            string connString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties=Excel 8.0;";
            OleDbConnection objConn = new OleDbConnection(connString);

            //创建表结构
            OleDbCommand objCmd = new OleDbCommand();

            //数据类型集合
            ArrayList DataTypeList = new ArrayList();
            DataTypeList.Add("System.Decimal");
            DataTypeList.Add("System.Double");
            DataTypeList.Add("System.Int16");
            DataTypeList.Add("System.Int32");
            DataTypeList.Add("System.Int64");
            DataTypeList.Add("System.Single");

            //遍历数据表的所有列，用于创建表结构
            foreach (DataColumn col in Table.Columns)
            {
                //如果列属于数字列，则设置该列的数据类型为double
                if (DataTypeList.IndexOf(col.DataType.ToString()) >= 0)
                {
                    para[i] = new OleDbParameter("@" + col.ColumnName, OleDbType.Double);
                    objCmd.Parameters.Add(para[i]);

                    //如果是最后一列
                    if (i + 1 == ColCount)
                    {
                        TableStructStr += col.ColumnName + " double)";
                    }
                    else
                    {
                        TableStructStr += col.ColumnName + " double,";
                    }
                }
                else
                {
                    para[i] = new OleDbParameter("@" + col.ColumnName, OleDbType.VarChar);
                    objCmd.Parameters.Add(para[i]);

                    //如果是最后一列
                    if (i + 1 == ColCount)
                    {
                        TableStructStr += col.ColumnName + " varchar)";
                    }
                    else
                    {
                        TableStructStr += col.ColumnName + " varchar,";
                    }
                }
                i++;
            }

            //创建Excel文件及文件结构
            try
            {
                objCmd.Connection = objConn;
                objCmd.CommandText = TableStructStr;

                if (objConn.State == ConnectionState.Closed)
                {
                    objConn.Open();
                }
                objCmd.ExecuteNonQuery();
            }
            catch (Exception exp)
            {
                throw exp;
            }

            //插入记录的SQL语句
            string InsertSql_1 = "Insert into " + Table.TableName + " (";
            string InsertSql_2 = " Values (";
            string InsertSql = "";

            //遍历所有列，用于插入记录，在此创建插入记录的SQL语句
            for (int colID = 0; colID < ColCount; colID++)
            {
                if (colID + 1 == ColCount)  //最后一列
                {
                    InsertSql_1 += Table.Columns[colID].ColumnName + ")";
                    InsertSql_2 += "@" + Table.Columns[colID].ColumnName + ")";
                }
                else
                {
                    InsertSql_1 += Table.Columns[colID].ColumnName + ",";
                    InsertSql_2 += "@" + Table.Columns[colID].ColumnName + ",";
                }
            }

            InsertSql = InsertSql_1 + InsertSql_2;

            //遍历数据表的所有数据行
            for (int rowID = 0; rowID < Table.Rows.Count; rowID++)
            {
                for (int colID = 0; colID < ColCount; colID++)
                {
                    if (para[colID].DbType == DbType.Double && Table.Rows[rowID][colID].ToString().Trim() == "")
                    {
                        para[colID].Value = 0;
                    }
                    else
                    {
                        para[colID].Value = Table.Rows[rowID][colID].ToString().Trim();
                    }
                }
                try
                {
                    objCmd.CommandText = InsertSql;
                    objCmd.ExecuteNonQuery();
                }
                catch (Exception exp)
                {
                    string str = exp.Message;
                }
            }
            try
            {
                if (objConn.State == ConnectionState.Open)
                {
                    objConn.Close();
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
            return true;
        }

        /// <summary>
        /// 导出Excel表
        /// </summary>
        /// <param name="table">DataTable</param>
        /// <param name="fileName">路径</param>
        /// <returns></returns>
        public static bool ExportDbExcel(DataTable table, string fileName)
        {
            return _ExportDbExcel(table, fileName);
        }
        #endregion

        #region =把Excel导入到DataTable=
        /// <summary>
        /// 把Excel数据导出到DataTable
        /// </summary>
        /// <param name="filePath">excel文件路径</param>
        /// <returns>DataTable</returns>
        private static DataTable _ImportExcel(string filePath)
        {
            ArrayList TableList = new ArrayList();
            TableList = GetExcelTables(filePath);
            if (TableList.Count <= 0)
            {
                return null;
            }
            DataTable dt = null;
            string contentString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + filePath + ";Extended Properties=Excel 8.0;";
            
            using (OleDbConnection con = new OleDbConnection(contentString))
            {
                using (OleDbCommand cmd = con.CreateCommand())
                {
                    try
                    {
                        if (con.State == ConnectionState.Closed)
                            con.Open();
                        for (int i = 0; i < TableList.Count; i++)
                        {
                            string dtname = TableList[i].ToString();
                            string sqlExcel = "select * from ["+dtname+"$]";
                            cmd.CommandType = CommandType.Text;
                            cmd.CommandText = sqlExcel;

                            dt = new DataTable(dtname);
                            OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                            da.Fill(dt);
                            da.Dispose();
                        }
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
            return dt;
        }

        /// <summary>
        /// 把Excel数据导出到DataTable
        /// </summary>
        /// <param name="filePath">excel文件路径</param>
        /// <returns>DataTable</returns>
        public static DataTable ImportExcel(string filePath)
        {
            return _ImportExcel(filePath);
        }
        #endregion

        #region =获取Excel中表列表=
        /// <summary>
        /// 获取Excel中数据表列表
        /// </summary>
        /// <param name="filePath">Excel文件路径</param>
        /// <returns></returns>
        private static ArrayList _GetExcelTables(string filePath)
        {
            //将Excel架构存入数据里
            System.Data.DataTable dt = new System.Data.DataTable();
            ArrayList TablesList = new ArrayList();

            if (File.Exists(filePath))
            {
                using (OleDbConnection conn = new OleDbConnection("Provider=Microsoft.Jet." +
                     "OLEDB.4.0;Extended Properties=\"Excel 8.0\";Data Source=" + filePath))
                {
                    try
                    {
                        if (conn.State == ConnectionState.Closed)
                            conn.Open();
                        dt = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, new object[] { null, null, null, "TABLE" });
                    }
                    catch
                    {
                        throw;
                    }

                    //获取数据表个数
                    int tablecount = dt.Rows.Count;
                    for (int i = 0; i < tablecount; i = i + 2)
                    {
                        string tablename = dt.Rows[i][2].ToString().Trim().TrimEnd('$');
                        if (TablesList.IndexOf(tablename) < 0)
                        {
                            TablesList.Add(tablename);
                        }
                    }
                }
            }
            return TablesList;
        }

        /// <summary>
        /// 获取Excel中数据表列表
        /// </summary>
        /// <param name="filePath">Excel文件路径</param>
        /// <returns></returns>
        public static ArrayList GetExcelTables(string filePath)
        {
            return _GetExcelTables(filePath);
        }
        #endregion

        #region Excel格式设置(内部公用方法)
        /// <summary>
        /// Set Column Text fit
        /// </summary>
        /// <param name="sheet"></param>
        private static void SetColumnFit(Microsoft.Office.Interop.Excel.Worksheet sheet)
        {
            try
            {
                char column = 'B';
                for (int i = 0; i < 25; i++)
                {
                    Microsoft.Office.Interop.Excel.Range range = sheet.get_Range(String.Format("{0}1", column.ToString()),
                        String.Format("{0}1", column.ToString()));
                    if (range != null)
                    {
                        range.EntireColumn.AutoFit();
                    }
                    column++;
                }
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region 通过Response方式导出数据
        //通过Response方式导出数据
        /// <summary>
        /// 通过Response方式导出数据
        /// </summary>
        /// <param name="workBook">Workbook对象</param>
        /// <param name="savefile">文件名</param>
        public void SaveToExcelToRespone(Workbook workBook, string savefile)
        {
            HttpResponse Response = System.Web.HttpContext.Current.Response;
            HttpRequest Request = System.Web.HttpContext.Current.Request;
            Response.Clear();
            Response.ClearHeaders();
            Response.Buffer = false;
            Response.ContentType = "application/octet-stream";
            Response.AppendHeader("Content-Disposition", "attachment;filename=" + HttpUtility.UrlEncode(savefile, System.Text.Encoding.UTF8));
            workBook.Save(Response.OutputStream,Aspose.Cells.FileFormatType.Excel2000);
            Response.End();
        }

        public void Write(DataTable dt, List<ExportFiled> ls, int startrow)
        {
            int oldstartrow = startrow + 1;
            for (int i = 0; i < ls.Count; i++)
            {
                WriteCell(startrow, i, ls[i].fieldtitle);
            }
            for (int k = 0; k < dt.Rows.Count; k++)
            {
                startrow++;
                DataRow dr = dt.Rows[k];
                for (int i = 0; i < ls.Count; i++)
                {
                    WriteCell(startrow, i, dr[ls[i].fieldname]);
                    //WriteCell(startrow, i, dr[i].ToString());
                }
            }
            startrow++;
            for (int i = 0; i < ls.Count; i++)
            {
                if (ls[i].totaltype == TotalType.none) continue;
                switch (ls[i].totaltype)
                {
                    case TotalType.count:
                        WriteCell(startrow, i, string.Format(ls[i].totalformat, dt.Rows.Count));
                        break;
                    case TotalType.sum:
                        WriteCellXsSum(startrow, i, oldstartrow);

                        break;
                }
                WriteCell(startrow, i, ls[i].fieldtitle);
            }
        }

        private Aspose.Cells.Cell WriteCell(int irow, int icol, object obj)
        {
            Aspose.Cells.Cell cl = sheet.Cells[irow, icol];
            cl.PutValue(obj.ToString());
            cl.Style = st;
            return cl;
        }

        private Aspose.Cells.Cell WriteCellXsSum(int irow, int icol, int startrow)
        {
            Aspose.Cells.Cell cl = sheet.Cells[irow, icol];
            WriteCell(irow, icol, null, 1);
            cl.Formula = string.Format("=sum({0}:{1})", sheet.Cells[startrow, icol].Name,
                sheet.Cells[irow - 1, icol].Name);
            return cl;
        }


        private Aspose.Cells.Cell WriteCell(int irow, int icol, object obj, int dq)
        {
            Aspose.Cells.Cell cl = sheet.Cells[irow, icol];
            cl.PutValue(obj);
            switch (dq)
            {
                case 1:
                    cl.Style.HorizontalAlignment = TextAlignmentType.Center;
                    break;
                case 2:
                    cl.Style.HorizontalAlignment = TextAlignmentType.Left;
                    break;
            }
            cl.Style = st;
            return cl;
        }

        #endregion


    }

    /// <summary>
    /// 导出的列名和对应的标题对象
    /// </summary>
    public class ExportFiled
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string fieldname;
        /// <summary>
        /// 标题
        /// </summary>
        public string fieldtitle;

        public TotalType totaltype = TotalType.none;
        public string totalformat = "{0}";

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="fieldname">列名</param>
        /// <param name="fieldtitle">列标题</param>
        public ExportFiled(string fieldname, string fieldtitle)
        {
            this.fieldtitle = fieldtitle;
            this.fieldname = fieldname;
        }

        public ExportFiled(string fieldname, string fieldtitle, TotalType totaltype, string totalformat)
        {
            this.fieldtitle = fieldtitle;
            this.fieldname = fieldname;
            this.totaltype = totaltype;
            this.totalformat = totalformat;
        }
    }
}
