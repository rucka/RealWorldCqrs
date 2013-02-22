using System;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain.Commands
{
    public sealed class RejectOrder
    {
        public RejectOrder(Guid id, string orderDescription, string reason)
        {
            Id = id;
            OrderDescription = orderDescription;
            Reason = reason;
        }

        public Guid Id { get; private set; }
        public string OrderDescription { get; private set; }
        public string Reason { get; private set; }
    }
}