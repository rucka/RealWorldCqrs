using System;
using System.Linq;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Model;
using ManagedDesigns.RealWorldCqrs.Core.Domain.QueryModel;
using ManagedDesigns.RealWorldCqrs.Core.Infrastructure;
using MassTransit;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain.Denormalizers
{
    public class ValidationOrderDenormilizer
        : Consumes<OrderValidatedByManager>.All
          , Consumes<OrderNotValidatedByManager>.All
    {
        private readonly IUpdateStorage storage;

        public ValidationOrderDenormilizer(IUpdateStorage storage)
        {
            if (storage == null) throw new ArgumentNullException("storage");
            this.storage = storage;
        }

        public void Consume(OrderNotValidatedByManager message)
        {
            if (
                storage.Items<ValidationOrder>().Any(
                    o => o.ManagerId == message.ManagerId.ToString() && o.OrderId == message.OrderId.ToString()))
            {
                return;
            }
            storage.Add(new ValidationOrder
                            {
                                OrderId = message.OrderId.ToString(),
                                ManagerId = message.ManagerId.ToString(),
                                ManagerName = message.ManagerName,
                                IsValidated = false,
                                NotValidatedReason = message.Reason
                            });
        }

        public void Consume(OrderValidatedByManager message)
        {
            if (
                storage.Items<ValidationOrder>().Any(
                    o => o.ManagerId == message.ManagerId.ToString() && o.OrderId == message.OrderId.ToString()))
            {
                return;
            }
            storage.Add(new ValidationOrder
                            {
                                OrderId = message.OrderId.ToString(),
                                ManagerId = message.ManagerId.ToString(),
                                ManagerName = message.ManagerName,
                                IsValidated = true
                            });
        }
    }
}