using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;

namespace Core.Game.Chunk.Dungeon.Data
{
    public class DungeonDataModel : ChunkDataModel
    {
        private ChunkDataManager _dungeonDataManager;

        protected override IChunkDataManager CreateDataManager()
        {
            _dungeonDataManager = new ChunkDataManager();
            return _dungeonDataManager;
        }

        protected override void InitializeDataManager()
        {
            _dungeonDataManager.RegisterTypeFactory<DungeonData>(() => new DungeonData());
            LoadAllDefs();
        }

        protected override void LoadAllDefs()
        {
            // TODO: 从JSON加载Dungeon配置
        }

        public DungeonData CreateDungeon(string defId) => _dungeonDataManager.CreateInstance<DungeonData>(defId);
        public DungeonData LoadDungeon(string instanceId) => _dungeonDataManager.LoadInstance<DungeonData>(instanceId);
        public DungeonData GetDungeon(string instanceId) => _dungeonDataManager.GetInstance(instanceId) as DungeonData;
    }
}