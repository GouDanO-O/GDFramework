using System;
using Core.Game.Chunk.Data.Interface;
using GDFrameworkCore;
using GDFrameworkExtend.Data;

namespace Core.Game.Chunk.Data
{
    /// <summary>
    /// 世界中,所有可互动数据的父类
    /// </summary>
    public abstract class ChunkData : IChunkData
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
        /// 实例ID
        /// </summary>
        public string InstanceId => TemporaryData?.InstanceId ?? string.Empty;
        
        /// <summary>
        /// 配置ID
        /// </summary>
        public string DefId => DtoDef?.DefId ?? string.Empty;

        /// <summary>
        /// 从配置创建新实例
        /// </summary>
        public virtual void InitFromDef(IChunkDtoDef dtoDef)
        {
            if (dtoDef == null)
                throw new ArgumentNullException(nameof(dtoDef));
            
            DtoDef = dtoDef;
            TemporaryData = CreateTemporaryData(dtoDef.DefId);
            
            OnInitFromDef(dtoDef);
        }

        /// <summary>
        /// 从已有实例加载
        /// </summary>
        public virtual void InitFromInstanceId(string instanceId, IChunkDtoDef dtoDef)
        {
            if (string.IsNullOrEmpty(instanceId))
                throw new ArgumentException("实例ID不能为空", nameof(instanceId));
            
            if (dtoDef == null)
                throw new ArgumentNullException(nameof(dtoDef));
            
            DtoDef = dtoDef;
            
            if (HasTemporaryData(instanceId))
            {
                LoadTemporaryData(instanceId);
            }
            else
            {
                TemporaryData = CreateTemporaryData(dtoDef.DefId);
                TemporaryData.InstanceId = instanceId;
            }
            
            OnInitFromInstanceId(instanceId, dtoDef);
        }

        /// <summary>
        /// 创建临时数据实例(子类可覆盖)
        /// </summary>
        protected virtual IChunkTemporaryData CreateTemporaryData(string defId)
        {
            return new ChunkTemporaryData(defId);
        }

        /// <summary>
        /// 加载临时数据
        /// </summary>
        protected virtual void LoadTemporaryData(string instanceId)
        {
            if (ES3.KeyExists(instanceId))
            {
                // 子类需要覆盖此方法以加载具体类型
                TemporaryData = ES3.Load<ChunkTemporaryData>(instanceId);
                OnTemporaryDataLoaded(TemporaryData);
            }
        }

        /// <summary>
        /// 保存临时数据
        /// </summary>
        public virtual void SaveTemporaryData()
        {
            if (TemporaryData == null)
                return;
            
            if (TemporaryData is ChunkTemporaryData tempData)
            {
                tempData.LastModifyTime = DateTime.Now;
            }
            
            ES3.Save(TemporaryData.InstanceId, TemporaryData);
            OnTemporaryDataSaved(TemporaryData);
        }

        /// <summary>
        /// 删除临时数据
        /// </summary>
        public virtual void DeleteTemporaryData()
        {
            if (TemporaryData != null && ES3.KeyExists(TemporaryData.InstanceId))
            {
                ES3.DeleteKey(TemporaryData.InstanceId);
                OnTemporaryDataDeleted(TemporaryData);
            }
        }

        /// <summary>
        /// 判断是否存在临时数据
        /// </summary>
        public bool HasTemporaryData(string instanceId)
        {
            return !string.IsNullOrEmpty(instanceId) && ES3.KeyExists(instanceId);
        }

        // ============================================
        // 子类覆盖的回调方法
        // ============================================
        
        protected virtual void OnInitFromDef(IChunkDtoDef dtoDef) { }
        protected virtual void OnInitFromInstanceId(string instanceId, IChunkDtoDef dtoDef) { }
        protected virtual void OnTemporaryDataLoaded(IChunkTemporaryData temporaryData) { }
        protected virtual void OnTemporaryDataSaved(IChunkTemporaryData temporaryData) { }
        protected virtual void OnTemporaryDataDeleted(IChunkTemporaryData temporaryData) { }
    }
}