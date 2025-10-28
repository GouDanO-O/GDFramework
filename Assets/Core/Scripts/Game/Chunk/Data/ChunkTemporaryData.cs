using System;
using Core.Game.Chunk.Data.Interface;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Data
{
    public class ChunkTemporaryData : TemporaryData,IChunkTemporaryData
    {
        [LabelText("实例ID"), ReadOnly]
        [InfoBox("每个实例的唯一标识,用于ES3存储")]
        public string InstanceId { get; set; }
        
        [LabelText("关联的配置ID"), ReadOnly]
        [InfoBox("指向ChunkDtoDef的DefId")]
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

        private string GenerateInstanceId()
        {
            return $"INST_{Guid.NewGuid().ToString("N").ToUpper()}";
        }
    }
}