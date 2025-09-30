using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Node;
using Core.Game.Chunk.Region;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Room
{
    [CreateAssetMenu(fileName = "RoomDto", menuName = "Core/RoomDto")]
    public class RoomDto : ChunkDto
    {
        [LabelText("房间里面拥有的互动节点")]
        public List<NodeDto> nodeDatas = new List<NodeDto>();

        [LabelText("房间里面拥有的节点ID"), ReadOnly]
        public List<string> nodeIds = new List<string>();
        
    }
}