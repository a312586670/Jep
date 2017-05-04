/*Enowit科技有限公司
 * 创建人:谢华良
 * 创建时间:2013年4月10号 11:00
 * 目标：Json转换帮助类
 */
using System;
using System.Collections.Generic;
using System.Text;

using System.Reflection;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace Jep.Json
{
    public class EnowitJson
    {
        #region 私有方法
        /// <summary>
        /// 过滤特殊字符
        /// </summary>
        private static string FilterJson(String str)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < str.Length; i++)
            {
                char c = str.ToCharArray()[i];
                switch (c)
                {
                    case '\"':
                        sb.Append("\\\""); break;
                    case '\\':
                        sb.Append("\\\\"); break;
                    case '/':
                        sb.Append("\\/"); break;
                    case '\b':
                        sb.Append("\\b"); break;
                    case '\f':
                        sb.Append("\\f"); break;
                    case '\n':
                        sb.Append("\\n"); break;
                    case '\r':
                        sb.Append("\\r"); break;
                    case '\t':
                        sb.Append("\\t"); break;
                    default:
                        sb.Append(c); break;
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 格式化字符型、日期型、布尔型
        /// </summary>
        private static string StringFormat(string str, Type type)
        {
            if (type == typeof(string))
            {
                str = FilterJson(str);
                str = "\"" + str + "\"";
            }
            else if (type == typeof(DateTime))
            {
                str = "\"" + str + "\"";
            }
            else if (type == typeof(bool))
            {
                str = str.ToLower();
            }
            else if (type != typeof(string) && string.IsNullOrEmpty(str))
            {
                str = "\"" + str + "\"";
            }
            return str;
        }
        #endregion

        #region =List转换成Json=
        /// <summary>
        /// list集合转换成Json
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">list对象</param>
        /// <returns></returns>
        public static string ListToJson<T>(IList<T> list)
        {
            object obj = list[0];
            return ListToJson<T>(list, obj.GetType().Name);
        }

        /// <summary>
        /// List转换成Json 
        /// </summary>
        private static string _ListToJson<T>(IList<T> list, string jsonName)
        {
            StringBuilder Json = new StringBuilder();
            try
            {
                if (string.IsNullOrEmpty(jsonName)) jsonName = list[0].GetType().Name;
                Json.Append("{\"" + jsonName + "\":[");
                if (list.Count > 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        //T obj = Activator.CreateInstance<T>();
                        //PropertyInfo[] pi = obj.GetType().GetProperties();
                        PropertyInfo[] pi = list[i].GetType().GetProperties();
                        Json.Append("{");
                        for (int j = 0; j < pi.Length; j++)
                        {
                            Type type = pi[j].GetValue(list[i], null).GetType();
                            Json.Append("\"" + pi[j].Name.ToString() + "\":" + StringFormat(pi[j].GetValue(list[i], null).ToString(), type));

                            if (j < pi.Length - 1)
                            {
                                Json.Append(",");
                            }
                        }
                        Json.Append("}");
                        if (i < list.Count - 1)
                        {
                            Json.Append(",");
                        }
                    }
                }
                Json.Append("]}");
            }
            catch {
                Json.Remove(0,Json.Length);
            }
            return Json.ToString();
        }

        /// <summary>
        /// list集合转换成Json
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="list">list对象</param>
        /// <param name="jsonName">jsonName</param>
        /// <returns></returns>
        public static string ListToJson<T>(IList<T> list, string jsonName)
        {
            return _ListToJson<T>(list, jsonName);
        }
        #endregion

        #region =对象转换为Json=
        /// <summary> 
        /// 对象转换为Json 
        /// </summary> 
        /// <param name="jsonObject">对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(object jsonObject)
        {
            string jsonString = "{";
            try
            {
                PropertyInfo[] propertyInfo = jsonObject.GetType().GetProperties();
                for (int i = 0; i < propertyInfo.Length; i++)
                {
                    object objectValue = propertyInfo[i].GetGetMethod().Invoke(jsonObject, null);
                    string value = string.Empty;
                    if (objectValue is DateTime || objectValue is Guid || objectValue is TimeSpan)
                    {
                        value = "'" + objectValue.ToString() + "'";
                    }
                    else if (objectValue is string)
                    {
                        //value = "'" + ToJson(objectValue.ToString()) + "'";
                        value = "'" + objectValue.ToString() + "'";
                    }
                    else if (objectValue is IEnumerable)
                    {
                        value = ToJson((IEnumerable)objectValue);
                    }
                    else
                    {
                        value = ToJson((object)objectValue.ToString());
                    }
                    //jsonString += "\"" + ToJson(propertyInfo[i].Name) + "\":" + value + ",";
                    jsonString += "\"" + propertyInfo[i].Name + "\":" + value + ",";
                }
                jsonString = jsonString.Remove(jsonString.Length - 1)+"}";
            }
            catch {
                jsonString =null;
            }
            return jsonString;
        }
        #endregion

        #region =对象集合转换Json=
        /// <summary> 
        /// 对象集合转换Json 
        /// </summary> 
        /// <param name="array">集合对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(IEnumerable array)
        {
            string jsonString = "[";
            foreach (object item in array)
            {
                jsonString += ToJson(item) + ",";
            }
            jsonString=jsonString.Remove(jsonString.Length - 1);
            return jsonString + "]";
        }
        #endregion

        #region =普通集合转换Json=
        /// <summary> 
        /// 普通集合转换Json 
        /// </summary> 
        /// <param name="array">集合对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToArrayString(IEnumerable array)
        {
            string jsonString = "[";
            foreach (object item in array)
            {
                jsonString += ToJson((object)item) + ",";
            }
            jsonString=jsonString.Remove(jsonString.Length - 1);
            return jsonString + "]";
        }
        #endregion

        #region  =DataSet转换为Json=
        /// <summary> 
        /// DataSet转换为Json 
        /// </summary> 
        /// <param name="dataSet">DataSet对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(DataSet dataSet)
        {
            string jsonString = "{";
            foreach (DataTable table in dataSet.Tables)
            {
                jsonString += "\"" + table.TableName + "\":" + ToJson(table) + ",";
            }
            jsonString = jsonString.TrimEnd(',');
            return jsonString + "}";
        }
        #endregion

        #region =Datatable转换为Json=
        /// <summary> 
        /// Datatable转换为Json 
        /// </summary> 
        /// <param name="table">Datatable对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(DataTable dt)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");
            DataRowCollection drc = dt.Rows;
            for (int i = 0; i < drc.Count; i++)
            {
                jsonString.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    string strKey = dt.Columns[j].ColumnName;
                    string strValue = drc[i][j].ToString();
                    Type type = dt.Columns[j].DataType;
                    jsonString.Append("\"" + strKey + "\":");
                    strValue = StringFormat(strValue, type);
                    if (j < dt.Columns.Count - 1)
                    {
                        jsonString.Append(strValue + ",");
                    }
                    else
                    {
                        jsonString.Append(strValue);
                    }
                }
                jsonString.Append("},");
            }
            jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("]");
            return jsonString.ToString();
        }

        /// <summary>
        /// DataTable转换为Json 
        /// </summary>
        public static string ToJson(DataTable dt, string jsonName)
        {
            StringBuilder Json = new StringBuilder();
            if (string.IsNullOrEmpty(jsonName)) jsonName = dt.TableName;
            Json.Append("{\"" + jsonName + "\":[");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Json.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        Type type = dt.Rows[i][j].GetType();
                        Json.Append("\"" + dt.Columns[j].ColumnName.ToString() + "\":" + StringFormat(dt.Rows[i][j].ToString(), type));
                        if (j < dt.Columns.Count - 1)
                        {
                            Json.Append(",");
                        }
                    }
                    Json.Append("}");
                    if (i < dt.Rows.Count - 1)
                    {
                        Json.Append(",");
                    }
                }
            }
            Json.Append("]}");
            return Json.ToString();
        }
        #endregion

        #region =DataReader转换为Json=
        /// <summary> 
        /// DataReader转换为Json 
        /// </summary> 
        /// <param name="dataReader">DataReader对象</param> 
        /// <returns>Json字符串</returns> 
        public static string ToJson(DbDataReader dataReader)
        {
            StringBuilder jsonString = new StringBuilder();
            jsonString.Append("[");
            while (dataReader.Read())
            {
                jsonString.Append("{");
                for (int i = 0; i < dataReader.FieldCount; i++)
                {
                    Type type = dataReader.GetFieldType(i);
                    string strKey = dataReader.GetName(i);
                    string strValue = dataReader[i].ToString();
                    jsonString.Append("\"" + strKey + "\":");
                    strValue = StringFormat(strValue, type);
                    if (i < dataReader.FieldCount - 1)
                    {
                        jsonString.Append(strValue + ",");
                    }
                    else
                    {
                        jsonString.Append(strValue);
                    }
                }
                jsonString.Append("},");
            }
            dataReader.Close();
            jsonString.Remove(jsonString.Length - 1, 1);
            jsonString.Append("]");
            return jsonString.ToString();
        }
        #endregion
    }
}
