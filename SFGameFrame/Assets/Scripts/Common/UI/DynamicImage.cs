using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DynamicImage : MonoBehaviour
{
	public Shader s;

    private float ScaleDuration = 1.3f;
    private float CycleInterval = 5;
    private float ScaleRatio = 0.1f;
    private Canvas ParentCanvas;

    private Image image;

    private Coroutine playingCoroutine;

    private static Material UIExpandMat = null;
    private static int TimeOffsetID = -1;
    private static int TimeScaleID = -1;
	private static int ScaleRatioID = -1;

    void Awake()
    {
        //初始化静态变量
        if (UIExpandMat == null)
        {
            UIExpandMat = new Material(s);
            TimeOffsetID = Shader.PropertyToID("_TimeOffset");
            TimeScaleID = Shader.PropertyToID("_TimeScale");
			ScaleRatioID = Shader.PropertyToID("_ScaleRatio");
        }

        UIExpandMat.SetFloat(TimeScaleID, 1 / ScaleDuration);
        UIExpandMat.SetFloat(ScaleRatioID, ScaleRatio);

	    ParentCanvas = GetComponentInParent<Canvas>();

	if (ParentCanvas == null || ParentCanvas.worldCamera == null)
        {
            Debug.LogError("The parent canvas of UIAutoExpand could not be empty!");
            return;
        }
        image = GetComponent<Image>();

    }

    private void OnEnable()
    {
        playingCoroutine = StartCoroutine(PlayExpandAni());
    }

    private void OnDisable()
    {
        if (playingCoroutine != null)
        {
            StopCoroutine(playingCoroutine);
        }
    }

    private void OnDestroy()
    {
        if (playingCoroutine != null)
        {
            StopCoroutine(playingCoroutine);
        }
    }

    private void StartScaleImage()
    {
        if (image == null)
        {
            return;
        }

        image.material = UIExpandMat;
    }

    private IEnumerator PlayExpandAni()
    {
        while (true)
        {
            UIExpandMat.SetFloat(TimeOffsetID, Time.timeSinceLevelLoad);
            StartScaleImage();
            yield return new WaitForSeconds(ScaleDuration);

            yield return new WaitForSeconds(CycleInterval);
        }
    }
}