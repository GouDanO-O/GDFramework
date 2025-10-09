using Core.Game.Chunk.Interface;
using GDFrameworkCore;
using GDFrameworkExtend.Data;

namespace Core.Game.Chunk.Data
{
    public abstract class ChunkDataModel : AbstractModel,IChunkData
    {
        protected override void OnInit()
        {
            
        }

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