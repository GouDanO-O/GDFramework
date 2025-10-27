using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Tile;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Room.Data
{
    /// <summary>
    /// 房间
    /// 房间里面存有所持有的所有格子块
    /// 包括格子块上的放置的物体
    /// </summary>
    public class RoomData : ChunkData
    {
        [LabelText("当前房间的固定数据")]
        private RoomDto _roomDto;

        [LabelText("当前房间的临时数据")]
        private RoomDtoTemporary _roomDtoTemporary;
        
        [LabelText("当前房间所持有的格子数据")]
        private Dictionary<string,TileData> _curHoldingNodeDtoDict = new Dictionary<string, TileData>();

        
    }
}