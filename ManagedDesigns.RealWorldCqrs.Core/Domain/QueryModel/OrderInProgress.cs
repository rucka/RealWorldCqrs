using System;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain.QueryModel
{
    public class OrderInProgress
    {
        public string Id { get; set; }
        public string OrderId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double TotalAmount { get; set; }
        public DateTime CreationDate { get; set; }
    }
}