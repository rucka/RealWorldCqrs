using System;
using CommonDomain;
using ManagedDesigns.RealWorldCqrs.Core.Infrastructure;

namespace ManagedDesigns.RealWorldCqrs.Core.Domain.Model
{
    public class Order : AggregateRoot<Order.OrderMemento>
    {
        public static Order Create(Guid id, string description, string productName, int quantity, double price)
        {
            return new Order(id, description, productName, quantity, price);
        }

        private Order(Guid id, string description, string productName, int quantity, double price)
            : this(id)
        {
            RaiseEvent(new OrderPlaced(Id, description, productName, quantity, price));
        }

        private Order(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException("id", "id cannot be empty");
            }
            Id = id;
        }

        public void Accept()
        {
            if (State.IsAccepted || State.IsRejected)
            {
                return;
            }
            RaiseEvent(new OrderAccepted(Id, State.Description));
        }

        public void Reject(string reason)
        {
            if (State.IsAccepted || State.IsRejected)
            {
                return;
            }
            RaiseEvent(new OrderRejected(Id, State.Description, reason));
        }

        public bool TotalAmountIsOver(double amount)
        {
            return (State.Price * State.Quantity) > amount;
        }

        #region ApplyEvents

        private void Apply(OrderPlaced @event)
        {
            State.Description = @event.Description;
            State.ProductName = @event.ProductName;
            State.Quantity = @event.Quantity;
            State.Price = @event.Price;
        }

        private void Apply(OrderAccepted @event)
        {
            State.IsAccepted = true;
        }

        private void Apply(OrderRejected @event)
        {
            State.IsRejected = true;
        }

        #endregion

        #region Nested type: OrderMemento

        public class OrderMemento : IMemento
        {
            public bool IsAccepted { get; set; }
            public bool IsRejected { get; set; }
            public string Description { get; set; }
            public string ProductName { get; set; }
            public double Price { get; set; }
            public int Quantity { get; set; }

            #region IMemento Members

            public Guid Id { get; set; }
            public int Version { get; set; }

            #endregion
        }

        #endregion
    }
}