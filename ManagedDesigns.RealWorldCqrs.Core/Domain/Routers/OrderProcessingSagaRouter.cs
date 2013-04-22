using System;
using CommonDomain;
using CommonDomain.Persistence;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Model;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Saga;
using ManagedDesigns.RealWorldCqrs.Core.Infrastructure;
using MassTransit;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain.Routers
{
    public class OrderProcessingSagaRouter
        : Consumes<OrderPlaced>.All
          , Consumes<OrderValidatedByManager>.All
          , Consumes<OrderNotValidatedByManager>.All
    {
        private readonly ISagaRepository repository;
        private readonly ISagaIdStore sagaIdStore;

        public OrderProcessingSagaRouter(ISagaRepository repository, ISagaIdStore sagaIdStore)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            if (sagaIdStore == null) throw new ArgumentNullException("sagaIdStore");
            this.sagaIdStore = sagaIdStore;
            this.repository = repository;
        }

        #region All Members

        public void Consume(OrderPlaced message)
        {
            ISaga saga = new OrderProcessingSaga();
            Guid sagaid = sagaIdStore.GetSagaIdFromCorrelationKey<OrderProcessingSaga>(message.Id);

            saga.Transition(new SagaStarted(sagaid, message.Id));
            saga.Transition(message);

            repository.Save(saga, Guid.NewGuid(), null);
        }

        #endregion

        #region All Members

        public void Consume(OrderNotValidatedByManager message)
        {
            Guid sagaid = sagaIdStore.GetSagaIdFromCorrelationKey<OrderProcessingSaga>(message.OrderId);
            ISaga saga = repository.GetById<OrderProcessingSaga>(sagaid);
            saga.Transition(message);

            repository.Save(saga, Guid.NewGuid(), null);
        }

        #endregion

        #region All Members

        public void Consume(OrderValidatedByManager message)
        {
            Guid sagaid = sagaIdStore.GetSagaIdFromCorrelationKey<OrderProcessingSaga>(message.OrderId);
            ISaga saga = repository.GetById<OrderProcessingSaga>(sagaid);
            saga.Transition(message);

            repository.Save(saga, Guid.NewGuid(), null);
        }

        #endregion
    }
}