using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
namespace Core.Game.Chunk.Region.Data
{
    public class RegionData : ChunkContainerData
    {
        public RegionDtoDef RegionDef => DtoDef as RegionDtoDef;
        public RegionTemporaryData RegionTempData => TemporaryData as RegionTemporaryData;
        
        protected override IChunkTemporaryData CreateTemporaryData(string defId)
        {
            return new RegionTemporaryData(defId);
        }
        
        protected override IChunkTemporaryData LoadTemporaryDataFromES3(string instanceId)
        {
            return ES3.Load<RegionTemporaryData>(instanceId);
        }

        public void AddDungeon(string dungeonInstanceId) => AddChild(dungeonInstanceId);
        public void RemoveDungeon(string dungeonInstanceId) => RemoveChild(dungeonInstanceId);
        public List<string> GetAllDungeonIds() => GetAllChildIds();
        public void SetActiveDungeon(string dungeonInstanceId) => SetActiveChild(dungeonInstanceId);
        public string GetActiveDungeonId() => GetActiveChildId();

        public void Load()
        {
            SaveTemporaryData();
        }

        public void Unload()
        {
            SaveTemporaryData();
        }
    }
}