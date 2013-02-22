using System;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain.Commands
{
    public sealed class AcceptOrder
    {
        public AcceptOrder(Guid id, string orderDescription)
        {
            Id = id;
            OrderDescription = orderDescription;
        }

        public Guid Id { get; private set; }
        public string OrderDescription { get; private set; }
    }
}