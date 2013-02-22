using CommonDomain;
using CommonDomain.Core;

namespace ManagedDesigns.RealWorldCqrs.Core.Infrastructure
{
    public abstract class AggregateRoot<TMemento>
        : AggregateBase
        where TMemento : IMemento, new()
    {
        protected AggregateRoot()
        {
            State = new TMemento();
        }

        protected AggregateRoot(IRouteEvents handler) : base(handler)
        {
            State = new TMemento();
        }

        protected TMemento State { get; private set; }

        protected override IMemento GetSnapshot()
        {
            State.Id = Id;
            State.Version = Version;
            return State;
        }
    }
}