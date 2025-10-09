using GDFrameworkExtend.Data;

namespace Core.Game.Chunk.Interface
{
    /// <summary>
    /// 世界中的数据
    /// 包括固定数据和临时数据
    /// </summary>
    public interface IChunkData
    {
        void InitDto(IChunkDto chunkDto);
        
        void InitTemporaryData(ITemporaryData  temporaryData);
        
        void SaveTemporaryData();
    }
}