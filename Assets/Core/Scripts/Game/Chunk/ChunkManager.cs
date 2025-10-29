using GDFrameworkExtend.SingletonKit;

namespace Core.Game.Chunk
{
    /// <summary>
    /// 区块管理器
    /// </summary>
    public class ChunkManager : Singleton<ChunkManager>
    {
        /// <summary>
        /// 是否是区块编辑模式
        /// 如果是,才能编辑并保存固定数据
        /// </summary>
        public bool IsChunkEditor = true;
    }
}