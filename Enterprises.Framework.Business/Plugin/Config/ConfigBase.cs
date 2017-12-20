using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using Enterprises.Framework.Utility;

namespace Enterprises.Framework.Plugin.Config
{
    /// <summary>
    /// 配置文件基础类
    /// </summary>
    public class ConfigBase
    {
        private XmlDocument _fConfig;
        private readonly string _fConfigFile = string.Empty;

        /// <summary>
        /// 构造函数 初始化配置文件
        /// </summary>
        /// <param name="configFile"></param>
        public ConfigBase(string configFile)
        {
            if (!FileUtility.ExistsFile(configFile))
            {
                FileUtility.CreateFile(configFile, @"<?xml version=""1.0"" encoding=""UTF-8""?>
<configuration>
	<appSettings>
    </appSettings>
</configuration>", Encoding.UTF8, false);
            }

            _fConfigFile = configFile;
        }

        /// <summary>
        /// [获取]配置对象，在开发阶段不需要缓存，正是环境下再缓存
        /// </summary>
        public XmlDocument Config
        {
            get
            {
                if (_fConfig == null)
                {
                    try
                    {
                        _fConfig = new XmlDocument();
                        _fConfig.Load(_fConfigFile);
                    }
                    catch (Exception ex)
                    {
                        var exNew = new Exception("读取config配置文件出错!", ex);
                        throw exNew;
                    }
                }

                return _fConfig;
            }
        }

        /// <summary>
        /// 获取Web.config或者App.config所处的路径
        /// </summary>
        /// <returns></returns>
        public static string GetWebOrAppConfigPath()
        {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        /// <summary>
        /// 保存配置
        /// </summary>
        public void SaveConfig()
        {
            FileUtility.SetReadOnly(_fConfigFile, false);
            Config.Save(_fConfigFile);
        }

        /// <summary>
        /// 获取指定路径下的内容
        /// </summary>
        /// <param name="path"></param>
        /// <param name="canEmpty"></param>
        /// <returns></returns>
        public string GetConfigValue(string path, bool canEmpty)
        {
            XmlNodeList xnl = Config.SelectNodes(path);
            if (canEmpty)
            {
                if (xnl == null || xnl.Count == 0)
                {
                    return string.Empty;
                }
            }

            if (xnl == null || xnl.Count == 0)
            {
                throw new Exception(string.Format("在文件{0}没有找到配置路径 : {1}.请确保配置了该节点并且大小写匹配", _fConfigFile, path));
            }
            if (xnl.Count > 1)
            {
                throw new Exception(string.Format("文件{0}配置错误，节点重复：key='{1}'", _fConfigFile, path));
            }
            return xnl[0].InnerText;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sectionGroupName">分组名称</param>
        /// <returns></returns>
        public List<ConfigSection> GetConfigSections(string sectionGroupName)
        {
            if (string.IsNullOrEmpty(sectionGroupName))
            {
                throw new Exception("sectionGroupName not all null or empty");
            }

            sectionGroupName = sectionGroupName.Replace("'", "''");

            var result = new List<ConfigSection>();
            string xPath = string.Format("/configuration/configSections/sectionGroup[@name='{0}']", sectionGroupName);
            XmlNodeList xnlSectionGroup = Config.SelectNodes(xPath);
            if (xnlSectionGroup == null || xnlSectionGroup.Count == 0)
            {
                throw new Exception(string.Format("在文件{0}没有找到配置路径 : {1}.请确保配置了该节点并且大小写匹配", _fConfigFile, xPath));
            }

            XmlNodeList xnlSection = xnlSectionGroup[0].SelectNodes("section");

            if (xnlSection == null || xnlSection.Count == 0)
            {
                throw new Exception(string.Format("在文件{0}没有找到配置路径 : {1}.请确保配置了该节点并且大小写匹配", _fConfigFile, xPath + "/section"));
            }

            foreach (XmlNode xn in xnlSection)
            {
                XmlNode xnName = xn.SelectSingleNode("@name");
                if (xnName == null)
                {
                    throw new Exception(string.Format("Section name is needed."));
                }

                XmlNode xnType = xn.SelectSingleNode("@type");
                if (xnType == null)
                {
                    throw new Exception(string.Format("Section type is needed."));
                }

                result.Add(new ConfigSection(xnName.InnerText, xnType.InnerText));
            }
            return result;
        }

        /// <summary>
        /// 获取appSettings节下指定key的value
        /// </summary>
        /// <param name="key">配置文件关键字</param>
        /// <param name="canEmpty">是否可以为空</param>
        /// <returns></returns>
        public string GetAppSettings(string key, bool canEmpty)
        {
            string path = string.Format(@"/configuration/appSettings/add[@key='{0}']/@value", key);
            return GetConfigValue(path, canEmpty);
        }

        /// <summary>
        /// 检查是否存在指定名称的配置节点
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ExistAppSettings(string key)
        {
            string path = string.Format(@"/configuration/appSettings/add[@key='{0}']/@value", key);
            XmlNodeList xnl = Config.SelectNodes(path);
            if (xnl == null || xnl.Count == 0)
            {
                return false;
            }

            if (xnl.Count > 1)
            {
                throw new Exception(string.Format("文件{0}配置错误，节点重复：key='{1}'", _fConfigFile, key));
            }
            return true;
        }

        /// <summary>
        /// 增加配置节点，可以设置如果已经有了，则覆盖原来的
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetAppSettings(string key, string value)
        {
            //修改当前节点
            XmlNode valueNode = Config.SelectSingleNode(string.Format("/configuration/appSettings/add[@key='{0}']/@value", key));
            if (valueNode != null)
            {
                valueNode.InnerText = value;
            }
            else
            {
                //增加节点
                XmlElement newNode = Config.CreateElement("add");
                XmlAttribute keyAtt = Config.CreateAttribute("key");
                keyAtt.InnerText = key;
                XmlAttribute valueAtt = Config.CreateAttribute("value");
                valueAtt.InnerText = value;
                newNode.Attributes.Append(keyAtt);
                newNode.Attributes.Append(valueAtt);

                XmlNode xnAppSetting = Config.SelectSingleNode("/configuration/appSettings");
                if (xnAppSetting != null) xnAppSetting.AppendChild(newNode);
            }
            SaveConfig();
        }
    }
}
