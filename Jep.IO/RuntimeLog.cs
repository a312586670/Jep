/*
 * 创建人：@谢华良
 * 创建时间：2013年4月11日 13:48
 * 目标：异常日志文件帮助类
 */
using System;
using System.Collections.Generic;
using System.Text;

using System.IO;
using System.Xml;

using Jep.Xml;
using System.Reflection;

namespace Jep.IO
{
    /// <summary>
    /// 提供应用程序运行时日志生成方法
    /// </summary>
    public class RuntimeLog
    {
        /// <summary>
        /// 运行时错误日志
        /// </summary>
        /// <param name="ex">Exception 对象</param>
        public static void WriteRuntimeErrorLog(Exception ex)
        {
            WriteErrorLog(ex, AppPath.GetPath());
        }

        /// <summary>
        /// web运行时错误日志
        /// </summary>
        /// <param name="ex">Exception 对象</param>
        public static void WriteWebRuntimeErrorLog(Exception ex)
        {
            WriteErrorLog(ex, AppPath.GetWebPath());
        }

        /// <summary>
        /// 写入活动日志
        /// </summary>
        /// <param name="content">日志内容</param>
        public static void WriteActivityLog(string content)
        {
            WriteLog(AppPath.GetPath(), content);
        }

        /// <summary>
        /// Web站点写入活动日志
        /// </summary>
        /// <param name="content"></param>
        public static void WriteWebActivityLog(string content)
        {
            WriteLog(AppPath.GetWebPath(), content);
        }

