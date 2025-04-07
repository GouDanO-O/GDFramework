
using GDFrameworkExtend.CoreKit;

namespace GDFramework_Extend.Data
{
    /// <summary>
    /// 持久化对象--池
    /// 需要实现Allocate
    /// </summary>
    public abstract class PersistentDataPoolable : PersistentData, IPoolable, IPoolType
    {
        public bool IsRecycled { get; set; }

        public abstract void OnRecycled();


        public abstract void Recycle2Cache();
    }
}