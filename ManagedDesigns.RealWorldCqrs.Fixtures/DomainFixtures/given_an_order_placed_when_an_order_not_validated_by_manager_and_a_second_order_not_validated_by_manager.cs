using System;
using System.Collections.Generic;
using System.Linq;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Commands;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Model;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Routers;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Saga;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpTestsEx;

namespace ManagedDesigns.RealWorldCqrs.Fixtures.DomainFixtures
{
    [TestClass]
    public class given_an_order_placed_when_an_order_not_validated_by_manager_and_a_second_order_not_validated_by_manager
        : SagaFixtureBase
    {
        private IEnumerable<object> commands;

        [TestInitialize()]
        public void Setup()
        {
            var orderPlaced = new OrderPlaced(Guid.NewGuid(), "20120610", "Implementing Domain-Driven Design by Vaughn Vernon", 1, 39.18);
            var orderNotValidatedByFirstManager = new OrderNotValidatedByManager(Guid.NewGuid(), orderPlaced.Id, "order not valid", "Max Cole");
            var orderNotValidatedBySecondManager = new OrderNotValidatedByManager(Guid.NewGuid(), orderPlaced.Id, "order not valid", "Sebastian Gray");

            var saga = new OrderProcessingSaga();
            saga.Transition(new SagaStarted(Guid.NewGuid(), orderPlaced.Id));
         
            When(saga, orderPlaced);
            When(saga, orderNotValidatedByFirstManager);
            this.commands = When(saga, orderNotValidatedBySecondManager);
        }

        [TestMethod]
        public void then_reject_order()
        {
            this.commands.Count().Should().Be.EqualTo(1);
            this.commands.Single().Should().Be.OfType<RejectOrder>();
        }
    }
}