using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Node;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Room
{
    public class RoomData : ChunkData
    {
        [LabelText("当前房间的固定数据")]
        private RoomDto roomDto;

        [LabelText("当前房间的临时数据")]
        private RoomDtoTemporary roomDtoTemporary;
        
        [LabelText("当前房间所持有的节点数据")]
        private Dictionary<string,NodeData> curHoldingNodeDtoDict = new Dictionary<string, NodeData>();

        
    }
}