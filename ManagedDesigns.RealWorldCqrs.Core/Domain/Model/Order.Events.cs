using System;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain.Model
{
    public class OrderPlaced
    {
        public OrderPlaced(Guid id, string description, string productName, int quantity, double price)
        {
            Quantity = quantity;
            Price = price;
            Id = id;
            Description = description;
            ProductName = productName;
            PlaceDate = DateTime.Now;
        }

        public Guid Id { get; private set; }
        public string Description { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public double Price { get; private set; }
        public DateTime PlaceDate { get; private set; }
    }

    public class OrderAccepted
    {
        public OrderAccepted(Guid id, string description)
        {
            Id = id;
            Description = description;
            AcceptedDate = DateTime.Now;
        }

        public Guid Id { get; private set; }
        public string Description { get; set; }
        public DateTime AcceptedDate { get; private set; }
    }

    public class OrderRejected
    {
        public OrderRejected(Guid id, string description, string reason)
        {
            Id = id;
            Description = description;
            Reason = reason;
            RejectedDate = DateTime.Now;
        }

        public Guid Id { get; private set; }
        public string Description { get; set; }
        public string Reason { get; private set; }
        public DateTime RejectedDate { get; private set; }
    }
}