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
    public class given_an_order_placed_when_an_order_validated_by_manager_and_a_second_order_validated_by_manager
        : SagaFixtureBase
    {
        private IEnumerable<object> commands;
        
        [TestInitialize()]
        public void Setup()
        {
            var orderPlaced = new OrderPlaced(Guid.NewGuid(), "20120610", "Implementing Domain-Driven Design by Vaughn Vernon", 3, 39.18);
            var orderValidatedByFirstManager = new OrderValidatedByManager(Guid.NewGuid(), orderPlaced.Id, "Max Cole");
            var orderValidatedBySecondManager = new OrderValidatedByManager(Guid.NewGuid(), orderPlaced.Id, "Sebastian Gray");

            var saga = new OrderProcessingSaga();
            saga.Transition(new SagaStarted(Guid.NewGuid(), orderPlaced.Id));
         
            When(saga, orderPlaced);
            When(saga, orderValidatedByFirstManager);
            this.commands = When(saga, orderValidatedBySecondManager);
        }

        [TestMethod]
        public void then_accept_order()
        {
            this.commands.Count().Should().Be.EqualTo(1);
            this.commands.Single().Should().Be.OfType<AcceptOrder>();
        }
    }
}