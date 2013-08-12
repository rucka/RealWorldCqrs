using System;
using System.IO;
using System.Linq;
using CommonDomain;
using CommonDomain.Core;
using CommonDomain.Persistence;
using CommonDomain.Persistence.EventStore;
using EventStore;
using EventStore.Dispatcher;
using ManagedDesigns.RealWorldCqrs.Core.Domain;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Commands;
using ManagedDesigns.RealWorldCqrs.Core.Infrastructure;
using MassTransit;
using MassTransit.NLogIntegration;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using NLog;
using NLog.Config;
using NLog.Targets;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Database.Server;

namespace ManagedDesigns.RealWorldCqrs.WorkerHost
{
    class Program
    {
        internal const string LOGGER_NAME = "WorkerHost";
        private static void Main(string[] args)
        {
            ILogger logger = new EmptyLogger(); 
            var container = new UnityContainer();
            try
            {
                container
                    .ConfigureLogger()
                    .ConfigureReadModel()
                    .ConfigureEventStore()
                    .ConfigureCommonDomain()
                    .ConfigureBus();

                logger = container.Resolve<ILogger>();
                logger.LogInfo(Program.LOGGER_NAME, "Configuration ended");

            }
            catch (Exception e)
            {
                logger.LogException(Program.LOGGER_NAME, e);
            }
            finally
            {
                logger.LogInfo(Program.LOGGER_NAME, "Press key to continue..");
                Console.ReadLine();
                logger.LogInfo(Program.LOGGER_NAME, "Worker shutdown..");
                var disposable = container.Resolve<IServiceBus>() as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }
    }

    static class ContainerExtension
    {
        public static IUnityContainer ConfigureLogger(this IUnityContainer source)
        {
            var logconfiguration = new LoggingConfiguration();
            var consoleTarget = new ColoredConsoleTarget();
            logconfiguration.AddTarget("console", consoleTarget);

            consoleTarget.Layout = @"${date:format=HH\:MM\:ss} ${logger} ${message}";
            var messageRule = new LoggingRule("MassTransit.Messages", LogLevel.Debug, consoleTarget);
            logconfiguration.LoggingRules.Add(messageRule);

            var sagaRule = new LoggingRule(LoggerNames.Saga, LogLevel.Debug, consoleTarget);
            logconfiguration.LoggingRules.Add(sagaRule);

            var denormalizerRule = new LoggingRule(LoggerNames.Denormalizer, LogLevel.Debug, consoleTarget);
            logconfiguration.LoggingRules.Add(denormalizerRule);

            var workerHostRule = new LoggingRule(Program.LOGGER_NAME, LogLevel.Debug, consoleTarget);
            logconfiguration.LoggingRules.Add(workerHostRule);

            source.RegisterInstance(logconfiguration);
            source.RegisterInstance<ILogger>(new NLogLogger(logconfiguration));
            return source;
        }

        public static IUnityContainer ConfigureEventStore(this IUnityContainer source)
        {
            var logger = source.Resolve<ILogger>();
            logger.LogInfo(Program.LOGGER_NAME, "Configuring eventstore");

            var eventstore = Wireup.Init()
                .UsingRavenPersistence(()=>CreateDocumentStore("escqrs.raven", 8081))
                .UsingSynchronousDispatchScheduler()
                .DispatchTo(new DelegateMessageDispatcher(c =>
                    {
                        var bus = source.Resolve<IServiceBus>();
                        DispatchCommit(bus, c);
                })).Build();

            source.RegisterInstance(eventstore);
            return source;
        }

        private const string SagaTypeHeader = "SagaType";
        private const string UndispatchedMessageHeader = "UndispatchedMessage.";

        private static void DispatchCommit(IServiceBus publisher, Commit commit)
        {
            bool commitIsAboutSaga = commit.Headers.ContainsKey(SagaTypeHeader);

            if (!commitIsAboutSaga)
            {
                var events = commit.Events.Select(e => e.Body).ToArray();
                if (events.Any())
                {
                    events.ForEach(publisher.Publish);
                }
            }

            if (commitIsAboutSaga)
            {
                var commands = commit
                    .Headers
                    .Where(h => h.Key.StartsWith(UndispatchedMessageHeader)).Select(h => h.Value)
                    .ToArray();
                if (commands.Any())
                {
                    commands.ForEach(publisher.Publish);
                }
            }
        }

