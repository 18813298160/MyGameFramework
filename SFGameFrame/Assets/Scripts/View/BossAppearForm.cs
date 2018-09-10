using UnityEngine;
using DG.Tweening;
using SUIFW;
using UnityEngine.UI;

public class BossAppearForm : BaseUIForm
{
    private Image bgImage;
    private Image bossImage;

	public override void ShowTween()
	{
		base.ShowTween();
        bgImage.transform.DOShakeScale(1).onComplete = () =>
        {
            CloseUIForm();
        };
	}

	public override void Display(params object[] args)
	{
		base.Display(args);
	}

    public void Awake()
    {
        //定义本窗体的性质(默认数值，可以不写)
        base.CurrentUIType.UIForms_Type = UIFormType.Normal;
        base.CurrentUIType.UIForms_ShowMode = UIFormShowMode.Normal;
        base.CurrentUIType.UIForm_LucencyType = UIFormLucenyType.Lucency;
        bgImage = GetComponent<Image>();
        bossImage = UnityHelper.GetTheChildNodeComponetScripts<Image>(gameObject, "Boss");
    }

}