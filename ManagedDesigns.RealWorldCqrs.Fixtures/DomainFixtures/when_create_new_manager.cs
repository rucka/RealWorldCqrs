using System;
using System.Collections.Generic;
using System.Linq;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpTestsEx;

namespace ManagedDesigns.RealWorldCqrs.Fixtures.DomainFixtures
{
    [TestClass]
    public class when_create_new_manager
        : AggregateFixtureBase
    {
        private IEnumerable<object> events;

        [TestInitialize()]
        public void Setup()
        {
            var root = Manager.Create(Guid.NewGuid(), "Max", "Cole");
            this.events = When(root, manager => { });
        }

        [TestMethod]
        public void then_manager_created()
        {
            this.events.Count().Should().Be.EqualTo(1);
            this.events.Single().Should().Be.OfType<ManagerCreated>();
        }
    }
}