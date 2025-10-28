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
        private UniverseDataManager _universeDataManager;

        protected override IChunkDataManager CreateDataManager()
        {
            _universeDataManager = new UniverseDataManager();
            return _universeDataManager;
        }

        protected override void InitializeDataManager()
        {
            // 注册类型工厂
            _universeDataManager.RegisterTypeFactory<UniverseData>(() => new UniverseData());
            
            // 加载配置
            LoadAllDefs();
        }

        protected override void LoadAllDefs()
        {
            // TODO: 从JSON加载所有宇宙配置
            // 示例:
            // var universeDefs = LoadUniverseDefsFromJson();
            // DataManager.RegisterDefs(universeDefs);
        }

        /// <summary>
        /// 创建新宇宙实例
        /// </summary>
        public UniverseData CreateUniverse(string defId)
        {
            return _universeDataManager.CreateInstance<UniverseData>(defId);
        }

        /// <summary>
        /// 加载宇宙实例
        /// </summary>
        public UniverseData LoadUniverse(string instanceId)
        {
            return _universeDataManager.LoadInstance<UniverseData>(instanceId);
        }

        /// <summary>
        /// 获取当前激活的宇宙
        /// </summary>
        public UniverseData GetActiveUniverse()
        {

            return null;
        }
    }
}