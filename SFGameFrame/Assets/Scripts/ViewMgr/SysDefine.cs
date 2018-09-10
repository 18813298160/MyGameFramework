using System.Collections;

namespace SUIFW
{

    #region 系统枚举类型

    /// <summary>
    /// UI窗体（位置）类型
    /// </summary>
    public enum UIFormType
    {
        //普通窗体
        Normal,   
        //固定窗体                              
        Fixed,
        //弹出窗体
        PopUp
    }

    /// <summary>
    /// UI窗体的显示类型
    /// </summary>
    public enum UIFormShowMode
    {
        //普通
        Normal,
        //反向切换
        ReverseChange,
        //隐藏其他
        HideOther
    }

    /// <summary>
    /// UI窗体透明度类型
    /// </summary>
    public enum UIFormLucenyType
    {
        //完全透明，不能穿透
        Lucency,
        //半透明，不能穿透
        Translucence,
        //低透明度，不能穿透
        ImPenetrable,
        //可以穿透
        Pentrate    
    }

    #endregion

    public class SysDefine
    {
        /* 路径常量 */
        public const string SYS_NAME_CANVAS = "Canvas";

		/* 标签常量 */
		public const string SYS_TAG_CANVAS = "_TagCanvas";
        /* 节点常量 */
        public const string SYS_NORMAL_NODE = "Normal";
        public const string SYS_FIXED_NODE = "Fixed";
        public const string SYS_POPUP_NODE = "PopUp";
		public const string SYS_SCRIPTMANAGER_NODE = "_ScriptMgr";
		public const string GUIDE_MASK = "GuideMask";
        /* 遮罩管理器中，透明度常量 */
        public const float SYS_UIMASK_LUCENCY_COLOR_RGB = 255 / 255F;
        public const float SYS_UIMASK_LUCENCY_COLOR_RGB_A = 0F / 255F;

        public const float SYS_UIMASK_TRANS_LUCENCY_COLOR_RGB = 220 / 255F;
        public const float SYS_UIMASK_TRANS_LUCENCY_COLOR_RGB_A = 50F / 255F;

        public const float SYS_UIMASK_IMPENETRABLE_COLOR_RGB = 50 / 255F;
        public const float SYS_UIMASK_IMPENETRABLE_COLOR_RGB_A = 200F / 255F;

		public const string UiModelLayer = "UiModel";

		public const string GuideProgressCfg = "GuideProgress";
		public const string GuideFileCfg = "GuideFileDefine";

		/* 摄像机层深的常量 */

	}
}