using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data.Interface;
using Sirenix.OdinInspector;
using Object = UnityEngine.Object;

namespace Core.Game.Chunk.Data
{
    [Serializable]
    public abstract class ChunkDto : IChunkDto
    {
        /// <summary>
        /// 玩家初始定位Chunk
        /// </summary>
        [LabelText("玩家初始位置")]
        protected IChunkDto InitialPlayerLocateChunk;
        
        /// <summary>
        /// 初始展示Dto列表
        /// </summary>
        [LabelText("初始展示列表")]
        protected List<IChunkDto> InitialShowingDtoReferenceList = new List<IChunkDto>();
        
        /// <summary>
        /// Dto引用列表
        /// </summary>
        [LabelText("Dto引用列表")]
        protected List<IChunkDto> DtoReferenceList = new List<IChunkDto>();
        
        /// <summary>
        /// 当数据改变时
        /// </summary>
        protected virtual void OnDtoDataChanged()
        {
            
        }

        #region DTO管理

        public virtual void SetInitialPlayerLocateChunk(IChunkDto dto)
        {
            InitialPlayerLocateChunk = dto;
            UpdateDtoDef();
        }
        
        public virtual void AddInitialShowingDtoReference(IChunkDto dtoReference)
        {
            if (!InitialShowingDtoReferenceList.Contains(dtoReference))
            {
                InitialShowingDtoReferenceList.Add(dtoReference);
                UpdateDtoDef();
            }
        }

        public virtual void RemoveInitialShowingDtoReference(IChunkDto dtoReference)
        {
            if (InitialShowingDtoReferenceList.Contains(dtoReference))
            {
                InitialShowingDtoReferenceList.Remove(dtoReference);
                UpdateDtoDef();
            }
        }

        public virtual void ClearInitialShowingDtoReference()
        {
            InitialShowingDtoReferenceList.Clear();
            UpdateDtoDef();
        }
        
        public virtual void AddDtoReference(IChunkDto dtoReference)
        {
            if (!DtoReferenceList.Contains(dtoReference))
            {
                DtoReferenceList.Add(dtoReference);
                UpdateDtoDef();
            }
        }

        public virtual void RemoveDtoReference(IChunkDto dtoReference)
        {
            if (DtoReferenceList.Contains(dtoReference))
            {
                DtoReferenceList.Remove(dtoReference);
                UpdateDtoDef();
            }
        }

        public virtual void ClearDtoReference()
        {
            DtoReferenceList.Clear();
            UpdateDtoDef();
        }
        
        protected virtual void UpdateDtoDef()
        {
            OnDtoDataChanged();
        }

        #endregion
        
        /// <summary>
        /// 创建运行时数据定义
        /// </summary>
        public abstract IChunkDtoDef CreateRuntimeDef();
    }
}