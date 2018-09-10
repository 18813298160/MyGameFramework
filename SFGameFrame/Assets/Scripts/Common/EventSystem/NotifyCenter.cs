using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    /// <summary>
    /// 定义事件分发委托
    /// </summary>
    public delegate void OnNotifyHandler(params object[] args);

    /// <summary>
    /// 单例通知中心
    /// </summary>
    public class NotifyCenter
    {

        /// <summary>
        /// 存储事件的字典
        /// </summary>
        private Dictionary<string, OnNotifyHandler> NotifyDic = new Dictionary<string, OnNotifyHandler>();

        private static NotifyCenter _instance;

        public static NotifyCenter instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NotifyCenter();
                }
                return _instance;
            }

        }


        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="EventName">事件Key</param>
        /// <param name="handler">事件监听器</param>

        public void RegistEvent(string EventName, OnNotifyHandler handler)
        {
            if (!NotifyDic.ContainsKey(EventName))
            //如果不存在，就添加新的数据结构进入缓存 
            {
                NotifyDic.Add(EventName, handler);
            }
            else
                NotifyDic[EventName] += handler;
            //已存在，就添加监听到数据结构的委托上
        }


        /// <summary>
        /// 移除事件
        /// </summary>
        /// <param name="EventName">事件Key</param>
        /// <param name="handler">事件监听器</param>
        public void UnRegistEvent(string EventName, OnNotifyHandler handler)
        {
            if (handler == null)
            {
                return;
            }

            if (NotifyDic.ContainsKey(EventName))
            //如果存在
            {
                NotifyDic[EventName] -= handler;
                //移除监听 
                if (NotifyDic[EventName] == null)
                //该类型是否还有回调，如果没有，移除
                {
                    NotifyDic.Remove(EventName);
                }
            }
        }

        //通过DispatchEvent方法来分发一个事件，事件的回调函数采用委托来实现

        /// <summary>
        /// 分发事件
        /// </summary>
        /// <param name="eventKey">事件Key</param>
        public void DispatchEvent(string eventKey, params object[] args)
        {
            if (!NotifyDic.ContainsKey(eventKey))
                return;
            OnNotifyHandler handler = NotifyDic[eventKey];
            handler(args);
        }
    }