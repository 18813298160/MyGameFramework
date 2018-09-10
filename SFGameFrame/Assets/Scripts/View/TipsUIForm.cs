using SUIFW;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 公共tip
/// </summary>
public class TipsUIForm : BaseUIForm
{
	public Text descLabel;
    private Vector3 hidePos = new Vector3(310, 128f, 0);

	public void Awake()
	{
		//定义本窗体的性质(默认数值，可以不写)
        base.CurrentUIType.UIForms_Type = UIFormType.PopUp;
	}

	public override void Display(params object[] args)
	{
		base.Display(args);

		// 显示
		descLabel.text = (string)args[0];

		float duration = 1.0f;
		if (args.Length >= 2)
			duration = (float)args[1];

        transform.DOLocalMoveX(5, 0.8f).onComplete = () =>
		{
			transform.DOShakeScale(duration, 0.3f, 3).onComplete = () =>
			{
                transform.localPosition = hidePos;
				CloseUIForm();
			};
		};
	}
}