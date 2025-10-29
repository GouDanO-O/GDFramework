using System.Collections.Generic;

namespace Core.Game.Chunk.Data.Interface
{
    /// <summary>
    /// 可包含子级的临时数据接口
    /// </summary>
    public interface IChunkContainerTemporaryData : IChunkTemporaryData
    {
        /// <summary>
        /// 子级实例ID列表
        /// </summary>
        List<string> ChildInstanceIds { get; set; }
        
        /// <summary>
        /// 当前激活的子级实例ID
        /// </summary>
        string ActiveChildInstanceId { get; set; }
    }
}