using System;
using System.Collections.Generic;
using System.Linq;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpTestsEx;

namespace ManagedDesigns.RealWorldCqrs.Fixtures.DomainFixtures
{
    [TestClass]
    public class given_an_order_with_total_amount_lower_than_100_given_a_manager_when_process_order
        : AggregateFixtureBase
    {
        private IEnumerable<object> events;

        [TestInitialize()]
        public void Setup()
        {
            var orderPlaced = new OrderPlaced(Guid.NewGuid(), "20120610", "Implementing Domain-Driven Design by Vaughn Vernon", 1, 39.18);
            var managerCreated = new ManagerCreated(Guid.NewGuid(), "Max", "Cole");

            var order = Given<Order>(orderPlaced.Id, orderPlaced);
            var manager = Given<Manager>(managerCreated.Id, managerCreated);

            this.events = When(manager, m => manager.ValidateOrder(order));

        }

        [TestMethod]
        public void then_order_not_validated_by_manager()
        {
            this.events.Count().Should().Be.EqualTo(1);
            this.events.Single().Should().Be.OfType<OrderNotValidatedByManager>();
        }
    }
}