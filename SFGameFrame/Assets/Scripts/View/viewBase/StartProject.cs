﻿using SUIFW;
using UnityEngine;

public class MonsterDamage
{
    public int turn;
    public int damage;
    public int life;
}

public class StartProject : MonoBehaviour
{
    void Start()
    {

#if UNITY_ANDROID
        //安卓平台关闭log，减少消耗
        //Debug.unityLogger.logEnabled = false;
#endif

        GlobalObj.Init();
        ResMgr.instance.Init();
        //GuideManager.instance.Init();
        //UiModelView.instance.Init();
        UIManager.GetInstance().ShowUIForms(ProConst.LOGON_FROMS);
    }
}