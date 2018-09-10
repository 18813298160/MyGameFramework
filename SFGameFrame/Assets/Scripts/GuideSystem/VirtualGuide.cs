using UnityEngine;
using System.Collections;
using SUIFW;
using System;

/// <summary>
/// 新手引导接入时间不足，暂时使用此文件
/// </summary>
public class VirtualGuide
{
    private static bool hasShowMoveoutTip = false;

    public static void DealMoveOutTip()
    {
        if(!hasShowMoveoutTip) 
        {
            UIManager.GetInstance().ShowUIForms(ProConst.TIPS_UI_FORM, "移到边缘可以取消选中状态哦～");
            hasShowMoveoutTip = true;
        }
    }

}
