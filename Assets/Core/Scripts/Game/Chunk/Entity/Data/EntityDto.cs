using System;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
using UnityEngine.Serialization;

namespace Core.Game.Chunk.Substance.Data
{
    [Serializable]
    public class EntityDto : ChunkDto
    {
        [FormerlySerializedAs("substanceDtoDef")]
        public EntityDtoDef entityDtoDef;
        
        public override IChunkDtoDef CreateRuntimeDef()
        {
            return entityDtoDef.Clone();
        }
    }
}