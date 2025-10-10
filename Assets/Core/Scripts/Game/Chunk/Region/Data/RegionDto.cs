using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Room;
using Core.Game.Chunk.Room.Data;
using Core.Game.Chunk.Room;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Region.Data
{
    [Serializable]
    public class RegionDto : ChunkDto
    {
        [LabelText("区块数据")]
        public RegionDtoDef regionDtoDef;

        public override ChunkDtoDef CreateRuntimeDef()
        {
            return regionDtoDef.Clone();
        }
    }
}