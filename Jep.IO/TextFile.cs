using System;
using System.Collections.Generic;
using System.Text;

using System.IO;

namespace Jep.IO
{
    /// <summary>
    /// 文本文件操作类
    /// </summary>
    public class TextFile
    {
        #region 读取Text文档
        private static string _ReadText(string path, Encoding encoding)
        {
            string temp = "";
            try
            {
                StreamReader sr = new StreamReader(path, encoding);
                temp = sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception ex)
            {
                RuntimeLog.WriteRuntimeErrorLog(ex);
            }
            return temp;
        }

        /// <summary>
        /// 以默认编码方式读取指定路径的Txt文本文件。
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static string ReadText(string path)
        {
            return _ReadText(path, Encoding.Default);
        }

        /// <summary>
        /// 以指定编码方式读取指定路径的Txt文本文件。
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="encoding">文件编码</param>
        /// <returns></returns>
        public static string ReadText(string path, Encoding encoding)
        {
            return _ReadText(path, encoding);
        }

        private static string[] _ReadAllLines(string path, Encoding encoding)
        {
            List<string> lines = new List<string>();
            try
            {
                StreamReader sr = new StreamReader(path, Encoding.Default);
                while (sr.Peek() > -1)
                    lines.Add(sr.ReadLine());
                sr.Close();
            }
            catch (Exception ex)
            {
                RuntimeLog.WriteRuntimeErrorLog(ex);
            }
            return lines.ToArray();
        }

        /// <summary>
        /// 已默认编码方式读取指定路径下的文本文件中的所有行。
        /// </summary>
        /// <param name="path">文本文件路径</param>
        /// <returns></returns>
        public static string[] ReadAllLines(string path)
        {
            return _ReadAllLines(path, Encoding.Default);
        }

        /// <summary>
        /// 已指定编码方式读取指定路径下的文本文件中的所有行。
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static string[] ReadAllLines(string path, Encoding encoding)
        {
            return _ReadAllLines(path, encoding);
        }
        #endregion

        #region 写Text文件
        private static bool _WriterText(List<string> content, string path, bool append, Encoding encoding)
        {
            bool RETURN = false;
            StreamWriter sw = null;
            try
            {
                sw = new StreamWriter(path, append, encoding);
                foreach (string temp in content)
                    sw.WriteLine(temp);
                sw.Flush();
                RETURN = true;
            }
            catch (Exception ex)
            {
                RuntimeLog.WriteRuntimeErrorLog(ex);
            }
            finally
            {
                sw.Close();
            }
            return RETURN;
        }

        /// <summary>
        /// 在指定路径创建文本文件。
        /// </summary>
        /// <param name="content">文件内容</param>
        /// <param name="path">保存路径</param>
        /// <param name="append">确定是否将数据追加到文件。如果该文件存在，并且append为false，则该文件被覆盖。如果该文件存在，并且append为true,则数据被追加到文件中。否则将创建新文件。</param>
        /// <param name="encoding">文件编码方式</param>
        /// <returns></returns>
        public static bool WriterText(List<string> content, string path, bool append, Encoding encoding)
        {
            return _WriterText(content, path, append, encoding);
        }
        #endregion

    }
}
