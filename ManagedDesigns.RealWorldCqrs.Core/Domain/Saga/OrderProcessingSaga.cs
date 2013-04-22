using System;
using System.Collections.Generic;
using CommonDomain.Core;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Commands;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Model;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Routers;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain.Saga
{
    public class OrderProcessingSaga
        : SagaBase<Object>
    {
        private readonly IList<Guid> managerIdNotValidatingOrder = new List<Guid>();
        private readonly IList<Guid> managerIdValidatingOrder = new List<Guid>();
        private bool accepted;
        private bool complete;
        private string orderDescription = string.Empty;
        private Guid orderId = Guid.Empty;
        private bool rejected;

        public OrderProcessingSaga()
        {
            Register<SagaStarted>(Handle);
            Register<OrderPlaced>(Handle);
            Register<OrderValidatedByManager>(Handle);
            Register<OrderNotValidatedByManager>(Handle);
        }

        private void Handle(SagaStarted message)
        {
            if (complete)
            {
                return;
            }

            if (message.CorrelationId == null)
            {
                throw new ArgumentException("message must contains a valid correlation key");
            }

            orderId = (Guid) message.CorrelationId;
            Id = message.SagaId;
        }

        private void Handle(OrderPlaced message)
        {
            if (complete)
            {
                return;
            }

            if (message.Id != orderId)
            {
                throw new ArgumentException("Order Id missmatch saga correlation Key", "message");
            }
            orderDescription = message.Description;
        }

        private void Handle(OrderValidatedByManager message)
        {
            if (complete)
            {
                return;
            }

            if (message.OrderId != orderId)
            {
                throw new ArgumentException("Order Id missmatch saga correlation Key", "message");
            }

            if (!managerIdValidatingOrder.Contains(message.ManagerId))
            {
                managerIdValidatingOrder.Add(message.ManagerId);
            }

            if (managerIdValidatingOrder.Count != 2)
            {
                return;
            }

            var cmd = new AcceptOrder(orderId, orderDescription);
            Dispatch(cmd);
            accepted = true;
            MarkAsComplete();
        }

        private void Handle(OrderNotValidatedByManager message)
        {
            if (complete)
            {
                return;
            }

            if (message.OrderId != orderId)
            {
                throw new ArgumentException("Order Id missmatch saga correlation Key", "message");
            }

            if (!managerIdNotValidatingOrder.Contains(message.ManagerId))
            {
                managerIdNotValidatingOrder.Add(message.ManagerId);
            }

            if (managerIdNotValidatingOrder.Count != 2)
            {
                return;
            }

            var cmd = new RejectOrder(orderId, orderDescription,
                                      string.Format("Manager {0} not validate order with reason {1}",
                                                    message.ManagerName, message.Reason));
            Dispatch(cmd);
            rejected = true;
            MarkAsComplete();
        }

        private void MarkAsComplete()
        {
            complete = true;
        }
    }
}