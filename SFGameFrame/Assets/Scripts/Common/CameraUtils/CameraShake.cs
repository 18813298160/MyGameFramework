using UnityEngine;
using System.Collections;

public class CameraShake : Singleton<CameraShake>
{

	// 抖动目标的transform(若未添加引用，怎默认为当前物体的transform)
	public Transform camTransform;

	//持续抖动的时长
	public float shake = 0f;

	// 抖动幅度（振幅）
	//振幅越大抖动越厉害
	public float shakeAmount = 1f;
	public float decreaseFactor = 1.0f;

	Vector3 originalPos;

	void Awake()
	{
		if (camTransform == null)
		{
			camTransform = GetComponent(typeof(Transform)) as Transform;
		}
	}

	void OnEnable()
	{
		originalPos = camTransform.localPosition;
	}

	void Update()
	{
		if (shake > 0)
		{
			camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

			shake -= Time.deltaTime * decreaseFactor;
		}
		else
		{
			shake = 0f;
			camTransform.localPosition = originalPos;
		}
	}

    public void SetShakeTime(float time)
    {
        time = Mathf.Clamp(time, 0, 1);
        shake = time;
    }
}