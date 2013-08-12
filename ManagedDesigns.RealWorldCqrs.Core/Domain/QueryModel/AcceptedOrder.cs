using System;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain.QueryModel
{
    public class AcceptedOrder
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public string Description { get; set; }
        public DateTime AcceptedDate { get; set; }
    }
}