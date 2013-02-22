using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CommonDomain.Core;
using CommonDomain.Persistence.EventStore;
using ManagedDesigns.RealWorldCqrs.Core.Domain;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Commands;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Denormalizers;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Handlers;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Model;
using ManagedDesigns.RealWorldCqrs.Core.Domain.QueryModel;
using ManagedDesigns.RealWorldCqrs.Core.Infrastructure;
using EventStore;
using EventStore.Dispatcher;
using MassTransit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SharpTestsEx;

namespace ManagedDesigns.RealWorldCqrs.Fixtures.IntegrationFixtures
{
    [TestClass]
    public class given_a_create_manager_command_when_publish
    {
        private IStoreEvents eventstore;
        private IServiceBus bus;
        private readonly IList<EventMessage> dispatchedEvents = new List<EventMessage>();

        private readonly AutoResetEvent sync = new AutoResetEvent(false);
        private Guid managerId = Guid.NewGuid();
        private IQueryStorage readStorage;


        [TestInitialize()]
        public void Setup()
        {
            this.eventstore = Wireup.Init()
                .UsingInMemoryPersistence()
                .UsingSynchronousDispatchScheduler()
                .DispatchTo(new DelegateMessageDispatcher(c =>
                                {
                                    c.Events.ForEach(dispatchedEvents.Add);
                                    c.Events.Select(m=>m.Body).ToList().ForEach(bus.Publish);
                                }))
                .Build();

            var aggregateRepository = new EventStoreRepository(
                eventstore
                , new AggregateFactory()
                ,new ConflictDetector());

            var updateStorage = new InMemoryDbUpdateStorage();
            this.readStorage = updateStorage;

            this.bus = ServiceBusFactory.New(sbc =>
                {
                    sbc.ReceiveFrom("loopback://localhost/test");
                    sbc.Subscribe(x => x.Consumer(() => new ManagerCommandHandler(aggregateRepository)));
                    sbc.Subscribe(x => x.Consumer(() => new ManagerInfoDenormalizer(updateStorage)));
                });

            var createManagerCommand = new CreateManager(managerId, "Max", "Cole");
            bus.Publish(createManagerCommand);

            TaskManager taskManager = new TaskManager(TimeSpan.FromMilliseconds(500));
            taskManager.
                When(() => this.readStorage.Items<ManagerInfo>().Any()).
                Then(()=>this.sync.Set());
            this.sync.WaitOne(TimeSpan.FromSeconds(5));
        }

        [TestMethod]
        public void then_one_event_will_be_dispatched()
        {
            dispatchedEvents.Should().Have.Count.EqualTo(1);
        }

        [TestMethod]
        public void then_event_manager_created_will_be_dispatched()
        {
            dispatchedEvents.Single().Body.Should().Be.OfType<ManagerCreated>();
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
