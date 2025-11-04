using System;
using Core.Game.Chunk.Data.Interface;
using Core.Game.Storage;
using GDFrameworkCore;
using GDFrameworkExtend.Data;

namespace Core.Game.Chunk.Data
{
    /// <summary>
    /// 世界中,所有可互动数据的父类
    /// </summary>
    public abstract class ChunkData : IChunkData,ICanGetSystem,ICanGetModel
    {
        /// <summary>
        /// 固定数据定义(配置)
        /// </summary>
        protected IChunkDtoDef DtoDef { get; set; }
        
        /// <summary>
        /// 临时数据(运行时状态)
        /// </summary>
        protected IChunkTemporaryData TemporaryData { get; set; }
        
        /// <summary>
        /// 配置ID--相当于实例ID
        /// 每个配置对应一个实例
        /// 配置中的数据除了defId其他的都可以相同
        /// </summary>
        public string DefId => DtoDef?.DefId ?? string.Empty;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }
        
        /// <summary>
        /// 初始化Chunk数据
        /// </summary>
        /// <param name="def"></param>
        /// <param name="instanceId"></param>
        public virtual void InitChunkData(IChunkDtoDef def)
        {
            SetDefData(def);
            SetTempData(def.DefId);
        }
        
        /// <summary>
        /// 设置def数据
        /// </summary>
        /// <param name="def"></param>
        public virtual void SetDefData(IChunkDtoDef def)
        {
            DtoDef = def;
        }

        /// <summary>
        /// 设置临时数据
        /// </summary>
        /// <param name="defId"></param>
        public virtual void SetTempData(string defId)
        {
            
        }

        /// <summary>
        /// 保存临时数据
        /// </summary>
        public void SaveTemporaryData()
        {
            if(DefId == string.Empty)
                return;
            this.GetSystem<StorageSystem>().SaveTemporaryData(TemporaryData);
        }

        /// <summary>
        /// 删除临时数据
        /// </summary>
        public void DeleteTemporaryData()
        {
            if(DefId == string.Empty)
                return;
            this.GetSystem<StorageSystem>().DeleteTemporaryData(DefId);
        }
    }
}