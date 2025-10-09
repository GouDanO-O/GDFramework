using Core.Game.Chunk.Data;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Universe.Data
{
    [CreateAssetMenu(fileName = "WorldDto", menuName = "Core/UniverseDto")]
    public class UniverseDto : ChunkDto
    {
        [LabelText("宇宙数据Def")]
        public UniverseDtoDef universeDtoDef;
    }
}