using System;
using GDFrameworkExtend.Data;

namespace Core.Game.Chunk.Data.Interface
{
    /// <summary>
    /// 临时数据接口
    /// </summary>
    public interface IChunkTemporaryData : ITemporaryData
    {
        string InstanceId { get; set; }
        
        string DefId { get; set; }
        
        DateTime CreateTime { get; set; }
        
        DateTime LastModifyTime { get; set; }
    }
}