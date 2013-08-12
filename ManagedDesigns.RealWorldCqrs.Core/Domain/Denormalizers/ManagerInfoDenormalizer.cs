using System;
using System.Linq;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Model;
using ManagedDesigns.RealWorldCqrs.Core.Domain.QueryModel;
using ManagedDesigns.RealWorldCqrs.Core.Infrastructure;
using MassTransit;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain.Denormalizers
{
    public class ManagerInfoDenormalizer
        : Consumes<ManagerCreated>.All
    {
        private readonly IUpdateStorage storage;
        private readonly ILogger logger;

        public ManagerInfoDenormalizer(IUpdateStorage storage)
            : this(storage, new EmptyLogger())
        {
        }

        public ManagerInfoDenormalizer(IUpdateStorage storage, ILogger logger)
        {
            if (storage == null) throw new ArgumentNullException("storage");
            if (logger == null) throw new ArgumentNullException("logger");
            this.storage = storage;
            this.logger = logger;
        }

        public void Consume(ManagerCreated message)
        {
            if (storage.Items<ManagerInfo>().Any(o => o.Id == message.Id.ToString()))
            {
                return;
            }
            var fullname = string.Format("{0} {1}", message.Firstname, message.Lastname);
            storage.Add(new ManagerInfo
                            {
                                Id = message.Id.ToString(),
                                Fullname = fullname
                            });
            logger.LogInfo(LoggerNames.Denormalizer, "Manager '{0}' created", fullname);
        }
    }
}