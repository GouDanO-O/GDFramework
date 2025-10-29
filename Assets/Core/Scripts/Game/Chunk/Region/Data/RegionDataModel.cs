using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;

namespace Core.Game.Chunk.Region.Data
{
    public class RegionDataModel : ChunkDataModel
    {
        private ChunkDataManager _regionDataManager;

        protected override IChunkDataManager CreateDataManager()
        {
            _regionDataManager = new ChunkDataManager();
            return _regionDataManager;
        }

        protected override void InitializeDataManager()
        {
            _regionDataManager.RegisterTypeFactory<RegionData>(() => new RegionData());
            LoadAllDefs();
        }

        protected override void LoadAllDefs()
        {
            // TODO: 从JSON加载Region配置
        }

        public RegionData CreateRegion(string defId) => _regionDataManager.CreateInstance<RegionData>(defId);
        public RegionData LoadRegion(string instanceId) => _regionDataManager.LoadInstance<RegionData>(instanceId);
        public RegionData GetRegion(string instanceId) => _regionDataManager.GetInstance(instanceId) as RegionData;
    }
}