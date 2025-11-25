using Sirenix.OdinInspector;

namespace Core.Game.Grid
{
    /// <summary>
    /// 墙面朝向
    /// </summary>
    public enum WallSide
    {
        [LabelText("前(+Z)")]
        Front,
        
        [LabelText("后(-Z)")]
        Back,
        
        [LabelText("左(-X)")]
        Left,
        
        [LabelText("右(+X)")]
        Right
    }
}