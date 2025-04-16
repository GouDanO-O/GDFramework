using GDFrameworkExtend.Data;
using GDFrameworkExtend.PoolKit;

namespace GDFramework.Mission
{
    public class SendMessageEvent<T> : TemporalityDataPoolable
    {
        public void SendMessage(T message)
        {
            
        }

        public static SendMessageEvent<T> Allocate()
        {
            return SafeObjectPool<SendMessageEvent<T>>.Instance.Allocate();
        }

        public override void DeInitData()
        {
            
        }

        public override void OnRecycled()
        {
            
        }

        public override void Recycle2Cache()
        {
            
        }
    }
}