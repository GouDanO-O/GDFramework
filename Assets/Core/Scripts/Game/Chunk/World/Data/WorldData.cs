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
    public class WorldData : ChunkData
    {
        [LabelText("当前世界的固定数据")]
        public WorldDtoDef WorldDef => DtoDef as WorldDtoDef;
        
        [LabelText("当前世界的临时数据")]
        public WorldTemporaryData WorldTempData => TemporaryData as WorldTemporaryData;
        
        [LabelText("当前世界的区块数据")]
        private Dictionary<string, RegionData> _curHoldingRegionDtoDict = new Dictionary<string, RegionData>();
        
        protected override IChunkTemporaryData CreateTemporaryData(string defId)
        {
            return new WorldTemporaryData 
            { 
                DefId = defId,
                RegionInstanceIds = new List<string>(),
                IsActive = false
            };
        }
        
        protected override void LoadTemporaryData(string instanceId)
        {
            if (ES3.KeyExists(instanceId))
            {
                TemporaryData = ES3.Load<WorldTemporaryData>(instanceId);
                OnTemporaryDataLoaded(TemporaryData);
            }
        }

        public void AddRegion(string regionInstanceId)
        {
            if (!WorldTempData.RegionInstanceIds.Contains(regionInstanceId))
            {
                WorldTempData.RegionInstanceIds.Add(regionInstanceId);
                SaveTemporaryData();
            }
        }

        public void RemoveRegion(string regionInstanceId)
        {
            if (WorldTempData.RegionInstanceIds.Contains(regionInstanceId))
            {
                WorldTempData.RegionInstanceIds.Remove(regionInstanceId);
                SaveTemporaryData();
            }
        }

        public void SetCurrentRegion(string regionInstanceId)
        {
            WorldTempData.CurrentRegionInstanceId = regionInstanceId;
            SaveTemporaryData();
        }

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