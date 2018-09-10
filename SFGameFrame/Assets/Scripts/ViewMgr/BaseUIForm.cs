using UnityEngine;

namespace SUIFW
{
	public class BaseUIForm : MonoBehaviour {
        private UIType _CurrentUIType=new UIType();

        //当前UI窗体类型
	    public UIType CurrentUIType
	    {
	        get { return _CurrentUIType; }
	        set { _CurrentUIType = value; }
	    }

        public virtual void RegistEvent()
        {
            
        }

        public virtual void UnRegistEvent()
        {
            
        }

        #region  窗体的四种(生命周期)状态

        /// <summary>
        /// 显示状态
        /// </summary>
	    public virtual void Display(params object[] args)
	    {
            RegistEvent();
            ShowTween();
            //设置模态窗体调用(必须是弹出窗体)
            if (_CurrentUIType.UIForms_Type==UIFormType.PopUp)
            {
                UIMaskMgr.GetInstance().SetMaskWindow(this.gameObject,_CurrentUIType.UIForm_LucencyType);
            }
	    }

        public virtual void ShowTween()
        {
			this.gameObject.SetActive(true);
        }

        /// <summary>
        /// 隐藏状态
        /// </summary>
	    public virtual void Hiding()
	    {
            UnRegistEvent();
            this.gameObject.SetActive(false);
            //取消模态窗体调用
            if (_CurrentUIType.UIForms_Type == UIFormType.PopUp)
            {
                UIMaskMgr.GetInstance().CancelMaskWindow();
            }
        }

        /// <summary>
        /// 重新显示状态
        /// </summary>
	    public virtual void Redisplay()
	    {
            this.gameObject.SetActive(true);
            //设置模态窗体调用(必须是弹出窗体)
            if (_CurrentUIType.UIForms_Type == UIFormType.PopUp)
            {
                UIMaskMgr.GetInstance().SetMaskWindow(this.gameObject, _CurrentUIType.UIForm_LucencyType);
            }
        }

        /// <summary>
        /// 冻结状态
        /// </summary>
	    public virtual void Freeze()
	    {
            this.gameObject.SetActive(true);
        }

        #endregion

        #region 封装子类常用的方法

        /// <summary>
        /// 注册按钮事件
        /// </summary>
        /// <param name="buttonName">按钮节点名称</param>
        /// <param name="delHandle">委托：需要注册的方法</param>
	    protected void RigisterButtonObjectEvent(string buttonName, System.Action<GameObject> delHandle)
	    {
            GameObject goButton = UnityHelper.FindTheChildNode(this.gameObject, buttonName).gameObject;
            //给按钮注册事件方法
            if (goButton != null)
            {
                EventTriggerListener.Get(goButton).onClick = delHandle;
            }	    
        }

        /// <summary>
        /// 打开UI窗体
        /// </summary>
        /// <param name="uiFormName"></param>
	    protected void OpenUIForm(string uiFormName)
	    {
            UIManager.GetInstance().ShowUIForms(uiFormName);
        }

        /// <summary>
        /// 关闭当前UI窗体
        /// </summary>
	    protected void CloseUIForm(string otherForm = "")
	    {
            if (!string.IsNullOrEmpty(otherForm))
            {
                UIManager.GetInstance().CloseUIForms(otherForm);
                return;
            }

	        string strUIFromName = string.Empty;            //处理后的UIFrom 名称
	        int intPosition = -1;

            strUIFromName=GetType().ToString();             //命名空间+类名
            intPosition=strUIFromName.IndexOf('.');
            if (intPosition!=-1)
            {
                //剪切字符串中“.”之间的部分
                strUIFromName = strUIFromName.Substring(intPosition + 1);
            }

            UIManager.GetInstance().CloseUIForms(strUIFromName);
        }

        /// <summary>
        /// 派发消息
        /// </summary>
	    protected void DispatchEvent(string eventKey, params object[] args)
	    {
            NotifyCenter.instance.DispatchEvent(eventKey, args);	    
        }

		/// <summary>
		/// 注册消息
		/// </summary>
		public void AddEvent(string eventName, OnNotifyHandler handler)
		{
			NotifyCenter.instance.RegistEvent(eventName, handler);
		}

		/// <summary>
		/// 注销消息
		/// </summary>
		public void RemoveEvent(string eventName, OnNotifyHandler handler)
		{
			NotifyCenter.instance.UnRegistEvent(eventName, handler);
		}

        public bool IsVisible()
        {
            if (gameObject == null)
                return false;
            return gameObject.activeInHierarchy;
        }

	    #endregion

    }
}