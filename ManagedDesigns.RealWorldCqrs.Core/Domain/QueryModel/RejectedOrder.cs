using System;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain.QueryModel
{
    public class RejectedOrder
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public string Description { get; set; }
        public string Reason { get; set; }
        public DateTime RejectedDate { get; set; }
    }
}