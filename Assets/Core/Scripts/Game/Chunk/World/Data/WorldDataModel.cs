using System.Collections.Generic;
using System.Linq;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;

namespace Core.Game.Chunk.World.Data
{
    public class WorldDataModel : ChunkDataModel
    {
        private ChunkDataManager _worldDataManager;

        protected override IChunkDataManager CreateDataManager()
        {
            _worldDataManager = new ChunkDataManager();
            return _worldDataManager;
        }

        protected override void InitializeDataManager()
        {
            _worldDataManager.RegisterTypeFactory<WorldData>(() => new WorldData());
            LoadAllDefs();
        }

        protected override void LoadAllDefs()
        {
            // TODO: 从JSON加载World配置
        }

        public WorldData CreateWorld(string defId) => _worldDataManager.CreateInstance<WorldData>(defId);
        public WorldData LoadWorld(string instanceId) => _worldDataManager.LoadInstance<WorldData>(instanceId);
        public WorldData GetWorld(string instanceId) => _worldDataManager.GetInstance(instanceId) as WorldData;
        public List<WorldData> GetAllWorlds() => _worldDataManager.GetAllInstanceIds().Select(id => GetWorld(id)).ToList();
    }
}