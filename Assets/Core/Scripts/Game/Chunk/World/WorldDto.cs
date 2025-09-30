using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Region;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.World
{
    [CreateAssetMenu(fileName = "WorldDto", menuName = "Core/WorldDto")]
    public class WorldDto : ChunkDto
    {
        [LabelText("世界数据")]
        public WorldDtoDef worldDtoDef;
    }
}