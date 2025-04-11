using GDFrameworkExtend.PoolKit;

namespace GDFrameworkExtend.Data
{
    /// <summary>
    /// 临时对象数据--池
    /// 需要实现Allocate
    /// </summary>
    public abstract class TemporalityDataPoolable : TemporalityData, IPoolable, IPoolType
    {
        public bool IsRecycled { get; set; }

        /// <summary>
        /// 注销(用来回收)
        /// </summary>
        public abstract void OnRecycled();

        /// <summary>
        /// 注销时执行
        /// </summary>
        public abstract void Recycle2Cache();
    }
}