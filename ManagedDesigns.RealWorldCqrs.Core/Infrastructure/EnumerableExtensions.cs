using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ManagedDesigns.RealWorldCqrs.Core.Infrastructure
{
    internal static class EnumerableExtensions
    {
        [DebuggerHidden]
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");
            foreach (T item in source)
            {
                action(item);
            }
        }
    }
}