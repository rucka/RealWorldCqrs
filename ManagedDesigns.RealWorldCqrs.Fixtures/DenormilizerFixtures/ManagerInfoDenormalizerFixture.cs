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
    public class when_manager_created
    {
        private readonly Guid managerId = Guid.NewGuid();
        private IQueryStorage readStorage;

        [TestInitialize()]
        public void Setup()
        {
            var updateStorage = new InMemoryDbUpdateStorage();
            this.readStorage = updateStorage;
            ManagerInfoDenormalizer denormalizer = new ManagerInfoDenormalizer(updateStorage);
            denormalizer.Consume(new ManagerCreated(managerId, "Max", "Cole"));
        }

        [TestMethod]
        public void then_managerInfo_will_be_created()
        {
            this.readStorage.Items<ManagerInfo>().Should().Have.Count.EqualTo(1);
        }

        [TestMethod]
        public void then_managerInfo_has_expected_managerId()
        {
            this.readStorage.Items<ManagerInfo>().Single().Id.Should().Be.EqualTo(managerId.ToString());
        }
    }
}
