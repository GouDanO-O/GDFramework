using System;
using Core.Game.Chunk.Interface;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Data
{
    [Serializable]
    public class ChunkDtoDef : IChunkDtoDef
    {
        [LabelText("数据唯一ID"),ReadOnly]
        public string chunkDtoDefId;

        public ChunkDtoDef(string parentChunkId,string thisChunkId)
        {
            
        }
    }
}