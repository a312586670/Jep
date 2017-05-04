/*
 * 创建人：@谢华良
 * 创建时间:2013年4月9日
 *目标: XML文档帮助类
 */
using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Xml;
using System.Data;

namespace Jep.Xml
{
    /// <summary>
    /// XML操作类
    /// </summary>
    public class EnowitXml
    {
        private static XmlDocument xmldoc = null;
        #region =根据根节点信息创建XML文档=
        /// <summary>
        /// 根据根节点的一些信息创建XML文件
        /// </summary>
        /// <param name="filePath">文件路径全名</param>
        /// <param name="encoding">编码</param>
        /// <param name="rootNodes">根节点</param>
        /// <returns></returns>
        private static void _CreateXmlFile(string filePath, Encoding encoding, EnowitXmlNode rootNodes)
        {
            if (!File.Exists(filePath))
            {
                string dir = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                xmldoc = new XmlDocument();
                xmldoc.AppendChild(xmldoc.CreateXmlDeclaration("1.0", encoding.BodyName, null));
                AddNode(filePath, encoding, rootNodes);
                try
                {
                    xmldoc.Save(filePath);
                }
                catch
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// 增加节点
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="encoding"></param>
        /// <param name="rootNodes"></param>
        private static void AddNode(string filePath, Encoding encoding, EnowitXmlNode rootNodes)
        {
            if (rootNodes.ParentNode == null)
            {
                //加入根节点
                XmlElement xmlelem = xmldoc.CreateElement("", rootNodes.Name, "");
                if (rootNodes.NodeAttribute != null && rootNodes.NodeAttribute.Count > 0)
                {
                    foreach (EnowitXmlNodeAttribute nodeAttribute in rootNodes.NodeAttribute)//添加节点属性
                    {
                        xmlelem.SetAttribute(nodeAttribute.Name, nodeAttribute.Value);
                    }
                }
                xmldoc.AppendChild(xmlelem);
            }
            if(rootNodes.ChildNode!=null)//添加孩子节点
            {
                foreach (EnowitXmlNode child in rootNodes.ChildNode)
                {
                    string _xPath = GetXPath(child).Substring(0, GetXPath(child).LastIndexOf('/'));//获取父节点全路径
                    XmlNode xmlNode = xmldoc.SelectSingleNode(_xPath);//获取父节点
                    CreateNode(child, xmlNode);
                    AddNode(filePath,encoding,child);
                }
            }
        }
        #endregion

        #region =节点数组来创建XML文档=
        /// <summary>
        /// 创建XML文档
        /// </summary>
        /// <param name="filePath">文件路径全名</param>
        /// <param name="encoding">编码</param>
        /// <param name="nodes">节点</param>
        /// <returns></returns>
        private static void _CreateXmlFile(string filePath, Encoding encoding, EnowitXmlNode[] nodes)
        {
            if (!File.Exists(filePath))
            {
                try
                {
                    string dir = Path.GetDirectoryName(filePath);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    xmldoc = new XmlDocument();
                    xmldoc.AppendChild(xmldoc.CreateXmlDeclaration("1.0", encoding.BodyName, null));
                    foreach (EnowitXmlNode node in nodes)
                    {
                        if (node.ParentNode == null)
                        {
                            //加入根节点
                            XmlElement xmlelem = xmldoc.CreateElement("", node.Name, "");
                            if (node.NodeAttribute != null && node.NodeAttribute.Count > 0)
                            {
                                foreach (EnowitXmlNodeAttribute nodeAttribute in node.NodeAttribute)//添加节点属性
                                {
                                    xmlelem.SetAttribute(nodeAttribute.Name, nodeAttribute.Value);
                                }
                            }
                            xmldoc.AppendChild(xmlelem);//添加根据点属性
                        }
                        else //有父节点
                        {
                            string _xPath=GetXPath(node).Substring(0,GetXPath(node).LastIndexOf('/'));
                            XmlNode xmlNode = xmldoc.SelectSingleNode(_xPath);//获取父节点
                            CreateNode(node, xmlNode);
                        }
                    }
                    xmldoc.Save(filePath);
                }
                catch
                {
                    throw;
                }
            }
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
        /// <param name="xPath">节点表达式:例如//节点1//节点2</param>
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
        private static string _ReadNode(string fileName,string attribute,string key)
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

        #region =内部公用方法=
        /// <summary>
        /// 添加节点
        /// </summary>
        /// <param name="node">要添加的节点</param>
        /// <param name="xmlNode">父节点</param>
        /// <returns></returns>
        private static void CreateNode(EnowitXmlNode node, XmlNode xmlNode)
        {
            XmlElement xmlelem = xmldoc.CreateElement("", node.Name, "");
            if (node.NodeAttribute != null && node.NodeAttribute.Count > 0)
            {
                foreach (EnowitXmlNodeAttribute nodeAttribute in node.NodeAttribute)//添加节点属性
                {
                    xmlelem.SetAttribute(nodeAttribute.Name, nodeAttribute.Value);
                }
            }
            if (!string.IsNullOrEmpty(node.Value))
            {
                XmlText xmlText = xmldoc.CreateTextNode(node.Value);
                xmlelem.AppendChild(xmlText);
            }
            xmlNode.AppendChild(xmlelem);
        }

        /// <summary>
        /// 获取节点父节点路径
        /// </summary>
        /// <param name="node">当前节点</param>
        /// <returns></returns>
        private static string GetXPath(EnowitXmlNode node)
        {
            string xPath = null;
            if (node.ParentNode == null || string.IsNullOrEmpty(node.ParentNode.Name))
            {
                xPath = node.Name;
            }
            else
            {
                xPath = GetXPath(node.ParentNode) + "/" + node.Name;
            }
            return xPath;
        }

        #endregion

        #region 根据根节点的一些信息创建XML文件=
        /// <summary>
        /// 根据根节点的一些信息创建XML文件
        /// </summary>
        /// <param name="filePath">文件路径全名</param>
        /// <param name="encoding">编码</param>
        /// <param name="rootNodes">根节点</param>
        /// <returns></returns>
        public static void CreateXmlFile(string filePath, Encoding encoding, EnowitXmlNode rootNodes)
        {
            _CreateXmlFile(filePath, encoding, rootNodes);
        }
        #endregion

        #region 创建XML文档=
        /// <summary>
        /// 创建XML文档
        /// </summary>
        /// <param name="filePath">文件路径全名</param>
        /// <param name="encoding">编码</param>
        /// <param name="nodes">节点</param>
        /// <returns></returns>
        public static void CreateXmlFile(string filePath, Encoding encoding, EnowitXmlNode[] nodes)
        {
            _CreateXmlFile(filePath, encoding, nodes);
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
        /// <param name="xPath">节点表达式:例如//节点1//节点2</param>
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

        #region 获取指定XPath表达式的节点对象
        /// <summary>
        /// 获取指定XPath表达式的节点对象
        /// </summary>     
        /// <param name="fileName"></param>
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        /// <returns></returns>
        private static XmlNode _GetNode(string fileName,string xPath)
        {
            XmlNode xmlNode = null;
            try
            {
                xmldoc = new XmlDocument();
                xmldoc.Load(fileName);
                xmlNode = xmldoc.SelectSingleNode(xPath);
            }
            catch {
                throw;
            }
            //返回XPath节点
            return xmlNode;
        }

        /// <summary>
        /// 获取指定XPath表达式的节点对象
        /// </summary>
        /// <param name="fileName">文件路径名</param>
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        public static XmlNode GetNode(string fileName, string xPath)
        {
            return _GetNode(fileName, xPath);
        }
        #endregion

        #region 获取指定XPath表达式节点的值
        /// <summary>
        /// 获取指定XPath表达式节点的值
        /// </summary>
        /// <param name="fileName">配置文件路径</param>
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        /// <returns></returns>
        public static string GetValue(string fileName, string xPath)
        {
            if (string.IsNullOrEmpty(GetNode(fileName, xPath).InnerText))
                return GetNode(fileName, xPath).InnerText;
            return null;
        }
        #endregion

        #region =获取指定XPath表达式节点的属性值=
        /// <summary>
        /// 获取指定XPath表达式节点的属性值
        /// </summary>
        /// <param name="fileName">配置文件路径</param>
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        /// <param name="attributeName">属性名</param>
        /// <returns></returns>
        public static string GetAttributeValue(string fileName, string xPath, string attributeName)
        {
            if (GetNode(fileName, xPath).Attributes[attributeName] != null)
                return GetNode(fileName, xPath).Attributes[attributeName].Value;
            return null;
        }
        #endregion

        #region 新增节点=
        /// <summary>
        /// 1. 功能：新增节点。
        /// 2. 使用条件：将任意节点插入到当前Xml文件中。
        /// </summary>  
        /// <param name="fileName">xml文件路径全名</param>
        /// <param name="xPath">要插入的节点位子XPath表达式,</param>
        /// <param name="xmlNode">要插入的Xml节点</param>
        /// <return></return>
        private static void _AppendNode(string fileName,string xPath, XmlNode xmlNode)
        {
            try
            {
                XmlNode startNode = GetNode(fileName, xPath);//目标节点
                //导入节点
                XmlNode node = xmldoc.ImportNode(xmlNode, true);
                //将节点插入到startNode下
                startNode.AppendChild(node);
                xmldoc.Save(fileName);
            }
            catch {
                throw;
            }
        }

        /// <summary>
        /// 1. 功能：新增节点。
        /// 2. 使用条件：将任意节点插入到当前Xml文件中的自定节点下。
        /// </summary>  
        /// <param name="fileName">xml文件路径全名</param>
        /// <param name="xPath">要插入的节点位子XPath表达式,</param>
        /// <param name="xmlNode">要插入的Xml节点</param>
        public static void AppendNode(string fileName, string xPath, XmlNode xmlNode)
        {
            _AppendNode(fileName, xPath, xmlNode);
        }
        #endregion

        #region 删除节点=
        /// <summary>
        /// 删除指定XPath表达式的节点
        /// </summary>    
        /// <param name="fileName">xml文件路径全名</param>
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        /// <return></return>
        private static void _RemoveNode(string fileName,string xPath)
        {
            try
            {
                XmlDocument xmlDocm = new XmlDocument();
                xmlDocm.Load(fileName);

                //获取要删除的节点
                XmlNode node = xmlDocm.SelectSingleNode(xPath);
                //删除节点
                xmlDocm.RemoveChild(node);
            }
            catch {
                throw;
            }
        }

        /// <summary>
        /// 删除指定XPath表达式的节点
        /// </summary>   
        /// <param name="fileName">xml文件路径全名</param>
        /// <param name="xPath">XPath表达式,
        /// 范例1: @"Skill/First/SkillItem", 等效于 @"//Skill/First/SkillItem"
        /// 范例2: @"Table[USERNAME='a']" , []表示筛选,USERNAME是Table下的一个子节点.
        /// 范例3: @"ApplyPost/Item[@itemName='岗位编号']",@itemName是Item节点的属性.
        /// </param>
        public static void RemoveNode(string fileName, string xPath)
        {
            _RemoveNode(fileName, xPath);
        }
        #endregion //删除节点

        #region 读取XML返回DataSet=
        /// <summary>
        /// 读取XML返回DataSet
        /// </summary>
        /// <param name="filePath">XML文件路径</param>
        /// <returns></returns>
        private static DataSet _GetDataSetByXml(string filePath)
        {
            try
            {
                DataSet ds = new DataSet();
                ds.ReadXml(filePath);
                if (ds.Tables.Count > 0)
                {
                    return ds;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 读取XML返回DataSet
        /// </summary>
        /// <param name="filePath">XML文件路径</param>
        /// <returns></returns>
        public static DataSet GetDataSetByXml(string filePath)
        {
            return _GetDataSetByXml(filePath);
        }
        #endregion

        #region 修改数据=
        /// <summary>
        /// 修改指定节点的数据
        /// </summary>
        /// <param name="filePath">xml文件路径</param>
        /// <param name="xPath">节点完整路径，xPath表达式:例如//根节点//节点2//节点3</param>
        /// <param name="value">要修改的值</param>
        /// <return></return>
        private static void _UpdateNodeText(string filePath,string xPath, string value)
        {
            try
            {
                XmlNode xNode= _GetNode(filePath, xPath);
                xNode.InnerText = value;
                xmldoc.Save(filePath);
            }
            catch {
                throw;
            }
        }

        /// <summary>
        /// 修改指定节点的数据
        /// </summary>
        /// <param name="filePath">xml文件路径</param>
        /// <param name="xPath">节点完整路径，xPath表达式:例如//根节点//节点2//节点3</param>
        /// <param name="value">要修改的值</param>
        /// <return></return>
        public static void UpdateNodeText(string filePath, string xPath, string value)
        {
            _UpdateNodeText(filePath, xPath, value);
        }
        #endregion

        #region =根据判断某节点的一级孩子属性和属性值来判断该节点的孩子节点是否存在是否存在这样的属性孩子节点=
        /// <summary>
        /// 根据判断某节点的一级孩子属性和属性值来判断该节点的孩子节点是否存在是否存在这样的属性孩子节点
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="xpath">XPath表达式</param>
        /// <param name="attributeName">孩子节点属性名字</param>
        /// <param name="AttKey">孩子节点属性值</param>
        /// <returns></returns>
        private static bool ExectsNode(string filePath, string xpath, string attributeName, string AttKey)
        {
            int count = 0;
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(filePath);
            XmlNode node = xmldoc.SelectSingleNode(xpath);
            //XmlElement userInfo = xmldoc.CreateElement(xpath);
            if (node.HasChildNodes)
            {
                if (node.ChildNodes.Count > 0)
                {
                    for (int i = 0; i < node.ChildNodes.Count; i++)
                    {
                        try
                        {
                            if (node.ChildNodes[i].Attributes[attributeName].Value.Equals(AttKey))
                            {
                                count++;
                            }
                        }
                        catch (NullReferenceException)
                        {
                            continue;
                        }
                    }
                }
            }
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据判断某节点的一级孩子属性和属性值来判断该节点的孩子节点是否存在是否存在这样的属性孩子节点
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <param name="xpath">XPath表达式</param>
        /// <param name="attributeName">孩子节点属性名字</param>
        /// <param name="AttKey">孩子节点属性值</param>
        /// <returns></returns>
        public static bool ExistNode(string filePath, string xpath, string attributeName, string AttKey)
        {
            return ExectsNode(filePath, xpath, attributeName, AttKey);
        }
        #endregion
    }
}
