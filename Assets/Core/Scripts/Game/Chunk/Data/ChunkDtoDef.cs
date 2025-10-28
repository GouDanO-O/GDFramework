using System;
using Core.Game.Chunk.Data.Interface;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Data
{
    [Serializable]
    public abstract class ChunkDtoDef : IChunkDtoDef
    {
        [LabelText("配置ID"), ReadOnly]
        [InfoBox("这是配置的唯一标识,多个实例可以共享同一个配置")]
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
        
        protected abstract string GetTypePrefix();

        public virtual IChunkDtoDef Clone()
        {
            string json = JsonConvert.SerializeObject(this, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            return JsonConvert.DeserializeObject<ChunkDtoDef>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
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