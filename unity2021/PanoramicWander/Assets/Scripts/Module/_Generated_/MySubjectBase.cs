
//*************************************************************************************
//   !!! Generated by the fmp-cli 1.83.0.  DO NOT EDIT!
//*************************************************************************************

namespace XTC.FMP.MOD.PanoramicWander.LIB.Unity
{
    public class MySubjectBase
    {
        /// <summary>
        /// 创建
        /// </summary>
        /// <example>
        /// var data = new Dictionary<string, object>();
        /// data["uid"] = "default";
        /// data["style"] = "default";
        /// data["uiSlot"] = "";
        /// data["worldSlot"] = "";
        /// model.Publish(/XTC/PanoramicWander/Create, data);
        /// </example>
        public const string Create = "/XTC/PanoramicWander/Create";

        /// <summary>
        /// 打开
        /// </summary>
        /// <remarks>
        /// 先加载资源，然后显示
        /// </remarks>
        /// <example>
        /// var data = new Dictionary<string, object>();
        /// data["uid"] = "default";
        /// data["source"] = "file";
        /// data["uri"] = "";
        /// data["delay"] = 0f;
        /// model.Publish(/XTC/PanoramicWander/Open, data);
        /// </example>
        public const string Open = "/XTC/PanoramicWander/Open";

        /// <summary>
        /// 显示
        /// </summary>
        /// <remarks>
        /// 仅显示，不执行其他任何操作
        /// </remarks>
        /// <example>
        /// var data = new Dictionary<string, object>();
        /// data["uid"] = "default";
        /// data["delay"] = 0f;
        /// model.Publish(/XTC/PanoramicWander/Show, data);
        /// </example>
        public const string Show = "/XTC/PanoramicWander/Show";

        /// <summary>
        /// 隐藏
        /// </summary>
        /// <remarks>
        /// 仅隐藏，不执行其他任何操作
        /// </remarks>
        /// <example>
        /// var data = new Dictionary<string, object>();
        /// data["uid"] = "default";
        /// data["delay"] = 0f;
        /// model.Publish(/XTC/PanoramicWander/Hide, data);
        /// </example>
        public const string Hide = "/XTC/PanoramicWander/Hide";

        /// <summary>
        /// 关闭
        /// </summary>
        /// <remarks>
        /// 先隐藏，然后释放资源
        /// </remarks>
        /// <example>
        /// var data = new Dictionary<string, object>();
        /// data["uid"] = "default";
        /// data["delay"] = 0f;
        /// model.Publish(/XTC/PanoramicWander/Close, data);
        /// </example>
        public const string Close = "/XTC/PanoramicWander/Close";

        /// <summary>
        /// 销毁
        /// </summary>
        /// <example>
        /// var data = new Dictionary<string, object>();
        /// data["uid"] = "default";
        /// model.Publish(/XTC/PanoramicWander/Close, data);
        /// </example>
        public const string Delete = "/XTC/PanoramicWander/Delete";
    }
}
