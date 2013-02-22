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

        public OrderInProgressDenormilizer(IUpdateStorage storage)
        {
            if (storage == null) throw new ArgumentNullException("storage");
            this.storage = storage;
        }

        public void Consume(OrderAccepted message)
        {
            if (!storage.Items<OrderInProgress>().Any(o => o.Id == message.Id.ToString()))
            {
                return;
            }
            storage.Remove(storage.Items<OrderInProgress>().First(o => o.Id == message.Id.ToString()));
        }

        public void Consume(OrderPlaced message)
        {
            if (storage.Items<OrderInProgress>().Any(o => o.Id == message.Id.ToString()))
            {
                return;
            }
            storage.Add(new OrderInProgress
                            {
                                Id = message.Id.ToString(),
                                ProductName = message.ProductName,
                                Description = message.Description,
                                Quantity = message.Quantity,
                                Price = message.Price,
                                TotalAmount = message.Price * message.Quantity
                            });
        }

        public void Consume(OrderRejected message)
        {
            if (!storage.Items<OrderInProgress>().Any(o => o.Id == message.Id.ToString()))
            {
                return;
            }
            storage.Remove(storage.Items<OrderInProgress>().First(o => o.Id == message.Id.ToString()));
        }
    }
}