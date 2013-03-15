using System;
using System.Collections.Generic;

namespace Codestellation.DarkFlow.Misc
{
    internal static class Extensions
    {
        public static TValue GetOrAddThreadSafe<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, Func<TValue> valueBuilder)
        {
            TValue result;

            if (self.TryGetValue(key, out result))
            {
                return result;
            }

            lock (self)
            {
                if (self.TryGetValue(key, out result))
                {
                    return result;
                }
                result = valueBuilder();
                self[key] = result;
            }

            return result;
        }

        public static TValue GetOrAddThreadSafe<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key, Func<TKey, TValue> valueBuilder)
        {
            TValue result;

            if (self.TryGetValue(key, out result))
            {
                return result;
            }

            lock (self)
            {
                if (self.TryGetValue(key, out result))
                {
                    return result;
                }
                result = valueBuilder(key);
                self[key] = result;
            }

            return result;
        }
    }
}