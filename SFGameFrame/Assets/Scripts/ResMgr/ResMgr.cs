using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;
using Newtonsoft.Json;

[Serializable]
public class ResItem
{
    public string name;
    public string path;
    public ResItem(string name, string path)
    {
        this.name = name;
        this.path = path;
    }
}

[Serializable]
public class ResourceRes<T> where T : Object
{
    public string name;
    public T res;

    public ResourceRes(string name,T res)
    {
        this.name = name;
        this.res = res;
    }
}


//资源管理类(需挂载在游戏物体上)
public class ResMgr : Singleton<ResMgr>
{
    private List<ResItem> ResList = new List<ResItem>();
	private Hashtable htCache = null;
	///资源配置文件
	private string configPath = ConfigFiles.ResConfig;

    public void Init()
    {
        LoadConfig();
        htCache = new Hashtable();
    }

    //读取配置表
    private void LoadConfig()
    {
		string content = (Resources.Load<TextAsset>(configPath) as TextAsset).text;
		ResList = JsonConvert.DeserializeObject<List<ResItem>>(content);
    }

    private ResItem GetItemFromName(string name)
    {
        ResItem resItem = ResList.Find((item) => 
        {
           return item.name == name;
        });
        return resItem;
    }

    public ResourceRes<T> Load<T>(string name, bool cache = false) where T : Object
    {
        ResItem item = null;
        item = GetItemFromName(name);

        if (item == null)
            return null;
        
        T tRes = null;
        if (htCache.Contains(item.path))
            tRes = htCache[item.path] as T;
        else
            tRes = Resources.Load<T>(item.path);

        if (tRes != null && cache && !htCache.ContainsKey(item.path))
            htCache.Add(item.path, tRes);
        
        Debug.Assert(tRes != null,item.path);
        return new ResourceRes<T>(name, tRes);
    }

    /// <summary>
    /// Loads the asset.
    /// </summary>
    /// <returns>The asset.</returns>
    /// <param name="name">Name.</param>
    /// <param name="isCatch">是否缓存资源</c> is catch.</param>
    /// <param name="origin">If set to <c>true</c> origin.</param>
	public GameObject LoadAsset(string name, bool isCatch, bool origin = false)
	{
        GameObject goObj = Load<GameObject>(name, isCatch).res;
		GameObject goObjClone = null;
		if (!origin)
			goObjClone = GameObject.Instantiate<GameObject>(goObj);
		else
			goObjClone = GameObject.Instantiate<GameObject>(goObj, Vector3.zero, Quaternion.identity);
        
		if (goObjClone == null)
		{
			Debug.LogError(GetType() + "/LoadAsset()/克隆资源不成功，请检查。 name=" + name);
		}
		return goObjClone;
	}

	//异步加载
	public void LoadAsync<T>(string name,Action<ResourceRes<T>> OnComplete) where T:Object
    {
        StartCoroutine(AsyncLoading<T>(name, OnComplete));
    }

    private IEnumerator AsyncLoading<T>(string name, Action<ResourceRes<T>> Oncomplete) where T:Object
    {
        ResItem item = GetItemFromName(name);
        yield return AsyncLoading<T>(item, Oncomplete);
    }

    private IEnumerator AsyncLoading<T>(ResItem item,Action<ResourceRes<T>> Oncomplete) where T:Object
    {
        T obj = null;
        if(!string.IsNullOrEmpty(item.path))
        {
            ResourceRequest req = Resources.LoadAsync<T>(item.path);
            if(req!=null)
            {
                while(!req.isDone)
                {
                    yield return null;
                }
                obj = req.asset as T;
            }
        }
        else
        {
            Debug.Log("path is null or empty");
        }

        if(Oncomplete != null)
        {
            Oncomplete(new ResourceRes<T>(item.name,obj));
        }
    }
}