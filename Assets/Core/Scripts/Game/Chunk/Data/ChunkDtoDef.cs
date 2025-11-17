using System;
using System.Collections.Generic;
using System.IO;
using Core.Game.Chunk.Data.Interface;
using Core.Game.Storage;
using GDFrameworkCore;
using GDFrameworkExtend.JsonKit;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Data
{
    [Serializable, JsonObject]
    public abstract class ChunkDtoDef : IChunkDtoDef,ICanGetSystem,ITrackableData
    {
        [LabelText("配置ID")]
        [InfoBox("这是配置的唯一标识(在编辑器保存时,只会生成一次,")]
        public string DefId { get; set; }

        [LabelText("配置名称")]
        public string DefName { get; set; }

        [LabelText("配置描述")]
        public string DefDescription { get; set; }
        
        [LabelText("一开始是否处于解锁状态")]
        public bool IsLockInInitial { get; set; }

        [LabelText("初始定位区块ID")]
        public string PlayerInitialLocateChildDtoDefId { get; set; }

        [LabelText("一开始就展示的子配置ID(解锁状态)")]
        public List<string> InitialShowChildDtoDefId { get; set; }

        [LabelText("拥有的子配置ID")]
        public List<string> OwnedChildDtoDefID { get; set; }

        public IArchitecture GetArchitecture()
        {
            return GameMain.Interface;
        }

        public void GenerateDefId()
        {
            string typePrefix = GetTypePrefix();
            string uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            DefId = $"{typePrefix}_DEF_{uniqueId}";
        }

        public virtual void SaveThisDef()
        {
            this.GetSystem<StorageSystem>().SaveDef(this);
        }

        public virtual void DeleteThisDef()
        {
            this.GetSystem<StorageSystem>().DeleteDef(this);
        }

        public abstract string GetTypePrefix();

        /// <summary>
        /// 获取文件名
        /// </summary>
        protected virtual string GetFileName()
        {
            return $"{DefId}.json";
        }

        public virtual bool Validate(out string error)
        {
            if (string.IsNullOrEmpty(DefId))
            {
                error = "配置ID不能为空";
                return false;
            }

            error = string.Empty;
            return true;
        }

        /// <summary>
        /// 创建当前数据的快照
        /// </summary>
        public string CreateSnapshot()
        {
            return JsonConvert.SerializeObject(this, JsonSettings.Compact);
        }

        /// <summary>
        /// 与快照比较是否有变化
        /// </summary>
        public bool HasChanges(string snapshot)
        {
            if (string.IsNullOrEmpty(snapshot))
                return true;
                
            string currentSnapshot = CreateSnapshot();
            return currentSnapshot != snapshot;
        }
    }
}