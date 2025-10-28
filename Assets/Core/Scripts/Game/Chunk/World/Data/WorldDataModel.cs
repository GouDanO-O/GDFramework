using System.Collections.Generic;
using System.Linq;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;

namespace Core.Game.Chunk.World.Data
{
    public class WorldDataModel : ChunkDataModel
    {
        private WorldDataManager _worldDataManager;

        protected override IChunkDataManager CreateDataManager()
        {
            _worldDataManager = new WorldDataManager();
            return _worldDataManager;
        }

        protected override void InitializeDataManager()
        {
            // 注册类型工厂
            _worldDataManager.RegisterTypeFactory<WorldData>(() => new WorldData());

            // 加载配置
            LoadAllDefs();
        }

        protected override void LoadAllDefs()
        {
            // TODO: 从JSON加载所有世界配置
        }

        /// <summary>
        /// 创建新世界
        /// </summary>
        public WorldData CreateWorld(string defId)
        {
            return _worldDataManager.CreateInstance<WorldData>(defId);
        }

        /// <summary>
        /// 加载世界
        /// </summary>
        public WorldData LoadWorld(string instanceId)
        {
            return _worldDataManager.LoadInstance<WorldData>(instanceId);
        }

        /// <summary>
        /// 获取所有世界实例
        /// </summary>
        public List<WorldData> GetAllWorlds()
        {
            return _worldDataManager.InstanceRegistry.Values
                .Cast<WorldData>()
                .ToList();
        }
    }
}