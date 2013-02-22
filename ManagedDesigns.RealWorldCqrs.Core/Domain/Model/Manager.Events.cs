using System;
using ManagedDesigns.RealWorldCqrs.Core.Infrastructure;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain.Model
{
    public class ManagerCreated
    {
        public ManagerCreated(Guid id, string firstname, string lastname)
        {
            Id = id;
            Firstname = firstname;
            Lastname = lastname;
        }

        public Guid Id { get; private set; }
        public string Firstname { get; private set; }
        public string Lastname { get; private set; }
    }

    public class OrderValidatedByManager
    {
        public OrderValidatedByManager(Guid managerId, Guid orderId, string managerName)
        {
            ManagerId = managerId;
            OrderId = orderId;
            ManagerName = managerName;
        }

        public Guid ManagerId { get; private set; }
        public Guid OrderId { get; private set; }
        public string ManagerName { get; private set; }
    }

    public class OrderNotValidatedByManager
    {
        public OrderNotValidatedByManager(Guid managerId, Guid orderId, string reason, string managerName)
        {
            ManagerId = managerId;
            OrderId = orderId;
            Reason = reason;
            ManagerName = managerName;
        }

        public Guid ManagerId { get; private set; }
        public Guid OrderId { get; private set; }
        public string Reason { get; private set; }
        public string ManagerName { get; private set; }
    }
}