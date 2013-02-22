using System;
using CommonDomain.Persistence;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Commands;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Model;
using MassTransit;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain.Handlers
{
    public class OrderCommandHandler
        : Consumes<PlaceOrder>.All
          , Consumes<AcceptOrder>.All
          , Consumes<RejectOrder>.All
    {
        private readonly IRepository repository;

        public OrderCommandHandler(IRepository repository)
        {
            this.repository = repository;
        }

        public void Consume(AcceptOrder command)
        {
            var aggregate = repository.GetById<Order>(command.Id);
            aggregate.Accept();
            repository.Save(aggregate, CreateNewCommitId());
        }

        public void Consume(PlaceOrder command)
        {
            var aggregate = Order.Create(command.Id, command.Description, command.ProductName, command.Quantity, command.Price);
            repository.Save(aggregate, CreateNewCommitId());
        }

        public void Consume(RejectOrder command)
        {
            var aggregate = repository.GetById<Order>(command.Id);
            aggregate.Reject(command.Reason);
            repository.Save(aggregate, CreateNewCommitId());
        }

        private Guid CreateNewCommitId()
        {
            return Guid.NewGuid();
        }
    }
}