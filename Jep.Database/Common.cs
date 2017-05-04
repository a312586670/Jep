/* 
 * 创建人：@谢华良
 * 创建时间:2013年4月7日
 *目标 Access数据库底层数据操作类
 */
using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Xml;
namespace Jep.Database
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// SqlServer数据库
        /// </summary>
        SERVERSQL = 1,
        /// <summary>
        /// MySql数据库
        /// </summary>
        MYSQL = 2,
        /// <summary>
        /// Orcle数据库
        /// </summary>
        ORCLE = 3,
        /// <summary>
        /// Access数据库
        /// </summary>
        ACCESS = 4,
        SQLITE = 5
    }
    /// <summary>
    /// Enowit科技公司
    /// 创建人@谢华良
    /// 时间2013年4月7日
    /// 目标 数据库底层操作类公共方法类
    /// </summary>
    public class Common
    {
        #region 创建数据库配置文件=
        /// <summary>
        /// 创建数据库配置文件
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="fileName">文件全称</param>
        /// <param name="dict">节点指点key-节点,value--节点文本</param>
        private static void _CreateXmlSecond(DataType dbType, string fileName, Dictionary<string, string> dict)
        {
            if (!File.Exists(fileName))
            {
                string dir = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.AppendChild(xmldoc.CreateXmlDeclaration("1.0", "UTF-8", null));

                //加入根元素
                XmlElement xmlRoot = xmldoc.CreateElement("", "System", "");
                xmldoc.AppendChild(xmlRoot);


                //加入Config节点
                XmlElement xmleRootSecond = xmldoc.CreateElement("", "Config", "");
                xmldoc.ChildNodes.Item(1).AppendChild(xmleRootSecond);

                //加入Config节点
                XmlElement xmleRootTemp = xmldoc.CreateElement("", "temp", "");
                xmldoc.ChildNodes.Item(1).AppendChild(xmleRootTemp);

                XmlNode xmlNode = xmldoc.SelectSingleNode("//System//Config");

                //加入数据库类型
                XmlElement xmlelemType = xmldoc.CreateElement("DbType");
                XmlText xmlTextType = xmldoc.CreateTextNode(dbType.ToString());
                xmlelemType.AppendChild(xmlTextType);
                xmlNode.AppendChild(xmlelemType);
                foreach (KeyValuePair<string, string> i in dict)
                {
                    XmlElement xmlelemNode = xmldoc.CreateElement(i.Key);
                    XmlText xmlText = xmldoc.CreateTextNode(i.Value);
                    xmlelemNode.AppendChild(xmlText);
                    xmlNode.AppendChild(xmlelemNode);
                }
                try
                {
                    xmldoc.Save(fileName);
                }
                catch
                {
                    throw;
                }
            }
        }
        #endregion

        #region =创建数据库配置文件
        /// <summary>
        /// 创建数据库配置文件
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <param name="fileName">文件全称</param>
        /// <param name="dict">节点指点key-节点,value--节点文本</param>
        public static void CreateXml(DataType dbType, string fileName, Dictionary<string, string> dict)
        {
            _CreateXmlSecond(dbType, fileName, dict);
        }
        #endregion

        #region =读取XML文档根节点文本
        /// <summary>
        /// 读取XML根节点
        /// </summary>
        /// <param name="fileName">文件全名称</param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static string _ReadRootNode(string fileName, string node)
        {
            string _name = null;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(fileName);
                XmlNode xmlNode = xmlDoc.DocumentElement[node];
                _name = xmlNode.InnerText;
            }
            catch
            {
                _name = null;
            }
            return _name;
        }
        #endregion

        #region =读取XML文档节点文本
        /// <summary>
        /// 获取XMl文件的节点
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="node">节点表达式:例如//节点1//节点2</param>
        /// <returns></returns>
        private static string _ReadNode(string fileName, string xPath)
        {
            string _name = null;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(fileName);
                XmlNode xmlNode = xmlDoc.SelectSingleNode(xPath);
                _name = xmlNode.InnerText;
            }
            catch
            {
                _name = null;
            }
            return _name;
        }
        #endregion

        #region =根据属性来读取XML文档节点文本=
        /// <summary>
        /// 根据节点属性和属性值来获取节点文本
        /// </summary>
        /// <param name="fileName">XML文件路径全称</param>
        /// <param name="attribute">属性名称</param>
        /// <param name="key">属性值</param>
        /// <returns>节点文本</returns>
        private static string _ReadNode(string fileName, string attribute, string key)
        {
            string _name = null;
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(fileName);
                XmlElement xmlElement = xmlDoc.DocumentElement;
                if (xmlElement.ChildNodes != null)
                {
                    for (int i = 0; i < xmlElement.ChildNodes.Count; i++)
                    {
                        XmlNode root = xmlElement.ChildNodes[i];
                        _name = GetText(root, attribute, key);//获取文本值
                    }
                }
            }
            catch
            {
                _name = null;
            }
            return _name;
        }

        /// <summary>
        /// 循环遍历节点获取节点文本值
        /// </summary>
        /// <param name="root">遍历的开始节点</param>
        /// <param name="attribute">属性名称</param>
        /// <param name="key">属性值</param>
        /// <returns></returns>
        private static string GetText(XmlNode root, string attribute, string key)
        {
            string _name = null;
            try
            {
                if (root.ChildNodes != null && root.ChildNodes.Count > 0)
                {
                    foreach (XmlNode child in root.ChildNodes)
                    {
                        if (child.Attributes != null && child.Attributes[attribute] != null)
                        {
                            if (child.Attributes[attribute].Value.Equals(key))
                            {
                                _name = child.InnerText;
                                return _name;
                            }
                        }
                        else
                        {
                            _name = GetText(child, attribute, key);//递归遍历孩子节点
                        }
                    }
                }
                else
                {
                    if (root.Attributes != null && root.Attributes[attribute] != null)
                    {
                        if (root.Attributes[attribute].Value.Equals(key))
                        {
                            _name = root.InnerText;
                            return _name;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
            return _name;
        }
        #endregion

        #region 读取XML文档根节点文本=
        /// <summary>
        /// 读取XML根节点
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="node">节点</param>
        /// <returns></returns>
        public static string ReadRootNode(string fileName, string node)
        {
            return _ReadRootNode(fileName, node);
        }
        #endregion

        #region 读取XML文档节点文本=
        /// <summary>
        /// 读取XMl文件的节点
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="node">节点表达式:例如//节点1//节点2</param>
        /// <returns></returns>
        public static string ReadNode(string fileName, string xPath)
        {
            return _ReadNode(fileName, xPath);
        }
        #endregion

        #region 根据属性来读取XML文档节点文本=
        /// <summary>
        /// 根据节点属性和属性值来获取节点文本
        /// </summary>
        /// <param name="fileName">XML文件路径全称</param>
        /// <param name="attributeName">属性名称</param>
        /// <param name="key">属性值</param>
        /// <returns>节点文本</returns>
        public static string ReadNode(string fileName, string attributeName, string key)
        {
            return _ReadNode(fileName, attributeName, key);
        }
        #endregion
    }
}
