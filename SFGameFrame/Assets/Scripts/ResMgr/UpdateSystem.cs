using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;

//使用方法
/*
 1.新建一个名字为Cube的预设体，在场景新建一个Cube，赋给预设体。 
   再在其它文件夹下面创建一个名字为Cube的预设体，赋一个其它物体比如圆形给预设体。 
 2.选定一个预设体，创建AssetBudle，后缀为.assetbundle,本地资源路径放入LOCAL_RES_PATH，仿服务端路径放在SERVER_RES_PATH.
 3.执行上面的脚本 文件夹会添加一个txt配置文件，本地的资源会自动被替换为服务端资源，配置文件也会修改。 
   最终，只要将服务端的资源文件随便修改，本地加载的资源都会相应的跟着修改
*/

/// <summary>
/// http://blog.csdn.net/Fatestay_DC/article/details/48313093

///http://blog.csdn.net/dingxiaowei2013/article/details/77814966
/// </summary>
public class UpdateSystem : MonoBehaviour
{
	private const string VERSION_FILE = "version.txt";

	//注意mac下的文件目录和window下的写法不同：

	//mac写法
	private const string SVR_RES_URL = "file:///Users/hushiyu/Desktop/svr/";
	private const string LOCAL_RES_URL = "file:///Users/hushiyu/Desktop/local/";
	private const string LOCAL_RES_PATH = "/Users/hushiyu/Desktop/local";
	private const string SERVER_RES_PATH = "/Users/hushiyu/Desktop/svr";
	//windows写法
	//private const string SVR_RES_URL = "file:///C:/Test/Res/";
	//private const string LOCAL_RES_PATH = "C:\\Test\\Res";

	private Dictionary<string, string> localResVersionDict;
	private Dictionary<string, string> serverResVersionDict;
	/// <summary>
	/// 需要下载的文件列表
	/// </summary>
	private List<string> needDownloadFiles;
	/// <summary>
	/// 是否需要更新下载
	/// </summary>
	private bool need2DownLoad = false;
	private delegate void OnDownloadFinish(WWW www);

	void Start()
	{

		CreateConfig(LOCAL_RES_PATH);
		CreateConfig(SERVER_RES_PATH);
		localResVersionDict = new Dictionary<string, string>();
		serverResVersionDict = new Dictionary<string, string>();
		needDownloadFiles = new List<string>();

		//加载本地version配置
		StartCoroutine(Download(LOCAL_RES_URL + VERSION_FILE, (localVersion) =>
		{
			ParseVersionFile(localVersion.text, localResVersionDict);
			//加载服务端version配置
			StartCoroutine(Download(SVR_RES_URL + VERSION_FILE, (svrVersion) =>
			{
				ParseVersionFile(svrVersion.text, serverResVersionDict);
				//计算出需要加载的资源
				CompareVersion();
				//加载需要更新的资源
				DownloadRes();
			}));
		}));
	}

	public void CreateConfig(string resPath)
	{
		string[] files = Directory.GetFiles(resPath, "*", SearchOption.AllDirectories);
		StringBuilder versions = new StringBuilder();
		for (int i = 0; i < files.Length; i++)
		{
			string filePath = files[i];
			var extension = filePath.Substring(filePath.LastIndexOf("."));
			if (extension == ".assetbundle")
			{
				//windows写法
				//string relativePath = filePath.Replace(resPath, "").Replace("\\", "/");

				//mac写法
				string relativePath = filePath.Replace(resPath, "");
				string md5 = GetMD5File(filePath);
				versions.Append(relativePath).Append(",").Append(md5).Append("\n");
			}
		}
		//windows写法
		//FileStream fs = new FileStream(resPath + "\\" + VERSION_FILE, FileMode.Create);

		FileStream fs = new FileStream(resPath + "/" + VERSION_FILE, FileMode.Create);
		byte[] data = Encoding.UTF8.GetBytes(versions.ToString());
		fs.Write(data, 0, data.Length);
		fs.Flush();
		fs.Close();
	}

	public static string GetMD5File(string file)
	{
		try
		{
			FileStream fs = new FileStream(file, FileMode.Open);
			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] result = md5.ComputeHash(fs);
			fs.Close();
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < result.Length; i++)
			{
				//转化为16进制
				sb.Append(result[i].ToString("x2"));
			}
			return sb.ToString();
		}

		catch (Exception ex)
		{
			throw new Exception(ex.Message);
		}
	}

	private void ReplaceLocalRes(string fileName, byte[] data)
	{
		string path = LOCAL_RES_PATH + fileName;
		var fs = new FileStream(path, FileMode.Create);
		fs.Write(data, 0, data.Length);
		fs.Flush();
		fs.Close();
	}

	private IEnumerator Show()
	{
		yield return null;
	}

	private IEnumerator Download(string url, OnDownloadFinish onFinish)
	{
		WWW www = new WWW(url);
		yield return www;
		if (onFinish != null)
		{
			onFinish(www);
		}
		www.Dispose();
	}

	private void CompareVersion()
	{
		foreach (var pair in serverResVersionDict)
		{
			string fileName = pair.Key;
			string svrMd5 = pair.Value;

			//if (!localResVersionDict.ContainsValue(svrMd5))
			//{
			//needDownloadFiles.Add(fileName);
			//}

			//新增加的资源
			if (!localResVersionDict.ContainsKey(fileName))
			{
				needDownloadFiles.Add(fileName);
			}
			else
			{
				//md5发生变化，代表是需要进行更新或替换的资源
				string localMd5 = "";
				localResVersionDict.TryGetValue(fileName, out localMd5);
				if (!localMd5.Equals(svrMd5))
					needDownloadFiles.Add(fileName);
			}
		}
		//如果有更新，则同步更新version.txt文件
		need2DownLoad = needDownloadFiles.Count > 0;
	}

	//dictionary作参数是引用类型
	private void ParseVersionFile(string content, Dictionary<string, string> dict)
	{
		if (content == null || content.Length == 0)
			return;
		string[] items = content.Split(new char[] { '\n' });
		foreach (var item in items)
		{
			if (!string.IsNullOrEmpty(item))
			{
				string[] infos = item.Split(new char[] { ',' });
				if (infos != null && infos.Length == 2)
					dict.Add(infos[0], infos[1]);
			}
		}
	}

	private void UpdateLocalVersion()
	{
		if (need2DownLoad)
		{
			StringBuilder sb = new StringBuilder();
			foreach (var item in serverResVersionDict)
			{
				sb.Append(item.Key).Append(",").Append(item.Value).Append("\n");
			}

			//windows写法
			//var fs = new FileStream(LOCAL_RES_PATH + "//" + VERSION_FILE, FileMode.Create);
			var fs = new FileStream(LOCAL_RES_PATH + "/" + VERSION_FILE, FileMode.Create);
			byte[] data = Encoding.UTF8.GetBytes(sb.ToString());
			fs.Write(data, 0, data.Length);
			fs.Flush();
			fs.Close();
		}

		StartCoroutine(Show());
	}

	//依次加载需要的资源
	private void DownloadRes()
	{
		if (needDownloadFiles.Count == 0)
		{
			UpdateLocalVersion();
			return;
		}

		//每次加载第一个，加载完成后从列表中去除
		string file = needDownloadFiles[0];
		needDownloadFiles.RemoveAt(0);

		StartCoroutine(Download(SVR_RES_URL + file, (www) =>
		{
			ReplaceLocalRes(file, www.bytes);
			//循环加载
			DownloadRes();
		}));
	}
}