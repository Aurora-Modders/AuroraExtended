using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroraExtended.Framework
{
    public class Cache<TKey, TValue>
    {
        private ConcurrentDictionary<TKey, TValue> Dictionary { get; } = new ConcurrentDictionary<TKey, TValue>();
        private int Capacity { get; }

        public Cache(int capacity = 1000)
        {
            Capacity = capacity;
        }

        public TValue Get(TKey key, Func<TKey, TValue> create)
        {
            if (Dictionary.Count > Capacity)
            {
                Dictionary.Clear();
            }

            return Dictionary.GetOrAdd(key, create);
        }
    }
}
