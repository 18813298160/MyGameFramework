using UnityEngine;
using UnityEngine.EventSystems;
using System;

namespace SUIFW
{
    public class EventTriggerListener :UnityEngine.EventSystems.EventTrigger 
    {
        public Action<GameObject> onClick;
        public Action<GameObject> onDown;
        public Action<GameObject> onEnter;
        public Action<GameObject> onExit;
        public Action<GameObject> onUp;
        public Action<GameObject> onSelect;
        public Action<GameObject> onUpdateSelect;


        /// <summary>
        /// 得到“监听器”组件
        /// </summary>
        /// <param name="go">监听的游戏对象</param>
        /// <returns>
        /// 监听器
        /// </returns>
        public static EventTriggerListener Get(GameObject go)
        {
            EventTriggerListener lister = go.GetComponent<EventTriggerListener>();
            if (lister==null)
            {
                lister = go.AddComponent<EventTriggerListener>();                
            }
            return lister;
        }

        public static EventTriggerListener Get(Transform trans)
		{
			EventTriggerListener lister = trans.AddOrGetComponent<EventTriggerListener>();
			return lister;
		}

        public override void OnPointerClick(PointerEventData eventData)
        {
            if(onClick!=null)
            {
                onClick(gameObject);
            }
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (onDown != null)
            {
                onDown(gameObject);
            }
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (onEnter != null)
            {
                onEnter(gameObject);
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (onExit != null)
            {
                onExit(gameObject);
            }
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (onUp != null)
            {
                onUp(gameObject);
            }
        }
    
        public override void OnSelect(BaseEventData eventBaseData)
        {
            if (onSelect != null)
            {
                onSelect(gameObject);
            }
        }

        public override void OnUpdateSelected(BaseEventData eventBaseData)
        {
            if (onUpdateSelect != null)
            {
                onUpdateSelect(gameObject);
            }
        }
	
    }//Class_end
}
