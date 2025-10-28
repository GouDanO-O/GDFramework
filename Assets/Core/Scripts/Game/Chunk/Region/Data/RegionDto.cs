using System;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Region.Data
{
    [Serializable]
    public class RegionDto : ChunkDto
    {
        [LabelText("区块数据")]
        public RegionDtoDef regionDtoDef;

        public override IChunkDtoDef CreateRuntimeDef()
        {
            return regionDtoDef.Clone();
        }
    }
}