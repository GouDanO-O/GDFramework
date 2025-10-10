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
        /// 玩家初始DTO
        /// </summary>
        protected ChunkDto InitialPlayerLocateChunk;
        
        /// <summary>
        /// 初始展示Dto列表
        /// </summary>
        protected List<ChunkDto> InitialShowingDtoReferenceList = new List<ChunkDto>();
        
        /// <summary>
        /// Dto列表
        /// </summary>
        protected List<ChunkDto> DtoReferenceList = new List<ChunkDto>();
        
        /// <summary>
        /// 当数据改变时
        /// </summary>
        protected virtual void ChangingDtoData()
        {
            
        }

        #region DTO管理

        public virtual void SetInitialPlayerLocateChunk(ChunkDto dto)
        {
            InitialPlayerLocateChunk = dto;
            UpdateDtoDef();
        }
        
        /// <summary>
        /// 添加初始展示DTO
        /// </summary>
        /// <param name="dtoReference"></param>
        public virtual void AddInitialShowingDtoReference(ChunkDto dtoReference)
        {
            if (InitialShowingDtoReferenceList.Contains(dtoReference) == false)
            {
                InitialShowingDtoReferenceList.Add(dtoReference);
                UpdateDtoDef();
            }

        }

        /// <summary>
        /// 移除初始展示DTO
        /// </summary>
        /// <param name="dtoReference"></param>
        public virtual void RemoveInitialShowingDtoReference(ChunkDto dtoReference)
        {
            if (InitialShowingDtoReferenceList.Contains(dtoReference))
            {
                InitialShowingDtoReferenceList.Remove(dtoReference);
                UpdateDtoDef();
            }
        }

        /// <summary>
        /// 清空初始展示DTO
        /// </summary>
        public virtual void ClearInitialShowingDtoReference()
        {
            InitialShowingDtoReferenceList.Clear();
            UpdateDtoDef();
        }
        
        /// <summary>
        /// 添加所拥有的DTO
        /// </summary>
        /// <param name="dtoReference"></param>
        public virtual void AddDtoReference(ChunkDto dtoReference)
        {
            if (DtoReferenceList.Contains(dtoReference) == false)
            {
                DtoReferenceList.Add(dtoReference);
                UpdateDtoDef();
            }
            UpdateDtoDef();
        }

        /// <summary>
        /// 移除所拥有的DTO
        /// </summary>
        /// <param name="dtoReference"></param>
        public virtual void RemoveDtoReference(ChunkDto dtoReference)
        {
            if (DtoReferenceList.Contains(dtoReference))
            {
                DtoReferenceList.Remove(dtoReference);
                UpdateDtoDef();
            }
        }

        /// <summary>
        /// 清空所拥有的DTO
        /// </summary>
        public virtual void ClearDtoReference()
        {
            DtoReferenceList.Clear();
            UpdateDtoDef();
        }
        
        /// <summary>
        /// 更新DtoDef中的数据项
        /// </summary>
        public virtual void UpdateDtoDef()
        {
            
        }

        #endregion
        
        /// <summary>
        /// 创建运行时数据实例(核心方法)
        /// </summary>
        public abstract ChunkDtoDef CreateRuntimeDef();
    }
}