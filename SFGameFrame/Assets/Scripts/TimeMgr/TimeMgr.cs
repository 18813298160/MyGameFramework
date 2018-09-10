using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 简单的时间管理器
/// </summary>
class TimeMgr : MonoBehaviour
{
    static List<Timer> timerList = new List<Timer>();

    public static void AddTimer(Timer timer)
    {
        timerList.Add(timer);
    }

    public static void RemoveTimer(Timer timer)
    {
        timerList.Remove(timer);
    }

    void Update()
    {
        for (int i = 0; i < timerList.Count; i++ )
        {
            timerList[i].Update();
        }
    }
}
