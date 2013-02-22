using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CommonDomain;

namespace ManagedDesigns.RealWorldCqrs.Fixtures
{
    public abstract class AggregateFixtureBase
    {
        public T Given<T>(Guid id, IEnumerable<object> events) where T: IAggregate
        {
            var aggregate = CreateAggregate<T>(id);
            aggregate.LoadFromHistory(events);
            aggregate.ClearUncommittedEvents();
            return aggregate;
        }

        public T Given<T>(Guid id, params object[] events) where T : IAggregate
        {
            return Given<T>(id, events.AsEnumerable());
        }

        public IEnumerable<object> When<T>(T root, Action<T> when) where T : IAggregate
        {
            if (when == null) when = m=>{};
            when(root);
            return root.GetUncommittedEvents().Cast<object>();
        }

        private static T CreateAggregate<T>(Guid id) where T : IAggregate
        {
            ConstructorInfo ctor = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Single(c => c.GetParameters().Count() == 1);
            return (T)ctor.Invoke(new object[] { id });
        }
    }

    /*
    public abstract class AggregateFixtureBase<T> where T : IAggregate
    {
        private T root;
        protected Exception Caugth { get; private set; }
        protected IEnumerable<Object> Events { get; private set; }

        protected abstract IEnumerable<object> Given();
        protected abstract void When(T root);

        [TestInitialize()]
        public void Setup()
        {
            try
            {
                root = CreateAggregate(Guid.NewGuid());
                root.LoadFromHistory(Given());
                When(root);
                this.Events = (root as IAggregate).GetUncommittedEvents().Cast<Object>();
            }
            catch (Exception ex)
            {
                this.Caugth = ex;
            }
        }

        private T CreateAggregate(Guid id)
        {
            ConstructorInfo ctor = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).Single(c => c.GetParameters().Count() == 1);
            return (T)ctor.Invoke(new object[] { id });
        }
    }*/
}