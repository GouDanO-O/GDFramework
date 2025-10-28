using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;

namespace Core.Game.Chunk.Universe.Data
{
    public class UniverseData : ChunkData
    {
        public UniverseDtoDef UniverseDef => DtoDef as UniverseDtoDef;
        public UniverseTemporaryData UniverseTempData => TemporaryData as UniverseTemporaryData;
        
        protected override IChunkTemporaryData CreateTemporaryData(string defId)
        {
            return new UniverseTemporaryData 
            { 
                DefId = defId,
                WorldInstanceIds = new List<string>()
            };
        }
        
        protected override void LoadTemporaryData(string instanceId)
        {
            if (ES3.KeyExists(instanceId))
            {
                TemporaryData = ES3.Load<UniverseTemporaryData>(instanceId);
                OnTemporaryDataLoaded(TemporaryData);
            }
        }

        public void AddWorld(string worldInstanceId)
        {
            if (!UniverseTempData.WorldInstanceIds.Contains(worldInstanceId))
            {
                UniverseTempData.WorldInstanceIds.Add(worldInstanceId);
                SaveTemporaryData();
            }
        }

        public void RemoveWorld(string worldInstanceId)
        {
            if (UniverseTempData.WorldInstanceIds.Contains(worldInstanceId))
            {
                UniverseTempData.WorldInstanceIds.Remove(worldInstanceId);
                SaveTemporaryData();
            }
        }

        public void SetCurrentWorld(string worldInstanceId)
        {
            UniverseTempData.CurrentWorldInstanceId = worldInstanceId;
            SaveTemporaryData();
        }
    }
}