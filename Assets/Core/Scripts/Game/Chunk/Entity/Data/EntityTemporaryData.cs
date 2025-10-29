using System;
using Core.Game.Chunk.Data;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Substance.Data
{
    [Serializable]
    public class EntityTemporaryData : ChunkTemporaryData
    {
        public int Health;
        public int MaxHealth;
        public bool IsDestroyed;
        
        public EntityTemporaryData() : base() { }
        public EntityTemporaryData(string defId) : base(defId) { }
    }
}