using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Room.Grid
{
    /// <summary>
    /// 地块编辑工具
    /// </summary>
    public enum TileEditTool
    {
        [LabelText("画笔")]
        Brush = 0,
        
        [LabelText("填充")]
        Fill = 1,
        
        [LabelText("矩形")]
        Rectangle = 2,
        
        [LabelText("橡皮擦")]
        Eraser = 3
    }
}