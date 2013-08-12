using System;
using System.Web.Mvc;
using ManagedDesigns.RealWorldCqrs.Core.Infrastructure;
using MassTransit;
using Microsoft.Practices.Unity;
using Raven.Client;
using Raven.Client.Document;
using Unity.Mvc3;

namespace ManagedDesigns.RealWorldCqrs.Web
{
    public static class UnityConfig
    {
        public static void RegisterContainer()
        {
            var container = BuildUnityContainer();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            container.RegisterType<IDocumentStore>(new TransientLifetimeManager()/*new ContainerControlledLifetimeManager()*/, new InjectionFactory(c =>
                {
                    var store = new DocumentStore()
                        {
                            Url = "http://localhost:8080"
                        };

                    //store.Conventions
                    //    .FindIdentityProperty = prop => prop.Name.Equals("key", StringComparison.InvariantCultureIgnoreCase); 
                    
                    store.Initialize();
                    return store;
                }))
            ;

            container.RegisterType<IQueryStorage, RavenDbUpdateStorage>();


            const string address = "rabbitmq://localhost/rwcqrs-web";
            var bus = ServiceBusFactory.New(sbc =>
            {
                sbc.UseRabbitMq();
                sbc.ReceiveFrom(address);
            });
            container.RegisterInstance(bus);
            return container;
        }
    }
}