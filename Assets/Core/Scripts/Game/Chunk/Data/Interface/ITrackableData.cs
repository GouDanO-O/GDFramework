namespace Core.Game.Chunk.Data.Interface
{
    /// <summary>
    /// 支持变更追踪的数据接口
    /// </summary>
    public interface ITrackableData
    {
        /// <summary>
        /// 创建当前数据的快照
        /// </summary>
        string CreateSnapshot();
        
        /// <summary>
        /// 与快照比较是否有变化
        /// </summary>
        bool HasChanges(string snapshot);
    }
}