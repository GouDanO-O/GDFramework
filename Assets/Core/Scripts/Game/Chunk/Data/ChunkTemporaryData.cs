using System;
using Core.Game.Chunk.Data.Interface;
using GDFrameworkExtend.Data;
using Sirenix.OdinInspector;

namespace Core.Game.Chunk.Data
{
    public class ChunkTemporaryData : IChunkTemporaryData
    {
        [LabelText("关联的配置ID")]
        public string DefId { get; set; }
        
        [LabelText("创建时间")]
        public DateTime CreateTime { get; set; }
        
        [LabelText("最后修改时间")]
        public DateTime LastModifyTime { get; set; }

        public ChunkTemporaryData()
        {
            CreateTime = DateTime.Now;
            LastModifyTime = DateTime.Now;
        }

        public ChunkTemporaryData(string defId) : this()
        {
            DefId = defId;
        }
    }
}