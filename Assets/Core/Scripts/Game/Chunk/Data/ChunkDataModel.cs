using System;
using Core.Game.Chunk.Data.Interface;
using GDFrameworkCore;
using GDFrameworkExtend.Data;

namespace Core.Game.Chunk.Data
{
    public abstract class ChunkDataModel : AbstractModel
    {
        protected IChunkDataManager DataManager { get; private set; }

        protected override void OnInit()
        {
            DataManager = CreateDataManager();
            InitializeDataManager();
        }

        /// <summary>
        /// 创建数据管理器(子类实现)
        /// </summary>
        protected abstract IChunkDataManager CreateDataManager();

        /// <summary>
        /// 初始化数据管理器(注册配置、工厂等)
        /// </summary>
        protected abstract void InitializeDataManager();

        /// <summary>
        /// 加载所有配置数据(从JSON)
        /// </summary>
        protected abstract void LoadAllDefs();

        /// <summary>
        /// 保存所有实例数据
        /// </summary>
        public virtual void SaveAll()
        {
            DataManager?.SaveAllInstances();
        }

        /// <summary>
        /// 清理所有实例
        /// </summary>
        public virtual void ClearAll()
        {
            DataManager?.ClearInstances();
        }
    }
}