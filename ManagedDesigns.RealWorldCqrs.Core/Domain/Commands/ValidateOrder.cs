using System;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain.Commands
{
    public sealed class ValidateOrder
    {
        public ValidateOrder(Guid managerId, Guid orderId)
        {
            Id = managerId;
            OrderId = orderId;
        }

        public Guid Id { get; private set; }

        public Guid ManagerId
        {
            get { return Id; }
        }

        public Guid OrderId { get; private set; }
    }
}