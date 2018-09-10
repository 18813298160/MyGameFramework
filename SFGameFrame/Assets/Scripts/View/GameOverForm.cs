using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using SUIFW;

public class GameOverForm : BaseUIForm 
{

	public override void ShowTween()
	{
		base.ShowTween();
		var canvasGroup = gameObject.AddOrGetComponent<CanvasGroup>();
		canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 2.5f);
	}

	public override void Display(params object[] args)
	{
        base.Display(args);
	}

	void Awake () 
    {
	    //窗体性质
        CurrentUIType.UIForms_Type = UIFormType.PopUp;

        RigisterButtonObjectEvent("RepeatBtn",
          p =>
          {
            CloseUIForm();
          });
    }
	
}