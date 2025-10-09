using Core.Game.Chunk.Interface;
using GDFrameworkCore;
using GDFrameworkExtend.Data;

namespace Core.Game.Chunk.Data
{
    /// <summary>
    /// 世界中,所有可互动数据的父类
    /// </summary>
    public abstract class ChunkData : IChunkData
    {
        public virtual void InitDto(IChunkDto chunkDto)
        {
            
        }

        public virtual void InitTemporaryData(ITemporaryData temporaryData)
        {
            
        }
        
        public virtual void SaveTemporaryData()
        {
            
        }
    }
}