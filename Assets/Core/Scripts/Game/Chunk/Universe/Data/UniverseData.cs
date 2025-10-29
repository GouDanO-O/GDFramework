using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
using Core.Game.Chunk.World.Data;

namespace Core.Game.Chunk.Universe.Data
{
    public class UniverseData : ChunkContainerData
    {
        public UniverseDtoDef UniverseDef => DtoDef as UniverseDtoDef;
        public UniverseTemporaryData UniverseTempData => TemporaryData as UniverseTemporaryData;
        
        protected override IChunkTemporaryData CreateTemporaryData(string defId)
        {
            return new UniverseTemporaryData(defId);
        }
        
        protected override IChunkTemporaryData LoadTemporaryDataFromES3(string instanceId)
        {
            return ES3.Load<UniverseTemporaryData>(instanceId);
        }

        // 便捷方法
        public void AddWorld(string worldInstanceId) => AddChild(worldInstanceId);
        public void RemoveWorld(string worldInstanceId) => RemoveChild(worldInstanceId);
        public List<string> GetAllWorldIds() => GetAllChildIds();
        public void SetActiveWorld(string worldInstanceId) => SetActiveChild(worldInstanceId);
        public string GetActiveWorldId() => GetActiveChildId();
    }
}