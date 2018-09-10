using UnityEngine;
using DG.Tweening;
using SUIFW;

public class LogonUIForm : BaseUIForm
{
    private Transform monsterTrans;

	public override void ShowTween()
	{
		base.ShowTween();
		monsterTrans = UnityHelper.FindTheChildNode(gameObject, "Monster");
		var canvasGroup = gameObject.AddOrGetComponent<CanvasGroup>();
		canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.5f).onComplete = () =>
        {
			monsterTrans.DOShakeScale(1, 0.5f, 7);
        };
	}

    public void Awake()
    {
        //定义本窗体的性质(默认数值，可以不写)
        base.CurrentUIType.UIForms_Type = UIFormType.Normal;
        base.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        base.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;

        RigisterButtonObjectEvent("Btn_OK", 
              p =>
              {
                  CloseUIForm();
                  UIManager.GetInstance().ShowUIForms(ProConst.BOSS_APPEAR_UI_FORM);
			  }
            );
    }

}