using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace RDPRemoteLoginPro
{
    public static class ConfigHelper
    {
        static string _configPath = System.Windows.Forms.Application.StartupPath + @"\app.config";

        static ConfigHelper()
        {
       
            var configDic = new Dictionary<string, object>
            {
                {"serverIP", ""},
                {"linkName", ""},
                {"user", ""},
                {"password", ""}

            };
            if (!File.Exists(_configPath))
            {
                //创建一个XML文档对象
                XmlDocument doc = new XmlDocument();
                //声明XML头部信息
                XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
                //添加进doc对象子节点
                doc.AppendChild(dec);
                //创建根节点
                XmlElement root = doc.CreateElement("configuration");

                doc.AppendChild(root);

                //再创建根节点下的子节点

                XmlElement appSettings = doc.CreateElement("appSettings");

                foreach (var kv in configDic)
                {
                    //子节点下再创建标记

                    XmlElement add = doc.CreateElement("add");
                    //<add>的内容

                    add.SetAttribute("key", kv.Key);
                    add.SetAttribute("value", kv.Value.ToString());
                    //再将<add>标记添加到<appSettings>标记的子节点下
                    appSettings.AppendChild(add);

                }


                //最后把<appSettings>标记添加到根节点<configuration>子节点下

                root.AppendChild(appSettings);

                //doc文档对象保存写入

                doc.Save(_configPath);

            }
        }
        /// <summary>
        /// 读操作
        /// </summary>
        /// <param name="appKey"></param>
        /// <returns></returns>
        public static string GetAppConfig(string appKey)
        {
            XmlDocument xDoc = new XmlDocument();
            try
            {
                xDoc.Load(_configPath);
                XmlNode xNode;
                XmlElement xElem;
                xNode = xDoc.SelectSingleNode("//appSettings");
                xElem = (XmlElement)xNode.SelectSingleNode("//add[@key='" + appKey + "']");
                if (xElem != null)
                    return xElem.GetAttribute("value");
                else
                    return "";
            }
            catch (Exception)
            {
                return "";
            }
        }



        ///在config中设置想要的值

        public static void SetValue(string AppKey, string AppValue)
        {
            XmlDocument xDoc = new XmlDocument();
            //获取可执行文件的路径和名称
            xDoc.Load(_configPath);
            XmlNode xNode;
            XmlElement xElem1;
            XmlElement xElem2;
            xNode = xDoc.SelectSingleNode("//appSettings");
            xElem1 = (XmlElement)xNode.SelectSingleNode("//add[@key='" + AppKey + "']");
            if (xElem1 != null) xElem1.SetAttribute("value", AppValue);
            else
            {
                xElem2 = xDoc.CreateElement("add");
                xElem2.SetAttribute("key", AppKey);
                xElem2.SetAttribute("value", AppValue);
                xNode.AppendChild(xElem2);
            }
            xDoc.Save(_configPath);
        }

        ///更新config的值
        public static void UpdateAppConfig(string p_strKey, string p_strValue)
        {
            try
            {

                // Assembly Asm = Assembly.GetExecutingAssembly();
                XmlDocument xmlDoc = new XmlDocument();

                xmlDoc.Load(_configPath);
                XmlNodeList nodeList = xmlDoc.SelectSingleNode("/configuration/appSettings").ChildNodes;
                foreach (XmlNode xn in nodeList)
                {
                    XmlElement xe = (XmlElement)xn;


                    if (xe.GetAttribute("key").IndexOf(p_strKey) != -1)
                    {
                        xe.SetAttribute("value", p_strValue);
                    }
                }
                xmlDoc.Save(_configPath);
            }
            catch (System.NullReferenceException NullEx)
            {
                throw NullEx;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
