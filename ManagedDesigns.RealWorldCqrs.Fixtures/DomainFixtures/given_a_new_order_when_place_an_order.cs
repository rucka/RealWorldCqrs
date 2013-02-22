using System;
using System.Linq;
using System.Collections.Generic;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpTestsEx;

namespace ManagedDesigns.RealWorldCqrs.Fixtures.DomainFixtures
{
    [TestClass]
    public class given_a_new_order_when_place_an_order
        : AggregateFixtureBase
    {
        private IEnumerable<object> events;
        
        [TestInitialize()]
        public void Setup()
        {
            var root = Order.Create(Guid.NewGuid(), "20120610", "Implementing Domain-Driven Design by Vaughn Vernon", 1, 39.18);
            this.events = When(root, order => { });
        }

        [TestMethod]
        public void then_order_placed()
        {
            this.events.Count().Should().Be.EqualTo(1);
            this.events.Single().Should().Be.OfType<OrderPlaced>();
        }
    }
}