        public static IUnityContainer ConfigureReadModel(this IUnityContainer source)
        {
            var logger = source.Resolve<ILogger>();
            logger.LogDebug(Program.LOGGER_NAME, "Configuring ReadModel");

            const string filename = "rwcqrs.raven";

            source.RegisterInstance<IDocumentStore>(CreateDocumentStore(filename, 8080));
            
            source.Resolve<IDocumentStore>();
            source.RegisterType<IUpdateStorage, RavenDbUpdateStorage>();
            return source;
        }

        private static IDocumentStore CreateDocumentStore(string filename, int port, Action<IDocumentStore> beforeInitialize = null)
        {
            NonAdminHttp.EnsureCanListenToWhenInNonAdminContext(port);
            var dbpath = string.Format("{0}{1}{2}", Path.GetTempPath(), Path.DirectorySeparatorChar,filename);
            var store = new EmbeddableDocumentStore { DataDirectory = dbpath, UseEmbeddedHttpServer = true };
            store.Configuration.Port = port;
            if (beforeInitialize != null)
            {
                beforeInitialize(store);
            }
            store.Initialize();
            return store;
        }

        public static IUnityContainer ConfigureCommonDomain(this IUnityContainer source)
        {
            var logger = source.Resolve<ILogger>();
            logger.LogDebug(Program.LOGGER_NAME, "Configuring aggregates and repositories");
            
            source.RegisterType<IConstructAggregates, AggregateFactory>();
            source.RegisterType<IDetectConflicts, ConflictDetector>();
            source.RegisterType<IRepository, EventStoreRepository>();

            source.RegisterType<ISagaIdStore, RavenDbSagaIdStore>();
            source.RegisterType<ISagaRepository, SagaEventStoreRepository>();
            return source;
        }

        const string Address = "rabbitmq://localhost/rwcqrs-worker";
        public static IUnityContainer ConfigureBus(this IUnityContainer source)
        {
            var logger = source.Resolve<ILogger>();
            logger.LogDebug(Program.LOGGER_NAME, "Configuring MassTransit bus");

            var logconfiguration = source.Resolve<LoggingConfiguration>();

            var typePerConsumers = 
            typeof(CreateManager)
                .Assembly.GetTypes()
                .Where(typeof(IConsumer).IsAssignableFrom).ToArray();
            typePerConsumers
                .ForEach(c => source.RegisterType(typeof (IConsumer), c, Guid.NewGuid().ToString()));

            var bus = ServiceBusFactory.New(sbc =>
                {
                    sbc.ReceiveFrom(Address);
                    sbc.UseRabbitMq();
                    sbc.UseNLog(new LogFactory(logconfiguration));
                    sbc.SetPurgeOnStartup(true);
                    
                    sbc.Subscribe(x => x.LoadFrom(source));
            });
            bus.ShutdownTimeout = TimeSpan.FromSeconds(5);
            source.RegisterInstance(bus);

            return source;
        }
    }

    static class RavenDbPersistanceExtension
    {
        public static RavenPersistenceWireup UsingRavenPersistence(this Wireup source,
                                                                   Func<IDocumentStore> createDocumentStore)
        {
            return new RavenPersistenceWireup(source, createDocumentStore);
        }
    }

    public class NLogLogger
        : ILogger
    {
      
        public NLogLogger(LoggingConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException("configuration");
            LogManager.Configuration = configuration;
        }

        public void LogDebug(string loggerName, string message, params object[] args)
        {
            var logger = LogManager.GetLogger(loggerName);
            logger.Debug(string.Format(message, args));
        }

        public void LogInfo(string loggerName, string message, params object[] args)
        {
            var logger = LogManager.GetLogger(loggerName);
            logger.Info(string.Format(message, args));
        }

        public void LogWarning(string loggerName, string message, params object[] args)
        {
            var logger = LogManager.GetLogger(loggerName);
            logger.Warn(string.Format(message, args));
        }

        public void LogError(string loggerName, string message, params object[] args)
        {
            var logger = LogManager.GetLogger(loggerName);
            logger.Error(string.Format(message, args));
        }

        public void LogException(string loggerName, Exception e)
        {
            var logger = LogManager.GetLogger(loggerName);
            logger.ErrorException("An exception occurs", e);
        }
    }
}
