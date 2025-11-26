using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Room.Grid
{
    /// <summary>
    /// 地块类型
    /// 定义房间中可使用的地面类型
    /// </summary>
    public enum TileType
    {
        [LabelText("无")]
        None = 0,
        
        [LabelText("草地")]
        Grass = 1,
        
        [LabelText("泥土")]
        Dirt = 2,
        
        [LabelText("石板")]
        Stone = 3,
        
        [LabelText("木地板")]
        Wood = 4,
        
        [LabelText("沙地")]
        Sand = 5,
        
        [LabelText("水")]
        Water = 6,
        
        [LabelText("地毯")]
        Carpet = 7,
        
        [LabelText("瓷砖")]
        Tile = 8,
        
        [LabelText("金属")]
        Metal = 9,
        
        [LabelText("玻璃")]
        Glass = 10,
        
        [LabelText("雪地")]
        Snow = 11,
        
        [LabelText("岩浆")]
        Lava = 12,
        
        [LabelText("冰面")]
        Ice = 13,
        
        [LabelText("自定义")]
        Custom = 99
    }

}