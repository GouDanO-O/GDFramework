using System;
using System.Collections.Generic;
using Core.Game.Chunk.Data;
using Core.Game.Chunk.Data.Interface;
using Core.Game.Chunk.World.Data;
using UnityEngine;

namespace Core.Game.Chunk.Universe.Data
{
    public class UniverseData : ChunkContainerData
    {
        /// <summary>
        /// 获取宇宙配置定义
        /// </summary>
        public UniverseDtoDef UniverseDef => DtoDef as UniverseDtoDef;
        
        /// <summary>
        /// 获取宇宙临时数据
        /// </summary>
        public UniverseTemporaryData UniverseTempData => TemporaryData as UniverseTemporaryData;

        protected override IChunkTemporaryData CreateNewTemporaryData()
        {
            return new UniverseTemporaryData(DefId);
        }

        protected override Type GetTemporaryDataType()
        {
            return typeof(UniverseTemporaryData);
        }

        #region World Management

        /// <summary>
        /// 获取宇宙中的所有世界DefId列表
        /// </summary>
        public List<string> GetAllWorldDefIds()
        {
            return UniverseTempData?.ActiveWorldDefIds ?? new List<string>();
        }

        /// <summary>
        /// 获取当前激活的世界DefId
        /// </summary>
        public string GetCurrentWorldDefId()
        {
            return UniverseTempData?.CurrentWorldDefId ?? string.Empty;
        }

        /// <summary>
        /// 添加世界到宇宙
        /// </summary>
        public override void AddChild(string worldDefId)
        {
            if (UniverseTempData == null)
            {
                Debug.LogError("UniverseTempData 为空,无法添加世界");
                return;
            }

            if (!UniverseTempData.ActiveWorldDefIds.Contains(worldDefId))
            {
                UniverseTempData.ActiveWorldDefIds.Add(worldDefId);
                
                if (string.IsNullOrEmpty(UniverseTempData.CurrentWorldDefId))
                {
                    UniverseTempData.CurrentWorldDefId = worldDefId;
                }
                
                SaveTemporaryData();
                Debug.Log($"添加世界到宇宙: {worldDefId}");
            }
        }

        /// <summary>
        /// 从宇宙移除世界
        /// </summary>
        public override void RemoveChild(string worldDefId)
        {
            if (UniverseTempData == null)
            {
                Debug.LogError("UniverseTempData 为空,无法移除世界");
                return;
            }

            if (UniverseTempData.ActiveWorldDefIds.Remove(worldDefId))
            {
                if (UniverseTempData.CurrentWorldDefId == worldDefId)
                {
                    UniverseTempData.CurrentWorldDefId = string.Empty;
                    
                    if (UniverseTempData.ActiveWorldDefIds.Count > 0)
                    {
                        UniverseTempData.CurrentWorldDefId = UniverseTempData.ActiveWorldDefIds[0];
                    }
                }
                
                SaveTemporaryData();
                Debug.Log($"从宇宙移除世界: {worldDefId}");
            }
        }

        /// <summary>
        /// 设置当前激活的世界
        /// </summary>
        public override void SetActiveChild(string worldDefId)
        {
            if (UniverseTempData == null)
            {
                Debug.LogError("UniverseTempData 为空,无法设置激活世界");
                return;
            }

            if (!UniverseTempData.ActiveWorldDefIds.Contains(worldDefId))
            {
                Debug.LogWarning($"世界 {worldDefId} 不属于这个宇宙");
                return;
            }

            if (!string.IsNullOrEmpty(UniverseTempData.CurrentWorldDefId))
            {
                UniverseTempData.LastFocusWorldDefId = UniverseTempData.CurrentWorldDefId;
            }

            UniverseTempData.CurrentWorldDefId = worldDefId;
            SaveTemporaryData();
            
            Debug.Log($"切换当前世界: {worldDefId}");
        }

        #endregion
        

    }
}