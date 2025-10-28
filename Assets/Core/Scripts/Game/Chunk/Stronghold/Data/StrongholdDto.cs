using System;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Stronghold.Data
{
    [Serializable]
    public class StrongholdDto : ChunkDto
    {
        [LabelText("副本数据")]
        public StrongholdDtoDef strongholdDtoDef;
        
        public override IChunkDtoDef CreateRuntimeDef()
        {
            return strongholdDtoDef.Clone();
        }
    }
}