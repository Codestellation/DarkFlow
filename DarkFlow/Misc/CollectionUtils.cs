using System;
using System.Collections.Generic;
using System.Threading;

namespace Codestellation.DarkFlow.Misc
{
    public static class CollectionUtils
    {
        public static bool ThreadSafeAdd<TItem>(ref HashSet<TItem> set, TItem item)
        {
            HashSet<TItem> afterCas;
            HashSet<TItem> beforeCas;
            do
            {
                beforeCas = set;
                Thread.MemoryBarrier();
                if (set.Contains(item)) return false;

                var newSet = new HashSet<TItem>(set, set.Comparer) {item};

                afterCas = Interlocked.CompareExchange(ref set, newSet, beforeCas);
            } while (beforeCas != afterCas);
            return true;
        }

        public static bool ThreadSafeRemove<TItem>(ref HashSet<TItem> set, TItem item)
        {
            HashSet<TItem> afterCas;
            HashSet<TItem> beforeCas;
            do
            {
                beforeCas = set;
                Thread.MemoryBarrier();
                
                if (!set.Contains(item)) return false;

                var newSet = new HashSet<TItem>(set, set.Comparer);
                newSet.Remove(item);

                afterCas = Interlocked.CompareExchange(ref set, newSet, beforeCas);
            } while (beforeCas != afterCas);
            return true;
        }

        public static bool ThreadSafeAdd<TKey, TValue>(ref Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            Dictionary<TKey, TValue> afterCas;
            Dictionary<TKey, TValue> beforeCas;
            do
            {
                beforeCas = dictionary;
                Thread.MemoryBarrier();
                if (dictionary.ContainsKey(key)) return false;

                var newDictionary = new Dictionary<TKey, TValue>(dictionary, dictionary.Comparer) { {key, value}};

                afterCas = Interlocked.CompareExchange(ref dictionary, newDictionary, beforeCas);
            } while (beforeCas != afterCas);
            return true;
        }
    }
}