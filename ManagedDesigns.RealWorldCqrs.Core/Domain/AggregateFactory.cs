using System;
using System.Reflection;
using CommonDomain;
using CommonDomain.Persistence;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain
{
    /// <summary>
    /// Factory for creating aggregates from with their Id using a private constructor that accespts
    /// only one paramenter, the id of the aggregate to create.
    /// This factory is used by the event store to create an aggregate prior to replaying it's events.
    /// </summary>
    public class AggregateFactory : IConstructAggregates
    {
        #region IConstructAggregates Members

        public IAggregate Build(Type type, Guid id, IMemento snapshot)
        {
            ConstructorInfo constructor = type.GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] {typeof (Guid)}, null);

            if (constructor == null)
            {
                throw new InvalidOperationException(
                    string.Format("Aggregate {0} cannot be created: constructor with only id parameter not provided",
                                  type.Name));
            }
            return constructor.Invoke(new object[] {id}) as IAggregate;
        }

        #endregion
    }
}