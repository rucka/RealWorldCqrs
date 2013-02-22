using System;
using System.Linq;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Denormalizers;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Model;
using ManagedDesigns.RealWorldCqrs.Core.Domain.QueryModel;
using ManagedDesigns.RealWorldCqrs.Core.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpTestsEx;

namespace ManagedDesigns.RealWorldCqrs.Fixtures.DenormalizerFixtures
{
    [TestClass]
    public class when_order_rejected
    {
        private readonly Guid orderId = Guid.NewGuid();
        private IQueryStorage readStorage;

        [TestInitialize()]
        public void Setup()
        {
            var updateStorage = new InMemoryDbUpdateStorage();
            this.readStorage = updateStorage;
            OrderRejectedDenormalizer denormalizer = new OrderRejectedDenormalizer(updateStorage);
            denormalizer.Consume(new OrderRejected(orderId, "fake order", "order not valid"));
        }

        [TestMethod]
        public void then_order_rejected_will_be_created()
        {
            this.readStorage.Items<RejectedOrder>().Should().Have.Count.EqualTo(1);
        }

        [TestMethod]
        public void then_order_in_accepted_has_expected_orderId()
        {
            this.readStorage.Items<RejectedOrder>().Single().Id.Should().Be.EqualTo(orderId.ToString());
        }
    }
}
