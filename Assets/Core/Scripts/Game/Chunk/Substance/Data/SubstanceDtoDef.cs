using System;
using Core.Game.Chunk.Data;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Substance.Data
{
    [Serializable]
    public class SubstanceDtoDef : ChunkDtoDef
    {
        [LabelText("贴图ID")]
        public string spriteId;
        
        
        
        protected override string GetTypePrefix()
        {
            return "Substance";
        }
    }
}