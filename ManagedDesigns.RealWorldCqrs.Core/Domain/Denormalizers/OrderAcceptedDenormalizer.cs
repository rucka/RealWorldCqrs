using System;
using System.Linq;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Model;
using ManagedDesigns.RealWorldCqrs.Core.Domain.QueryModel;
using ManagedDesigns.RealWorldCqrs.Core.Infrastructure;
using MassTransit;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain.Denormalizers
{
    public class OrderAcceptedDenormalizer
        : Consumes<OrderAccepted>.All
    {
        private readonly IUpdateStorage storage;
        private readonly ILogger logger;

        //public OrderAcceptedDenormalizer(IUpdateStorage storage)
        //    : this(storage, new EmptyLogger())
        //{
        //}

        public OrderAcceptedDenormalizer(IUpdateStorage storage, ILogger logger)
        {
            if (storage == null) throw new ArgumentNullException("storage");
            if (logger == null) throw new ArgumentNullException("logger");
            this.storage = storage;
            this.logger = logger;
        }

        public void Consume(OrderAccepted message)
        {
            if (storage.Items<AcceptedOrder>().Any(o => o.OrderId == message.Id.ToString()))
            {
                return;
            }
            storage.Add(new AcceptedOrder
                            {
                                OrderId = message.Id.ToString(),
                                Description = message.Description,
                                AcceptedDate = message.AcceptedDate
                            });
            logger.LogInfo(LoggerNames.Denormalizer, "Order '{0}' accepted in date '{1}'", message.Id.ToString(), message.AcceptedDate.ToString("dd/MM/yyyy hh:mm:ss"));
        }
    }
}