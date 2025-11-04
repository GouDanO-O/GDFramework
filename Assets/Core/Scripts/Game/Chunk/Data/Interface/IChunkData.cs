using GDFrameworkExtend.Data;

namespace Core.Game.Chunk.Data.Interface
{
    /// <summary>
    /// 运行时数据
    /// 包括固定数据和临时数据
    /// </summary>
    public interface IChunkData
    {
        string DefId { get; }

        void InitChunkData(IChunkDtoDef def);

        void SetDefData(IChunkDtoDef def);

        void SetTempData(string defId);

        void SaveTemporaryData();

        void DeleteTemporaryData();
    }
}