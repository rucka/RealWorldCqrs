using System;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain.Commands
{
    public sealed class PlaceOrder
    {
        public PlaceOrder(Guid id, string description, string productName, int quantity, double price)
        {
            Id = id;
            Description = description;
            ProductName = productName;
            Quantity = quantity;
            Price = price;
        }

        public Guid Id { get; private set; }
        public string Description { get; private set; }
        public string ProductName { get; private set; }
        public int Quantity { get; private set; }
        public double Price { get; private set; }
    }
}