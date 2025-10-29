using System;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Dungeon.Data
{
    [Serializable]
    public class DungeonDto : ChunkDto
    {
        [LabelText("副本数据")]
        public DungeonDtoDef DungeonDtoDef;
        
        public override IChunkDtoDef CreateRuntimeDef()
        {
            return DungeonDtoDef.Clone();
        }
    }
}