using System;
using Core.Game.Chunk.Data.Interface;
using Newtonsoft.Json;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Data
{
    [Serializable]
    public abstract class ChunkDtoDef : IChunkDtoDef
    {
        [LabelText("昵称")]
        public string chunkDtoDefName;
        
        [LabelText("描述")]
        public string chunkDtoDefDescription;

        [LabelText("ID"),ReadOnly]
        [InfoBox("每个Chunk的数据可能雷同,但是ID一定不能也不会雷同")]
        public string chunkDtoDefId;

        public ChunkDtoDef()
        {
            GenerateChunkDtoDefId();
        }

        private void GenerateChunkDtoDefId()
        {
            string typePrefix = GetTypePrefix();
            string uniqueId = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();
            chunkDtoDefId = $"{typePrefix}_{uniqueId}";
        }
        
        protected abstract string GetTypePrefix();

        /// <summary>
        /// 深拷贝
        /// </summary>
        public virtual ChunkDtoDef Clone()
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
        
        /// <summary>
        /// 数据验证
        /// </summary>
        public virtual bool Validate(out string error)
        {
            if (string.IsNullOrEmpty(chunkDtoDefId))
            {
                error = "ID不能为空";
                return false;
            }
            error = string.Empty;
            return true;
        }
    }
}