using System;
using System.Collections.Generic;
using System.Linq;

namespace RoundScheduler.Utils
{
    public static class Extensions
    {
        public static void ForEach<TItem>(this IEnumerable<TItem> collection, Action<TItem> action)
        {
            foreach (var item in collection)
                action(item);
        }

        public static bool IsNullOrEmpty<TItem>(this IEnumerable<TItem> collection)
        {
            return collection == null || !collection.Any();
        }

        public static bool IsNotNullOrEmpty<TItem>(this IEnumerable<TItem> collection)
        {
            return !collection.IsNullOrEmpty();
        }
    }
}
