using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Node;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Room
{
    [Serializable]
    public class RoomDtoDef : ChunkDtoDef
    {
        [LabelText("房间里面拥有的互动节点")]
        public List<NodeDto> nodeDatas = new List<NodeDto>();

        [LabelText("房间里面拥有的节点ID"), ReadOnly]
        public List<string> nodeIds = new List<string>();
    }
}