﻿using System;
using System.Collections.Concurrent;
using System.Threading;

namespace GDFramework.GAS.General.Util
{
    /// <summary>
    /// 线程安全的无锁对象池
    /// </summary>
    public class Pool
    {
        private readonly Type ObjectType;
        private readonly int MaxCapacity;
        private int NumItems;
        private readonly ConcurrentQueue<object> _items = new();
        private object FastItem;

        public Pool(Type objectType, int maxCapacity)
        {
            ObjectType = objectType;
            MaxCapacity = maxCapacity;
        }

        public object Get()
        {
            var item = FastItem;
            if (item == null || Interlocked.CompareExchange(ref FastItem, null, item) != item)
            {
                if (_items.TryDequeue(out item))
                    Interlocked.Decrement(ref NumItems);
                else
                    item = Activator.CreateInstance(ObjectType);
            }

            return item;
        }

        public void Return(object obj)
        {
            if (FastItem != null || Interlocked.CompareExchange(ref FastItem, obj, null) != null)
            {
                if (Interlocked.Increment(ref NumItems) <= MaxCapacity)
                {
                    _items.Enqueue(obj);
                    return;
                }

                Interlocked.Decrement(ref NumItems);
            }
        }
    }
}