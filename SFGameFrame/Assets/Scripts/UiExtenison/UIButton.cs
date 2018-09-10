using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace UnityEngine.UI
{
    // 扩展按钮 UIButton, 支持更多的事件 支持选中状态
    public class UIButton : Selectable, IPointerClickHandler, ISubmitHandler
    {
        // 这两个是通用设置
        [NonSerialized] public static float onLongWaitTime = 0.3f; // 长按等待时间，这些参数可在lua里进行设置
		[NonSerialized] public static float onDoubleClickTimeSpan = 0.2f; // 双击最长时间间隔

        [Header("按钮点击响应间隔")]
        public float clickIntervalTime = 0.05f; // 按钮点击响应时间间隔
        [NonSerialized] public static string defaultClickSound = "UI_Click"; // 默认的点击音效

        public float _animationTime = 0.2f; // 自定义动画的时间
        public float _animationScale = 0.9f; // 自定义动画的缩放比例
        public bool _customAnimation = false;   // 是否自定义动画
        public UICollider _collider;            // 如果有勾选自定义动画，则需要有一个子Collider，防止按钮缩小后无法触发点击区域
        public Sprite selectedSprite; // 选中状态的图片

        // 禁用状态是否灰化(权衡了一下，由于像背包按钮这样量比较大，如果在lua中设置会有大量的lua和c#的交互)
        [SerializeField] private bool _enableGrayEffect = true;
        public bool enableGrayEffect {get {return _enableGrayEffect;} set { _enableGrayEffect = value; } }

        // 点击音效
        [SerializeField] private bool _enableClickSound = true; 
        public bool enableClickSound { get { return _enableClickSound; } set { _enableClickSound = value; } }
        public string clickSound { get; set; } // 点击音效路径，默认为空则播放默认点击音效，如果需要配置的话，在lua里面设置

        [SerializeField] private bool m_IsOn = false;
        public class ButtonClickedEvent : UnityEvent
        {
        }

        private ButtonClickedEvent m_OnClick;
        private ButtonClickedEvent m_OnLongClick;
	    private ButtonClickedEvent m_OnDoubleClick;
        private ButtonClickedEvent m_OnDown;
        private ButtonClickedEvent m_OnUp;
        private ButtonClickedEvent m_OnEnter;
        private ButtonClickedEvent m_OnExit;

        private Coroutine m_longClickCoroutine;
        private Coroutine m_doubleClickCoroutine;
        private static WaitForSeconds m_longClickWait = new WaitForSeconds(onLongWaitTime);
        private static WaitForSeconds m_doubleClickWait = new WaitForSeconds(onDoubleClickTimeSpan);

        protected UIButton()
        {
        }

        private bool isPointerDown = false;
        private bool isPointerInside = false;

        private Vector3 _basicScale;

        // 是否处于选中状态
        public bool isOn
        {
            get { return selectedSprite != null && m_IsOn; }
            set
            {
                if (selectedSprite == null) return;

                m_IsOn = value;

                if (image)
                {
                    image.overrideSprite = m_IsOn ? selectedSprite : null;
                }
            }
        }

        // 点击事件
        public ButtonClickedEvent onClick
        {
            get {
                if (m_OnClick == null)
                {
                    m_OnClick = new ButtonClickedEvent();
                }
                return m_OnClick;
            }
        }

        // 长按事件
        public ButtonClickedEvent onLongClick
        {
            get
            {
                if (m_OnLongClick == null)
                {
                    m_OnLongClick = new ButtonClickedEvent();
                }
                return m_OnLongClick;
            }
        }

		// 双击事件
		public ButtonClickedEvent onDoubleClick
		{
			get
			{
				if (m_OnDoubleClick == null)
				{
					m_OnDoubleClick = new ButtonClickedEvent();
				}
				return m_OnDoubleClick;
			}
		}

		// 按下事件
		public ButtonClickedEvent onDown
        {
            get
            {
                if (m_OnDown == null)
                {
                    m_OnDown = new ButtonClickedEvent();
                }
                return m_OnDown;
            }
        }

        // 松开事件
        public ButtonClickedEvent onUp
        {
            get
            {
                if (m_OnUp == null)
                {
                    m_OnUp = new ButtonClickedEvent();
                }
                return m_OnUp;
            }
        }

        // 进入事件
        public ButtonClickedEvent onEnter
        {
            get
            {
                if (m_OnEnter == null)
                {
                    m_OnEnter = new ButtonClickedEvent();
                }
                return m_OnEnter;
            }
        }

        // 离开事件
        public ButtonClickedEvent onExit
        {
            get 
            {
                if (m_OnExit == null)
                {
                    m_OnExit = new ButtonClickedEvent();
                }
                return m_OnExit;
            }                  
        }

        protected override void Awake()
        {
            base.Awake();
            _basicScale = transform.localScale;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            isOn = m_IsOn;
        }

#endif // if UNITY_EDITOR
        
	    private float _firstClickTime = 0f; // 双击事件上次点击时间
        private float _lastClickTime = 0f; // 上次响应点击时间
        private void Press()
        {
            if (!IsActive() || !IsInteractable()) return;

            // 播放点击音效
            if (_enableClickSound)
            {
                if (string.IsNullOrEmpty(clickSound))
                {
                    //AudioController.Play(defaultClickSound, 0);
                }
                else
                {
                    //AudioController.Play(clickSound, 0);
                }
            }

			//如果没有双击事件，则不用后续判断，直接是单击操作
	        if (m_OnDoubleClick == null)
	        {
                Click();
		        return;
	        }

			if (Time.realtimeSinceStartup - _firstClickTime > onDoubleClickTimeSpan)
			{
                // 开始双击计时
				_firstClickTime = Time.realtimeSinceStartup;

                // 如果有单击事件，则需要一个延迟响应的单击事件
			    if (m_OnClick != null)
			    {
			        m_doubleClickCoroutine = StartCoroutine(wait());
                }
			}
			else
			{
			    if (m_doubleClickCoroutine != null)
			    {
			        StopCoroutine(m_doubleClickCoroutine);
                }
				_firstClickTime = 0;

                // 响应双击事件
				DoubleClick();
			}

		}

		private void Click()
		{
            float curTime = Time.realtimeSinceStartup;
            if (m_OnClick != null && curTime - _lastClickTime >= clickIntervalTime)
			{
                _lastClickTime = curTime;
                m_OnClick.Invoke();
			}
		}

		private void DoubleClick()
		{
			if (m_OnDoubleClick != null)
			{
				m_OnDoubleClick.Invoke();
			}
		}

		private void Down()
        {
            if (!IsActive() || !IsInteractable()) return;
            if (_customAnimation)
            {
                PlayPressEffect();
            }
            if (m_OnDown != null) m_OnDown.Invoke();

            // 如果需要响应长按的话，开始长按计时
            if (m_OnLongClick != null)
            {
                m_longClickCoroutine = StartCoroutine(grow());
            }
        }

        private void Up()
        {
            if (!IsActive() || !IsInteractable() || !isPointerDown) return;
            if (_customAnimation)
            {
                PlayReleaseEffect();
            }
            if (m_longClickCoroutine != null)
            {
                StopCoroutine(m_longClickCoroutine);
            }

            if (m_OnUp != null) m_OnUp.Invoke();
        }

        private void Enter()
        {
            if (!IsActive()) return;
            if (m_OnEnter != null) m_OnEnter.Invoke();
        }

        private void Exit()
        {
            if (!IsActive() || !isPointerInside) return;
            if (m_OnExit != null) m_OnExit.Invoke();
        }

        private void LongClick()
        {
            if (!IsActive() || !isPointerDown) return;
	        if (m_OnLongClick != null) m_OnLongClick.Invoke();
        }
        
        // 长按事件
        private IEnumerator grow()
        {
            yield return m_longClickWait;
            LongClick();
        }

        // 如果超过双击时间，则响应单击事件
		private IEnumerator wait()
		{
			yield return m_doubleClickWait;
			Click();
		}

		protected override void OnDisable()
        {
            isPointerDown = false;
            isPointerInside = false;

            if (m_doubleClickCoroutine != null)
            {
                StopCoroutine(m_doubleClickCoroutine);
            }

            if (m_longClickCoroutine != null)
            {
                StopCoroutine(m_longClickCoroutine);
            }
            base.OnDisable();
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            Press();
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            isPointerDown = true;
            Down();
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left) return;
            Up();
            isPointerDown = false;
            base.OnPointerUp(eventData);
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            isPointerInside = true;
            Enter();
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            Exit();
            isPointerInside = false;
            base.OnPointerExit(eventData);
        }

        public virtual void OnSubmit(BaseEventData eventData)
        {
            Press();

            // if we get set disabled during the press
            // don't run the coroutine.
            if (!IsActive() || !IsInteractable()) return;

            DoStateTransition(SelectionState.Pressed, false);
            StartCoroutine(OnFinishSubmit());
        }

        private IEnumerator OnFinishSubmit()
        {
            yield return new WaitForSecondsRealtime(colors.fadeDuration);
            DoStateTransition(currentSelectionState, false);
        }

        private bool _isDisable = false;

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            if (state == SelectionState.Disabled)
            {
                // 灰化状态特殊处理
                _isDisable = true;
                if (_enableGrayEffect)
                {
                    //MyUtils.SetGray(gameObject, true, true);
                }
            }
            else
            {
                // 如果处于灰化状态，则恢复
                if (_isDisable)
                {
                    _isDisable = false;
                    //MyUtils.SetGray(gameObject, false, true);
                }

                if (!_customAnimation)
                {
                    // 默认的动画
                    base.DoStateTransition(state, instant);
                }
            }
        }

        // 按下的放大效果
        protected void PlayPressEffect()
        {
            if (!_collider)
            {
                // 先寻找子控件ButtonCollider
                var buttonCollider = transform.Find("ButtonCollider");
                if (buttonCollider)
                {
                    _collider = buttonCollider.GetComponent<UICollider>();
                }
            }

            // 没有找到的话，添加一个默认的Collider
            if (!_collider)
            {
                var col = new GameObject("ButtonCollider", typeof(RectTransform), typeof(UICollider));
                var tr = col.GetComponent<RectTransform>();
                tr.SetParent(transform);
                tr.anchorMax = Vector2.zero;
                tr.anchorMin = Vector2.zero;
                _collider = col.GetComponent<UICollider>();
            }

            // 放大collider，防止缩小后触摸点离开点击区域
            if (_collider)
            {
                _collider.transform.localScale = Vector3.one / _animationScale / _animationScale;
            }
            transform.DOScale(_basicScale * _animationScale, _animationTime);
        }

        // 按钮还原
        protected void PlayReleaseEffect()
        {
            if (_collider)
            {
                _collider.transform.localScale = Vector3.one;
            }
            transform.DOScale(_basicScale, _animationTime);
        }

        // 设置是否可见
        public void SetActive(bool active)
        {
            if (gameObject.activeSelf != active)
            {
                gameObject.SetActive(active);
            }
        }
    }
}