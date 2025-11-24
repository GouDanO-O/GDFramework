using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Room
{
    /// <summary>
    /// 瓦片类型枚举
    /// </summary>
    public enum ETileType
    {
        [LabelText("空瓦片")]
        Empty = 0,

        [LabelText("地板")]
        Floor = 1,

        [LabelText("墙壁")]
        Wall = 2,

        [LabelText("门")]
        Door = 3,

        [LabelText("窗户")]
        Window = 4,

        [LabelText("楼梯")]
        Stairs = 5,

        [LabelText("水面")]
        Water = 6,

        [LabelText("草地(户外)")]
        Grass = 7,

        [LabelText("泥土(户外)")]
        Dirt = 8,

        [LabelText("石头(户外)")]
        Stone = 9
    }
}