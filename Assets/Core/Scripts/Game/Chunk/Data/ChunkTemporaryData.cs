using System;
using Core.Game.Chunk.Data.Interface;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Data
{
    public class ChunkTemporaryData : IChunkTemporaryData
    {
        [LabelText("实例ID"), ReadOnly]
        public string InstanceId { get; set; }
        
        [LabelText("关联的配置ID"), ReadOnly]
        public string DefId { get; set; }
        
        [LabelText("创建时间")]
        public DateTime CreateTime { get; set; }
        
        [LabelText("最后修改时间")]
        public DateTime LastModifyTime { get; set; }

        public ChunkTemporaryData()
        {
            InstanceId = GenerateInstanceId();
            CreateTime = DateTime.Now;
            LastModifyTime = DateTime.Now;
        }

        public ChunkTemporaryData(string defId) : this()
        {
            DefId = defId;
        }

        protected virtual string GenerateInstanceId()
        {
            return $"INST_{Guid.NewGuid().ToString("N").ToUpper()}";
        }
    }
}