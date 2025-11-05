using System;
using GDFrameworkExtend.Data;

namespace Core.Game.Chunk.Data.Interface
{
    /// <summary>
    /// 运行时数据
    /// 包括固定数据和临时数据
    /// </summary>
    public interface IChunkData
    {
        /// <summary>
        /// 配置ID (唯一标识)
        /// </summary>
        string DefId { get; }

        /// <summary>
        /// 初始化Chunk数据
        /// </summary>
        void InitChunkData(IChunkDtoDef def);

        /// <summary>
        /// 设置配置数据
        /// </summary>
        void SetDefData(IChunkDtoDef def);

        /// <summary>
        /// 设置临时数据 (自动从 DefId 查找)
        /// </summary>
        void SetTempData();

        /// <summary>
        /// 保存临时数据
        /// </summary>
        void SaveTemporaryData();

        /// <summary>
        /// 删除临时数据
        /// </summary>
        void DeleteTemporaryData();
    }
}