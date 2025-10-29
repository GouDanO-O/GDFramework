using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
using Core.Game.Chunk.Region;
using Core.Game.Chunk.Region.Data;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;
using UnityEngine.Analytics;

namespace Core.Game.Chunk.World.Data
{
    public class WorldData : ChunkContainerData
    {
        public WorldDtoDef WorldDef => DtoDef as WorldDtoDef;
        public WorldTemporaryData WorldTempData => TemporaryData as WorldTemporaryData;
        
        protected override IChunkTemporaryData CreateTemporaryData(string defId)
        {
            return new WorldTemporaryData(defId);
        }
        
        protected override IChunkTemporaryData LoadTemporaryDataFromES3(string instanceId)
        {
            return ES3.Load<WorldTemporaryData>(instanceId);
        }

        public void AddRegion(string regionInstanceId) => AddChild(regionInstanceId);
        public void RemoveRegion(string regionInstanceId) => RemoveChild(regionInstanceId);
        public List<string> GetAllRegionIds() => GetAllChildIds();
        public void SetActiveRegion(string regionInstanceId) => SetActiveChild(regionInstanceId);
        public string GetActiveRegionId() => GetActiveChildId();

        public void Activate()
        {
            WorldTempData.IsActive = true;
            SaveTemporaryData();
        }

        public void Deactivate()
        {
            WorldTempData.IsActive = false;
            SaveTemporaryData();
        }
    }
}