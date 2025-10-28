using System;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;

namespace Core.Game.Chunk.Substance.Data
{
    [Serializable]
    public class SubstanceDto : ChunkDto
    {
        public SubstanceDtoDef substanceDtoDef;
        
        public override IChunkDtoDef CreateRuntimeDef()
        {
            return substanceDtoDef.Clone();
        }
    }
}