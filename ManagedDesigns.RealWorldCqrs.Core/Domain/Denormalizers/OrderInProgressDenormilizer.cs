using System;
using System.Linq;
using ManagedDesigns.RealWorldCqrs.Core.Domain.Model;
using ManagedDesigns.RealWorldCqrs.Core.Domain.QueryModel;
using ManagedDesigns.RealWorldCqrs.Core.Infrastructure;
using MassTransit;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain.Denormalizers
{
    public class OrderInProgressDenormilizer
        : Consumes<OrderPlaced>.All
          , Consumes<OrderAccepted>.All
          , Consumes<OrderRejected>.All
    {
        private readonly IUpdateStorage storage;
        private readonly ILogger logger;

        //public OrderInProgressDenormilizer(IUpdateStorage storage)
        //    : this(storage, new EmptyLogger())
        //{}
            
        public OrderInProgressDenormilizer(IUpdateStorage storage, ILogger logger)
        {
            if (storage == null) throw new ArgumentNullException("storage");
            if (logger == null) throw new ArgumentNullException("logger");
            this.storage = storage;
            this.logger = logger;
        }

        public void Consume(OrderAccepted message)
        {
            if (!storage.Items<OrderInProgress>().Any(o => o.OrderId == message.Id.ToString()))
            {
                return;
            }
            var order = storage.Items<OrderInProgress>().First(o => o.OrderId == message.Id.ToString());
            storage.Remove(order);
        }

        public void Consume(OrderPlaced message)
        {
            if (storage.Items<OrderInProgress>().Any(o => o.OrderId == message.Id.ToString()))
            {
                return;
            }
            var totalAmount = message.Price*message.Quantity;
            storage.Add(new OrderInProgress
                            {
                                OrderId = message.Id.ToString(),
                                ProductName = message.ProductName,
                                Description = message.Description,
                                Quantity = message.Quantity,
                                Price = message.Price,
                                TotalAmount = totalAmount,
                                CreationDate = message.PlaceDate
                            });
            logger.LogInfo(LoggerNames.Denormalizer, "order '{0}' of amount '{1}' placed at '{2}'", message.Id.ToString(), totalAmount.ToString(), message.PlaceDate.ToString("dd/MM/yyyy hh:mm:ss"));

        }

        public void Consume(OrderRejected message)
        {
            if (!storage.Items<OrderInProgress>().Any(o => o.OrderId == message.Id.ToString()))
            {
                return;
            }
            var order = storage.Items<OrderInProgress>().First(o => o.OrderId == message.Id.ToString());
            storage.Remove(order);
        }
    }
}