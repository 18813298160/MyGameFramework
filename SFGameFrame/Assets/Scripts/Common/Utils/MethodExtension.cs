using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity的扩展方法
/// </summary>
static public class MethodExtension
{
    static public T AddOrGetComponent<T>(this Component obj, System.Action addCb = null) where T : Component
    {
        T result = obj.GetComponent<T>();
        if (result == null)
        {
            result = obj.gameObject.AddComponent<T>();
            if(addCb != null)
            {
                addCb.Invoke();
            }
        }
        //var bcomp = result as Behaviour;
        //if(bcomp != null)
            //bcomp.enabled = false;
        return result;
    }

    static public T AddOrGetComponent<T>(this GameObject obj) where T : Component
	{
		T result = obj.GetComponent<T>();
		if (result == null)
			result = obj.AddComponent<T>();
		//var bcomp = result as Behaviour;
		//if (bcomp != null)
			//bcomp.enabled = false;
		return result;
	}

    static public T SetPosition<T>(this T selfBehavior, Vector3 pos, bool setPos = false) where T : Component
    {
        if(setPos)
            selfBehavior.transform.localPosition = pos;
        return selfBehavior;
    }

    static public T SetScale<T>(this T selfBehavior, float scale) where T : Component
    {
        selfBehavior.transform.localScale *= scale;
		return selfBehavior;
    }

}
