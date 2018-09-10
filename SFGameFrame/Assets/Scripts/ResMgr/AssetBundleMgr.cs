using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class AssetBundleMgr
{

	/// <summary>
	/// Gets the asset bundle.
	/// </summary>
	/// <returns>The asset bundle.</returns>
	/// <param name="filePath">Assets全路径</param>
	public AssetBundle GetAssetBundle(string filePath)
	{
		AssetBundle ab = null;
		string abName = AssetName2BundleName(filePath);
		m_assetDic.TryGetValue(abName, out ab);
		return ab;
	}

	/// <summary>
	/// 加载Manifest, 用来处理关联资源
	/// </summary>
	public void LoadManifest()
	{
		AssetBundle bundle = AssetBundle.LoadFromFile(m_manifestPath);
		m_manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
		//完成后释放
		bundle.Unload(false);
		bundle = null;
	}

	//使用时，直接传入打出的assetBundle名就行，暂不纠结此问题
	/// <summary>
	/// assetBundle同步加载.
	/// </summary>
	/// <returns>The load.</returns>
	/// <param name="fileName">Asset全路径.</param>
	public AssetBundle Load(string fileName)
	{
		AssetBundle ab = null;
		string abName = AssetName2BundleName(fileName);
		if (m_assetDic.ContainsKey(abName))
		{
			ab = m_assetDic[abName];
		}
		else
		{
			//dependences为所有的abName
			string[] dependences = m_manifest.GetAllDependencies(abName);
			for (int i = 0; i < dependences.Length; i++)
			{
				LoadLocally(dependences[i]);
			}
			ab = LoadLocally(abName);
		}
		return ab;
	}

	/// <summary>
	/// assetBundle异步加载
	/// </summary>
	/// <returns>The async.</returns>
	/// <param name="fileName">Asset全路径.</param>
	public IEnumerator LoadAsync(string fileName)
	{
		string abName = AssetName2BundleName(fileName);
		if (m_assetDic.ContainsKey(abName))
		{
			yield break;
		}
		//dependences为所有的abName
		string[] dependences = m_manifest.GetAllDependencies(abName);
		for (int i = 0; i < dependences.Length; i++)
		{
			yield return LoadLocallyAsync(dependences[i]);
		}
		yield return LoadLocallyAsync(abName);
	}

	private string AssetName2BundleName(string fileName)
	{
		//这一步暂时无用
		string name = fileName.Replace('/', '.').ToLower();
		//name += ".assetbundle";
		return name;
	}

	private string BundleName2FileName(string abName)
	{
		string name = Path.Combine(Application.dataPath + "/StreamingAssets/", abName);
		return name;
	}

	private AssetBundle LoadLocally(string abName)
	{
		if (m_assetDic.ContainsKey(abName))
		{
			return m_assetDic[abName];
		}
		string fileName = BundleName2FileName(abName);
		AssetBundle ab = AssetBundle.LoadFromFile(fileName);
		m_assetDic.Add(abName, ab);
		return ab;
	}

	//从本地文件异步加载
	private IEnumerator LoadLocallyAsync(string abName)
	{
		if (m_assetDic.ContainsKey(abName))
		{
			yield break;
		}
		string fileName = BundleName2FileName(abName);
		AssetBundleCreateRequest createReq = AssetBundle.LoadFromFileAsync(fileName);
		yield return createReq;
		m_assetDic.Add(abName, createReq.assetBundle);
	}

	//从内存异步加载
	public IEnumerator LoadFromMemoryAsync(string abName)
	{
		if (m_assetDic.ContainsKey(abName))
		{
			yield break;
		}
		string filePath = BundleName2FileName(abName);
		AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(File.ReadAllBytes(filePath));
		yield return request;
		m_assetDic.Add(abName, request.assetBundle);
	}

	//注意：此api即将被UnityWebRequest代替

	//从服务器或者本地加载
	//加载时先判断本地是否有该资源，如果有就使用本地资源，否则从服务器下载
	public IEnumerator LoadFromCacheOrDownLoad(string abName)
	{
		//本地缓存未完成
		while (!Caching.ready)
		{
			yield return null;
		}
		string urlFile = string.Empty;
		string urlHttp = string.Empty;
		//url分两种情况
		//1.本地,这里路径代指具体路径,可以为file:// 或者 file:///
		urlFile = "file:///" + BundleName2FileName(abName);
		//2.服务器下载
		urlHttp = "http://www.myserver.com/myAssetBundle/" + abName;
		WWW www = WWW.LoadFromCacheOrDownload(urlFile, 5);
		if (string.IsNullOrEmpty(www.error))
		{
			Debug.LogError(www.error);
			yield break;
		}
		AssetBundle ab = www.assetBundle;
	}

	public void UnLoad(string abName, bool force = false)
	{
		AssetBundle ab = null;
		if (!m_assetDic.TryGetValue(abName, out ab)) return;
		if (!ab) return;
		ab.Unload(force);
		ab = null;
		m_assetDic.Remove(abName);
	}


	public void ReleaseAll()
	{
		foreach (var pair in m_assetDic)
		{
			var bundle = pair.Value;
			if (bundle != null)
			{
				bundle.Unload(true);
				bundle = null;
			}
		}
		m_assetDic.Clear();
	}

	public void UnLoadUnuseAssets()
	{
		Resources.UnloadUnusedAssets();
		GC.Collect();
	}

	public static AssetBundleMgr instance
	{
        get
        {
            return Single<AssetBundleMgr>.instance;
        }
	}

    private AssetBundleManifest m_manifest = null;
	private Dictionary<string, AssetBundle> m_assetDic = new Dictionary<string, AssetBundle>();
	private string m_manifestPath = Application.dataPath + "/StreamingAssets/StreamingAssets";
}