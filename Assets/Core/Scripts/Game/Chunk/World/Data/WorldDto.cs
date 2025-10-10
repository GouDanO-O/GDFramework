using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Region;
using Core.Game.Chunk.Region.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Game.Chunk.World.Data
{
    [Serializable]
    public class WorldDto : ChunkDto
    {
        [LabelText("世界数据")]
        public WorldDtoDef worldDtoDef;
        
        public override ChunkDtoDef CreateRuntimeDef()
        {
            return worldDtoDef.Clone();
        }
    }
}