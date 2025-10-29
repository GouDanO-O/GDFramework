using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Substance.Data
{
    /// <summary>
    /// 世界中,一切可以进行互动的物体
    /// 人,也是一个物体,只不过他的行为会比物体会更复杂
    /// </summary>
    public class EntityData : ChunkData
    {
        public EntityDtoDef EntityDef => DtoDef as EntityDtoDef;
        public EntityTemporaryData EntityTempData => TemporaryData as EntityTemporaryData;
        
        protected override IChunkTemporaryData CreateTemporaryData(string defId)
        {
            return new EntityTemporaryData(defId) 
            { 
                Health = 100,
                MaxHealth = 100,
                IsDestroyed = false
            };
        }
        
        protected override IChunkTemporaryData LoadTemporaryDataFromES3(string instanceId)
        {
            return ES3.Load<EntityTemporaryData>(instanceId);
        }

        public void TakeDamage(int damage)
        {
            EntityTempData.Health -= damage;
            if (EntityTempData.Health <= 0)
            {
                EntityTempData.Health = 0;
                EntityTempData.IsDestroyed = true;
            }
            SaveTemporaryData();
        }

        public void Repair(int amount)
        {
            if (!EntityTempData.IsDestroyed)
            {
                EntityTempData.Health = Mathf.Min(EntityTempData.Health + amount, EntityTempData.MaxHealth);
                SaveTemporaryData();
            }
        }
    }
}