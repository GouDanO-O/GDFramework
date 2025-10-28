namespace Core.Game.Chunk.Data.Interface
{
    /// <summary>
    /// 固定数据定义接口(配置)
    /// </summary>
    public interface IChunkDtoDef
    {
        string DefId { get; }
        string DefName { get; }
        string DefDescription { get; }
        
        bool Validate(out string error);
        IChunkDtoDef Clone();
    }
}