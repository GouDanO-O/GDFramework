using System;
using Core.Game.Chunk.Data;

namespace Core.Game.Chunk.Substance.Data
{
    [Serializable]
    public class SubstanceDtoDef : ChunkDtoDef
    {
        protected override string GetTypePrefix()
        {
            return "Substance";
        }
    }
}