using System;
using System.Linq.Expressions;
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
        private readonly ILogger logger;

        public OrderProcessingSagaRouter(ISagaRepository repository, ISagaIdStore sagaIdStore, ILogger logger)
        {
            if (repository == null) throw new ArgumentNullException("repository");
            if (sagaIdStore == null) throw new ArgumentNullException("sagaIdStore");
            if (logger == null) throw new ArgumentNullException("logger");
            this.sagaIdStore = sagaIdStore;
            this.logger = logger;
            this.repository = repository;
        }

        public void Consume(OrderPlaced message)
        {
            ISaga saga = new OrderProcessingSaga();
            Guid sagaid = sagaIdStore.GetSagaIdFromCorrelationKey<OrderProcessingSaga>(message.Id);

            saga.Transition(new SagaStarted(sagaid, message.Id));
            saga.Transition(message);

            repository.Save(saga, Guid.NewGuid(), null);
            LogSagaMessage(message, m => m.Id);
        }

        public void Consume(OrderNotValidatedByManager message)
        {
            Guid sagaid = sagaIdStore.GetSagaIdFromCorrelationKey<OrderProcessingSaga>(message.OrderId);
            ISaga saga = repository.GetById<OrderProcessingSaga>(sagaid);
            saga.Transition(message);

            repository.Save(saga, Guid.NewGuid(), null);
            LogSagaMessage(message, m => m.OrderId);
        }

        public void Consume(OrderValidatedByManager message)
        {
            Guid sagaid = sagaIdStore.GetSagaIdFromCorrelationKey<OrderProcessingSaga>(message.OrderId);
            ISaga saga = repository.GetById<OrderProcessingSaga>(sagaid);
            saga.Transition(message);

            repository.Save(saga, Guid.NewGuid(), null);
            LogSagaMessage(message, m=> m.OrderId);
        }

        private void LogSagaMessage<TMessage>(TMessage message, Expression<Func<TMessage, Guid>> getCorrelationKey)
        {
            var memberExpression = getCorrelationKey.Body as MemberExpression;

            if (memberExpression == null)
                throw new InvalidOperationException("Invalid get operation key"); 

            var name = memberExpression.Member.Name;

            Guid correlationId = getCorrelationKey.Compile().Invoke(message);
            Guid sagaId = sagaIdStore.GetSagaIdFromCorrelationKey<OrderProcessingSaga>(correlationId);

            logger.LogDebug(LoggerNames.Saga, "Saga type '{0}' id '{1}' receive an '{2}' event with correlation '{3}' '{4}'", typeof(OrderProcessingSaga).FullName, sagaId, message.GetType().FullName, name, correlationId);
        }
    }
}