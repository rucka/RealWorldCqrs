using System;
using System.Collections.Generic;
using System.Linq;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Model;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Routers;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Saga;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpTestsEx;

namespace ManagedDesigns.RealWorldCqrs.Fixtures.DomainFixtures
{
    [TestClass]
    public class given_an_order_placed_when_an_order_not_processed_by_two_manager
        : SagaFixtureBase
    {
        private IEnumerable<object> commands;

        [TestInitialize()]
        public void Setup()
        {
            var orderPlaced = new OrderPlaced(Guid.NewGuid(), "20120610", "Implementing Domain-Driven Design by Vaughn Vernon", 3, 39.18);
            var orderValidatedByFirstManager = new OrderValidatedByManager(Guid.NewGuid(), orderPlaced.Id, "Max Cole");

            var saga = new OrderProcessingSaga();
            saga.Transition(new SagaStarted(Guid.NewGuid(), orderPlaced.Id));

            When(saga, orderPlaced);
            this.commands = When(saga, orderValidatedByFirstManager);
        }

        [TestMethod]
        public void then_nothing_happens()
        {
            this.commands.Count().Should().Be.EqualTo(0);
        }
    }
}