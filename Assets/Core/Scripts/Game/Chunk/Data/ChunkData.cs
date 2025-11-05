using System;
using Core.Game.Chunk.Data.Interface;
using Core.Game.Storage;
using GDFrameworkCore;

namespace Core.Game.Chunk.Data
{
    /// <summary>
    /// 区块数据
    /// </summary>
    public abstract class ChunkData : IChunkData, ICanGetSystem, ICanGetModel
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
        /// 配置ID (就是唯一标识,不再需要 InstanceId)
        /// </summary>
        public string DefId => DtoDef?.DefId ?? string.Empty;

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }
        
        /// <summary>
        /// 初始化Chunk数据
        /// DefId 就是唯一标识,直接用来查找临时数据
        /// </summary>
        public virtual void InitChunkData(IChunkDtoDef def)
        {
            SetDefData(def);
            SetTempData();
        }
        
        /// <summary>
        /// 设置def数据
        /// </summary>
        public virtual void SetDefData(IChunkDtoDef def)
        {
            DtoDef = def;
        }

        /// <summary>
        /// 设置临时数据
        /// 直接使用 DefId 查找
        /// </summary>
        public virtual void SetTempData()
        {
            if (string.IsNullOrEmpty(DefId))
            {
                UnityEngine.Debug.LogError("DefId 为空,无法设置临时数据");
                return;
            }

            var storageSystem = this.GetSystem<StorageSystem>();
            
            // 尝试加载已存在的临时数据
            var tempData = storageSystem?.LoadTemporaryData(DefId, GetTemporaryDataType());
            
            if (tempData != null)
            {
                TemporaryData = tempData;
                UnityEngine.Debug.Log($"加载临时数据: DefId={DefId}");
            }
            else
            {
                // 创建新的临时数据
                TemporaryData = CreateNewTemporaryData();
                SaveTemporaryData();
                UnityEngine.Debug.Log($"创建新临时数据: DefId={DefId}");
            }
        }

        /// <summary>
        /// 创建新的临时数据 (子类实现)
        /// </summary>
        protected abstract IChunkTemporaryData CreateNewTemporaryData();

        /// <summary>
        /// 获取临时数据类型 (子类实现)
        /// </summary>
        protected abstract Type GetTemporaryDataType();

        /// <summary>
        /// 保存临时数据
        /// </summary>
        public void SaveTemporaryData()
        {
            if (string.IsNullOrEmpty(DefId))
            {
                UnityEngine.Debug.LogWarning("DefId 为空,无法保存临时数据");
                return;
            }

            if (TemporaryData == null)
            {
                UnityEngine.Debug.LogWarning($"临时数据为空,无法保存: {DefId}");
                return;
            }

            this.GetSystem<StorageSystem>().SaveTemporaryData(DefId, TemporaryData);
        }

        /// <summary>
        /// 删除临时数据
        /// </summary>
        public void DeleteTemporaryData()
        {
            if (string.IsNullOrEmpty(DefId))
            {
                UnityEngine.Debug.LogWarning("DefId 为空,无法删除临时数据");
                return;
            }

            this.GetSystem<StorageSystem>().DeleteTemporaryData(DefId);
        }
    }
}