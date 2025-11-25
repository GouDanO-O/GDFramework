using Sirenix.OdinInspector;

namespace Core.Game.Grid
{
    /// <summary>
    /// 墙壁生成类型
    /// </summary>
    public enum WallGenerationType
    {
        [LabelText("四面墙")]
        AllSides,
        
        [LabelText("仅前后墙")]
        FrontAndBack,
        
        [LabelText("仅左右墙")]
        LeftAndRight,
        
        [LabelText("不生成墙")]
        None
    }
}