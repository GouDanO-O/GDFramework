using System;
using Core.Game.Chunk.Data;

namespace Core.Game.Chunk.Substance.Data
{
    [Serializable]
    public class SubstanceDto : ChunkDto
    {
        public SubstanceDtoDef substanceDtoDef;
        
        public override ChunkDtoDef CreateRuntimeDef()
        {
            return substanceDtoDef.Clone();
        }
    }
}