        /// <summary>
        /// 写入活动日志
        /// </summary>
        /// <param name="path"></param>
        /// <param name="content"></param>
        private static void WriteLog(string path, string content)
        {
            string _fileName = DateTime.Now.ToString("yyyy-MM-dd") + ".xml";//文件名
            string _filePath = path + "//logs//" + _fileName;//文件路径
            string _version = null;
            OperatingSystem os = Environment.OSVersion;
            Version vs = Environment.Version;
            try
            {
                try
                {
                    _version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
                }
                catch
                { 
                }
                if (!File.Exists(_filePath))//第一次创建
                {
                    EnowitXmlNode root = new EnowitXmlNode("Runtime");//根节点
                    EnowitXmlNode logsNode = InitLogNode(content);
                    root.ChildNode.Add(logsNode);
                    EnowitXml.CreateXmlFile(_filePath, Encoding.UTF8, root);//创建XMl文件
                }
                else
                {
                    //string version = "";
                    try
                    {
                        //version = EnowitXml.GetAttributeValue(_filePath, "//Runtime//logs//log", "Verson");
                        bool Flag = EnowitXml.ExistNode(_filePath, "//Runtime//logs", "Verson", _version);
                        if (Flag)//版本相同怎继续在下面添加日志节点
                        {
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.Load(_filePath);

                            XmlElement xmlContent = xmlDoc.CreateElement("content");
                            xmlContent.SetAttribute("time", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            xmlContent.InnerText = content;

                            string xPath = string.Format("//Runtime//logs//log[@Verson='{0}']", _version);
                            EnowitXml.AppendNode(_filePath, xPath, xmlContent);
                        }
                        else//版本不同时再次创建log节点
                        {
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.Load(_filePath);
                            XmlElement xmlElement = xmlDoc.CreateElement("log");
                            xmlElement.SetAttribute("Verson", System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString());
                            xmlElement.SetAttribute("Assembly", System.Reflection.Assembly.GetEntryAssembly().GetName().FullName.ToString());

                            XmlElement xmlContent = xmlDoc.CreateElement("content");
                            xmlContent.SetAttribute("time", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            xmlContent.InnerText = content;
                            xmlElement.AppendChild(xmlContent);

                            EnowitXml.AppendNode(_filePath, "//Runtime//logs", xmlElement);
                        }
                    }
                    catch(NullReferenceException){
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(_filePath);
                        XmlElement xmlLogs = xmlDoc.CreateElement("logs");

                        XmlElement xmlElement = xmlDoc.CreateElement("log");
                        xmlElement.SetAttribute("Verson", System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString());
                        xmlElement.SetAttribute("Assembly", System.Reflection.Assembly.GetEntryAssembly().GetName().Name.ToString());
                       

                        XmlElement xmlContent = xmlDoc.CreateElement("content");
                        xmlContent.SetAttribute("time", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                        xmlContent.InnerText = content;
                        xmlElement.AppendChild(xmlContent);

                        xmlLogs.AppendChild(xmlElement);
                        EnowitXml.AppendNode(_filePath, "//Runtime", xmlLogs);
                    }
                }
            }
            catch
            {
            }
        }

        private static void WriteErrorLog(Exception ex, string path)
        {
            string _fileName = DateTime.Now.ToString("yyyy-MM-dd") + ".xml";//文件名
            string _filePath = path + "//logs//" + _fileName;//文件路径
            OperatingSystem os = Environment.OSVersion;
            Version vs = Environment.Version;
            try
            {
                if (!File.Exists(_filePath))//第一次创建Exception日志
                {
                    EnowitXmlNode root = new EnowitXmlNode("Runtime");//根节点
                    EnowitXmlNode expectionsNode = InitErrorNode(ex, os, vs);
                    root.ChildNode.Add(expectionsNode);

                    EnowitXml.CreateXmlFile(_filePath, Encoding.UTF8, root);//创建XMl文件
                }
                else
                {
                    //Assembly assembly = Assembly.GetExecutingAssembly().GetName().Version;
                    //string _version = "";
                    string version = Assembly.GetExecutingAssembly().GetName().Version.Major.ToString();
                    //string version = System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString();
                    try
                    {
                        //_version = EnowitXml.GetAttributeValue(_filePath, "//Runtime//exceptions//error", "Verson");
                        bool Flag = EnowitXml.ExistNode(_filePath, "//Runtime//exceptions", "Verson", System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString());
                        if (Flag)//有相同版本时继续在几点下添加内容节点
                        {
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.Load(_filePath);
                            string xPath = string.Format("//Runtime//exceptions//error[@Verson='{0}']", version);
                            XmlElement xmlExpecion = InitErrorElement(xmlDoc, os, vs,ex);
                            EnowitXml.AppendNode(_filePath, xPath, xmlExpecion);
                        }
                        else//没有添加令一版本的节点Exception
                        {
                            XmlDocument xmlDoc = new XmlDocument();
                            xmlDoc.Load(_filePath);
                            //error节点
                            XmlElement xmlError = xmlDoc.CreateElement("error");
                            xmlError.SetAttribute("Verson", version);
                            xmlError.SetAttribute("Assembly", ex.Source);
                            XmlElement xmlExpecion = InitErrorElement(xmlDoc, os, vs,ex);
                            xmlError.AppendChild(xmlExpecion);
                            EnowitXml.AppendNode(_filePath, "//Runtime//exceptions", xmlError);
                        }
                    }
                    catch(NullReferenceException) {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(_filePath);
                        XmlElement xmlExceptions = xmlDoc.CreateElement("exceptions");

                        //error节点
                        XmlElement xmlError = xmlDoc.CreateElement("error");
                        xmlError.SetAttribute("Verson", version);
                        xmlError.SetAttribute("Assembly", ex.Source);
                        XmlElement xmlExpecion = InitErrorElement(xmlDoc, os, vs,ex);
                        xmlError.AppendChild(xmlExpecion);
                        xmlExceptions.AppendChild(xmlError);
                        EnowitXml.AppendNode(_filePath, "//Runtime", xmlExceptions);
                    }
                }
            }
            catch
            {
            }
        }

        private static EnowitXmlNode InitLogNode(string content)
        {
            EnowitXmlNode logsNode = new EnowitXmlNode("logs");
            ///log节点
            EnowitXmlNode logNode = new EnowitXmlNode("log");
            try
            {
                logNode.NodeAttribute.Add(new EnowitXmlNodeAttribute("Verson", System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString()));
                logNode.NodeAttribute.Add(new EnowitXmlNodeAttribute("Assembly", System.Reflection.Assembly.GetEntryAssembly().GetName().FullName.ToString()));
            }
            catch
            { 
            }
            //log下的content节点
            EnowitXmlNode logContentNode = new EnowitXmlNode("content");
            logContentNode.NodeAttribute.Add(new EnowitXmlNodeAttribute("time", DateTime.Now.ToString("yyyy-MM-dd HH:mm")));
            logContentNode.Value = content;
            logNode.ChildNode.Add(logContentNode);

            logsNode.ChildNode.Add(logNode);

            return logsNode;
        }

        /// <summary>
        /// 返回error节点
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="os"></param>
        /// <param name="vs"></param>
        /// <returns></returns>
        private static EnowitXmlNode InitErrorNode(Exception ex, OperatingSystem os, Version vs)
        {
           
            EnowitXmlNode errorNode = new EnowitXmlNode("error");

            try
            {
                errorNode.NodeAttribute.Add(new EnowitXmlNodeAttribute("Verson", System.Reflection.Assembly.GetEntryAssembly().GetName().Version.ToString()));
            }
            catch { 
                
            }
            errorNode.NodeAttribute.Add(new EnowitXmlNodeAttribute("Assembly",ex.Source));

            EnowitXmlNode errorException = new EnowitXmlNode("exception");
            errorException.NodeAttribute.Add(new EnowitXmlNodeAttribute("time",DateTime.Now.ToString("yyyy-MM-dd HH:mm")));


            EnowitXmlNode errorVersionContentNode = new EnowitXmlNode("content");
            errorVersionContentNode.NodeAttribute.Add(new EnowitXmlNodeAttribute("name", "OSVersion"));
            errorVersionContentNode.Value = os.VersionString;

            EnowitXmlNode errorServicePackContentNode = new EnowitXmlNode("content");
            errorServicePackContentNode.NodeAttribute.Add(new EnowitXmlNodeAttribute("name", "ServerPack"));
            errorServicePackContentNode.Value = os.ServicePack;

            EnowitXmlNode errorCPUCountContentNode = new EnowitXmlNode("content");
            errorCPUCountContentNode.NodeAttribute.Add(new EnowitXmlNodeAttribute("name", "CPUCount"));
            errorCPUCountContentNode.Value = Environment.ProcessorCount.ToString();

            EnowitXmlNode errorProcessCmdContentNode = new EnowitXmlNode("content");
            errorProcessCmdContentNode.NodeAttribute.Add(new EnowitXmlNodeAttribute("name", "ProcessCmd"));
            errorProcessCmdContentNode.Value = Environment.CommandLine;

            EnowitXmlNode errorSystemDirContentNode = new EnowitXmlNode("content");
            errorSystemDirContentNode.NodeAttribute.Add(new EnowitXmlNodeAttribute("name", "SystemDir"));
            errorSystemDirContentNode.Value = Environment.SystemDirectory;

            EnowitXmlNode errorCLRVersionContentNode = new EnowitXmlNode("content");
            errorCLRVersionContentNode.NodeAttribute.Add(new EnowitXmlNodeAttribute("name", "CLRVersion"));
            errorCLRVersionContentNode.Value = vs.Major.ToString();

            EnowitXmlNode errorCLRSeccondVersionContentNode = new EnowitXmlNode("content");
            errorCLRSeccondVersionContentNode.NodeAttribute.Add(new EnowitXmlNodeAttribute("name", "CLRSeccondVersion"));
            errorCLRSeccondVersionContentNode.Value = vs.Minor.ToString();

            EnowitXmlNode errorStackTraceNode = new EnowitXmlNode("content");
            errorStackTraceNode.NodeAttribute.Add(new EnowitXmlNodeAttribute("name", "StackTrace"));
            errorStackTraceNode.Value = Environment.StackTrace;

            EnowitXmlNode errorServicePackContentNodeMSG = new EnowitXmlNode("content");
            errorServicePackContentNodeMSG.NodeAttribute.Add(new EnowitXmlNodeAttribute("name", "Message"));
            errorServicePackContentNodeMSG.Value = ex.Message;

            errorNode.ChildNode.Add(errorException);

            //增加errorNode孩子节点
            errorException.ChildNode.Add(errorProcessCmdContentNode);
            errorException.ChildNode.Add(errorServicePackContentNode);
            errorException.ChildNode.Add(errorSystemDirContentNode);
            errorException.ChildNode.Add(errorVersionContentNode);
            errorException.ChildNode.Add(errorCLRSeccondVersionContentNode);
            errorException.ChildNode.Add(errorCLRVersionContentNode);
            errorException.ChildNode.Add(errorCPUCountContentNode);
            errorException.ChildNode.Add(errorStackTraceNode);
            errorException.ChildNode.Add(errorServicePackContentNodeMSG);

            return errorNode;
        }

        private static XmlElement InitErrorElement(XmlDocument xmlDoc, OperatingSystem os, Version vs,Exception ex)
        {
            

            XmlElement expection = xmlDoc.CreateElement("exception");
            expection.SetAttribute("time", DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

            XmlElement errorProcessCmdContentNode = xmlDoc.CreateElement("content");
            errorProcessCmdContentNode.SetAttribute("name", "ProcessCmd");
            errorProcessCmdContentNode.InnerText = Environment.CommandLine;

            XmlElement errorServicePackContentNode = xmlDoc.CreateElement("content");
            errorServicePackContentNode.SetAttribute("name", "ServerPack");
            errorServicePackContentNode.InnerText = os.ServicePack;

            XmlElement errorSystemDirContentNode = xmlDoc.CreateElement("content");
            errorSystemDirContentNode.SetAttribute("name", "SystemDir");
            errorSystemDirContentNode.InnerText = Environment.SystemDirectory;

            XmlElement errorVersionContentNode = xmlDoc.CreateElement("content");
            errorVersionContentNode.SetAttribute("name", "OSVersion");
            errorVersionContentNode.InnerText = os.VersionString;

            XmlElement errorCLRSeccondVersionContentNode = xmlDoc.CreateElement("content");
            errorCLRSeccondVersionContentNode.SetAttribute("name", "CLRSeccondVersion");
            errorCLRSeccondVersionContentNode.InnerText = vs.Minor.ToString();

            XmlElement errorCLRVersionContentNode = xmlDoc.CreateElement("content");
            errorCLRVersionContentNode.SetAttribute("name", "CLRVersion");
            errorCLRVersionContentNode.InnerText = vs.Major.ToString();

            XmlElement errorCPUCountContentNode = xmlDoc.CreateElement("content");
            errorCPUCountContentNode.SetAttribute("name", "CPUCount");
            errorCPUCountContentNode.InnerText = Environment.ProcessorCount.ToString();

            XmlElement errorStackTraceNode = xmlDoc.CreateElement("content");
            errorStackTraceNode.SetAttribute("name", "StackTrace");
            errorStackTraceNode.InnerText = Environment.StackTrace;

            XmlElement errorStackTraceNodeMsg = xmlDoc.CreateElement("content");
            errorStackTraceNodeMsg.SetAttribute("name", "Message");
            errorStackTraceNodeMsg.InnerText =ex.Message;

            expection.AppendChild(errorProcessCmdContentNode);
            expection.AppendChild(errorServicePackContentNode);
            expection.AppendChild(errorSystemDirContentNode);
            expection.AppendChild(errorVersionContentNode);
            expection.AppendChild(errorCLRSeccondVersionContentNode);
            expection.AppendChild(errorCLRVersionContentNode);
            expection.AppendChild(errorCPUCountContentNode);
            expection.AppendChild(errorStackTraceNode);
            expection.AppendChild(errorStackTraceNodeMsg);
            
            return expection;
        }
    }
}
