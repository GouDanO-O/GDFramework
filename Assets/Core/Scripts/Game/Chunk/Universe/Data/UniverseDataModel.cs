using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
using Core.Game.Chunk.World;
using Core.Game.Chunk.World.Data;
using GDFrameworkCore;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Universe.Data
{
    public class UniverseDataModel : ChunkDataModel
    {
        private ChunkDataManager _universeDataManager;

        protected override IChunkDataManager CreateDataManager()
        {
            _universeDataManager = new ChunkDataManager();
            return _universeDataManager;
        }

        protected override void InitializeDataManager()
        {
            _universeDataManager.RegisterTypeFactory<UniverseData>(() => new UniverseData());
            LoadAllDefs();
        }

        protected override void LoadAllDefs()
        {
            // TODO: 从JSON加载Universe配置
        }

        public UniverseData CreateUniverse(string defId) => _universeDataManager.CreateInstance<UniverseData>(defId);
        public UniverseData LoadUniverse(string instanceId) => _universeDataManager.LoadInstance<UniverseData>(instanceId);
        public UniverseData GetUniverse(string instanceId) => _universeDataManager.GetInstance(instanceId) as UniverseData;
    }
}