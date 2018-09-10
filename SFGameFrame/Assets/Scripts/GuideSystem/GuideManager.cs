using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using SUIFW;

public class GuideManager : Singleton<GuideManager> {

    private Transform maskTra;
    private Transform finger;
    private Vector3 offset = new Vector2(28, -33);

    private string curGuideFile;
    private int nowIndex;
    private bool isFinish = false;//是否完成所有的新手引导
    private string[] nameArray;

	static ConfigReader<GuideProgressConfig> guideProgressCfg;
	static Dictionary<string, GuideProgressConfig> guideProgressConfigDic;

    static ConfigReader<GuideFileConfig> guideFileCfg;
	static Dictionary<string, GuideFileConfig> guideFileCfgDic;

    public void Init()
    {
        InitCfg();
        //读取进度
        curGuideFile = guideProgressConfigDic["Progress"].curFile;
        nowIndex = guideProgressConfigDic["Progress"].curIndex;
        isFinish = guideProgressConfigDic["Progress"].isFinish == 1;

        //读取需要高亮的组件的Hierarchy路径
        if (!isFinish)
        {
            string content = guideFileCfgDic[curGuideFile].btns;
            nameArray = content.Split(new string[] { ";" }, System.StringSplitOptions.RemoveEmptyEntries);
        } 
    }

	public static void InitCfg()
	{
		guideProgressCfg = new ConfigReader<GuideProgressConfig>(SysDefine.GuideProgressCfg);
		guideProgressConfigDic = guideProgressCfg.LoadConfig();

		guideFileCfg = new ConfigReader<GuideFileConfig>(SysDefine.GuideFileCfg);
		guideFileCfgDic = guideFileCfg.LoadConfig();
	}

    public void Next()
    {
        if (nowIndex < nameArray.Length)
        {
            ShowHightLight(nameArray[nowIndex]);
            nowIndex++;
        }
        else//加载下一个文件
        {
			maskTra.gameObject.SetActive(false);
     
            int index = int.Parse(curGuideFile.Substring(curGuideFile.Length - 1));
            index++;
            curGuideFile = curGuideFile.Substring(0, curGuideFile.Length - 1) + index.ToString();

            string content = null;
            try
            {
                content = guideFileCfgDic[curGuideFile].btns;
            }
            catch (Exception e) 
            {
                isFinish = true;
                Debug.Log("finish " + e.Message);
                return;
            }
            nowIndex = 0;
            nameArray = content.Split(new string[] { ";" }, System.StringSplitOptions.RemoveEmptyEntries);  
        }
    }

    void ShowHightLight(string name, bool checkIsClone = true)
    {
        if(checkIsClone && name.Contains("/"))
        {
            name = name.Insert(name.IndexOf('/'), "(Clone)");
        }
        StartCoroutine(FindUI(name));
    }

    void CancelHighLight(GameObject go)
    {
        Destroy(go.GetComponent<GraphicRaycaster>());
        Destroy(go.GetComponent<Canvas>());

        Next();
        EventTriggerListener.Get(go).onClick -= CancelHighLight;
    }

    IEnumerator FindUI(string name)
    {
        //寻找目标
        Transform go = UnityHelper.FindTheChildNode(GlobalObj.Canvas, name);
        while(go == null)
        {
            yield return new WaitForSeconds(0.1f);
            Debug.Log("wait");
            go = UnityHelper.FindTheChildNode(GlobalObj.Canvas, name);
        }

        //高亮
        maskTra = GlobalObj.GuideMask.transform;
        maskTra.gameObject.SetActive(true);
        maskTra.SetAsLastSibling();

        finger = UnityHelper.FindTheChildNode(maskTra.gameObject, "finger");
		Vector3 newPos = maskTra.InverseTransformPoint(go.transform.position);
        finger.localPosition = newPos + offset;
        finger.AddOrGetComponent<Canvas>().overrideSorting = true;
		finger.GetComponent<Canvas>().sortingOrder = 2;

		go.AddOrGetComponent<Canvas>().overrideSorting = true;
        go.GetComponent<Canvas>().sortingOrder = 1;
        go.AddOrGetComponent<GraphicRaycaster>();

        //设置监听
        EventTriggerListener.Get(go).onClick += CancelHighLight;
    }

}

