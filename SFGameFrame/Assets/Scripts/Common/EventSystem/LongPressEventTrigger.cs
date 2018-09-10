using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class LongPressEventTrigger : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
	//[Tooltip("How long must pointer be down on this object to trigger a long press")]
	private float durationThreshold = 0.8f;

	public UnityEvent onLongPress = new UnityEvent();

    public Action onUp;

	private bool isPointerDown = false;
	private bool longPressTriggered = false;
	private float timePressStarted;

	private void Update()
	{
		if (isPointerDown && !longPressTriggered)
		{
			if (Time.time - timePressStarted > durationThreshold)
			{
				longPressTriggered = true;
				onLongPress.Invoke();
			}
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		timePressStarted = Time.time;
		isPointerDown = true;
		longPressTriggered = false;
	}

    public void OnPointerUp(PointerEventData eventData)
	{
		isPointerDown = false;
        if(onUp != null)
        {
            onUp.Invoke();
        }
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		isPointerDown = false;
	}
}