using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Room.Grid
{
    /// <summary>
    /// 编辑器操作模式
    /// </summary>
    public enum EditorMode
    {
        [LabelText("无")]
        None = 0,
        
        [LabelText("查看模式")]
        View = 1,
        
        [LabelText("地块编辑")]
        TileEdit = 2,
        
        [LabelText("物品放置")]
        ObjectPlace = 3,
        
        [LabelText("物品选择")]
        ObjectSelect = 4,
        
        [LabelText("删除模式")]
        Delete = 5
    }
}