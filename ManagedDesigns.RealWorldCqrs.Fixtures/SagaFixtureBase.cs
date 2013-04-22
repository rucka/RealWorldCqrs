using System.Collections.Generic;
using System.Linq;
using CommonDomain;

namespace ManagedDesigns.RealWorldCqrs.Fixtures
{
    public abstract class SagaFixtureBase
    {
        public IEnumerable<object> When<T>(T saga, object @event) where T : ISaga
        {
            saga.Transition(@event);
            return saga.GetUndispatchedMessages().Cast<object>();
        }


        public static T LoadSagaFromHistory<T>(IEnumerable<object> events) where T: ISaga, new()
        {
            ISaga saga = new T();
            foreach (var @event in events)
            {
                saga.Transition(@event);
            }
            saga.ClearUncommittedEvents();
            saga.ClearUndispatchedMessages();
            return (T)saga;
        }

        public static T LoadSagaFromHistory<T>(params object[] events) where T : ISaga, new()
        {
            return LoadSagaFromHistory<T>(events.AsEnumerable());
        }
    }
}