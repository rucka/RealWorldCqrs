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

        public OrderAcceptedDenormalizer(IUpdateStorage storage)
        {
            if (storage == null) throw new ArgumentNullException("storage");
            this.storage = storage;
        }

        public void Consume(OrderAccepted message)
        {
            if (storage.Items<AcceptedOrder>().Any(o => o.Id == message.Id.ToString()))
            {
                return;
            }
            storage.Add(new AcceptedOrder
                            {
                                Id = message.Id.ToString(),
                                Description = message.Description
                            });
        }
    }
}