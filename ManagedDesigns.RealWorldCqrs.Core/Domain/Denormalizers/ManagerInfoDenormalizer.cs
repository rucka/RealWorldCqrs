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

        public ManagerInfoDenormalizer(IUpdateStorage storage)
        {
            if (storage == null) throw new ArgumentNullException("storage");
            this.storage = storage;
        }

        public void Consume(ManagerCreated message)
        {
            if (storage.Items<ManagerInfo>().Any(o => o.Id == message.Id.ToString()))
            {
                return;
            }
            storage.Add(new ManagerInfo
                            {
                                Id = message.Id.ToString(),
                                Fullname = string.Format("{0} {1}", message.Firstname, message.Lastname)
                            });
        }
    }
}