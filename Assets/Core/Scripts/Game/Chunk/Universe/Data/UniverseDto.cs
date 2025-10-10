using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.World.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Game.Chunk.Universe.Data
{
    [Serializable]
    public class UniverseDto : ChunkDto
    {
        [LabelText("宇宙数据Def")]
        public UniverseDtoDef universeDtoDef;
        
        public override ChunkDtoDef CreateRuntimeDef()
        {
            return universeDtoDef.Clone();
        }
    }
}