using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.IO;
using System;

/// <summary>
/// 读取配置
/// </summary>
public class ConfigReader<T> where T : BaseConfig
{
	public ConfigReader(string name, bool isWritable = false)
	{
        configData = new ConfigData(name, isWritable);
	}

    /// <summary>
    /// 总配置
    /// </summary>
    /// <returns>The config.</returns>
    public Dictionary<string, T> LoadConfig()
    {
        if (!File.Exists(configData.configPath))
		{
			Debug.LogError("path not exist");
			return null;
		}

        foreach(var node in configData.xmlBaseNodeList)
        {
            XmlElement e = node as XmlElement;
            T obj = Fill(e);
            dic.Add(e.GetAttribute("name").ToString(), obj);
        }

        return dic;
    }

	private T Fill(XmlElement e)
	{
		T obj = Activator.CreateInstance<T>();
		var fields = typeof(T).GetFields();
		string nodeName = string.Empty;
		foreach (var field in fields)
		{
			nodeName = e.GetAttribute(field.Name);
			field.SetValue(obj, Convert.ChangeType(nodeName, field.FieldType));
		}

		return obj;
	}

    public void ChangeAttr(string attrName, string data)
    {
        if (!configData.writable) 
        {
            Debug.LogError("this config file is not writable!");
            return;
        }

        foreach (XmlElement xl1 in configData.xmlBaseNodeList)
		{
            if (xl1.GetAttribute(attrName) != null && xl1.GetAttribute(attrName) != data)
			{
				xl1.SetAttribute(attrName, data);
			}

		}
		configData.xmlFile.Save(configData.configPath);
    }

	/// <summary>
	/// 配置数据
	/// </summary>
    ConfigData configData;
    private Dictionary<string, T> dic = new Dictionary<string, T>();

    private class ConfigData
    {
		public string configName { get; set; }
		/// <summary>
		/// 是否可写入内容
		/// </summary>
		public bool writable { get; private set; }
        public string configPath { get; private set; }
		public XmlDocument xmlFile { get; private set; }
        public XmlNodeList xmlBaseNodeList { get; private set; }


        public ConfigData(string name, bool isWritable)
        {
            configName = name;
			configPath = GetXmlFilePath(configName);
			xmlFile = new XmlDocument();
            xmlFile.Load(configPath);
            writable = isWritable;
            if(isWritable)
                xmlBaseNodeList = xmlFile.SelectSingleNode("LocalSaveData").ChildNodes;
            else
				xmlBaseNodeList = xmlFile.SelectSingleNode("Root").ChildNodes;
		}

		private string GetXmlFilePath(string fileName)
		{
			fileName = Path.Combine(Application.dataPath + "/Resources/Configs/", fileName);

			if (!fileName.EndsWith(".xml") && !fileName.EndsWith(".bytes"))
			{
				fileName += ".xml";
			}
			return fileName;
		}

	}
}