using System;
using System.IO;
using Core.Game.Chunk.Data.Interface;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Core.Game.Chunk.Data
{
    [Serializable, JsonObject]
    public abstract class ChunkDtoDef : IChunkDtoDef
    {
        [LabelText("配置ID"), ReadOnly]
        [InfoBox("这是配置的唯一标识(在编辑器保存时,只会生成一次,")]
        public string DefId { get; protected set; }

        [LabelText("配置名称")]
        public string DefName { get; set; }

        [LabelText("配置描述")]
        public string DefDescription { get; set; }

        public ChunkDtoDef()
        {
            GenerateDefId();
        }

        private void GenerateDefId()
        {
            string typePrefix = GetTypePrefix();
            string uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            DefId = $"{typePrefix}_DEF_{uniqueId}";
        }

        public virtual void SaveThisDef()
        {
            
        }

        public virtual void DeleteThisDef()
        {
            
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
    }
}