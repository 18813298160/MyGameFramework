using UnityEngine;
using SUIFW;
using DG.Tweening;

public class CameraCtrl : Singleton<CameraCtrl>
{
    private bool canDragCamera;
    private float cachedFieldView;

    void Awake()
    {
        cachedFieldView = GlobalObj.mainCamera.fieldOfView;
    }

    void Update()
	{
		//Zoom out  
		if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			if (Camera.main.fieldOfView <= 100)
				Camera.main.fieldOfView += 2;
			if (Camera.main.orthographicSize <= 20)
				Camera.main.orthographicSize += 0.5F;
		}
		//Zoom in  
		if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			if (Camera.main.fieldOfView > 2)
				Camera.main.fieldOfView -= 2;
			if (Camera.main.orthographicSize >= 1)
				Camera.main.orthographicSize -= 0.5F;
		}

        if (Input.GetMouseButton(1) && canDragCamera)
        {
			transform.Rotate(-Input.GetAxis("Mouse Y") * 2, Input.GetAxis("Mouse X") * 2, 0);
		}
	}

    public Quaternion SetDrag()
    {
        Quaternion cachedRot = transform.rotation;
        canDragCamera = true;
        return cachedRot;
    }

    public void ResetFieldView()
    {
        GlobalObj.mainCamera.fieldOfView = cachedFieldView;
    }

    public void SetRot(Quaternion rot)
    {
        transform.rotation = rot;
    }

    public void CorrectCamera(float zValue = -1)
    {
        Quaternion q = Quaternion.Euler(transform.eulerAngles.x, -90, 0);
        transform.DORotateQuaternion(q, 1);
        if(zValue != -1)
            transform.DOMoveZ(zValue, 1);
    }
}