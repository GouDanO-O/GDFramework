using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Node;
using Core.Game.Chunk.Region;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Room.Data
{
    [CreateAssetMenu(fileName = "RoomDto", menuName = "Core/RoomDto")]
    public class RoomDto : ChunkDto
    {
        [LabelText("房间数据")]
        public RoomDtoDef roomDtoDef;
        
    }
}