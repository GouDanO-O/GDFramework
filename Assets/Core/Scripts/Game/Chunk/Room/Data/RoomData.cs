using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Node;
using Core.Game.Chunk.Node.Data;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Room.Data
{
    public class RoomData : ChunkData
    {
        [LabelText("当前房间的固定数据")]
        private RoomDto _roomDto;

        [LabelText("当前房间的临时数据")]
        private RoomDtoTemporary _roomDtoTemporary;
        
        [LabelText("当前房间所持有的节点数据")]
        private Dictionary<string,NodeData> _curHoldingNodeDtoDict = new Dictionary<string, NodeData>();

        
    }
}