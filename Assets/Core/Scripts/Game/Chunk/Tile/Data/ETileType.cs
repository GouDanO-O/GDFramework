using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Tile
{
    [LabelText("地图瓦片类型")]
    public enum ETileType
    {
        Ground = 0,       //地面(可以行走,可以放置物体)
        Wall = 1,         //墙壁
        Door = 2,         //门
        Obstacle = 3,     //障碍物
    }
}