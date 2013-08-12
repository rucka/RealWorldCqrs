using System;
using System.Linq;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Model;
using ManagedDesigns.RealWorldCqrs.Core.Domain.QueryModel;
using ManagedDesigns.RealWorldCqrs.Core.Infrastructure;
using MassTransit;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain.Denormalizers
{
    public class OrderRejectedDenormalizer
        : Consumes<OrderRejected>.All
    {
        private readonly IUpdateStorage storage;
        private readonly ILogger logger;
        
        //public OrderRejectedDenormalizer(IUpdateStorage storage)
        //    : this(storage, new EmptyLogger())
        //{ }

        public OrderRejectedDenormalizer(IUpdateStorage storage, ILogger logger)
        {
            if (storage == null) throw new ArgumentNullException("storage");
            if (logger == null) throw new ArgumentNullException("logger");
            this.storage = storage;
            this.logger = logger;
        }

        public void Consume(OrderRejected message)
        {
            if (storage.Items<RejectedOrder>().Any(o => o.OrderId == message.Id.ToString()))
            {
                return;
            }
            storage.Add(new RejectedOrder
                            {
                                OrderId = message.Id.ToString(),
                                Description = message.Description,
                                Reason = message.Reason,
                                RejectedDate = message.RejectedDate
                            });     
            logger.LogInfo(LoggerNames.Denormalizer, "order '{0}' rejected at '{1}'", message.Id.ToString(), message.RejectedDate.ToString("dd/MM/yyyy hh:mm:ss"));
        }
    }
}