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
    public class when_order_validated_by_manager
    {
        private readonly Guid orderId = Guid.NewGuid();
        private readonly Guid managerId = Guid.NewGuid();
        private IQueryStorage readStorage;

        [TestInitialize()]
        public void Setup()
        {
            var updateStorage = new InMemoryDbUpdateStorage();
            this.readStorage = updateStorage;
            ValidationOrderDenormilizer denormilizer = new ValidationOrderDenormilizer(updateStorage);
            denormilizer.Consume(new OrderValidatedByManager(managerId, orderId, "Max Cole"));
        }

        [TestMethod]
        public void then_validation_order_will_be_created()
        {
            this.readStorage.Items<ValidationOrder>().Should().Have.Count.EqualTo(1);
        }

        [TestMethod]
        public void then_validation_order_has_expected_managerId()
        {
            this.readStorage.Items<ValidationOrder>().Single().OrderId.Should().Be.EqualTo(orderId.ToString());
        }

        [TestMethod]
        public void then_validation_order_has_expected_orderId()
        {
            this.readStorage.Items<ValidationOrder>().Single().ManagerId.Should().Be.EqualTo(managerId.ToString());
        }

        [TestMethod]
        public void then_validation_order_is_validated()
        {
            this.readStorage.Items<ValidationOrder>().Single().IsValidated.Should().Be.True();
        }
    }

    [TestClass]
    public class when_order_not_validated_by_manager
    {
        private readonly Guid orderId = Guid.NewGuid();
        private readonly Guid managerId = Guid.NewGuid();
        private IQueryStorage readStorage;

        [TestInitialize()]
        public void Setup()
        {
            var updateStorage = new InMemoryDbUpdateStorage();
            this.readStorage = updateStorage;
            ValidationOrderDenormilizer denormilizer = new ValidationOrderDenormilizer(updateStorage);
            denormilizer.Consume(new OrderNotValidatedByManager(managerId, orderId, "order not valid", "Max Cole"));
        }

        [TestMethod]
        public void then_validation_order_will_be_created()
        {
            this.readStorage.Items<ValidationOrder>().Should().Have.Count.EqualTo(1);
        }

        [TestMethod]
        public void then_validation_order_has_expected_managerId()
        {
            this.readStorage.Items<ValidationOrder>().Single().OrderId.Should().Be.EqualTo(orderId.ToString());
        }

        [TestMethod]
        public void then_validation_order_has_expected_orderId()
        {
            this.readStorage.Items<ValidationOrder>().Single().ManagerId.Should().Be.EqualTo(managerId.ToString());
        }

        [TestMethod]
        public void then_validation_order_is_not_validated()
        {
            this.readStorage.Items<ValidationOrder>().Single().IsValidated.Should().Be.False();
        }
    }
}
