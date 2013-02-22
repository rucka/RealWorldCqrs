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

        public OrderRejectedDenormalizer(IUpdateStorage storage)
        {
            if (storage == null) throw new ArgumentNullException("storage");
            this.storage = storage;
        }

        public void Consume(OrderRejected message)
        {
            if (storage.Items<RejectedOrder>().Any(o => o.Id == message.Id.ToString()))
            {
                return;
            }
            storage.Add(new RejectedOrder
                            {
                                Id = message.Id.ToString(),
                                Description = message.Description,
                                Reason = message.Reason
                            });
        }
    }
}