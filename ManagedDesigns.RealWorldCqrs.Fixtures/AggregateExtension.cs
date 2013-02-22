using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommonDomain;

namespace ManagedDesigns.RealWorldCqrs.Fixtures
{
    static class AggregateExtension
    {
        public static void LoadFromHistory(this IAggregate source, IEnumerable<object> events)
        {
            foreach (var @event in events)
            {
                source.ApplyEvent(@event);
            }
        }
    }
}