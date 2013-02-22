using System;
using CommonDomain.Persistence;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Commands;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Model;
using MassTransit;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain.Handlers
{
    public class ManagerCommandHandler
        : Consumes<CreateManager>.All
          , Consumes<ValidateOrder>.All
    {
        private readonly IRepository repository;

        public ManagerCommandHandler(IRepository repository)
        {
            this.repository = repository;
        }

        public void Consume(CreateManager command)
        {
            var aggregate = Manager.Create(command.Id, command.Firstname, command.Lastname);
            repository.Save(aggregate, CreateNewCommitId());
        }

        public void Consume(ValidateOrder command)
        {
            var manager = repository.GetById<Manager>(command.Id);

            var order = repository.GetById<Order>(command.OrderId);
            manager.ValidateOrder(order);

            repository.Save(manager, CreateNewCommitId());
        }

        private Guid CreateNewCommitId()
        {
            return Guid.NewGuid();
        }
    }
}