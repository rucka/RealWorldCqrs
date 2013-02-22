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
    public class when_order_placed
    {
        private readonly Guid orderId = Guid.NewGuid();
        private IQueryStorage readStorage;

        [TestInitialize()]
        public void Setup()
        {
            var updateStorage = new InMemoryDbUpdateStorage();
            this.readStorage = updateStorage;
            OrderInProgressDenormilizer denormilizer = new OrderInProgressDenormilizer(updateStorage);
            denormilizer.Consume(new OrderPlaced(orderId, "fake order", "fake_product", 1, 10));
        }

        [TestMethod]
        public void then_order_in_progress_will_be_created()
        {
            this.readStorage.Items<OrderInProgress>().Should().Have.Count.EqualTo(1);
        }

        [TestMethod]
        public void then_order_in_progress_has_expected_orderId()
        {
            this.readStorage.Items<OrderInProgress>().Single().Id.Should().Be.EqualTo(orderId.ToString());
        }
    }

    [TestClass]
    public class given_order_in_progress_when_order_accepted
    {
        private readonly Guid orderId = Guid.NewGuid();
        private IQueryStorage readStorage;

        [TestInitialize()]
        public void Setup()
        {
            var updateStorage = new InMemoryDbUpdateStorage();
            updateStorage.Add(new OrderInProgress()
                                  {
                                      Id = orderId.ToString(),
                                      Description = "fake order",
                                      ProductName = "fake_product",
                                      Price = 1,
                                      Quantity = 10,
                                      TotalAmount = 10
                                  });
            this.readStorage = updateStorage;
            OrderInProgressDenormilizer denormilizer = new OrderInProgressDenormilizer(updateStorage);
            denormilizer.Consume(new OrderAccepted(orderId, "fake order"));
        }

        [TestMethod]
        public void then_order_in_progress_will_be_removed()
        {
            this.readStorage.Items<OrderInProgress>().Should().Have.Count.EqualTo(0);
        }
    }

    [TestClass]
    public class given_order_in_progress_when_order_rejected
    {
        private readonly Guid orderId = Guid.NewGuid();
        private IQueryStorage readStorage;

        [TestInitialize()]
        public void Setup()
        {
            var updateStorage = new InMemoryDbUpdateStorage();
            updateStorage.Add(new OrderInProgress()
            {
                Id = orderId.ToString(),
                Description = "fake order",
                ProductName = "fake_product",
                Price = 1,
                Quantity = 10,
                TotalAmount = 10
            });
            this.readStorage = updateStorage;
            OrderInProgressDenormilizer denormilizer = new OrderInProgressDenormilizer(updateStorage);
            denormilizer.Consume(new OrderRejected(orderId, "fake order", "order is not valid!"));
        }

        [TestMethod]
        public void then_order_in_progress_will_be_removed()
        {
            this.readStorage.Items<OrderInProgress>().Should().Have.Count.EqualTo(0);
        }
    }
}
