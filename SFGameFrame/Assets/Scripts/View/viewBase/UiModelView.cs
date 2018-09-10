using UnityEngine;
using SUIFW;

public class UiModelView
{

    public static UiModelView instance
    {
        get
        {
            return Single<UiModelView>.instance;
        }
    }
    private Camera UiModelCamera;
    public RenderTexture Rt { get; private set; }

	public void Init () 
    {
        if (!GlobalObj.UiModelCamera) return;
        UiModelCamera = GlobalObj.UiModelCamera;
        UiModelCamera.clearFlags = CameraClearFlags.SolidColor;
        UiModelCamera.cullingMask = 1 << LayerMask.NameToLayer(SysDefine.UiModelLayer);
        Rt = new RenderTexture(Screen.width, Screen.height, 0);
        Rt.format = RenderTextureFormat.ARGB32;
        UiModelCamera.targetTexture = Rt;
	}
	
}
