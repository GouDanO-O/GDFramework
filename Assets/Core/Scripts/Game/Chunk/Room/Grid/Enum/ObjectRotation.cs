using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Room.Grid
{
    /// <summary>
    /// 物品旋转方向
    /// </summary>
    public enum ObjectRotation
    {
        [LabelText("0°")]
        Deg0 = 0,

        [LabelText("90°")]
        Deg90 = 90,

        [LabelText("180°")]
        Deg180 = 180,

        [LabelText("270°")]
        Deg270 = 270
    }
}