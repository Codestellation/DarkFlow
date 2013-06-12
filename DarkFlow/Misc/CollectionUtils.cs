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

        // Not sure I should DRY this. Duplication looks simplier.
        //public delegate bool TryModify<TItem>(HashSet<TItem> originalSet, TItem item, out HashSet<TItem> modifiedSet);

        //private static bool ModifyTasks<TItem>(TryModify<TItem> modifier, ref HashSet<TItem> original, TItem task)
        //{
        //    HashSet<TItem> afterCas;
        //    HashSet<TItem> beforeCas;
        //    do
        //    {
        //        beforeCas = original;
        //        Thread.MemoryBarrier();

        //        HashSet<TItem> newSet;

        //        if (!modifier(original, task, out newSet))
        //        {
        //            return false;
        //        }

                
        //        afterCas = Interlocked.CompareExchange(ref original, newSet, beforeCas);
        //    } while (beforeCas != afterCas);
        //    return true;
        //}
    }
}