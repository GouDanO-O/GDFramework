using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Room;
using Core.Game.Chunk.World;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Region.Data
{
    [CreateAssetMenu(fileName = "RegionDto", menuName = "Core/RegionDto")]
    public class RegionDto : ChunkDto
    {
        [LabelText("区块数据")]
        public RegionDtoDef regionDtoDef;
    }
}