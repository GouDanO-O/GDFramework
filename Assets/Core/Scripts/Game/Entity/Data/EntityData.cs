using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
using Core.Game.Chunk.Substance.Interface;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Substance.Data
{
    /// <summary>
    /// 世界中,一切可以进行互动的物体
    /// 人,也是一个物体,只不过他的行为会比物体会更复杂
    /// </summary>
    public abstract class EntityData : IEntityData
    {
        public EntityDtoDef DtoDef { get; set; }
        
        public EntityTemporaryData TemporaryData { get; set; }
        
    }
}