using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Room.Data
{
    [Serializable]
    public class RoomDto : ChunkDto
    {
        [LabelText("房间数据")]
        public RoomDtoDef roomDtoDef;
        
        public override ChunkDtoDef CreateRuntimeDef()
        {
            return roomDtoDef.Clone();
        }
    }
}