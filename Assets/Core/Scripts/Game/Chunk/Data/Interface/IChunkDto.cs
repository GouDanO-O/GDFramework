namespace Core.Game.Chunk.Data.Interface
{
    /// <summary>
    /// 固定数据传输对象接口
    /// 用于序列化/反序列化场景
    /// </summary>
    public interface IChunkDto
    {
        /// <summary>
        /// 创建运行时定义
        /// </summary>
        IChunkDtoDef CreateRuntimeDef();
    }
}