using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Room.Grid
{
    /// <summary>
    /// 物品类别
    /// </summary>
    public enum ObjectCategory
    {
        [LabelText("家具")]
        Furniture = 0,
        
        [LabelText("装饰")]
        Decoration = 1,
        
        [LabelText("植物")]
        Plant = 2,
        
        [LabelText("照明")]
        Lighting = 3,
        
        [LabelText("存储")]
        Storage = 4,
        
        [LabelText("交互点")]
        Interactive = 5,
        
        [LabelText("传送点")]
        Teleport = 6,
        
        [LabelText("NPC")]
        NPC = 7,
        
        [LabelText("其他")]
        Other = 99
    }
}