using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace ManagedDesigns.RealWorldCqrs.Web.Components
{
    public static class EnumerableExtension
    {
        [DebuggerHidden]
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }
    }
}