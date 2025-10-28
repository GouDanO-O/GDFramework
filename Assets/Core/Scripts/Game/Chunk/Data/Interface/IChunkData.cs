using GDFrameworkExtend.Data;

namespace Core.Game.Chunk.Data.Interface
{
    /// <summary>
    /// 运行时数据
    /// 包括固定数据和临时数据
    /// </summary>
    public interface IChunkData
    {
        string InstanceId { get; }
        string DefId { get; }

        void InitFromDef(IChunkDtoDef dtoDef);
        void InitFromInstanceId(string instanceId, IChunkDtoDef dtoDef);

        bool HasTemporaryData(string instanceId);
        void SaveTemporaryData();
        void DeleteTemporaryData();
    }
}