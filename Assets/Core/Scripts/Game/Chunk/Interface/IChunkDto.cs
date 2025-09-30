namespace Core.Game.Chunk.Interface
{
    /// <summary>
    /// 固定数据接口
    /// </summary>
    public interface IChunkDto
    {
        string DtoName { get; set; }
        
        int UniqueDtoId { get; set; }
        
        string DtoId { get; set; }
        
        string DtoDescription { get; set; }
        
        
    }
}